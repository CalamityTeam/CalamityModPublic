using System;
using System.Linq;
using CalamityMod.Balancing;
using CalamityMod.CalPlayer;
using CalamityMod.Cooldowns;
using CalamityMod.DataStructures;
using CalamityMod.Enums;
using CalamityMod.Events;
using CalamityMod.FluidSimulation;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Wulfrum;
using CalamityMod.Items.Critters;
using CalamityMod.Items.Dyes;
using CalamityMod.Items.Placeables.FurniturePlagued;
using CalamityMod.Items.Potions.Alcohol;
using CalamityMod.NPCs;
using CalamityMod.NPCs.Abyss;
using CalamityMod.NPCs.Astral;
using CalamityMod.NPCs.DraedonLabThings;
using CalamityMod.NPCs.SunkenSea;
using CalamityMod.Packets;
using CalamityMod.Particles;
using CalamityMod.Projectiles;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.Systems;
using CalamityMod.Systems.Collections;
using CalamityMod.Systems.Mechanic;
using CalamityMod.Tiles;
using CalamityMod.Utilities.Daybreak;
using CalamityMod.Walls;
using CalamityMod.Walls.UnsafeWalls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.GameContent.Events;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.Graphics.Light;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI.Gamepad;

namespace CalamityMod.ILEditing
{
    public partial class ILChanges
    {
        private static int labDoorOpen = -1;
        private static int labDoorClosed = -1;
        private static int aLabDoorOpen = -1;
        private static int aLabDoorClosed = -1;
        private static int exoDoorOpen = -1;
        private static int exoDoorClosed = -1;

        //private static readonly MethodInfo textureGetValueMethod = typeof(Asset<Texture2D>).GetMethod("get_Value", BindingFlags.Public | BindingFlags.Instance);

        #region Dash Fixes and Improvements
        private static void MakeShieldSlamIFramesConsistent(ILContext il)
        {
            var cursor = new ILCursor(il);

            // Find the direct assignment of the player's iframes.
            if (!cursor.TryGotoNext(MoveType.AfterLabel, i => i.MatchStfld<Player>("immuneTime")))
            {
                LogFailure("Shield Slam Consistent Immunity Frames", "Could not locate the assignment of the player's immune time.");
                return;
            }

            // Delete this instruction. The stack still contains the player and amount of iframes to give.
            cursor.Remove();

            // Emit a delegate which calls the Calamity utility to consistently provide iframes.
            // 17APR2024: Ozzatron: Consistent shield slam iframes are not boosted by Cross Necklace at all and are fixed.
            cursor.EmitDelegate<Action<Player, int>>((p, frames) => CalamityUtils.GiveUniversalIFrames(p, frames, false));
        }

        private static readonly Func<Player, int> CalamityDashEquipped = (Player p) => p.Calamity().HasCustomDash ? 1 : 0;

        private static void FixAllDashMechanics(ILContext il)
        {
            var cursor = new ILCursor(il);

            //
            // FIX DASH COOLDOWN RESET FOR CALAMITY DASHES
            //

            // Vanilla resets dash cooldown if no vanilla dash is equipped. Calamity dashes must also be considered.
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchLdfld<Player>("dash")))
            {
                LogFailure("Vanilla Dash Fixes", "Could not locate the check for vanilla dash ID.");
                return;
            }

            // Load the player itself onto the stack so that it becomes an argument for the following delegate.
            cursor.Emit(OpCodes.Ldarg_0);

            // Emit a delegate which places whether the player has a Calamity dash equipped onto the stack.
            cursor.EmitDelegate<Func<Player, int>>(CalamityDashEquipped);

            // Bitwise OR the two values together. This will only return zero if both values were zero.
            // This will occur precisely when the player has no vanilla OR Calamity dash items equipped.
            cursor.Emit(OpCodes.Or);

            // CIT 22JUL2025: Shield of Cthulhu bonk is reimplemented in a separate On edit; removed the change made to it via this IL edit.

            //
            // SOLAR FLARE ARMOR
            //

