using CalamityMod.NPCs;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace CalamityMod.ILEditing
{
    public partial class ILChanges
    {
        // This list should contain all vanilla NPCs present in Boss Rush which ARE NOT bosses and whose health is boosted over 32,767.
        private static readonly List<int> NeedsFourLifeBytes = new List<int>()
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

        #region Fixing NPC HP Sync Byte Counts in Boss Rush
        // CONTEXT FOR FIX: When NPCs sync they have a pre-determined amount of bytes that are used to store HP/Max NPC information in packets for efficiency.
        // However, this may not always coincide with the true HP of the NPC when it's created if it has more HP than the allocated bytes can sufficiently store, which can
        // happen for various minor enemies in boss rush, as the byte count is only reset by specific, unpredictable events, such as players respawning or entering the world.
        // To give an example, the Ancient Visions in the culst fight have a few thousand HP to work with, placing it in the 2-life-bytes category, and thusly allowing HP values
        // of up to 2^15 - 1 (32767). However, in Boss Rush, as of writing this, it has 50000 HP. In order to mitigate this, it must be placed in the 4-byte category, which allows
        // values up to the integer limit.
        private static void BossRushLifeBytes(On.Terraria.Main.orig_InitLifeBytes orig)
        {
            orig();
            foreach (int npcType in NeedsFourLifeBytes)
                Main.npcLifeBytes[npcType] = 4;
        }
        #endregion Fixing NPC HP Sync Byte Counts in Boss Rush

        #region Fixing Splitting Worm Banner Spam in Deathmode
        // CONTEXT FOR FIX: In Death Mode, normal worms are capable of splitting similarly to the Eater of Worlds. This, as expected, comes with problems with loot dropping, as you can kill multiple
        // head segments from the same original worm. Thankfully, TML allows us to safely handle this with its drop hooks. Unfortunately, however, this does not apply to banner dropping logic based on total kills.
        // As such, we must IL Edit the vanilla drop method to stop it from registering kills based on worms that can still be split. This references the same blocking logic as the aforementioned hooks.
        private static void FixSplittingWormBannerDrops(ILContext il)
        {
            var cursor = new ILCursor(il);

            // Locate the area after all the banner logic by using a nearby constant type.
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcI4(23)))
            {
                LogFailure("splitting worm banner spam fix", "Could not locate the first hooking constant.");
                return;
            }
            if (!cursor.TryGotoPrev(MoveType.Before, i => i.MatchLdarg(0)))
            {
                LogFailure("splitting worm banner spam fix", "Could not locate the second hooking constant.");
                return;
            }

            ILLabel afterBannerLogic = cursor.DefineLabel();

            // Set this area after as a place to return to later.
            cursor.MarkLabel(afterBannerLogic);

            // Go to the beginning of the banner drop logic.
            cursor.Goto(0);
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdsfld<NPC>("killCount")))
            {
                LogFailure("splitting worm banner spam fix", "Could not locate the NPC kill count.");
                return;
            }

            // Load the NPC caller onto the stack.
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.EmitDelegate<Func<NPC, bool>>(npc => CalamityGlobalNPCLoot.SplittingWormLootBlockWrapper(npc, CalamityMod.Instance));

            // If the block is false (indicating the drop logic should stop), skip all the ahead banner drop logic.
            cursor.Emit(OpCodes.Brfalse, afterBannerLogic);
        }
        #endregion Fixing Splitting Worm Banner Spam in Deathmode

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
                return Main.itemAnimations[itemType]?.GetFrame(Main.itemTexture[itemType]) ?? null;
            });
        }
        #endregion Make Mouse Hover Items Work with Animated Items
    }
}
