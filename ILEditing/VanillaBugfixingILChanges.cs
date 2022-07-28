using CalamityMod.NPCs;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.ILEditing
{
    public partial class ILChanges
    {
        public struct OrderedProjectileEntry
        {
            public int OriginalIndex;
            public Projectile Proj;
        }

        // This list should contain all vanilla NPCs present in Boss Rush which ARE NOT bosses and whose health is boosted over 32,767.
        private static List<int> NeedsFourLifeBytes => new()
        {
            // King Slime
            NPCID.BlueSlime,
            NPCID.SlimeSpiked,
            NPCID.RedSlime,
            NPCID.PurpleSlime,
            NPCID.YellowSlime,
            NPCID.IceSlime,
            NPCID.UmbrellaSlime,
            NPCID.RainbowSlime,
            NPCID.Pinky,

            // Eye of Cthulhu
            NPCID.ServantofCthulhu,

            // Eater of Worlds
            NPCID.EaterofWorldsHead,
            NPCID.EaterofWorldsBody,
            NPCID.EaterofWorldsTail,

            // Brain of Cthulhu
            NPCID.Creeper,

            // Skeletron
            NPCID.SkeletronHand,

            // Wall of Flesh
            NPCID.WallofFleshEye,

            // The Destroyer
            NPCID.Probe,

            // Skeletron Prime
            NPCID.PrimeVice,
            NPCID.PrimeSaw,
            NPCID.PrimeLaser,
            NPCID.PrimeCannon,

            // Plantera
            NPCID.PlanterasTentacle,

            // Golem
            NPCID.GolemHead,
            NPCID.GolemHeadFree,
            NPCID.GolemFistLeft,
            NPCID.GolemFistRight,

            // Cultist
            NPCID.CultistDragonHead,
            NPCID.CultistDragonBody1,
            NPCID.CultistDragonBody2,
            NPCID.CultistDragonBody3,
            NPCID.CultistDragonBody4,
            NPCID.CultistDragonTail,
            NPCID.AncientCultistSquidhead,
        };

        public static List<OrderedProjectileEntry> OrderedProjectiles = new();

        /*
        #region Fixing NPC HP Sync Byte Counts in Boss Rush
        // CONTEXT FOR FIX: When NPCs sync they have a pre-determined amount of bytes that are used to store HP/Max NPC information in packets for efficiency.
        // However, this may not always coincide with the true HP of the NPC when it's created if it has more HP than the allocated bytes can sufficiently store, which can
        // happen for various minor enemies in boss rush, as the byte count is only reset by specific, unpredictable events, such as players respawning or entering the world.
        // To give an example, the Ancient Visions in the culst fight have a few thousand HP to work with, placing it in the 2-life-bytes category, and thusly allowing HP values
        // of up to 2^15 - 1 (32767). However, in Boss Rush, as of writing this, it has 50000 HP. In order to mitigate this, it must be placed in the 4-byte category, which allows
        // values up to the integer limit.
        // NOTE -- This mechanic appears to have been removed in 1.4.
        private static void BossRushLifeBytes(On.Terraria.Main.orig_InitLifeBytes orig)
        {
            orig();
            foreach (int npcType in NeedsFourLifeBytes)
                Main.npcLifeBytes[npcType] = 4;
        }
        #endregion Fixing NPC HP Sync Byte Counts in Boss Rush
        */

        #region Fixing Splitting Worm Banner Spam in Deathmode
        // CONTEXT FOR FIX: In Death Mode, normal worms are capable of splitting similarly to the Eater of Worlds. This, as expected, comes with problems with loot dropping, as you can kill multiple
        // head segments from the same original worm. TML allows us to safely handle this with its drop hooks. Unfortunately, however, this does not apply to banner dropping logic based on total kills or
        // for bestiary registrations.
        // As such, we must IL Edit the vanilla drop method to stop it from registering kills based on worms that can still be split. This references the same blocking logic as the aforementioned hooks.
        private static void FixSplittingWormBannerDrops(ILContext il)
        {
            var cursor = new ILCursor(il);

            // Find the first return in the method. This will be marked as a label to jump to if the splitting loot check is failed, effectively terminating any and all
            // loot code, including banners and bestiary stuff.
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchRet()))
            {
                LogFailure("Splitting worm banner spam fix", "Could not locate the first method return.");
            }

            // Save the ret as a place to return to.
            ILLabel ret = cursor.MarkLabel();

            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchStloc(0)))
            {
                LogFailure("Splitting worm banner spam fix", "Could not locate the closest player storage.");
                return;
            }

            // Load the NPC caller onto the stack.
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.EmitDelegate<Func<NPC, bool>>(npc => CalamityGlobalNPC.SplittingWormLootBlockWrapper(npc, CalamityMod.Instance));

            // If the block is false (indicating the drop logic should stop), return the method early.
            cursor.Emit(OpCodes.Brfalse, ret);
        }
        #endregion Fixing Splitting Worm Banner Spam in Deathmode

        #region Let Rogue Items Be Reforgeable
        private static bool LetRogueItemsBeReforgeable(On.Terraria.Item.orig_Prefix orig, Item self, int pre)
        {
            if (self.CountsAsClass<RogueDamageClass>() && self.maxStack == 1 && pre == -3)
            {
                PrefixLoader.Roll(self, ref pre, 40, WorldGen.gen ? WorldGen.genRand : Main.rand, PrefixCategory.AnyWeapon);
                return true;
            }

            return orig(self, pre);
        }
        #endregion Let Rogue Items Be Reforgeable

        #region Fix Projectile Update Priority Problems

        // This IL edit is commented out because it seems to be causing issues, sometimes creating errors preventing Terraria from updating
        // CONTEXT FOR FIX: The way projectile updating works is via looping, starting from 0 and ending at 999. For most cases this works sufficiently.
        // However, in contexts where two projectile's update logic are dependent on each-other in some way (such as mechworm segment movement) it is possible
        // that one projectile will update unexpectedly before the other, creating gaps. By making the update logic ordered based on a priority system, this can be
        // alleviated.
        private static void FixProjectileUpdatePriorityProblems(ILContext il)
        {
            var cursor = new ILCursor(il);

            // Locate the location where the projectile loop starts.
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchCallOrCallvirt<LockOnHelper>("SetUP")))
            {
                LogFailure("Projectile Update Priority fix", "Could not locate the LockOnHelper.SetUP method.");
                return;
            }

            // Before doing anything else, prepare the list of ordered projectiles.
            cursor.EmitDelegate<Func<List<OrderedProjectileEntry>>>(() =>
            {
                List<OrderedProjectileEntry> cache = new List<OrderedProjectileEntry>();
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    cache.Add(new OrderedProjectileEntry()
                    {
                        Proj = Main.projectile[i] ?? new(),
                        OriginalIndex = i
                    });
                }
                return cache.OrderByDescending(p => p.Proj.active ? p.Proj.Calamity().UpdatePriority : 0f).ToList();
            });
            cursor.Emit(OpCodes.Stsfld, typeof(ILChanges).GetField("OrderedProjectiles", BindingFlags.Static | BindingFlags.Public));

            // Find the update loop update local index.
            int updateLoopLocalIndex = -1;
            cursor.GotoNext(i => i.MatchStloc(out updateLoopLocalIndex));

            // Go before the declaration of the projectile loop index and declare the ordered projectile.
            cursor.Goto(0);
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchStsfld<Main>("ProjectileUpdateLoopIndex")))
            {
                LogFailure("Projectile Update Priority fix", "Could not locate the ProjectileUpdateLoopIndex field.");
                return;
            }
            int updateIndexPosition = cursor.Index;

            // Replace the projectile reference on the Projectile.Update call.
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchLdelemRef()))
            {
                LogFailure("Projectile Update Priority fix", $"Could not locate the Main.projectile index load reference.");
                return;
            }

            // Pop the Main.projectile[i] reference and replace it with OrderedProjectiles[i].Proj.
            cursor.Emit(OpCodes.Pop);
            cursor.Emit(OpCodes.Ldsfld, typeof(ILChanges).GetField("OrderedProjectiles", BindingFlags.Static | BindingFlags.Public));
            cursor.Emit(OpCodes.Ldloc, updateLoopLocalIndex);
            cursor.Emit(OpCodes.Callvirt, typeof(List<OrderedProjectileEntry>).GetMethod("get_Item"));
            cursor.Emit(OpCodes.Ldfld, typeof(OrderedProjectileEntry).GetField("Proj"));

            // Pop the direct i reference in the Update call and replace it with the original index in the cache, to prevent update ID
            // mismatches.
            cursor.Goto(updateIndexPosition);

            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchCall<Projectile>("Call")))
            {
                LogFailure("Projectile Update Priority fix", "Could not locate the projectile Update call.");
                return;
            }
            cursor.Emit(OpCodes.Pop);
            cursor.Emit(OpCodes.Ldloc, updateLoopLocalIndex);
            cursor.EmitDelegate<Func<int, int>>(i => OrderedProjectiles[i].OriginalIndex);
        }
        #endregion Fix Projectile Update Priority Problems

        #region Make Mouse Hover Items Work with Animated Items
        private static void MakeMouseHoverItemsSupportAnimations(ILContext il)
        {
            var cursor = new ILCursor(il);

            // Locate the location where the frame rectangle is created.
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchNewobj<Rectangle?>()))
                return;

            int endIndex = cursor.Index;

            // And then go back to where it began, right after the draw position vector.
            if (!cursor.TryGotoPrev(MoveType.After, i => i.MatchNewobj<Vector2>()))
            {
                LogFailure("HoverItem Animation Support", "Could not locate the creation of the draw position vector.");
                return;
            }

            // And delete the range that creates the rectangle with intent to replace it.
            cursor.RemoveRange(Math.Abs(endIndex - cursor.Index));

            cursor.Emit(OpCodes.Ldloc_0);
            cursor.EmitDelegate<Func<int, Rectangle?>>(itemType =>
            {
                return Main.itemAnimations[itemType]?.GetFrame(TextureAssets.Item[itemType].Value) ?? null;
            });
        }
        #endregion Make Mouse Hover Items Work with Animated Items
    }
}