            // Move onto the next dash (Solar Flare set bonus) by looking for the base damage of the direct contact strike.
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(150f)))
            {
                LogFailure("Vanilla Dash Fixes", "Could not locate Solar Flare Armor shield slam base damage.");
                return;
            }

            // Replace vanilla's base damage of 150 with Calamity's custom base damage.
            cursor.Next.Operand = BalancingConstants.SolarFlareBaseDamage;

            // Move to the immunity frame setting code for the Solar Flare set bonus. Find the constant 4 given as iframes.
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcI4(4)))
            {
                LogFailure("Vanilla Dash Fixes", "Could not locate Solar Flare shield slam iframes.");
                return;
            }

            // Replace it with Calamity's number of iframes.
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_I4, BalancingConstants.SolarFlareIFrames);

            //
            // DASH COOLDOWNS
            //

            // Move to the dash cooldown code by finding the location where the dash cooldown local variable is initialized.
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchStloc(22)))
            {
                LogFailure("Vanilla Dash Fixes", "Could not locate default dash cooldown initialization.");
                return;
            }

            // The instruction right before this is Ldc.I4 20, so replace its operand directly.
            // This is the default, so set it to the default universal dash cooldown.
            cursor.Previous.Operand = BalancingConstants.UniversalDashCooldown;

            // dash == 1 (aka Tabi) uses the default cooldown and thus does not need edits.

            // Same thing as last time, but this time it's the cooldown for dash == 2 (aka Shield of Cthulhu).
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchStloc(22)))
            {
                LogFailure("Vanilla Dash Fixes", "Could not locate dash cooldown assignment for Shield of Cthulhu.");
                return;
            }

            // The instruction right before this is Ldc.I4 30, so replace its operand directly.
            // This is for Shield of Cthulhu, so set it to the shield bonk cooldown.
            cursor.Previous.Operand = BalancingConstants.UniversalShieldBonkCooldown;

            // Same thing as last time, but this time it's the cooldown for dash == 3 (aka Solar Flare Armor).
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchStloc(22)))
            {
                LogFailure("Vanilla Dash Fixes", "Could not locate dash cooldown assignment for Solar Flare Armor.");
                return;
            }

            // The instruction right before this is Ldc.I4 30, so replace its operand directly.
            // This is for Solar Flare Armor, so set it to the shield slam cooldown.
            cursor.Previous.Operand = BalancingConstants.UniversalShieldSlamCooldown;

            // Same thing as last time, but this time it's the cooldown for dash == 4 (?????).
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchStloc(22)))
            {
                LogFailure("Vanilla Dash Fixes", "Could not locate dash cooldown assignment for UNKNOWN DASH ID 4.");
                return;
            }

            // The instruction right before this is Ldc.I4 20, so replace its operand directly.
            cursor.Previous.Operand = BalancingConstants.UniversalDashCooldown;

            // dash == 5 (aka Crystal Assassin Armor) uses the default cooldown and thus does not need edits.
        }

        private static void NerfShieldOfCthulhuBonkSafety(ILContext il)
        {
            // Reduce the number of "no-collide frames" (they are NOT iframes) granted by the Shield of Cthulhu bonk.
            var cursor = new ILCursor(il);
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdfld<Player>("eocDash"))) // Loading the remaining frames of the SoC dash
            {
                LogFailure("Shield of Cthulhu Bonk Nerf", "Could not locate Shield of Cthulhu dash remaining frame counter.");
                return;
            }

            // Find the 0 this is normally compared to. We will be replacing this value.
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcI4(0)))
            {
                LogFailure("Shield of Cthulhu Bonk Nerf", "Could not locate the zero comparison.");
                return;
            }

            // Remove the zero and replace it with a calculated value.
            // This is the total length of the EoC bonk (10) minus the number of safe frames allowed by Calamity.
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_I4, 10 - BalancingConstants.ShieldOfCthulhuBonkNoCollideFrames);
        }

        private static void ApplyDashKeybind(On_Player.orig_DoCommonDashHandle orig, Player self, out int dir, out bool dashing, Player.DashStartAction dashStartAction)
        {
            // we feasting multiplayer bugs
            if (self.whoAmI != Main.myPlayer)
            {
                orig(self, out dir, out dashing, dashStartAction);
                return;
            }

            if (CalamityKeybinds.DashHotkey.JustPressed)
            {
                // Out of safety in the steps following, set the player direction itself here.
                // If you are holding D but not A, then always dash right.
                if (self.controlRight && !self.controlLeft)
                    self.direction = 1;
                // If you are holding A but not D, then always dash left.
                else if (self.controlLeft && !self.controlRight)
                    self.direction = -1;
                // If you are moving, set dash in the direction the player is moving.
                else if (MathF.Abs(self.velocity.X) > 0.01f)
                    self.direction = self.velocity.X > 0f ? 1 : -1;

                dir = self.direction;
                dashing = true;

                // CIT 16OCT2024: Commented this code out, as there's no reason for custom dash hotkey to use Player.dashTime
                // and was causing the return of Celestial Starboard's dash bug from early 1.4 versions
                /*if ((self.dashTime <= 0 && self.direction == -1) || (self.dashTime >= 0 && self.direction == 1))
                {
                    self.dashTime = 15;
                    return;
                }*/

                dashing = true;
                self.dashTime = 0;
                self.timeSinceLastDashStarted = 0;
                dashStartAction?.Invoke(dir);
                return;
            }

            if (CalamityKeybinds.DashHotkey.GetAssignedKeysOrEmpty().Count == 0)
                orig(self, out dir, out dashing, dashStartAction);
            else
            {
                dir = 1;
                dashing = false;
                self.dashTime = 0;
            }
        }

        private static void DisableDoubleTapOnConfig(On_Player.orig_KeyDoubleTap orig, Player self, int keyDir)
        {
            if (self.whoAmI != Main.myPlayer)
            {
                orig(self, keyDir);
                return;
            }

            if ((CalamityKeybinds.ArmorSetBonusHotKey.GetAssignedKeysOrEmpty().Count != 0 && CalamityClientConfig.Instance.SetBonusDoubleTap == SetBonusDoubleTapOptions.Auto) || CalamityClientConfig.Instance.SetBonusDoubleTap == SetBonusDoubleTapOptions.Off)
                return;

            orig(self, keyDir);
        }
        #endregion

        #region Prevent Vanilla Bosses From Being Marked as Defeated in Boss Rush
        private static void PreventVanillaBossDeathsInBossRush(ILContext il)
        {
            // GOAL: Prevent vanilla boss defeated flags from getting set during Boss Rush.
            // All of this is handled within a switch case directly in the DoDeathEvents function.
            // Thus, the objective here is to add a branch past the entire switch case if it is Boss Rush.
            var cursor = new ILCursor(il);

            // Move to the point right before the switch case.
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchCall<NPC>("SpawnOnPlayer")))
            {
                LogFailure("Prevent Vanilla Defeated Flags in Boss Rush", "Could not move to before the switch case.");
                return;
            }

            // Define a branch label, load the Boss Rush check, and create the branch if it is true.
            var label = il.DefineLabel();
            cursor.Emit(OpCodes.Ldsfld, typeof(BossRushEvent).GetField("BossRushActive"));
            cursor.Emit(OpCodes.Brtrue, label);

            // Move to after the switch case to place the label.
            for (int i = 0; i < 2; i++)
            {
                if (!cursor.TryGotoNext(MoveType.After, i => i.MatchCall<NPC>("SpawnBoss")))
                {
                    LogFailure("Prevent Vanilla Defeated Flags in Boss Rush", "Could not move to after the switch case.");
                    return;
                }
            }
            if (!cursor.TryGotoNext(MoveType.AfterLabel, i => i.MatchLdarg0()))
            {
                LogFailure("Prevent Vanilla Defeated Flags in Boss Rush", "Could not move to after the switch case.");
                return;
            }
            cursor.MarkLabel(label);
        }
        #endregion

        #region Enabling of Fusion Feeder Sand Digging
        private static void AllowFusionFeederToDigThroughSand(On_NPC.orig_ApplyTileCollision orig, NPC self, bool fall, Vector2 cPosition, int cWidth, int cHeight)
        {
            if (self.active && self.type == ModContent.NPCType<FusionFeeder>())
            {
                self.velocity = Collision.AdvancedTileCollision(TileID.Sets.ForAdvancedCollision.ForSandshark, cPosition, self.velocity, cWidth, cHeight, fall, fall, 1);
                return;
            }
            orig(self, fall, cPosition, cWidth, cHeight);
        }
        #endregion

        #region Prevent Diabolists from Dropping Stuff in GFB Before Plantera
        private static void PreventDiabolistLootLogic(On_NPC.orig_NPCLoot orig, NPC self)
        {
            if (self.type == NPCID.DiabolistWhite && Main.getGoodWorld && !NPC.downedPlantBoss)
                return;
            orig(self);
        }
        #endregion

        #region Town NPC Spawning Improvements
        private static void PermitNighttimeTownNPCSpawning(ILContext il)
        {
            // Don't do town NPC spawning at the end (which lies after a !Main.dayTime return).
            // Do it at the beginning, without the arbitrary time restriction.
            var cursor = new ILCursor(il);
            cursor.EmitDelegate<Action>(() =>
            {
                if (Main.dayTime || CalamityServerConfig.Instance.TownNPCsSpawnAtNight)
                    Main.UpdateTime_SpawnTownNPCs();
            });

            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchCallOrCallvirt<Main>(nameof(Main.UpdateTime_SpawnTownNPCs))))
            {
                CalamityMod.Log.Warn("Town NPC spawn editing code failed.");
                return;
            }

            cursor.Emit(OpCodes.Ret);
        }

        private static void AlterTownNPCSpawnRate(On_Main.orig_UpdateTime_SpawnTownNPCs orig)
        {
            double oldWorldRate = Main.desiredWorldTilesUpdateRate;
            Main.desiredWorldTilesUpdateRate *= CalamityServerConfig.Instance.TownNPCSpawnRateMultiplier;
            orig();
            Main.desiredWorldTilesUpdateRate = oldWorldRate;
        }
        #endregion

        #region Dodge Mechanic Adjustments
        private static void DodgeMechanicAdjustments(ILContext il)
        {
            var cursor = new ILCursor(il);

            // Skip past the first half of the function. We do not care about the following opening steps of Player.Hurt:
            // 1. AllowShimmerDodge
            // 2. Journey's god mode
            // 3. TML PlayerLoader.ImmuneTo
            // 4. vanilla iframe check
            // 5. ModifyHurt
            // 6. ogre knockback
            //
            // To skip to the part we care about, go after the one and only call to HurtModifiers.ToHurtInfo.
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchCall(typeof(Player.HurtModifiers), nameof(Player.HurtModifiers.ToHurtInfo))))
            {
                LogFailure("Dodge Mechanic Adjustments", "Could not locate the call to HurtModifiers.ToHurtInfo.");
                return;
            }

            // The load for the dodgeable boolean is impossible to decipher due to being an ldarg_s (optional parameter).
            // This is used to make Day Empress' attacks undodgeable.
            // Instead, find the immediately following brfalse.
            // If Calamity's old Armageddon bool which "Disables all dodges" is enabled, EVERY attack is considered undodgeable.
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchBrfalse(out ILLabel branchEnd)))
            {
                LogFailure("Dodge Mechanic Adjustments", "Could not locate the dodgeable boolean branch.");
                return;
            }

            // AND with an emitted delegate which takes the player and gets whether Calamity is allowing dodges for them right now
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.EmitDelegate((Player p) => !p.Calamity().disableAllDodges);
            cursor.Emit(OpCodes.And);

            // Skip ahead to Black Belt
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchLdfld<Player>("blackBelt")))
            {
                LogFailure("Dodge Mechanic Adjustments", "Could not locate the Black Belt equipped boolean.");
                return;
            }

            // Destroy the value, utterly, and in its place put zero.
            // The player is never considered to have Black Belt equipped from the perspective of vanilla code.
            // Calamity re-implements Black Belt in CalamityPlayer.ConsumableDodge
            cursor.Emit(OpCodes.Pop);
            cursor.Emit(OpCodes.Ldc_I4_0);

            // Skip ahead to Brain of Confusion
            // Here the part we skip to is actually
            // brainOfConfusionItem != null
            // This is implemented as a brfalse, so just do the same thing as above.
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchLdfld<Player>("brainOfConfusionItem")))
            {
                LogFailure("Dodge Mechanic Adjustments", "Could not locate the Brain of Confusion tracked equipped item.");
                return;
            }

            // The player is never considered to have Brain of Confusion equipped from the perspective of vanilla code.
            // Calamity re-implements Brain of Confusion in CalamityPlayer.ConsumableDodge
            // Instead of being limited by its buff, it is limited by the Global Dodge Cooldown.
            cursor.Emit(OpCodes.Pop);
            cursor.Emit(OpCodes.Ldc_I4_0);

            // No interference with ShadowDodge (previously Titanium armor, now Hallowed armor)
        }

        private static void AddHolyProtectionCooldown(On_Player.orig_PutHallowedArmorSetBonusOnCooldown orig, Player self)
        {
            // Adds Calamity's Holy Protection cooldown when triggering Hallowed armor's dodge.
            orig(self);
            self.AddCooldown(HolyProtection.ID, CalamityUtils.SecondsToFrames(30));
        }
        #endregion

        #region Custom Gate Door Logic
        private static bool OpenDoor_LabDoorOverride(On_WorldGen.orig_OpenDoor orig, int i, int j, int direction)
        {
            Tile tile = Main.tile[i, j];

            // If it's one of the two lab doors, use custom code to open the door and sync tiles in multiplayer.
            if (tile.TileType == labDoorClosed)
                return OpenLabDoor(tile, i, j, labDoorOpen);
            else if (tile.TileType == aLabDoorClosed)
                return OpenLabDoor(tile, i, j, aLabDoorOpen);
            else if (tile.TileType == exoDoorClosed)
                return OpenLabDoor(tile, i, j, exoDoorOpen);

            // If it's anything else, let vanilla and/or TML handle it.
            return orig(i, j, direction);
        }

        private static bool CloseDoor_LabDoorOverride(On_WorldGen.orig_CloseDoor orig, int i, int j, bool forced)
        {
            Tile tile = Main.tile[i, j];

            // If it's one of the two lab doors, use custom code to open the door and sync tiles in multiplayer.
            if (tile.TileType == labDoorOpen)
                return CloseLabDoor(tile, i, j, labDoorClosed);
            else if (tile.TileType == aLabDoorOpen)
                return CloseLabDoor(tile, i, j, aLabDoorClosed);
            else if (tile.TileType == exoDoorOpen)
                return CloseLabDoor(tile, i, j, exoDoorClosed);

            // If it's anything else, let vanilla and/or TML handle it.
            return orig(i, j, forced);
        }
        #endregion

        #region Incorporate Enchantments in Item Names
        private static string IncorporateEnchantmentInAffix(On_Item.orig_AffixName orig, Item self)
        {
            string result = orig(self);

            // This hook could occur before CalamityGlobalItem is loaded and throw an error.
            try
            {
                if (!self.IsAir && self.Calamity().AppliedEnchantment.HasValue)
                    result = $"{self.Calamity().AppliedEnchantment.Value.Name} {result}";
            }
            catch { }
            return result;
        }
        #endregion

        #region Apply Projectile Variables Upon Creation
        private static int IncorporateExtraProjectileVariables(On_Projectile.orig_NewProjectile_IEntitySource_float_float_float_float_int_int_float_int_float_float_float orig, IEntitySource spawnSource, float x, float y, float xSpeed, float ySpeed, int type, int damage, float knockback, int owner, float ai0, float ai1, float ai2)
        {
            // This is unfortunately not something that can be done via SetDefaults since owner is set
            // after that method is called. Doing it directly when the projectile is spawned appears to be the only reasonable way.
            int proj = orig(spawnSource, x, y, xSpeed, ySpeed, type, damage, knockback, owner, ai0, ai1, ai2);
            Projectile projectile = Main.projectile[proj];

            Player player = Main.player[projectile.owner];
            if (Main.gameMenu || !player.active)
                return proj;

            if (!projectile.TryGetGlobalProjectile<CalamityGlobalProjectile>(out var calProj))
                return proj;

            // Old Fashioned
            if (spawnSource is EntitySource_Parent parentSource)
            {
                // Parent-spawned projectiles involve 5 relevant sources: Buff, Item, NPC, Player, Projectile
                // 1. Buffs: Old Fashioned inherently does not affect them, they have no effect

                /* 2. Item: If detected to be from weapons, are assumedly debuffed
                *The criteria includes: A. has damage, B. has use animation, and C. came out of an item
                *This rules out items such as Bombs or Shield of Cthulhu (hypothetically were it to spawn projectiles)
                *This source is *not* from item use, but is still used for a multitude of purposes in weapons */
                if (parentSource.Entity is Item item && item.damage > 0 && item.useAnimation > 0)
                    calProj.buffedByOldFashioned = false;

                // 3. NPCs: Not considered at all; no effect

                // 4. Players: Only considered if the source owner matches the projectile's, otherwise no effect
                else if (parentSource.Entity is Player parentPlayer && parentPlayer.whoAmI == projectile.owner)
                {
                    // 4A. The primary player-based source is item use, which follows the same logic as items (see above)
                    // Edge case: Wulfrum Fusion Cannon is coded like a weapon. There may be a better way to approach this but an exclusion works for now
                    if (spawnSource is EntitySource_ItemUse itemSource && itemSource.Item is Item usedItem
                    && usedItem.damage > 0 && usedItem.useAnimation > 0 && usedItem.type != ModContent.ItemType<WulfrumFusionCannon>())
                        calProj.buffedByOldFashioned = false;
                    // 4B. Every non item-use source is assumed safe to buff
                    else
                        calProj.buffedByOldFashioned = true;
                }

                // 5. Projectiles: Directly inherited by its parent projectile
                else if (parentSource.Entity is Projectile parentProj)
                    calProj.buffedByOldFashioned = parentProj.Calamity().buffedByOldFashioned;
            }

            // Hellbound Enchantment
            // This only applies to minions spawned by weapon uses, preventing other minions such as Luxor's Gift from exploding
            if (projectile.minion && spawnSource is EntitySource_ItemUse useSource && useSource.Item is Item weapon && weapon.useAnimation > 0)
            {
                CalamityPlayer.EnchantHeldItemEffects(player, player.Calamity(), player.HeldItem);
                if (player.Calamity().explosiveMinionsEnchant)
                    calProj.ExplosiveEnchantCountdown = CalamityGlobalProjectile.ExplosiveEnchantTime;
            }
            // Minion shots inherit the "explode countdown" from the parent minion
            // This is only to inherit the damage scaling of the minion and does NOT mean the shot will explode too
            else if (ProjectileID.Sets.MinionShot[projectile.type])
            {
                // Going down the chain
                if (spawnSource is EntitySource_Parent trueSource && trueSource.Entity is Projectile parent)
                    calProj.ExplosiveEnchantCountdown = parent.Calamity().ExplosiveEnchantCountdown;
            }
            return proj;
        }
        #endregion

        #region Apply Old Fashioned Damage to Miscellanous Hits
        private static void ApplyOldFashionedDamageToMiscHits(On_Player.orig_ApplyDamageToNPC orig, Player self, NPC npc, int damage, float knockback, int direction, bool crit = false, DamageClass? damageType = null, bool damageVariation = false)
        {
            if (self.Calamity().oldFashioned)
                damage = (int)(damage * OldFashioned.DamageBoostMultiplier);
            if (self.GetModPlayer<IVDripPlayer>().HasAlcohol(AlcoholType.OldFashioned))
                damage = (int)(damage * OldFashioned.DamageBoostMultiplier);
            orig(self, npc, damage, knockback, direction, crit, damageType, damageVariation);
        }
        #endregion

        #region Chaos Stone and Chalice of the Blood God
        private static void ChaliceBufferHeal(ILContext il)
        {
            ILCursor cursor = new ILCursor(il);

            //
            // The following section enables Chalice of the Blood God's feature of healing potions clearing 50% of its bleedout buffer.
            //

            // Start by finding the moment where health is restored from a healing item.
            if (!cursor.TryGotoNext(MoveType.After, c => c.MatchStfld<Player>("statLife")))
            {
                LogFailure("Chalice of the Blood God Bleedout Heal", "Could not locate the player's health being restored");
                return;
            }

            // Load the player and the healing potion used onto the stack for use in the following delegate.
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldarg_1);

            // Insert a delegate which applies Chalice of the Blood God's function as appropriate.
            cursor.EmitDelegate<Action<Player, Item>>((player, potion) =>
            {
                // If the consumed item heals 0 health, don't bother.
                if (!player.active || player.dead || potion.healLife <= 0)
                    return;

                CalamityPlayer modPlayer = player.Calamity();
                if (modPlayer is null)
                    return;

                // CIT 17MAY2025: Bleedout clear has a special interaction with Bloom Stone handled elsewhere; do not run this if wearing Bloom Stone
                if (modPlayer.chaliceOfTheBloodGod && modPlayer.chaliceBleedoutBuffer > 0D && !modPlayer.bloomStone)
                {
                    // 20FEB2024: Ozzatron: to prevent abuse, buffer clearing is now 50% of the potion instead of 50% of your buffer
                    float amountOfBleedToClear = ChaliceOfTheBloodGod.HealingPotionRatioForBufferClear * player.GetHealLife(potion, true);

                    // Actually clear the buffer
                    modPlayer.chaliceBleedoutBuffer -= amountOfBleedToClear;

                    // Display text indicating that healing was applied to the bleedout buffer.
                    if (!Main.dedServ)
                    {
                        string text = $"(+{amountOfBleedToClear})";
                        Rectangle location = new Rectangle((int)player.position.X + 4, (int)player.position.Y - 3, player.width - 4, player.height - 4);
                        CombatText.NewText(location, ChaliceOfTheBloodGod.BleedoutBufferDamageTextColor, Language.GetTextValue(text), dot: true);
                    }
                }
            });
        }
        #endregion

        #region Chaos Stone Mana Burn changes
        private static bool AllowNegativeCheckMana(On_Player.orig_CheckMana_int_bool_bool orig, Player self, int amount, bool pay, bool blockQuickMana)
        {
            if (self.Calamity().ChaosStone)
            {
                if (pay)
                    self.statMana -= amount;
                if (self.statMana < -self.statManaMax2)
                    self.statMana = -self.statManaMax2;
                return true;
            }
            return orig(self, amount, pay, blockQuickMana);
        }

        private static bool AllowNegativeCheckMana(On_Player.orig_CheckMana_Item_int_bool_bool orig, Player self, Item item, int amount, bool pay, bool blockQuickMana)
        {
            if (self.Calamity().ChaosStone)
            {
                if (pay)
                    self.statMana -= item.mana;
                if (self.statMana < -self.statManaMax2)
                    self.statMana = -self.statManaMax2;
                return true;
            }
            return orig(self, item, amount, pay, blockQuickMana);
        }
        #endregion

        #region Fire Cursor Effect for the Calamity Accessory
        private static void UseCoolFireCursorEffect(On_Main.orig_DrawCursor orig, Vector2 bonus, bool smart)
        {
            Player player = Main.LocalPlayer;

            // Do nothing special if the player has a regular mouse or is on the menu/map.
            if (Main.gameMenu || Main.mapFullscreen || !player.Calamity().blazingCursorVisuals)
            {
                orig(bonus, smart);
                return;
            }

            if (player.dead)
            {
                Main.ClearSmartInteract();
                Main.TileInteractionLX = (Main.TileInteractionHX = (Main.TileInteractionLY = (Main.TileInteractionHY = -1)));
            }

            Color flameColor = Color.Lerp(Color.DarkRed, Color.OrangeRed, (float)Math.Cos(Main.GlobalTimeWrappedHourly * 7.4f) * 0.5f + 0.5f);
            Color cursorColor = flameColor * 1.9f;
            Vector2 baseDrawPosition = Main.MouseScreen + bonus;

            // Draw the mouse as usual if the player isn't using the gamepad.
            if (!PlayerInput.UsingGamepad)
            {
                int cursorIndex = smart.ToInt();

                Color desaturatedCursorColor = cursorColor;
                desaturatedCursorColor.R /= 5;
                desaturatedCursorColor.G /= 5;
                desaturatedCursorColor.B /= 5;
                desaturatedCursorColor.A /= 2;

                Vector2 drawPosition = baseDrawPosition;
                Vector2 desaturatedDrawPosition = drawPosition + Vector2.One;

                // If the blazing mouse is actually going to do damage, draw an indicator aura.
                if (!Main.mapFullscreen)
                {
                    int size = 370;
                    FluidFieldManager.AdjustSizeRelativeToGraphicsQuality(ref size);

                    float scale = MathHelper.Max(Main.screenWidth, Main.screenHeight) / size;
                    ref FluidField calamityFireDrawer = ref player.Calamity().CalamityFireDrawer;
                    ref Vector2 firePosition = ref player.Calamity().FireDrawerPosition;
                    if (calamityFireDrawer is null || calamityFireDrawer.Size != size)
                        calamityFireDrawer = FluidFieldManager.CreateField(size, scale, 0.1f, 50f, 0.992f);

                    // Update the fire draw position.
                    firePosition = new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f;

                    int x = (int)((drawPosition.X - firePosition.X) / calamityFireDrawer.Scale);
                    int y = (int)((drawPosition.Y - firePosition.Y) / calamityFireDrawer.Scale);
                    int horizontalArea = (int)Math.Ceiling(8f / calamityFireDrawer.Scale);
                    int verticalArea = (int)Math.Ceiling(8f / calamityFireDrawer.Scale);

                    calamityFireDrawer.ShouldUpdate = player.miscCounter % 2 == 0;
                    calamityFireDrawer.UpdateAction = () =>
                    {
                        Color color = Color.Lerp(Color.Red, Color.Orange, (float)Math.Sin(Main.GlobalTimeWrappedHourly * 6f) * 0.5f + 0.5f);

                        // Use a rainbow color if the player has the rainbow cursor equipped as well as Calamity.
                        if (player.hasRainbowCursor)
                            color = Color.Lerp(color, Main.hslToRgb(Main.GlobalTimeWrappedHourly * 0.97f % 1f, 1f, 0.6f), 0.75f);

                        // Make the flame more greyscale if a dye is being applied, so that it doesn't conflict with the specific oranges and reds to create gross colors.
                        if (player.Calamity().CalamityFireDyeShader is not null)
                            color = Color.Lerp(color, Color.White, 0.75f);

                        float offsetAngleValue = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 6.7f) * 0.99f;
                        int origin = size / 2;

                        for (int i = -horizontalArea; i <= horizontalArea; i++)
                        {
                            float offsetAngle = offsetAngleValue;
                            offsetAngle += i / (float)horizontalArea * 0.34f;
                            Vector2 velocity = Main.MouseWorld - UIManagementSystem.PreviousMouseWorld;
                            if (velocity.Length() < 64f)
                            {
                                offsetAngle *= 0.5f;
                                velocity = Vector2.Zero;
                            }
                            UIManagementSystem.PreviousMouseWorld = Main.MouseWorld;

                            velocity = velocity.SafeNormalize(-Vector2.UnitY).RotatedBy(offsetAngle) * 3f;

                            // Add a tiny bit of randomness to the velocity.
                            // Chaos in the advection calculations should result in different flames being made over time, instead of a
                            // static animation.
                            velocity *= Main.rand.NextFloat(0.9f, 1.1f);

                            for (int j = -verticalArea; j <= verticalArea; j++)
                                player.Calamity().CalamityFireDrawer.CreateSource(x + origin + i, y + origin + j, 1f, color, i == 0 && j == 0 ? velocity : Vector2.Zero);
                        }
                    };

                    calamityFireDrawer.Draw(firePosition, true, Main.UIScaleMatrix, Main.UIScaleMatrix, output =>
                    {
                        var armorShader = player.Calamity().CalamityFireDyeShader;
                        if (armorShader is null)
                            return;

                        armorShader.Apply(null, new(output, Vector2.Zero, Color.White));
                    });
                }

                Main.spriteBatch.Draw(TextureAssets.Cursors[cursorIndex].Value, drawPosition, null, desaturatedCursorColor, 0f, Vector2.Zero, Main.cursorScale, SpriteEffects.None, 0f);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.UIScaleMatrix);
                GameShaders.Misc["CalamityMod:FireMouse"].UseColor(Color.Red);
                GameShaders.Misc["CalamityMod:FireMouse"].UseSecondaryColor(Color.Lerp(Color.Red, Color.Orange, 0.75f));
                GameShaders.Misc["CalamityMod:FireMouse"].Apply();

                Main.spriteBatch.Draw(TextureAssets.Cursors[cursorIndex].Value, desaturatedDrawPosition, null, cursorColor, 0f, Vector2.Zero, Main.cursorScale * 1.075f, SpriteEffects.None, 0f);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.EffectMatrix);
                return;
            }

            // Don't bother doing any more drawing if the player is dead.
            if (Main.LocalPlayer.dead && !Main.LocalPlayer.ghost && !Main.gameMenu)
                return;

            if (PlayerInput.InvisibleGamepadInMenus)
                return;

            // Draw a white circle instead if using the smart cursor.
            if (smart && !(UILinkPointNavigator.Available && !PlayerInput.InBuildingMode))
            {
                cursorColor = Color.White * Main.GamepadCursorAlpha;
                int frameX = 0;
                Texture2D smartCursorTexture = TextureAssets.Cursors[13].Value;
                Rectangle frame = smartCursorTexture.Frame(2, 1, frameX, 0);
                Main.spriteBatch.Draw(smartCursorTexture, baseDrawPosition, frame, cursorColor, 0f, frame.Size() * 0.5f, Main.cursorScale, SpriteEffects.None, 0f);
                return;
            }

            // Otherwise draw an ordinary crosshair at the mouse position.
            cursorColor = Color.White;
            Texture2D crosshairTexture = TextureAssets.Cursors[15].Value;
            Main.spriteBatch.Draw(crosshairTexture, baseDrawPosition, null, cursorColor, 0f, crosshairTexture.Size() * 0.5f, Main.cursorScale, SpriteEffects.None, 0f);
        }
        #endregion

        #region Custom DoDraw Changes
        private static void CustomDoDrawChanges(ILContext il)
        {
            // Allows for drawing additive blend projectiles using IAdditiveDrawer.
            ILCursor cursor = new ILCursor(il);

            // Move before the place we want to inject code.
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchCallOrCallvirt<Main>("DrawInfernoRings")))
                return;
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchCall<ScreenObstruction>("Draw")))
                return;

            cursor.EmitDelegate<Action>(() =>
            {
                Main.spriteBatch.SetBlendState(BlendState.Additive);

                // Draw Projectiles.
                foreach (Projectile p in Main.ActiveProjectiles)
                {
                    if (p.ModProjectile is IAdditiveDrawer d)
                        d.AdditiveDraw(Main.spriteBatch);
                }

                // Draw NPCs.
                foreach (NPC n in Main.ActiveNPCs)
                {
                    if (n.ModNPC is IAdditiveDrawer d)
                        d.AdditiveDraw(Main.spriteBatch);
                }

                Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);
            });
        }
        #endregion

        #region General Particle Rendering
        private static void DrawFusableParticles(On_Main.orig_SortDrawCacheWorms orig, Main self)
        {
            DeathAshParticle.DrawAll();

            if (Main.LocalPlayer.dye.Any(dyeItem => dyeItem.type == ModContent.ItemType<ProfanedMoonlightDye>()))
                Main.LocalPlayer.Calamity().ProfanedMoonlightAuroraDrawer?.Draw(Main.LocalPlayer.Center - Main.screenPosition, false, Main.GameViewMatrix.TransformationMatrix, Matrix.Identity);

            orig(self);
        }

        #endregion

        #region Disable Moon Lord Style Flashes With Photosensitivity Config
        private static void DisableFlashesWithPhotosensitivityConfig(On_MoonlordDeathDrama.orig_RequestLight orig, float light, Vector2 spot)
        {
            // Disable this function from running if the Photosensitivity config is enabled.
            if (CalamityClientConfig.Instance.Photosensitivity)
                return;

            orig(light, spot);
        }
        #endregion

        #region Statue Additions
        private static void AddTwinklersToStatue(ILContext il)
        {
            // Allow Twinklers to be spawned by the Firefly Statue.
            var cursor = new ILCursor(il);

            // Move to the method which randomly selects an NPC for the Statue to spawn. This is the second call of the method in this function.
            for (int i = 0; i < 2; i++)
            {
                if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchCall(typeof(Utils), nameof(Utils.SelectRandom))))
                {
                    LogFailure("Add Twinkler to Firefly Statue", "Could not move to the SelectRandom method.");
                    return;
                }
            }

            // Emit a delegate which resizes the array and adds Twinkler to it.
            cursor.EmitDelegate<Func<short[], short[]>>(arr =>
            {
                Array.Resize(ref arr, arr.Length + 1);
                arr[arr.Length - 1] = (short)ModContent.NPCType<Twinkler>();
                return arr;
            });
        }
        #endregion

        #region Make Tax Collector Worth it
        private static void MakeTaxCollectorUseful(ILContext il)
        {
            ILCursor cursor = new(il);
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchCall<Item>(nameof(Item.buyPrice))))
            {
                LogFailure("Tax Collector Money Boosts", "Could not locate the amount of money to collect per town NPC.");
                return;
            }
            cursor.Emit(OpCodes.Pop);
            cursor.Emit<CalamityGlobalTownNPC>(OpCodes.Call, $"get_{nameof(CalamityGlobalTownNPC.TotalTaxesPerNPC)}");

            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchCall<Item>(nameof(Item.buyPrice))))
            {
                LogFailure("Tax Collector Money Boosts", "Could not locate the maximum amount of money to collect.");
                return;
            }
            cursor.Emit(OpCodes.Pop);
            cursor.Emit<CalamityGlobalTownNPC>(OpCodes.Call, $"get_{nameof(CalamityGlobalTownNPC.TaxesToCollectLimit)}");
        }
        #endregion

        #region Tile ping overlay
        private static void ClearTilePings(On_TileDrawing.orig_Draw orig, TileDrawing self, bool solidLayer, bool forRenderTargets, bool intoRenderTargets, int waterStyleOverride)
        {
            //Retro & Trippy light modes are fine. Just reset the cache before every time stuff gets drawn.
            if (Lighting.UpdateEveryFrame)
            {
                //But only if its not on the non solid layer, assumedly because it draws first or something
                if (!solidLayer)
                    TilePingerSystem.ClearTiles();
            }

            else
            {
                //For the white color mode, we also can simply clear all the cache at once, but this time its only on the solid layers. Don't ask me why i don't know it just works
                if (Lighting.Mode == LightMode.White)
                {
                    if (solidLayer)
                        TilePingerSystem.ClearTiles();
                }

                //In color mode, the tiles get cleared alternating between solid and non solid tiles
                else
                    TilePingerSystem.ClearTiles(solidLayer);

            }
            orig(self, solidLayer, forRenderTargets, intoRenderTargets, waterStyleOverride);
        }
        #endregion

        #region Custom Grappling hooks

        /// <summary>
        /// Determines if the custom grapple movement should take place or not. Useful for hooks that only do movement tricks in some cases
        /// </summary>
        private static void CustomGrappleMovementCheck(On_Player.orig_GrappleMovement orig, Player self)
        {
            WulfrumPackPlayer mp = self.GetModPlayer<WulfrumPackPlayer>();

            if (mp.GrappleMovementDisabled)
                return;

            orig(self);
        }

        /// <summary>
        /// This is called right before the game decides wether or not to update the players velocity based on "real" physics (aka not tongued or hooked or with a pulley)
        /// </summary>
        private static void CustomGrapplePreDefaultMovement(On_Player.orig_UpdatePettingAnimal orig, Player self)
        {
            orig(self);

            WulfrumPackPlayer mp = self.GetModPlayer<WulfrumPackPlayer>();
            mp.hookCache = -1;

            //if tongued, dnc.
            if (self.tongued)
                return;

            //Cache the player's grapple and remove it temporarily (Gets re added in the modplayer's PostUpdateRunSpeeds)
            if (self.grappling[0] >= 0 && mp.GrappleMovementDisabled && Main.projectile[self.grappling[0]].type == ModContent.ProjectileType<WulfrumHook>())
            {
                mp.hookCache = self.grappling[0];
                self.grappling[0] = -1;
                self.grapCount = 0;
            }
        }

        /// <summary>
        /// Used before the player steps up a half tile. If we don't do that, players that are grappled but don't use hook movement won't be able to go over tiles.
        /// The hook cache is reset in PreUpdateMovement
        /// </summary>
        private static void CustomGrapplePreStepUp(On_Player.orig_SlopeDownMovement orig, Player self)
        {
            orig(self);

            WulfrumPackPlayer mp = self.GetModPlayer<WulfrumPackPlayer>();
            if (self.grappling[0] >= 0 && mp.GrappleMovementDisabled && Main.projectile[self.grappling[0]].type == ModContent.ProjectileType<WulfrumHook>())
            {
                mp.hookCache = self.grappling[0];
                self.grappling[0] = -1;
                self.grapCount = 0;
            }
        }

        /// <summary>
        /// This is done to put the hook if it was cached during the frame instruction.
        /// </summary>
        private static void CustomGrapplePostFrame(On_Player.orig_PlayerFrame orig, Player self)
        {
            orig(self);
            WulfrumPackPlayer mp = self.GetModPlayer<WulfrumPackPlayer>();

            if (mp.hookCache > -1)
            {
                self.grappling[0] = mp.hookCache;
                self.grapCount = 1;
            }

            mp.hookCache = -1;
        }
        #endregion

        #region Scopes Require Visibility to Zoom
        private static void ScopesRequireVisibilityToZoom(ILContext il)
        {
            var cursor = new ILCursor(il);

            // Search for the only place in the function where Player.scope is set to true.
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchStfld<Player>("scope")))
            {
                LogFailure("Scopes Require Visibility to Zoom", "Could not locate where player is set to have a scope equipped.");
                return;
            }

            // Pop the useless 1 off the stack.
            cursor.Emit(OpCodes.Pop);

            // Load argument 2 (hideVisual bool) onto the stack.
            cursor.Emit(OpCodes.Ldarg_2);

            cursor.EmitDelegate<Func<bool, bool>>((x) => !x);

            // The next (untouched) instruction stores this value into Player.scope.
        }
        #endregion

        #region Custom world selection difficulties
        internal static void GetDifficultyOverride(On_AWorldListItem.orig_GetDifficulty orig, AWorldListItem self, out string expertText, out Color gameModeColor)
        {
            // Run the original code and pull out the original text and text color
            orig(self, out expertText, out gameModeColor);

            string difficultyText = expertText;
            Color difficultyColor = gameModeColor;

            // Journey Mode takes ultimate priority
            if (difficultyColor == Main.creativeModeColor)
            {
                return;
            }

            // Go through the World Selection Difficulty System's World Difficulty list backwards and choose the latest difficulty that applies
            for (int i = WorldSelectionDifficultySystem.WorldDifficulties.Count - 1; i >= 0; i--)
            {
                WorldSelectionDifficultySystem.WorldDifficulty d = WorldSelectionDifficultySystem.WorldDifficulties[i];
                {
                    if (d.function(self))
                    {
                        difficultyText = d.name;
                        difficultyColor = d.color;
                        break;
                    }
                }
            }

            // Set the text and text color
            expertText = difficultyText;
            gameModeColor = difficultyColor;
        }
        #endregion

        #region Shimmer effect edits
        public static void ShimmerEffectEdits(On_Item.orig_GetShimmered orig, Item self)
        {
            // Make Plagued Containment Bricks turn into Plagued Nanodroids if shimmered before defeating Golem
            if (self.type == ModContent.ItemType<PlaguedContainmentBrick>())
            {
                if (NPC.downedGolemBoss)
                    orig(self);
                else
                {
                    for (int i = 0; i < 3; i++)
                    {
                        int nanodroidType = Main.rand.NextBool() ? ModContent.NPCType<NanodroidPlagueGreen>() : ModContent.NPCType<NanodroidPlagueRed>();
                        NPC droids = NPC.NewNPCDirect(self.GetSource_FromThis(), (int)self.Center.X, (int)self.Center.Y, nanodroidType);
                        droids.velocity = -self.velocity.RotatedByRandom(MathHelper.Pi / 20f) * 2f;
                        droids.netUpdate = true;
                        droids.shimmerTransparency = 1f;
                        NetMessage.SendData(MessageID.ShimmerActions, -1, -1, null, 2, droids.whoAmI);
                    }

                    self.TurnToAir();
                    self.shimmerWet = true;
                    self.wet = true;
                    self.velocity *= 0.1f;
                    if (Main.netMode == NetmodeID.SinglePlayer)
                    {
                        Item.ShimmerEffect(self.Center);
                    }
                    else
                    {
                        NetMessage.SendData(MessageID.ShimmerActions, -1, -1, null, 0, (int)self.Center.X, (int)self.Center.Y);
                        NetMessage.SendData(MessageID.SyncItemsWithShimmer, -1, -1, null, self.whoAmI, 1f);
                    }
                }
            }
            else
            {
                orig(self);
            }
        }
        #endregion

        #region Block Abyss from Teleportation Potions

        public static void TPOverride(On_Player.orig_Teleport orig, Player self, Vector2 newPos, int Style = 0, int extraInfo = 0)
        {
            // Grab the tile from where the potion wants to teleport
            Tile t = CalamityUtils.ParanoidTileRetrieval(newPos.ToTileCoordinates().X, newPos.ToTileCoordinates().Y);
            // Check if it's a Teleportation Potion
            if (Style == 2)
            {
                // Check if it's an Abyss wall
                if (CalamityTileSets.IsAbyssWall[t.WallType])
                {
                    // If an Abyss wall is detected, try to find another teleportation location
                    bool canSpawn = false;
                    int teleportStartX = 100;
                    int teleportRangeX = Main.maxTilesX - 200;
                    int teleportStartY = 100;
                    int underworldLayer = Main.UnderworldLayer;
                    Vector2 newerPos = self.CheckForGoodTeleportationSpot(ref canSpawn, teleportStartX, teleportRangeX, teleportStartY, underworldLayer, new Player.RandomTeleportationAttemptSettings
                    {
                        avoidLava = true,
                        avoidHurtTiles = true,
                        maximumFallDistanceFromOrignalPoint = 100,
                        attemptsBeforeGivingUp = 1000
                    });
                    // Attempt teleporting again with the new location
                    orig(self, newerPos, Style, extraInfo);
                }
                else
                {
                    orig(self, newPos, Style, extraInfo);
                }
            }
            else
            {
                orig(self, newPos, Style, extraInfo);
                // Potion of Return triggers Jared if used in the Abyss
                if (Style == 8)
                {
                    if (CalamityTileSets.IsAbyssWall[t.WallType])
                        self.AddBuff(BuffID.ChaosState, 2);
                }
            }
        }
        #endregion

        #region Optimized GlowMask Rendering on Tile
        private static void GlowMaskTileRender(On_TileDrawing.orig_DrawSingleTile orig, TileDrawing self, TileDrawInfo drawData, bool solidLayer, int waterStyleOverride, Vector2 screenPosition, Vector2 screenOffset, int tileX, int tileY)
        {
            orig(self, drawData, solidLayer, waterStyleOverride, screenPosition, screenOffset, tileX, tileY);

            var type = drawData.typeCache;
            if (type >= GlowMaskTile.LookupLength)
                return;

            var glowMaskTile = GlowMaskTile.InstanceLookup[type];
            if (glowMaskTile is null || !TileDrawing.IsVisible(drawData.tileCache))
                return;

            var glowMask = glowMaskTile.GlowMask;
            var xPos = drawData.tileFrameX + drawData.addFrX;
            var yPos = drawData.tileFrameY + drawData.addFrY;
            if (glowMask.HasContentInFramePos(xPos, yPos))
            {
                ref Tile tileCache = ref drawData.tileCache;
                int colType = tileCache.TileColor;

                Color drawColor = glowMaskTile.GetGlowMaskColor(tileX, tileY, drawData);

                if (glowMaskTile.GlowMaskAffectedByLight)
                {
                    Color tileLight = drawData.tileLight;

                    if (tileLight.R > drawColor.R) drawColor.R = tileLight.R;
                    if (tileLight.G > drawColor.G) drawColor.G = tileLight.G;
                    if (tileLight.B > drawColor.B) drawColor.B = tileLight.B;
                }

                drawColor = glowMaskTile.GlowMaskPaintInteraction switch
                {
                    GlowMaskTile.PaintColorTint.OnlyByDeepPaint => CalamityUtils.ApplyPaint(colType, drawColor, deepPaintOnly: true),
                    GlowMaskTile.PaintColorTint.ByEveryPaint => CalamityUtils.ApplyPaint(colType, drawColor, deepPaintOnly: false),
                    _ => drawColor
                };

                if (glowMaskTile.GlowMaskCanBeCulled)
                {
                    // Cull no lit and too dark colors
                    if (drawColor.R <= 1 && drawColor.G <= 1 && drawColor.B <= 1)
                        return;
                }

                drawColor.A = 255;

                if (Main.tileSolid[type])
                {
                    var drawRect = new Rectangle(xPos, yPos, 16, 16);
                    TileFramingSystem.SlopedGlowmask(in tileCache, tileX, tileY, glowMask.Texture, drawRect, drawColor, default);
                }
                else
                {
                    Vector2 drawPos = new Vector2(tileX * 16, tileY * 16) - screenPosition + screenOffset;
                    Rectangle drawRect = new Rectangle(xPos, yPos, 16, 16);
                    Main.spriteBatch.Draw(glowMask.Texture, drawPos, drawRect, drawColor, 0.0f, default, 1.0f, drawData.tileSpriteEffect, 0.0f);
                }
            }
        }
        #endregion

        #region Allow Cannons to use jellyfish
        public static void AllowCannonJellyfishUse(On_Player.orig_PlaceThing_CannonBall orig, Player self)
        {
            // Check if the player is holding a jelly
            if (self.HeldItem.type == ModContent.ItemType<BabyCannonballJellyfishItem>())
            {
                // I have no comments here.
                bool veryLongRangeCheck = self.position.X / 16f - (float)Player.tileRangeX - (float)self.HeldItem.tileBoost - (float)self.blockRange <= (float)Player.tileTargetX
                    && (self.position.X + (float)self.width) / 16f + (float)Player.tileRangeX + (float)self.HeldItem.tileBoost - 1f + (float)self.blockRange >= (float)Player.tileTargetX
                    && self.position.Y / 16f - (float)Player.tileRangeY - (float)self.HeldItem.tileBoost - (float)self.blockRange <= (float)Player.tileTargetY
                    && (self.position.Y + (float)self.height) / 16f + (float)Player.tileRangeY + (float)self.HeldItem.tileBoost - 2f + (float)self.blockRange >= (float)Player.tileTargetY;

                int targX = Player.tileTargetX;
                int targY = Player.tileTargetY;

                Tile t = CalamityUtils.ParanoidTileRetrieval(targX, targY);

                // All vanilla cannon types (such as the Bunny Cannon) are a single tile ID, this lets us determine that this tile is the normal Cannon
                int cannonType = t.TileFrameX / 72;

                // If the player's target tile is within range, is a normal Cannon, they are using an item, and their item time is zero, shoot the baby
                if (t.TileType == TileID.Cannon && cannonType == 0 && self.ItemTimeIsZero && self.controlUseItem && veryLongRangeCheck)
                {
                    self.cursorItemIconEnabled = true;
                    self.cursorItemIconID = ModContent.ItemType<BabyCannonballJellyfishItem>();
                    self.HeldItem.makeNPC = -1; // Prevent it from spawning critters when used

                    // Determines where all the action should happen and what the angle of the cannon is
                    int tileX = t.TileFrameX / 18;
                    int angle = 0;
                    while (tileX >= 4)
                    {
                        tileX -= 4;
                    }
                    tileX = targX - tileX;
                    int tileY;
                    for (tileY = t.TileFrameY / 18; tileY >= 3; tileY -= 3)
                    {
                        angle++;
                    }
                    tileY = targY - tileY;

                    self.ApplyItemTime(self.HeldItem);

                    float speedX = 0f;
                    float speedY = 0f;

                    // Vanilla code for determining the velocity of shot cannonballs kept intact for consistency
                    if (angle == 0)
                    {
                        speedX = 10f;
                        speedY = 0f;
                    }
                    if (angle == 1)
                    {
                        speedX = 7.5f;
                        speedY = -2.5f;
                    }
                    if (angle == 2)
                    {
                        speedX = 5f;
                        speedY = -5f;
                    }
                    if (angle == 3)
                    {
                        speedX = 2.75f;
                        speedY = -6f;
                    }
                    if (angle == 4)
                    {
                        speedX = 0f;
                        speedY = -10f;
                    }
                    if (angle == 5)
                    {
                        speedX = -2.75f;
                        speedY = -6f;
                    }
                    if (angle == 6)
                    {
                        speedX = -5f;
                        speedY = -5f;
                    }
                    if (angle == 7)
                    {
                        speedX = -7.5f;
                        speedY = -2.5f;
                    }
                    if (angle == 8)
                    {
                        speedX = -10f;
                        speedY = 0f;
                    }
                    Vector2 spawnPosition = new Vector2((tileX + 2) * 16, (tileY + 2) * 16);
                    // Finally shoot the projectile
                    int jellyfishb = Projectile.NewProjectile(new EntitySource_TileInteraction(self, tileX, tileY), spawnPosition.X, spawnPosition.Y, speedX, speedY, ModContent.ProjectileType<BabyCannonballProjectile>(), self.HeldItem.damage, 8f, self.whoAmI);
                    Main.projectile[jellyfishb].originatedFromActivableTile = true;
                    // Shlorb
                    SoundEngine.PlaySound(SoundID.Item95, spawnPosition);
                }
                else
                {
                    // Reset the item to be able to spawn critters again if any of the conditions aren't met
                    self.HeldItem.makeNPC = ModContent.NPCType<BabyCannonballJellyfish>();
                }
            }
            else
                orig(self);
        }
        #endregion

        #region Hellscape for GlowMask ModPlants

        #region Tree Trunk / Cactus GlowMask
        private static void DrawTreeTrunkAndCactusGlowMask(On_TileDrawing.orig_DrawBasicTile orig, TileDrawing self, Vector2 screenPosition, Vector2 screenOffset, int tileX, int tileY, TileDrawInfo drawData, Rectangle normalTileRect, Vector2 normalTilePosition)
        {
            orig(self, screenPosition, screenOffset, tileX, tileY, drawData, normalTileRect, normalTilePosition);

            var type = drawData.typeCache;

            #region GlowMask Cactus
            if (type == TileID.Cactus)
            {
                var frameX = drawData.tileFrameX;
                var frameY = drawData.tileFrameY;
                var xPos = drawData.tileFrameX + drawData.addFrX;
                var yPos = drawData.tileFrameY + drawData.addFrY;

                WorldGen.GetCactusType(tileX, tileY, frameX, frameY, out var sandType);
                if (PlantLoader.Get<ModCactus>(TileID.Cactus, sandType) is GlowMaskCactus glowCacti)
                {
                    Vector2 drawPos = new Vector2(tileX * 16, tileY * 16) - screenPosition + screenOffset;
                    Rectangle drawRect = new Rectangle(xPos, yPos, 16, 16);

                    // Fruit Check
                    var isFruit = drawData.tileFrameX == 204 || drawData.tileFrameY == 202;
                    var textureToDraw = isFruit ? glowCacti.GetFruitGlowTexture() : glowCacti.GetGlowTexture();
                    var texture = textureToDraw?.Value;
                    if (texture is not null)
                        Main.spriteBatch.Draw(texture, drawPos, drawRect, glowCacti.GetGlowColor(tileX, tileY), 0.0f, default, 1.0f, drawData.tileSpriteEffect, 0.0f);
                }
                return;
            }
            #endregion

            #region GlowMask Tree Trunk
            else if (type == TileID.Trees)
            {
                var xPos = drawData.tileFrameX + drawData.addFrX;
                var yPos = drawData.tileFrameY + drawData.addFrY;

                WorldGen.GetTreeBottom(tileX, tileY, out var groundX, out var groundY);
                Tile tile = Main.tile[groundX, groundY];
                if (PlantLoader.Get<ModTree>(TileID.Trees, tile.TileType) is GlowMaskTree glowTree)
                {
                    Vector2 drawPos = new Vector2(tileX * 16, tileY * 16) - screenPosition + screenOffset;
                    Rectangle drawRect = new Rectangle(xPos, yPos, 16, 16);

                    var texture = glowTree.GetGlowTexture()?.Value;
                    if (texture is not null)
                        Main.spriteBatch.Draw(texture, drawPos, drawRect, glowTree.GetGlowColor(tileX, tileY), 0.0f, default, 1.0f, drawData.tileSpriteEffect, 0.0f);
                }
                return;
            }
            #endregion

            #region GlowMask Palm Tree Trunk
            else if (type == TileID.PalmTree)
            {
                var xPos = drawData.tileFrameX + drawData.addFrX;
                var yPos = drawData.tileFrameY + drawData.addFrY;

                WorldGen.GetTreeBottom(tileX, tileY, out var groundX, out var groundY);
                Tile tile = Main.tile[groundX, groundY];
                if (PlantLoader.Get<ModPalmTree>(TileID.PalmTree, tile.TileType) is GlowMaskPalmTree glowTree)
                {
                    Rectangle drawRect = new Rectangle(xPos, yPos, 16, 16);

                    var texture = glowTree.GetGlowTexture()?.Value;
                    if (texture is not null)
                        Main.spriteBatch.Draw(texture, normalTilePosition, drawRect, glowTree.GetGlowColor(tileX, tileY), 0.0f, default, 1.0f, drawData.tileSpriteEffect, 0.0f);
                }
                return;
            }
            #endregion
        }
        #endregion

        #region Disable Tree/Cactus Culling
        private static void DisableCullingForTreeAndCactus(ILContext il)
        {
            // Justification:
            //   Culling for Tree/Cactus should be disabled to simplify the GlowMask rendering on Plants
            //   Without this We would need to setup more ilchanges to properly gather draw position
            //   Furthermore we might need to clone the whole vanilla code for tree rendering
            //
            //   And disable culling for pitch black tree/cactus would not affect the performance that much as it's really niche case

            var cursor = new ILCursor(il);

            int visibleFlagIndex = 0;
            if (!cursor.TryGotoNext(MoveType.Before,
            [
                x => x.MatchLdloc(out visibleFlagIndex),
                x => x.MatchLdarg(out _),
                x => x.MatchLdfld(out _),
                x => x.MatchCallOrCallvirt<TileDrawing>("IsVisible")
            ]))
            {
                LogFailure("Disable Tree And Cactus Culling", "Unable to Locate IsVisible Call");
                return;
            }

            if (!cursor.TryGotoPrev(MoveType.Before, x => x.MatchBrfalse(out _)))
            {
                LogFailure("Disable Tree And Cactus Culling", "Unable to Locate Brfalse.s");
                return;
            }

            cursor.EmitLdarg1(); // TileDrawInfo drawInfo
            cursor.EmitDelegate((TileDrawInfo drawInfo) =>
            {
                var type = drawInfo.typeCache;
                return type == TileID.Cactus || type == TileID.Trees || type == TileID.PalmTree;
            });
            cursor.EmitLdloc(visibleFlagIndex);
            cursor.EmitOr(); // visible || isTree || isCactus || isPalmTree
            cursor.EmitStloc(visibleFlagIndex);
        }
        #endregion

        #region Tree Parts GlowMask
        private static void DrawTreeGlowMask(ILContext il)
        {
            var cursor = new ILCursor(il);

            if (!cursor.TryGotoNext(MoveType.After, x => x.MatchLdfld<Point>("X")))
            {
                LogFailure("GlowMask Tree Rendering", "Unable to Locate Ldfld for Point::X");
                return;
            }

            if (!cursor.Next.MatchStloc(out var xLocaIdx))
            {
                LogFailure("GlowMask Tree Rendering", "Unable to Locate Stloc Index for Point::X");
                return;
            }

            if (!cursor.TryGotoNext(MoveType.After, x => x.MatchLdfld<Point>("Y")))
            {
                LogFailure("GlowMask Tree Rendering", "Unable to Locate Ldfld for Point::Y");
                return;
            }

            if (!cursor.Next.MatchStloc(out var yLocaIdx))
            {
                LogFailure("GlowMask Tree Rendering", "Unable to Locate Stloc Index for Point::Y");
                return;
            }

            ApplyTreeGlowMaskSubParts<GlowMaskTree>(cursor, "GlowMaskTree-Top", "GetTreeTopTexture", xLocaIdx, yLocaIdx,
                (glowMaskTree, tileX, tileY) =>
            {
                return new GlowMaskPlantDrawInfo()
                {
                    Texture = glowMaskTree.GetTopGlowTextures().Value,
                    Color = glowMaskTree.GetGlowColor(tileX, tileY)
                };
            });

            ApplyTreeGlowMaskSubParts<GlowMaskTree>(cursor, "GlowMaskTree-R", "GetTreeBranchTexture", xLocaIdx, yLocaIdx,
                (glowMaskTree, tileX, tileY) =>
            {
                return new GlowMaskPlantDrawInfo()
                {
                    Texture = glowMaskTree.GetBranchGlowTextures().Value,
                    Color = glowMaskTree.GetGlowColor(tileX, tileY)
                };
            });

            ApplyTreeGlowMaskSubParts<GlowMaskTree>(cursor, "GlowMaskTree-L", "GetTreeBranchTexture", xLocaIdx, yLocaIdx,
                (glowMaskTree, tileX, tileY) =>
            {
                return new GlowMaskPlantDrawInfo()
                {
                    Texture = glowMaskTree.GetBranchGlowTextures().Value,
                    Color = glowMaskTree.GetGlowColor(tileX, tileY)
                };
            });

            ApplyPalmTreeGlowMaskSubParts<GlowMaskPalmTree>(cursor, "GlowMaskPalmTree-Top", "GetTreeTopTexture", xLocaIdx, yLocaIdx,
                (glowMaskPalmTree, tileX, tileY) =>
            {
                return new GlowMaskPlantDrawInfo()
                {
                    Texture = WorldGen.IsPalmOasisTree(tileX) ?
                        glowMaskPalmTree.GetOasisTopGlowTextures().Value :
                        glowMaskPalmTree.GetTopGlowTextures().Value,

                    Color = glowMaskPalmTree.GetGlowColor(tileX, tileY)
                };
            });
        }
        #endregion

        #region GlowMask Patch Tree Sub Parts
        private static void ApplyTreeGlowMaskSubParts<PlantType>(
            ILCursor cursor,
            string debugSubpartName,
            string getTextureMethodName,
            int xLocaIdx,
            int yLocaIdx,
            Func<PlantType, int, int, GlowMaskPlantDrawInfo?> onPlantDrawn) where PlantType : IPlant
        {
            if (!cursor.TryGotoNext(MoveType.Before, x => x.MatchCallOrCallvirt<TileDrawing>(getTextureMethodName)))
            {
                LogFailure("GlowMask Tree Rendering", $"Could not locate First {getTextureMethodName} call ({debugSubpartName})");
                return;
            }

            if (!cursor.TryGotoPrev([x => x.MatchLdcI4(out var styleEq) && styleEq == 14, x => x.MatchBneUn(out _)]))
            {
                LogFailure("GlowMask Tree Rendering", $"Could not locate Bne.Un for TreeStyleIndex ({debugSubpartName})");
                return;
            }

            if (!cursor.Prev.MatchLdloc(out var treeStyleLocaIdx))
            {
                LogFailure("GlowMask Tree Rendering", $"Prev Instruction is not a LdLoc. Cannot locate TreeStyleIndex ({debugSubpartName})");
                return;
            }

            if (!cursor.TryGotoNext(MoveType.Before, x => x.MatchCallOrCallvirt<SpriteBatch>("Draw")))
            {
                LogFailure("GlowMask Tree Rendering", $"Could not locate DrawCall ({debugSubpartName})");
                return;
            }

            cursor.EmitLdloc(treeStyleLocaIdx); // TreeStyle
            cursor.EmitLdloc(xLocaIdx);
            cursor.EmitLdloc(yLocaIdx);
            cursor.EmitDelegate((
                SpriteBatch spriteBatch,
                Texture2D texture,
                Vector2 position,
                Rectangle? sourceRectangle,
                Color color,
                float rotation,
                Vector2 origin,
                float scale,
                SpriteEffects effects,
                float layerDepth,

                int style,
                int tileX,
                int tileY) =>
            {
                spriteBatch.Draw(texture, position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth);

                // Spooky Hardcoded Index: style above 100 is modded tile
                if (style < 100)
                    return;

                int lookup = style - 100;
                if (PlantLoader.Get<ModTree>(TileID.Trees, lookup) is not PlantType plant)
                    return;

                GlowMaskPlantDrawInfo? drawInfo = onPlantDrawn?.Invoke(plant, tileX, tileY) ?? default;
                if (drawInfo.HasValue)
                {
                    var info = drawInfo.Value;
                    spriteBatch.Draw(info.Texture, position, sourceRectangle, info.Color, rotation, origin, scale, effects, layerDepth);
                }
            });
            cursor.Next.OpCode = OpCodes.Nop; // remove next drawcall
        }
        #endregion

        #region GlowMask Patch Palm Tree Sub Parts
        private static void ApplyPalmTreeGlowMaskSubParts<PlantType>(
            ILCursor cursor,
            string debugSubpartName,
            string getTextureMethodName,
            int xLocaIdx,
            int yLocaIdx,
            Func<PlantType, int, int, GlowMaskPlantDrawInfo?> onPlantDrawn)
        {
            if (!cursor.TryGotoNext(MoveType.Before, x => x.MatchCallOrCallvirt<TileDrawing>(getTextureMethodName)))
            {
                LogFailure("GlowMask Tree Rendering", $"Could not locate First {getTextureMethodName} call ({debugSubpartName})");
                return;
            }

            if (!cursor.TryGotoNext(MoveType.Before, x => x.MatchCallOrCallvirt<SpriteBatch>("Draw")))
            {
                LogFailure("GlowMask Tree Rendering", $"Could not locate DrawCall ({debugSubpartName})");
                return;
            }

            cursor.EmitLdloc(xLocaIdx);
            cursor.EmitLdloc(yLocaIdx);
            cursor.EmitDelegate((
                SpriteBatch spriteBatch,
                Texture2D texture,
                Vector2 position,
                Rectangle? sourceRectangle,
                Color color,
                float rotation,
                Vector2 origin,
                float scale,
                SpriteEffects effects,
                float layerDepth,

                int tileX,
                int tileY) =>
            {
                spriteBatch.Draw(texture, position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth);

                // TODO: This can be replace to read style and biome index from ilcode
                // ... When we can figure out how to extract info from them
                // Check TileDrawing.DrawTrees for context
                WorldGen.GetTreeBottom(tileX, tileY, out var sandX, out var sandY);
                var sandType = Main.tile[sandX, sandY].TileType;

                if (PlantLoader.Get<ModPalmTree>(TileID.PalmTree, sandType) is not PlantType plant)
                    return;

                GlowMaskPlantDrawInfo? drawInfo = onPlantDrawn?.Invoke(plant, tileX, tileY) ?? default;
                if (drawInfo.HasValue)
                {
                    var info = drawInfo.Value;
                    spriteBatch.Draw(info.Texture, position, sourceRectangle, info.Color, rotation, origin, scale, effects, layerDepth);
                }
            });
            cursor.Next.OpCode = OpCodes.Nop; // remove next drawcall
        }
        #endregion

        #endregion

        #region Make Celestial Onion give the Master Mode slot
        public static bool MasterModeCelestialOnionCheck(On_Player.orig_IsItemSlotUnlockedAndUsable orig, Player self, int slot)
        {
            if ((slot == 9 || slot == 19) && self.Calamity().extraAccessoryML && !Main.gameMenu)
            {
                return true;
            }
            return orig(self, slot);
        }
        #endregion

        #region Allow Grappling Hooks to grab arena walls
        public static void AllowHooksToGrabArenabox(On_Projectile.orig_AI_007_GrapplingHooks orig, Projectile self)
        {
            if (Main.player[self.owner].dead || Main.player[self.owner].stoned || Main.player[self.owner].webbed || Main.player[self.owner].frozen)
            {
                self.Kill();
                return;
            }
            static bool Vector2PointCollision(Vector2 position, Vector2 size, Vector2 point)
            {
                return (point.X >= position.X && point.X <= position.X + size.X && point.Y >= position.Y && point.Y <= position.Y + size.Y);
            }

            bool intersectingWall = false;
            if (self.Calamity().arenaBox is not null)
            {
                var box = self.Calamity().arenaBox;
                if (ArenaWallSystem.ActiveBoxes.Contains(box))
                {
                    self.Center = box.TopLeft + box.Size * self.Calamity().arenaBoxPosition;
                }
                else
                {
                    self.Calamity().arenaBox = null;
                }
            }
            if (ArenaWallSystem.ActiveBoxes.Count > 0)
            {
                foreach (var box in ArenaWallSystem.ActiveBoxes)
                {
                    if (!Vector2PointCollision(box.TopLeft, box.Size, self.Center) && Vector2PointCollision(box.TopLeft - new Vector2(box.borderThickness), box.Size + new Vector2(box.borderThickness) * 2, self.Center))
                    {
                        if (self.ai[0] == 0)
                        {
                            if (Main.myPlayer == self.owner)
                            {
                                if (self.type == ProjectileID.QueenSlimeHook)
                                {
                                    Main.player[self.owner].DoQueenSlimeHookTeleport(self.Center);
                                }
                                NetMessage.SendData(MessageID.PlayerControls, -1, -1, null, self.owner);
                            }
                            SoundEngine.PlaySound(SoundID.DD2_EtherianPortalSpawnEnemy with { Volume = 0.75f }, self.Center);
                            self.ai[0] = 2;
                            self.velocity = Vector2.Zero;
                            self.Calamity().arenaBoxPosition = new Vector2(Utils.Remap(self.Center.X, box.TopLeft.X, box.BottomRight.X, 0, 1, false), Utils.Remap(self.Center.Y, box.TopLeft.Y, box.BottomRight.Y, 0, 1, false));
                            self.Calamity().arenaBox = box;
                        }
                        if (self.ai[0] == 2) //don't run this if the hook is returning!
                        {
                            self.rotation = self.DirectionFrom(Main.player[self.owner].Center).ToRotation() + MathHelper.PiOver2;
                            intersectingWall = true;

                            if (Main.player[self.owner].grapCount < 10)
                            {
                                Main.player[self.owner].grappling[Main.player[self.owner].grapCount] = self.whoAmI;
                                Main.player[self.owner].grapCount++;
                            }
                        }
                    }
                }
            }
            if (!intersectingWall)
                orig(self);
        }
        #endregion

        #region Arena Collision for other things

        private static bool ArenaCollision_Vector2_int_int_bool(On_Collision.orig_SolidCollision_Vector2_int_int_bool orig, Vector2 Position, int Width, int Height, bool acceptTopSurfaces)
        {
            if (ArenaWallSystem.ActiveBoxes.Count > 0)
            {
                foreach (var item in ArenaWallSystem.ActiveBoxes)
                {
                    if (item.Vector2PairInWall(Position, new(Width, Height)))
                        return true;
                }
            }
            return orig(Position, Width, Height, acceptTopSurfaces);
        }

        private static bool ArenaCollision_Vector2_int_int(On_Collision.orig_SolidCollision_Vector2_int_int orig, Vector2 Position, int Width, int Height)
        {
            if (ArenaWallSystem.ActiveBoxes.Count > 0)
            {
                foreach (var item in ArenaWallSystem.ActiveBoxes)
                {
                    if (item.Vector2PairInWall(Position, new(Width, Height)))
                        return true;
                }
            }
            return orig(Position, Width, Height);
        }


        private static Vector2 ArenaCollision_TileCollision(On_Collision.orig_TileCollision orig, Vector2 Position, Vector2 Velocity, int Width, int Height, bool fallThrough, bool fall2, int gravDir)
        {
            Velocity = orig(Position, Velocity, Width, Height, fallThrough, fall2, gravDir);
            if (ArenaWallSystem.ActiveBoxes.Count > 0 && Velocity != Vector2.Zero)
            {
                foreach (var item in ArenaWallSystem.ActiveBoxes)
                {
                    var oldVel = Velocity;
                    if (item.InnerEffect(Position, new Vector2(Width, Height)))
                        Velocity = ArenaCollisionLogic(item, Position, Width, Height, Velocity);
                    var dif = (oldVel - Velocity).Length();
                }
            }
            return Velocity;
        }

        private static Vector2 ArenaCollisionLogic(ArenaWallSystem.Box box, Vector2 Position, int Width, int Height, Vector2 Velocity)
        {
            var originalVelocity = Velocity;
            var originalTopLeft = Position;
            var originalBottomRight = Position + new Vector2(Height, Width);
            Position += originalVelocity;

            if (Position.X < box.TopLeft.X)
            {
                Velocity.X = box.TopLeft.X - originalTopLeft.X;
            }
            if (Position.X + Width > box.BottomRight.X)
            {
                Velocity.X = box.BottomRight.X - originalBottomRight.X;
            }
            if (Position.Y < box.TopLeft.Y)
            {
                Velocity.Y = box.TopLeft.Y - originalTopLeft.Y;
            }

            if (Position.Y + Height > box.BottomRight.Y)
            {
                Velocity.Y = box.BottomRight.Y - originalBottomRight.Y;
            }
            return Velocity;
        }
        #endregion
    }
}
