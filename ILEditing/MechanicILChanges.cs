using CalamityMod.Balancing;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer;
using CalamityMod.Cooldowns;
using CalamityMod.FluidSimulation;
using CalamityMod.Items.Dyes;
using CalamityMod.NPCs.Astral;
using CalamityMod.NPCs.AstrumAureus;
using CalamityMod.NPCs.Crabulon;
using CalamityMod.NPCs.Ravager;
using CalamityMod.Particles;
using CalamityMod.Projectiles;
using CalamityMod.Systems;
using CalamityMod.Waters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using ReLogic.Content;
using System;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Liquid;
using Terraria.GameInput;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Gamepad;
using Terraria.Utilities;

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

        // Holds the vanilla game function which spawns town NPCs, wrapped in a delegate for reflection purposes.
        // This function is (optionally) invoked manually in an IL edit to enable NPCs to spawn at night.
        private static Action VanillaSpawnTownNPCs;

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
            cursor.EmitDelegate<Action<Player, int>>((p, frames) => CalamityUtils.GiveIFrames(p, frames, false));
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

            //
            // SHIELD OF CTHULHU
            //

            // Move to Shield of Cthulhu's code by finding its function call for iframes.
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchCall<Player>("GiveImmuneTimeForCollisionAttack")))
            {
                LogFailure("Vanilla Dash Fixes", "Could not locate function call for Shield of Cthulhu iframes.");
                return;
            }

            if (!cursor.TryGotoPrev(MoveType.AfterLabel, i => i.MatchLdcI4(30)))
            {
                LogFailure("Vanilla Dash Fixes", "Could not locate amount of frames of dash cooldown applied on impact with Shield of Cthulhu.");
                return;
            }

            // Remove the instruction and replace it with one which gives Calamity's (customizable) amount of dash cooldown.
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_I4, BalancingConstants.OnShieldBonkCooldown);

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

            // Now that the new base damage has been applied to the direct contact strike, also apply it to the Solar Counter projectile.
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcI4(150)))
            {
                LogFailure("Vanilla Dash Fixes", "Could not locate Solar Flare Armor \"Solar Counter\" base damage.");
                return;
            }

            // Replace vanilla's flat 150 damage (doesn't even scale with melee stats!) with the already-calculated base damage, then cast it to int.
            cursor.Remove();
            cursor.Emit(OpCodes.Ldloc, 13);
            cursor.Emit(OpCodes.Conv_I4);

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
        #endregion Dash Fixes and Improvements

        #region Enabling of Triggered NPC Platform Fallthrough
        // Why this isn't a mechanism provided by TML itself or vanilla itself is beyond me.
        private static void AllowTriggeredFallthrough(On.Terraria.NPC.orig_ApplyTileCollision orig, NPC self, bool fall, Vector2 cPosition, int cWidth, int cHeight)
        {
            if (self.active && self.type == ModContent.NPCType<FusionFeeder>())
            {
                self.velocity = Collision.AdvancedTileCollision(TileID.Sets.ForAdvancedCollision.ForSandshark, cPosition, self.velocity, cWidth, cHeight, fall, fall, 1);
                return;
            }

            if (self.active && self.Calamity().ShouldFallThroughPlatforms)
                fall = true;
            orig(self, fall, cPosition, cWidth, cHeight);
        }
        #endregion Enabling of Triggered NPC Platform Fallthrough

        #region Town NPC Spawning Improvements
        private static void PermitNighttimeTownNPCSpawning(ILContext il)
        {
            // Don't do town NPC spawning at the end (which lies after a !Main.dayTime return).
            // Do it at the beginning, without the arbitrary time restriction.
            var cursor = new ILCursor(il);
            cursor.EmitDelegate<Action>(() =>
            {
                // A cached delegate is used here instead of direct reflection for performance reasons
                // since UpdateTime is called every frame.
                if (Main.dayTime || CalamityConfig.Instance.TownNPCsSpawnAtNight)
                    VanillaSpawnTownNPCs();
            });

            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchCallOrCallvirt<Main>("UpdateTime_SpawnTownNPCs")))
            {
                CalamityMod.Instance.Logger.Warn("Town NPC spawn editing code failed.");
                return;
            }

            cursor.Emit(OpCodes.Ret);
        }

        private static void AlterTownNPCSpawnRate(On.Terraria.Main.orig_UpdateTime_SpawnTownNPCs orig)
        {
            double oldWorldRate = Main.desiredWorldTilesUpdateRate;
            Main.desiredWorldTilesUpdateRate *= CalamityConfig.Instance.TownNPCSpawnRateMultiplier;
            orig();
            Main.desiredWorldTilesUpdateRate = oldWorldRate;
        }
        #endregion Town NPC Spawning Improvements

        #region Removal of Dodge RNG
        private static readonly Func<Player, int> CalamityDodgeAvailable = (Player p) =>
        {
            CalamityPlayer mp = p.Calamity();
            // If your dodges are universally disabled, then they simply "never come off cooldown" and always have 1 frame left.
            if (mp.disableAllDodges)
                return 1;

            bool dodgeCooldownActive = p.HasCooldown(GlobalDodge.ID);
            return dodgeCooldownActive ? 1 : 0;
        };

        private static void RemoveRNGFromDodges(ILContext il)
        {
            //
            // BLACK BELT
            //

            // Change the random chance of the Black Belt to 100%, but don't let it work if Calamity's cooldown is active.
            var cursor = new ILCursor(il);
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcI4(10))) // 1 in 10 Main.rand call for Black Belt activation.
            {
                LogFailure("No RNG Black Belt", "Could not locate the dodge chance.");
                return;
            }
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_I4_1); // Replace with Main.rand.Next(1), aka 100% chance.

            // Move forwards past the Main.rand.Next call now that it has been edited.
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchCallvirt<UnifiedRandom>("Next")))
            {
                LogFailure("No RNG Black Belt", "Could not locate the Random.Next call.");
                return;
            }

            // Load the player itself onto the stack so that it becomes an argument for the following delegate.
            cursor.Emit(OpCodes.Ldarg_0);

            // Emit a delegate which places the player's dodge availability onto the stack.
            // This is typically the Calamity dodge cooldown -- zero lets you dodge, anything else doesn't.
            cursor.EmitDelegate<Func<Player, int>>(CalamityDodgeAvailable);

            // Bitwise OR the "RNG result" (always zero) with the dodge cooldown. This will only return zero if both values were zero.
            // The code path which calls NinjaDodge can ONLY occur if the result of this operation is zero,
            // because it is now the value checked by the immediately following branch-if-true.
            cursor.Emit(OpCodes.Or);

            // Move forwards past the NinjaDodge call. We need to set the dodge cooldown here.
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchCall<Player>("NinjaDodge")))
            {
                LogFailure("No RNG Black Belt", "Could not locate the Player.NinjaDodge call.");
                return;
            }

            // Load the player itself onto the stack so that it becomes an argument for the following delegate.
            cursor.Emit(OpCodes.Ldarg_0);

            // Emit a delegate which sets the player's Calamity dodge cooldown and sends a sync packet appropriately.
            cursor.EmitDelegate<Action<Player>>((Player p) => p.AddCooldown(GlobalDodge.ID, BalancingConstants.BeltDodgeCooldown));

            //
            // BRAIN OF CONFUSION
            //

            // Change the random chance of the Brain of Confusion to 100%, but don't let it work if Calamity's cooldown is active.
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcI4(6))) // 1 in 6 Main.rand call for Brain of Confusion activation.
            {
                LogFailure("No RNG Brain of Confusion", "Could not locate the dodge chance.");
                return;
            }
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_I4_1); // Replace with Main.rand.Next(1), aka 100% chance.

            // Move forwards past the Main.rand.Next call now that it has been edited.
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchCallvirt<UnifiedRandom>("Next")))
            {
                LogFailure("No RNG Brain of Confusion", "Could not locate the Random.Next call.");
                return;
            }

            // Load the player itself onto the stack so that it becomes an argument for the following delegate.
            cursor.Emit(OpCodes.Ldarg_0);

            // Emit a delegate which places the player's dodge availability onto the stack.
            // This is typically the Calamity dodge cooldown -- zero lets you dodge, anything else doesn't.
            cursor.EmitDelegate<Func<Player, int>>(CalamityDodgeAvailable);

            // Bitwise OR the "RNG result" (always zero) with the dodge cooldown. This will only return zero if both values were zero.
            // The code path which calls BrainOfConfusionDodge can ONLY occur if the result of this operation is zero,
            // because it is now the value checked by the immediately following branch-if-true.
            cursor.Emit(OpCodes.Or);

            // Move forwards past the BrainOfConfusionDodge call. We need to set the dodge cooldown here.
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchCall<Player>("BrainOfConfusionDodge")))
            {
                LogFailure("No RNG Brain of Confusion", "Could not locate the Player.BrainOfConfusionDodge call.");
                return;
            }

            // Load the player itself onto the stack so that it becomes an argument for the following delegate.
            cursor.Emit(OpCodes.Ldarg_0);

            // Emit a delegate which sets the player's Calamity dodge cooldown and sends a sync packet appropriately.
            cursor.EmitDelegate<Action<Player>>((Player p) => p.AddCooldown(GlobalDodge.ID, BalancingConstants.BrainDodgeCooldown));
        }
        #endregion Removal of Dodge RNG

        #region Custom Gate Door Logic
        private static bool OpenDoor_LabDoorOverride(On.Terraria.WorldGen.orig_OpenDoor orig, int i, int j, int direction)
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

        private static bool CloseDoor_LabDoorOverride(On.Terraria.WorldGen.orig_CloseDoor orig, int i, int j, bool forced)
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
        #endregion Custom Gate Door Logic

        #region Platform Collision Checks for Grounded Bosses
        private static bool EnableCalamityBossPlatformCollision(On.Terraria.NPC.orig_Collision_DecideFallThroughPlatforms orig, NPC self)
        {
            if ((self.type == ModContent.NPCType<AstrumAureus>() || self.type == ModContent.NPCType<Crabulon>() || self.type == ModContent.NPCType<RavagerBody>() ||
                self.type == ModContent.NPCType<RockPillar>() || self.type == ModContent.NPCType<FlamePillar>()) &&
                self.target >= 0 && Main.player[self.target].position.Y > self.position.Y + self.height)
                return true;

            return orig(self);
        }
        #endregion Platform Collision Checks for Grounded Bosses

        #region Teleporter Disabling During Boss Fights
        private static void DisableTeleporters(On.Terraria.Wiring.orig_Teleport orig)
        {
            if (CalamityPlayer.areThereAnyDamnBosses)
                return;

            orig();
        }
        #endregion Teleporter Disabling During Boss Fights

        #region Incorporate Enchantments in Item Names
        private static string IncorporateEnchantmentInAffix(On.Terraria.Item.orig_AffixName orig, Item self)
        {
            string result = orig(self);
            if (!self.IsAir && self.Calamity().AppliedEnchantment.HasValue)
                result = $"{self.Calamity().AppliedEnchantment.Value.Name} {result}";
            return result;
        }
        #endregion Incorporate Enchantments in Item Names

        #region Hellbound Enchantment Projectile Creation Effects
        private static int IncorporateMinionExplodingCountdown(On.Terraria.Projectile.orig_NewProjectile_IEntitySource_float_float_float_float_int_int_float_int_float_float orig, IEntitySource spawnSource, float x, float y, float xSpeed, float ySpeed, int type, int damage, float knockback, int owner, float ai0, float ai1)
        {
            // This is unfortunately not something that can be done via SetDefaults since owner is set
            // after that method is called. Doing it directly when the projectile is spawned appears to be the only reasonable way.
            int proj = orig(spawnSource, x, y, xSpeed, ySpeed, type, damage, knockback, owner, ai0, ai1);
            Projectile projectile = Main.projectile[proj];
            if (projectile.minion)
            {
                Player player = Main.player[projectile.owner];
                CalamityPlayer.EnchantHeldItemEffects(player, player.Calamity(), player.ActiveItem());
                if (player.Calamity().explosiveMinionsEnchant)
                    projectile.Calamity().ExplosiveEnchantCountdown = CalamityGlobalProjectile.ExplosiveEnchantTime;
            }
            return proj;
        }
        #endregion Hellbound Enchantment Projectile Creation Effects

        #region Mana Sickness Replacement for Chaos Stone
        private static void ConditionallyReplaceManaSickness(ILContext il)
        {
            ILCursor cursor = new ILCursor(il);

            // Start by finding the vanilla code which applies Mana Sickness (buff ID 94).
            if (!cursor.TryGotoNext(c => c.MatchLdcI4(BuffID.ManaSickness)))
            {
                LogFailure("Conditionally Replace Mana Sickness", "Could not locate the mana sickness buff ID.");
                return;
            }

            // Remove the constant buff ID.
            cursor.Remove();

            // Load the player onto the stack for use in the following delegate.
            cursor.Emit(OpCodes.Ldarg_0);

            // Emit code which checks for the Chaos Stone. If equipped, the player gets Mana Burn instead of Mana Sickness.
            cursor.EmitDelegate<Func<Player, int>>(player =>
            {
                if (!player.active || !player.Calamity().ChaosStone)
                    return BuffID.ManaSickness;
                return ModContent.BuffType<ManaBurn>();
            });
        }
        #endregion Mana Sickness Replacement for Chaos Stone

        #region Fire Cursor Effect for the Calamity Accessory
        private static void UseCoolFireCursorEffect(On.Terraria.Main.orig_DrawCursor orig, Vector2 bonus, bool smart)
        {
            // Do nothing special if the player has a regular mouse or is on the menu.
            if (Main.gameMenu || !Main.LocalPlayer.Calamity().blazingCursorVisuals)
            {
                orig(bonus, smart);
                return;
            }

            if (Main.LocalPlayer.dead)
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
                if (Main.LocalPlayer.Calamity().blazingCursorDamage && !Main.mapFullscreen)
                {
                    int size = 450;
                    FluidFieldManager.AdjustSizeRelativeToGraphicsQuality(ref size);

                    float scale = MathHelper.Max(Main.screenWidth, Main.screenHeight) / size;
                    ref FluidField calamityFireDrawer = ref Main.LocalPlayer.Calamity().CalamityFireDrawer;
                    ref Vector2 firePosition = ref Main.LocalPlayer.Calamity().FireDrawerPosition;
                    if (calamityFireDrawer is null || calamityFireDrawer.Size != size)
                        calamityFireDrawer = FluidFieldManager.CreateField(size, scale, 0.1f, 50f, 0.992f);

                    // Update the fire draw position.
                    firePosition = new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f;

                    int x = (int)((drawPosition.X - firePosition.X) / calamityFireDrawer.Scale);
                    int y = (int)((drawPosition.Y - firePosition.Y) / calamityFireDrawer.Scale);
                    int horizontalArea = (int)Math.Ceiling(5f / calamityFireDrawer.Scale);
                    int verticalArea = (int)Math.Ceiling(5f / calamityFireDrawer.Scale);

                    calamityFireDrawer.ShouldUpdate = true;
                    calamityFireDrawer.UpdateAction = () =>
                    {
                        Color color = Color.Lerp(Color.Red, Color.Orange, (float)Math.Sin(Main.GlobalTimeWrappedHourly * 6f) * 0.5f + 0.5f);

                        // Use a rainbow color if the player has the rainbow cursor equipped as well as Calamity.
                        if (Main.LocalPlayer.hasRainbowCursor)
                            color = Color.Lerp(color, Main.hslToRgb(Main.GlobalTimeWrappedHourly * 0.97f % 1f, 1f, 0.6f), 0.75f);

                        for (int i = -horizontalArea; i <= horizontalArea; i++)
                        {
                            float offsetAngle = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 6.7f) * 0.99f;
                            offsetAngle += i / (float)horizontalArea * 0.34f;
                            Vector2 velocity = Main.MouseWorld - UIManagementSystem.PreviousMouseWorld;
                            if (velocity.Length() < 64f)
                            {
                                offsetAngle *= 0.5f;
                                velocity = Vector2.Zero;
                            }
                            UIManagementSystem.PreviousMouseWorld = Main.MouseWorld;

                            velocity = velocity.SafeNormalize(-Vector2.UnitY).RotatedBy(offsetAngle) * 0.2f;

                            // Add a tiny bit of randomness to the velocity.
                            // Chaos in the advection calculations should result in different flames being made over time, instead of a
                            // static animation.
                            velocity *= Main.rand.NextFloat(0.9f, 1.1f);

                            for (int j = -verticalArea; j <= verticalArea; j++)
                                Main.LocalPlayer.Calamity().CalamityFireDrawer.CreateSource(x + size / 2 + i, y + size / 2 + j, 1f, color, velocity);
                        }
                    };

                    calamityFireDrawer.Draw(firePosition, true, Main.UIScaleMatrix, Main.UIScaleMatrix);
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
        #endregion Fire Cursor Effect for the Calamity Accessory

        #region General Particle Rendering
        private static void DrawGeneralParticles(On.Terraria.Main.orig_DrawInterface orig, Main self, GameTime gameTime)
        {
            GeneralParticleHandler.DrawAllParticles(Main.spriteBatch);
            DeathAshParticle.DrawAll();

            if (Main.LocalPlayer.dye.Count(dyeItem => dyeItem.type == ModContent.ItemType<ProfanedMoonlightDye>()) > 0)
                Main.LocalPlayer.Calamity().ProfanedMoonlightAuroraDrawer?.Draw(Main.LocalPlayer.Center - Main.screenPosition, false, Main.GameViewMatrix.TransformationMatrix, Matrix.Identity);

            orig(self, gameTime);
        }

        private static void DrawFusableParticles(On.Terraria.Main.orig_SortDrawCacheWorms orig, Main self)
        {
            FusableParticleManager.RenderAllFusableParticles();

            orig(self);
        }

        #endregion General Particle Rendering

        #region Custom Lava Visuals
        private static void ResetRenderTargetSizes(On.Terraria.Main.orig_SetDisplayMode orig, int width, int height, bool fullscreen)
        {
            if (FusableParticleManager.HasBeenFormallyDefined)
                FusableParticleManager.LoadParticleRenderSets(true, width, height);
            orig(width, height, fullscreen);
        }

        private static void DrawCustomLava(ILContext il)
        {
            ILCursor cursor = new ILCursor(il);

            FieldInfo liquidTexturesField = typeof(TextureAssets).GetField("Liquid");
            FieldInfo liquidSlopeTexturesField = typeof(TextureAssets).GetField("LiquidSlope");
            MethodInfo textureGetValueMethod = typeof(Asset<Texture2D>).GetMethod("get_Value");

            void replaceLiquidTexture(LiquidTileType type)
            {
                // While this may seem crazy, under no circumstances should there not be a load after exactly 3 instructions.
                // The order is load is texture array field -> load index -> load the reference to the texture at that index -> call get_Value().
                cursor.Index += 4;
                cursor.EmitDelegate<Func<Texture2D, Texture2D>>(initialTexture => SelectLavaTexture(initialTexture, type));
            }

            void replaceLiquidColor(bool sloped)
            {
                // Pass the texture in so that the method can ensure it is not messing around with non-lava textures.
                cursor.Emit(OpCodes.Ldsfld, sloped ? liquidSlopeTexturesField : liquidTexturesField);
                cursor.Emit(OpCodes.Ldarg, 4);
                cursor.Emit(OpCodes.Ldelem_Ref);
                cursor.Emit(OpCodes.Callvirt, textureGetValueMethod);
                cursor.EmitDelegate<Func<Color, Texture2D, Color>>((initialColor, initialTexture) => SelectLavaColor(initialTexture, initialColor));
            }

            // Replace initial textures and colors.
            if (!cursor.TryGotoNext(c => c.MatchLdsfld(liquidTexturesField)))
            {
                LogFailure("Custom Lava Drawing", "Could not locate the liquid texture array load.");
                return;
            }
            replaceLiquidTexture(LiquidTileType.Waterflow);

            if (!cursor.TryGotoNext(MoveType.After, c => c.MatchLdarg(5)))
            {
                LogFailure("Custom Lava Drawing", "Could not locate the liquid light color.");
                return;
            }
            replaceLiquidColor(false);

            // Replace sloped textures and colors.
            for (int i = 0; i < 4; i++)
            {
                if (!cursor.TryGotoNext(c => c.MatchLdsfld(liquidSlopeTexturesField)))
                {
                    LogFailure("Custom Lava Drawing", "Could not locate the sloped liquid texture array load.");
                    return;
                }
                replaceLiquidTexture(LiquidTileType.Slope);

                if (!cursor.TryGotoNext(MoveType.After, c => c.MatchLdarg(5)))
                {
                    LogFailure("Custom Lava Drawing", "Could not locate the liquid light color.");
                    return;
                }
                replaceLiquidColor(true);
            }
        }

        private static void DrawCustomLava2(ILContext il)
        {
            ILCursor cursor = new ILCursor(il);

            if (!cursor.TryGotoNext(c => c.MatchLdfld<LiquidRenderer>("_liquidTextures")))
            {
                LogFailure("Custom Lava Drawing", "Could not locate the liquid texture array load.");
                return;
            }

            // While this may seem crazy, under no circumstances should there not be a load after exactly 3 instructions.
            // The order is load is texture array field -> load index -> load the reference to the texture at that index -> call get_Value().
            cursor.Index += 4;
            cursor.EmitDelegate<Func<Texture2D, Texture2D>>(initialTexture => SelectLavaTexture(initialTexture, LiquidTileType.Waterflow));

            if (!cursor.TryGotoNext(MoveType.After, c => c.MatchLdloc(9)))
            {
                LogFailure("Custom Lava Drawing", "Could not locate the liquid light color.");
                return;
            }

            // Pass the texture in so that the method can ensure it is not messing around with non-lava textures.
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldfld, typeof(LiquidRenderer).GetField("_liquidTextures"));
            cursor.Emit(OpCodes.Ldloc, 8);
            cursor.Emit(OpCodes.Ldelem_Ref);
            cursor.EmitDelegate<Func<VertexColors, Texture2D, VertexColors>>((initialColor, initialTexture) =>
            {
                initialColor.TopLeftColor = SelectLavaColor(initialTexture, initialColor.TopLeftColor);
                initialColor.TopRightColor = SelectLavaColor(initialTexture, initialColor.TopRightColor);
                initialColor.BottomLeftColor = SelectLavaColor(initialTexture, initialColor.BottomLeftColor);
                initialColor.BottomRightColor = SelectLavaColor(initialTexture, initialColor.BottomRightColor);
                return initialColor;
            });
        }

        private static void DrawCustomLava3(ILContext il)
        {
            ILCursor cursor = new ILCursor(il);

            // Select the lava color.
            if (!cursor.TryGotoNext(MoveType.After, c => c.MatchCallOrCallvirt<Lighting>("get_NotRetro")))
            {
                LogFailure("Custom Lava Drawing", "Could not locate the retro style check.");
                return;
            }

            // Pass the texture in so that the method can ensure it is not messing around with non-lava textures.
            cursor.Emit(OpCodes.Ldloc, 13);
            cursor.Emit(OpCodes.Ldsfld, typeof(TextureAssets).GetField("Liquid"));
            cursor.Emit(OpCodes.Ldloc, 15);
            cursor.Emit(OpCodes.Ldelem_Ref);
            cursor.Emit(OpCodes.Call, typeof(Asset<Texture2D>).GetMethod("get_Value", BindingFlags.Public | BindingFlags.Instance));
            cursor.EmitDelegate<Func<Color, Texture2D, Color>>((initialColor, initialTexture) => SelectLavaColor(initialTexture, initialColor));
            cursor.Emit(OpCodes.Stloc, 13);

            // Go back to the start and change textures as necessary.
            cursor.Index = 0;

            while (cursor.TryGotoNext(c => c.MatchLdsfld(typeof(TextureAssets).GetField("Liquid"))))
            {
                // While this may seem crazy, under no circumstances should there not be a load after exactly 3 instructions.
                // The order is load is texture array field -> load index -> load the reference to the texture at that index -> call get_Value().
                cursor.Index += 4;
                cursor.EmitDelegate<Func<Texture2D, Texture2D>>(initialTexture => SelectLavaTexture(initialTexture, LiquidTileType.Block));
            }
        }

        private static void DrawCustomLavafalls(ILContext il)
        {
            ILCursor cursor = new ILCursor(il);

            // Search for the color and alter it based on the same conditions as the lava.
            if (!cursor.TryGotoNext(c => c.MatchCallOrCallvirt(typeof(Color).GetConstructor(new Type[] { typeof(int), typeof(int), typeof(int), typeof(int) }))))
            {
                LogFailure("Custom Lavafall Drawing", "Could not locate the waterfall color.");
                return;
            }

            // Determine the waterfall type. This happens after all the "If Lava do blahblahblah" color checks, meaning it will have the same
            // color properties as lava.
            cursor.Emit(OpCodes.Ldloc, 13);
            cursor.EmitDelegate<Func<int, int>>(initialWaterfallStyle => CustomLavaManagement.SelectLavafallStyle(initialWaterfallStyle));
            cursor.Emit(OpCodes.Stloc, 13);

            cursor.Emit(OpCodes.Ldloc, 13);
            cursor.Emit(OpCodes.Ldloc, 56);
            cursor.EmitDelegate<Func<int, Color, Color>>((initialWaterfallStyle, initialLavafallColor) => CustomLavaManagement.SelectLavafallColor(initialWaterfallStyle, initialLavafallColor));
            cursor.Emit(OpCodes.Stloc, 56);
        }
        #endregion Custom Lava Visuals

        #region Statue Additions
        /// <summary>
        /// Change the following code sequence in Wiring.HitWireSingle
        /// num8 = (int) Utils.SelectRandom<short>(Main.rand, new short[2]
        /// {
        ///     355,
        ///     358
        /// });
        ///
        /// to
        ///
        /// var arr = new short[2]
        /// {
        ///     355,
        ///     358
        /// });
        /// arr = arr.ToList().Add(id).ToArray();
        /// num8 = Utils.SelectRandom(Main.rand, arr);
        ///
        /// </summary>
        /// <param name="il"></param>
        private static void AddTwinklersToStatue(ILContext il)
        {
            // obtain a cursor positioned before the first instruction of the method
            // the cursor is used for navigating and modifying the il
            var c = new ILCursor(il);

            // the exact location for this hook is very complex to search for due to the hook instructions not being unique, and buried deep in control flow
            // switch statements are sometimes compiled to if-else chains, and debug builds litter the code with no-ops and redundant locals

            // in general you want to search using structure and function rather than numerical constants which may change across different versions or compile settings
            // using local variable indices is almost always a bad idea

            // we can search for
            // switch (*)
            //   case 54:
            //     Utils.SelectRandom *

            // in general you'd want to look for a specific switch variable, or perhaps the containing switch (type) { case 105:
            // but the generated IL is really variable and hard to match in this case

            // we'll just use the fact that there are no other switch statements with case 54, followed by a SelectRandom

            ILLabel[] targets = null;
            while (c.TryGotoNext(i => i.MatchSwitch(out targets)))
            {
                // some optimising compilers generate a sub so that all the switch cases start at 0
                // ldc.i4.s 51
                // sub
                // switch
                int offset = 0;
                if (c.Prev.MatchSub() && c.Prev.Previous.MatchLdcI4(out offset))
                {
                    ;
                }

                // get the label for case 54: if it exists
                int case54Index = 54 - offset;
                if (case54Index < 0 || case54Index >= targets.Length || !(targets[case54Index] is ILLabel target))
                {
                    continue;
                }

                // move the cursor to case 54:
                c.GotoLabel(target);
                // there's lots of extra checks we could add here to make sure we're at the right spot, such as not encountering any branching instructions
                c.GotoNext(i => i.MatchCall(typeof(Utils), nameof(Utils.SelectRandom)));

                // goto next positions us before the instruction we searched for, so we can insert our array modifying code right here
                c.EmitDelegate<Func<short[], short[]>>(arr =>
                {
                    // resize the array and add our custom firefly
                    Array.Resize(ref arr, arr.Length+1);
                    arr[arr.Length-1] = (short)ModContent.NPCType<Twinkler>();
                    return arr;
                });

                // hook applied successfully
                return;
            }

            // couldn't find the right place to insert
            throw new Exception("Hook location not found, switch(*) { case 54: ...");
        }
        #endregion Statue Additions
    }
}
