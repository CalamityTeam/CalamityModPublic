using CalamityMod.CalPlayer;
using CalamityMod.NPCs;
using CalamityMod.Projectiles;
using CalamityMod.Tiles.DraedonStructures;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.GameInput;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Gamepad;
using Terraria.Utilities;
using Terraria.World.Generation;

namespace CalamityMod.ILEditing
{
    public class ILChanges
    {
        // This list should contain all vanilla NPCs present in Boss Rush which ARE NOT bosses and whose health is boosted over 32,768.
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

        // Holds the vanilla game function which spawns town NPCs, wrapped in a delegate for reflection purposes.
        // This function is (optionally) invoked manually in an IL edit to enable NPCs to spawn at night.
        private static Action VanillaSpawnTownNPCs;

        private static int labDoorOpen = -1;
        private static int labDoorClosed = -1;
        private static int aLabDoorOpen = -1;
        private static int aLabDoorClosed = -1;

		/// <summary>
		/// Loads all IL Editing changes in the mod.
		/// </summary>
		internal static void Load()
        {
            // Wrap the vanilla town NPC spawning function in a delegate so that it can be tossed around and called at will.
            var updateTime = typeof(Main).GetMethod("UpdateTime_SpawnTownNPCs", BindingFlags.Static | BindingFlags.NonPublic);
            VanillaSpawnTownNPCs = Delegate.CreateDelegate(typeof(Action), updateTime) as Action;

            // Cache the four lab door tile types for efficiency.
            labDoorOpen = ModContent.TileType<LaboratoryDoorOpen>();
            labDoorClosed = ModContent.TileType<LaboratoryDoorClosed>();
            aLabDoorOpen = ModContent.TileType<AgedLaboratoryDoorOpen>();
            aLabDoorClosed = ModContent.TileType<AgedLaboratoryDoorClosed>();

            ApplyLifeBytesChanges();
			AdjustChlorophyteBullets();
			RemoveAerialBaneDamageBoost();
			AdjustDamageVariance();
			RemoveExpertHardmodeScaling();
			IncreaseChlorophyteSpreadChance();
			ReduceTileBoostedRunSpeeds();
			ReduceWingHoverVelocities();
            RemoveRNGFromBlackBelt();
            ApplyBossZenDuringSlimeRain();
            PreventDungeonAbyssInteraction();
            BlockLivingTreesNearOcean();
            LabDoorFixes();
            AlterTownNPCSpawnRate();
			DisableDemonAltarGeneration();
			DisableTeleportersDuringBossFights();
            FixSplittingWormBannerDrops();
            IncorporateMinionExplodingCountdown();
            UseCoolFireCursorEffect();
            MakeMouseHoverItemsSupportAnimations();
        }

        /// <summary>
        /// Currently mostly useless, but clears static variables.
        /// </summary>
        internal static void Unload()
        {
            VanillaSpawnTownNPCs = null;
            labDoorOpen = labDoorClosed = aLabDoorOpen = aLabDoorClosed = -1;
        }

        #region IL Editing Routines
        private static void ApplyLifeBytesChanges() => On.Terraria.Main.InitLifeBytes += BossRushLifeBytes;

		private static void AdjustChlorophyteBullets()
		{
			// Reduce dust from 10 to 5 and homing range.
			IL.Terraria.Projectile.AI_001 += (il) =>
			{
				var cursor = new ILCursor(il);
				cursor.GotoNext(MoveType.Before, i => i.MatchLdcI4(207)); // The ID of Chlorophyte Bullets.
				cursor.GotoNext(MoveType.Before, i => i.MatchLdcI4(10)); // The number of dust spawned by the bullet.
				cursor.Remove();
				cursor.Emit(OpCodes.Ldc_I4, 5); // Decrease dust to 5.

				cursor.GotoNext(MoveType.Before, i => i.MatchLdcR4(300f)); // The 300 unit distance required to home in.
				cursor.Remove();
				cursor.Emit(OpCodes.Ldc_R4, 150f); // Reduce homing range by 50%.
			};
		}

		private static void RemoveAerialBaneDamageBoost()
		{
			IL.Terraria.Projectile.Damage += (il) =>
			{
				var cursor = new ILCursor(il);
				cursor.GotoNext(MoveType.Before, i => i.MatchLdcI4(710)); // The ID of Aerial Bane projectiles.
				cursor.GotoNext(MoveType.Before, i => i.MatchLdcR4(1.5f)); // The damage multiplier.
				cursor.Remove();
				cursor.Emit(OpCodes.Ldc_R4, 1f); // Multiplying by 1 means no damage bonus.
			};
		}

		private static void AdjustDamageVariance()
		{
			// Change the damage variance from +-15% to +-5%.
			IL.Terraria.Main.DamageVar += (il) =>
			{
				var cursor = new ILCursor(il);
				cursor.GotoNext(MoveType.Before, i => i.MatchLdcI4(-15)); // The -15% lower bound of the variance.
				cursor.Remove();
				cursor.Emit(OpCodes.Ldc_I4, -5); // Increase to -5%.

				cursor.GotoNext(MoveType.Before, i => i.MatchLdcI4(16)); // The +15% upper bound of the variance.
				cursor.Remove();
				cursor.Emit(OpCodes.Ldc_I4, 6); // Decrease to +5%.
			};
		}

		private static void RemoveExpertHardmodeScaling()
		{
			// Completely disable the weak enemy scaling that occurs when Hardmode is active in Expert Mode.
			IL.Terraria.NPC.scaleStats += (il) =>
			{
				var cursor = new ILCursor(il);
				cursor.GotoNext(MoveType.Before, i => i.MatchLdcI4(1000)); // The less than 1000 HP check in order for the scaling to take place.
				cursor.Remove();
				cursor.Emit(OpCodes.Ldc_I4_M1); // Replace the 1000 with -1, no NPC can have less than -1 HP on spawn, so it fails to run.
			};
		}

		private static void IncreaseChlorophyteSpreadChance()
		{
			// Change the Chlorophyte spawn rate.
			IL.Terraria.WorldGen.hardUpdateWorld += (il) =>
			{
				var cursor = new ILCursor(il);
				cursor.GotoNext(MoveType.Before, i => i.MatchLdcI4(300)); // 1 in 300 genRand call used to generate Chlorophyte in mud tiles near jungle grass.
				cursor.Remove();
				cursor.Emit(OpCodes.Ldc_I4, 150); // Increase the chance to 1 in 150.
			};

			// Change the Chlorophyte spawn limits.
			IL.Terraria.WorldGen.Chlorophyte += (il) =>
			{
				var cursor = new ILCursor(il);
				cursor.GotoNext(MoveType.Before, i => i.MatchLdcI4(40)); // Find the 40 Chlorophyte tile limit. This limit is checked within a 71x71-tile square, with the reference tile as the center.
				cursor.Remove();
				cursor.Emit(OpCodes.Ldc_I4, 60); // Increase the limit to 60.

				cursor.GotoNext(MoveType.Before, i => i.MatchLdcI4(130)); // Find the 130 Chlorophyte tile limit. This limit is checked within a 171x171-tile square, with the reference tile as the center.
				cursor.Remove();
				cursor.Emit(OpCodes.Ldc_I4, 200); // Increase the limit to 200.
			};
		}

		private static void ReduceTileBoostedRunSpeeds()
		{
			// Reduce the run speed boost while running on Asphalt, Frozen Slime Blocks and Ice Blocks.
			IL.Terraria.Player.Update += (il) =>
			{
				var cursor = new ILCursor(il);
				cursor.GotoNext(MoveType.Before, i => i.MatchLdcR8(1.6)); // Movement speed cap (removed in 1.4).
				cursor.Remove();
				cursor.Emit(OpCodes.Ldc_R8, 3D); // Increase it to some higher amount.

				cursor.GotoNext(MoveType.Before, i => i.MatchLdcR4(1.6f)); // Movement speed cap (removed in 1.4).
				cursor.Remove();
				cursor.Emit(OpCodes.Ldc_R4, 3f); // Increase it to some higher amount.

				cursor.GotoNext(MoveType.Before, i => i.MatchLdcR4(3.5f)); // The max run speed multiplier for Asphalt.
				cursor.Remove();
				cursor.Emit(OpCodes.Ldc_R4, 1.75f); // Reduce by 1.75.

				cursor.GotoNext(MoveType.Before, i => i.MatchLdcR4(1.25f)); // The max run speed multiplier for Frozen Slime Blocks.
				cursor.Remove();
				cursor.Emit(OpCodes.Ldc_R4, 1f); // Reduce boost to 0.

				cursor.GotoNext(MoveType.Before, i => i.MatchLdcR4(1.25f)); // The max run speed multiplier for Ice Blocks.
				cursor.Remove();
				cursor.Emit(OpCodes.Ldc_R4, 1f); // Reduce boost to 0.
			};
		}

		private static void ReduceWingHoverVelocities()
		{
			// Reduce wing hover horizontal velocities. Hoverboard is fine because both stats are at 10.
			IL.Terraria.Player.Update += (il) =>
			{
				var cursor = new ILCursor(il);
				cursor.GotoNext(MoveType.Before, i => i.MatchLdcR4(6.25f)); // The accRunSpeed variable is set to this specific value before hover adjustments occur.
				cursor.GotoNext(MoveType.Before, i => i.MatchLdcR4(12f)); // The accRunSpeed for Vortex Booster and Nebula Mantle.
				cursor.Remove();
				cursor.Emit(OpCodes.Ldc_R4, 10.8f); // Reduce by 10%.

				cursor.GotoNext(MoveType.Before, i => i.MatchLdcR4(12f)); // The runAcceleration for Vortex Booster and Nebula Mantle.
				cursor.Remove();
				cursor.Emit(OpCodes.Ldc_R4, 10.8f); // Reduce by 10%.

				cursor.GotoNext(MoveType.Before, i => i.MatchLdcR4(12f)); // The accRunSpeed for Betsy Wings.
				cursor.Remove();
				cursor.Emit(OpCodes.Ldc_R4, 10.8f); // Reduce by 10%.

				cursor.GotoNext(MoveType.Before, i => i.MatchLdcR4(12f)); // The runAcceleration for Betsy Wings.
				cursor.Remove();
				cursor.Emit(OpCodes.Ldc_R4, 10.8f); // Reduce by 10%.
			};
		}

		private static void RemoveRNGFromBlackBelt()
        {
            // Change the random chance of the Black Belt to 100%, but don't let it work if Calamity's cooldown is active.
            IL.Terraria.Player.Hurt += (il) =>
            {
                var cursor = new ILCursor(il);
                cursor.GotoNext(MoveType.Before, i => i.MatchLdcI4(10)); // 1 in 10 Main.rand call for Black Belt activation.
                cursor.Remove();
                cursor.Emit(OpCodes.Ldc_I4_1); // Replace with Main.rand.Next(1), aka 100% chance.

                // Move forwards past the Main.rand.Next call now that it has been edited.
                cursor.GotoNext(MoveType.After, i => i.MatchCallvirt<UnifiedRandom>("Next"));

                // Load the player itself onto the stack so that it becomes an argument for the following delegate.
                cursor.Emit(OpCodes.Ldarg_0);

                // Emit a delegate which places the player's Calamity dodge cooldown onto the stack.
                cursor.EmitDelegate<Func<Player, int>>((Player p) => p.Calamity().dodgeCooldownTimer);

                // Bitwise OR the "RNG result" (always zero) with the dodge cooldown. This will only return zero if both values were zero.
                // The code path which calls NinjaDodge can ONLY occur if the result of this operation is zero,
                // because it is now the value checked by the immediately following branch-if-true.
                cursor.Emit(OpCodes.Or);

                // Move forwards past the NinjaDodge call. We need to set the dodge cooldown here.
                cursor.GotoNext(MoveType.After, i => i.MatchCall<Player>("NinjaDodge"));

                // Load the player itself onto the stack so that it becomes an argument for the following delegate.
                cursor.Emit(OpCodes.Ldarg_0);

                // Emit a delegate which sets the player's Calamity dodge cooldown and sends a sync packet appropriately.
                cursor.EmitDelegate<Action<Player>>((Player p) =>
                {
                    CalamityPlayer calPlayer = p.Calamity();
                    calPlayer.dodgeCooldownTimer = CalamityPlayer.BeltDodgeCooldown;
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                        calPlayer.SyncDodgeCooldown(false);
                });
            };
        }

        private static void ApplyBossZenDuringSlimeRain() => On.Terraria.NPC.SlimeRainSpawns += PreventBossSlimeRainSpawns;

		private static void ReplacePharaohSetInPyramids()
		{
			// Replace the Pharaoh's Set in Pyramid gen with something actually useful.
			IL.Terraria.NPC.scaleStats += (il) =>
			{
				var cursor = new ILCursor(il);
				cursor.GotoNext(MoveType.Before, i => i.MatchLdcI4(848)); // The ID of the Pharaoh's Mask.
				cursor.Remove();
				cursor.Emit(OpCodes.Ldc_I4, 1240); // Replace the Mask with a Ruby Hook, in 1.4 I will replace this with an Amber Hook so it makes more sense.
				// Note: There is no need to replace the other Pharaoh piece, due to how the vanilla code works.
			};
		}

		private static void PreventDungeonAbyssInteraction()
        {
            // Prevent the Dungeon from appearing near the Sulph sea.
            IL.Terraria.WorldGen.MakeDungeon += il =>
            {
                var cursor = new ILCursor(il);
                cursor.GotoNext(MoveType.After, i => i.MatchStsfld<WorldGen>("dungeonY"));

                cursor.EmitDelegate<Action>(() =>
                {
                    WorldGen.dungeonX = Utils.Clamp(WorldGen.dungeonX, SulphurousSea.BiomeWidth + 100, Main.maxTilesX - SulphurousSea.BiomeWidth - 100);

                    // Adjust the Y position of the dungeon to accomodate for the X shift.
                    WorldUtils.Find(new Point(WorldGen.dungeonX, WorldGen.dungeonY), Searches.Chain(new Searches.Down(9001), new Conditions.IsSolid()), out Point result);
                    WorldGen.dungeonY = result.Y - 10;
                });
            };

            // And prevent its halls from getting anywhere near the Abyss.
            IL.Terraria.WorldGen.DungeonHalls += il =>
            {
                var cursor = new ILCursor(il);

                // Forcefully clamp the X position of the new hall end.
                // This prevents a hall, and as a result, the dungeon, from ever impeding on the Abyss/Sulph Sea.
                cursor.GotoNext(MoveType.After, i => i.MatchStloc(6));

                cursor.Emit(OpCodes.Ldloc, 6);
                cursor.EmitDelegate<Func<Vector2, Vector2>>(unclampedValue =>
                {
                    unclampedValue.X = MathHelper.Clamp(unclampedValue.X, SulphurousSea.BiomeWidth + 25, Main.maxTilesX - SulphurousSea.BiomeWidth - 25);
                    return unclampedValue;
                });
                cursor.Emit(OpCodes.Stloc, 6);
            };
        }

        private static void BlockLivingTreesNearOcean()
        {
            IL.Terraria.WorldGen.GrowLivingTree += (il) =>
            {
                var cursor = new ILCursor(il);
                cursor.Emit(OpCodes.Ldarg_0);
                cursor.EmitDelegate<Func<int, int>>(x => Utils.Clamp(x, 560, Main.maxTilesX - 560));
                cursor.Emit(OpCodes.Starg, 0);
            };
        }

        private static void LabDoorFixes()
        {
            On.Terraria.WorldGen.OpenDoor += OpenDoor_LabDoorOverride;
            On.Terraria.WorldGen.CloseDoor += CloseDoor_LabDoorOverride;
        }

        private static void AlterTownNPCSpawnRate()
        {
            IL.Terraria.Main.UpdateTime += (il) =>
            {
                // Don't do town NPC spawning at the end (after a !Main.dayTime return).
                // Do it at the beginning in spite of time.

                var cursor = new ILCursor(il);
                cursor.EmitDelegate<Action>(() =>
                {
                    // A cached delegate is used here instead of direct reflection for performance reasons
                    // since UpdateTime is called every frame.
                    if (Main.dayTime || CalamityConfig.Instance.CanTownNPCsSpawnAtNight)
                        VanillaSpawnTownNPCs();
                });

                if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchCallOrCallvirt<Main>("UpdateTime_SpawnTownNPCs")))
                {
                    CalamityMod.Instance.Logger.Warn("Town NPC spawn editing code failed.");
                    return;
                }

                cursor.Emit(OpCodes.Ret);
            };

            On.Terraria.Main.UpdateTime_SpawnTownNPCs += (orig) =>
            {
                int oldWorldRate = Main.worldRate;
                Main.worldRate *= CalamityConfig.Instance.TownNPCSpawnRateMultiplier;
                orig();
                Main.worldRate = oldWorldRate;
            };
        }

		private static void DisableDemonAltarGeneration() => On.Terraria.WorldGen.SmashAltar += PreventSmashAltarCode;

		private static void DisableTeleportersDuringBossFights() => On.Terraria.Wiring.Teleport += DisableTeleporters;

        private static void FixSplittingWormBannerDrops()
        {
            IL.Terraria.NPC.NPCLoot += (il) =>
            {
                var cursor = new ILCursor(il);

                // Locate the area after all the banner logic by using a nearby constant type.
                cursor.GotoNext(MoveType.Before, i => i.MatchLdcI4(23));
                cursor.GotoPrev(MoveType.Before, i => i.MatchLdarg(0));

                ILLabel afterBannerLogic = cursor.DefineLabel();

                // Set this area after as a place to return to later.
                cursor.MarkLabel(afterBannerLogic);

                // Go to the beginning of the banner drop logic.
                cursor.Goto(0);
                cursor.GotoNext(MoveType.Before, i => i.MatchLdsfld<NPC>("killCount"));

                // Load the NPC caller onto the stack.
                cursor.Emit(OpCodes.Ldarg_0);
                cursor.EmitDelegate<Func<NPC, bool>>(npc => CalamityGlobalNPCLoot.SplittingWormLootBlockWrapper(npc, CalamityMod.Instance));

                // Emit 0 (false) onto the stack.
                cursor.Emit(OpCodes.Ldc_I4_0);

                // If the block is equal to false (indicating the drop logic should stop), skip all the ahead banner drop logic.
                cursor.Emit(OpCodes.Beq, afterBannerLogic);
            };
        }

        private static void IncorporateMinionExplodingCountdown()
		{
            On.Terraria.Projectile.NewProjectile_float_float_float_float_int_int_float_int_float_float += (orig, x, y, xSpeed, ySpeed, type, damage, knockback, owner, ai0, ai1) =>
            {
                // This is unfortunately not something that can be done via SetDefaults since owner is set
                // after that method is called. Doing it directly when the projectile is spawned appears to be the only reasonable way.
                int proj = orig(x, y, xSpeed, ySpeed, type, damage, knockback, owner, ai0, ai1);
                Projectile projectile = Main.projectile[proj];
                if (projectile.minion)
                {
                    Player player = Main.player[projectile.owner];
                    CalamityPlayerMiscEffects.EnchantHeldItemEffects(player, player.Calamity(), player.ActiveItem());
                    if (player.Calamity().explosiveMinionsEnchant)
                        projectile.Calamity().ExplosiveEnchantCountdown = CalamityGlobalProjectile.ExplosiveEnchantTime;
                }
                return proj;
            };
		}

        private static void MakeMouseHoverItemsSupportAnimations()
        {
            IL.Terraria.Main.DrawInterface_40_InteractItemIcon += (il) =>
            {
                var cursor = new ILCursor(il);

                // Locate the location where the frame rectangle is created.
                if (!cursor.TryGotoNext(MoveType.After, i => i.MatchNewobj<Rectangle?>()))
                    return;

                int endIndex = cursor.Index;

                // And then go back to where it began, right after the draw position vector.
                if (!cursor.TryGotoPrev(MoveType.After, i => i.MatchNewobj<Vector2>()))
                    return;

                // And delete the range that creates the rectangle with intent to replace it.
                cursor.RemoveRange(Math.Abs(endIndex - cursor.Index));

                cursor.Emit(OpCodes.Ldloc_0);
                cursor.EmitDelegate<Func<int, Rectangle?>>(itemType =>
                {
                    return Main.itemAnimations[itemType]?.GetFrame(Main.itemTexture[itemType]) ?? null;
                });
            };
        }

        #endregion

		#region IL Editing Injected/Hooked Functions
		private static void BossRushLifeBytes(On.Terraria.Main.orig_InitLifeBytes orig)
        {
            orig();
            foreach (int npcType in NeedsFourLifeBytes)
                Main.npcLifeBytes[npcType] = 4;
        }

        private static void PreventBossSlimeRainSpawns(On.Terraria.NPC.orig_SlimeRainSpawns orig, int plr)
        {
            if (!Main.player[plr].Calamity().bossZen)
                orig(plr);
        }

        private static bool OpenDoor_LabDoorOverride(On.Terraria.WorldGen.orig_OpenDoor orig, int i, int j, int direction)
        {
            Tile tile = Main.tile[i, j];
            // If the tile is somehow null, that's vanilla's problem, we're outta here
            if (tile is null)
                return orig(i, j, direction);

            // If it's one of the two lab doors, use custom code to open the door and sync tiles in multiplayer.
            else if (tile.type == labDoorClosed)
                return OpenLabDoor(tile, i, j, labDoorOpen);
            else if (tile.type == aLabDoorClosed)
                return OpenLabDoor(tile, i, j, aLabDoorOpen);

            // If it's anything else, let vanilla and/or TML handle it.
            return orig(i, j, direction);
        }

        private static bool CloseDoor_LabDoorOverride(On.Terraria.WorldGen.orig_CloseDoor orig, int i, int j, bool forced)
        {
            Tile tile = Main.tile[i, j];
            // If the tile is somehow null, that's vanilla's problem, we're outta here
            if (tile is null)
                return orig(i, j, forced);

            // If it's one of the two lab doors, use custom code to open the door and sync tiles in multiplayer.
            else if (tile.type == labDoorOpen)
                return CloseLabDoor(tile, i, j, labDoorClosed);
            else if (tile.type == aLabDoorOpen)
                return CloseLabDoor(tile, i, j, aLabDoorClosed);

            // If it's anything else, let vanilla and/or TML handle it.
            return orig(i, j, forced);
        }

        private static void UseCoolFireCursorEffect()
        {
            On.Terraria.Main.DrawCursor += (orig, bonus, smart) =>
            {
                // Do nothing special if the player has a regular mouse or is on the menu.
                if (Main.gameMenu || !Main.LocalPlayer.Calamity().ableToDrawBlazingMouse)
                {
                    orig(bonus, smart);
                    return;
                }

                if (Main.LocalPlayer.dead)
                {
                    Main.SmartInteractShowingGenuine = false;
                    Main.SmartInteractShowingFake = false;
                    Main.SmartInteractNPC = -1;
                    Main.SmartInteractNPCsNearby.Clear();
                    Main.SmartInteractTileCoords.Clear();
                    Main.SmartInteractTileCoordsSelected.Clear();
                    Main.TileInteractionLX = (Main.TileInteractionHX = (Main.TileInteractionLY = (Main.TileInteractionHY = -1)));
                }

                Color flameColor = Color.Lerp(Color.DarkRed, Color.OrangeRed, (float)Math.Cos(Main.GlobalTime * 7.4f) * 0.5f + 0.5f);
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
                    if (Main.LocalPlayer.Calamity().blazingMouseDamageEffects && !Main.mapFullscreen)
                    {
                        Texture2D auraTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/CalamityAura");
                        Rectangle auraFrame = auraTexture.Frame(1, 6, 0, (int)(Main.GlobalTime * 12.3f) % 6);
                        float auraScale = MathHelper.Lerp(0.95f, 1f, (float)Math.Sin(Main.GlobalTime * 1.1f) * 0.5f + 0.5f);

                        for (int i = 0; i < 12; i++)
                        {
                            Color auraColor = Color.Orange * Main.LocalPlayer.Calamity().blazingMouseAuraFade * 0.125f;
                            auraColor.A = 0;
                            Vector2 offsetDrawPosition = baseDrawPosition + (MathHelper.TwoPi * i / 12f + Main.GlobalTime * 5f).ToRotationVector2() * 2.5f;
                            offsetDrawPosition.Y -= 18f;

                            Main.spriteBatch.Draw(auraTexture, offsetDrawPosition, auraFrame, auraColor, 0f, auraFrame.Size() * 0.5f, Main.cursorScale * auraScale, SpriteEffects.None, 0f);
                        }
                    }

                    Main.spriteBatch.Draw(Main.cursorTextures[cursorIndex], drawPosition, null, desaturatedCursorColor, 0f, Vector2.Zero, Main.cursorScale, SpriteEffects.None, 0f);

                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.UIScaleMatrix);
                    GameShaders.Misc["CalamityMod:FireMouse"].UseColor(Color.Red);
                    GameShaders.Misc["CalamityMod:FireMouse"].UseSecondaryColor(Color.Lerp(Color.Red, Color.Orange, 0.75f));
                    GameShaders.Misc["CalamityMod:FireMouse"].Apply();

                    Main.spriteBatch.Draw(Main.cursorTextures[cursorIndex], desaturatedDrawPosition, null, cursorColor, 0f, Vector2.Zero, Main.cursorScale * 1.075f, SpriteEffects.None, 0f);
                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.EffectMatrix);
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
                    Texture2D smartCursorTexture = Main.cursorTextures[13];
                    Rectangle frame = smartCursorTexture.Frame(2, 1, frameX, 0);
                    Main.spriteBatch.Draw(smartCursorTexture, baseDrawPosition, frame, cursorColor, 0f, frame.Size() * 0.5f, Main.cursorScale, SpriteEffects.None, 0f);
                    return;
                }

                // Otherwise draw an ordinary crosshair at the mouse position.
                cursorColor = Color.White;
                Texture2D crosshairTexture = Main.cursorTextures[15];
                Main.spriteBatch.Draw(crosshairTexture, baseDrawPosition, null, cursorColor, 0f, crosshairTexture.Size() * 0.5f, Main.cursorScale, SpriteEffects.None, 0f);
            };
        }

        private static void PreventSmashAltarCode(On.Terraria.WorldGen.orig_SmashAltar orig, int i, int j)
		{
			if (CalamityConfig.Instance.EarlyHardmodeProgressionRework)
				return;

			orig(i, j);
		}

		private static void DisableTeleporters(On.Terraria.Wiring.orig_Teleport orig)
		{
			if (CalamityPlayer.areThereAnyDamnBosses)
				return;

			orig();
		}
		#endregion

		#region Helper Functions
		private static int FindTopOfDoor(int i, int j, Tile rootTile)
        {
            Tile t = Main.tile[i, j];
            int topY = j;
            while(t != null && t.active() && t.type == rootTile.type)
            {
                // Immediately stop at the top of the world, if you got there somehow.
                if (topY == 0)
                    return topY;
                // Go up one space and re-assign the current tile.
                --topY;
                t = Main.tile[i, topY];
            }
            
            // The above loop will have gone 1 past the top of the door. Correct for this.
            return ++topY;
        }

        private static bool OpenLabDoor(Tile tile, int i, int j, int openID)
        {
            int topY = FindTopOfDoor(i, j, tile);
            return DirectlyTransformLabDoor(i, topY, openID);
        }

        private static bool CloseLabDoor(Tile tile, int i, int j, int closedID)
        {
            int topY = FindTopOfDoor(i, j, tile);
            return DirectlyTransformLabDoor(i, topY, closedID);
        }

        private static bool DirectlyTransformLabDoor(int doorX, int doorY, int newDoorID, int wireHitY = -1)
        {
            // Transform the door one tile at a time.
            // If applicable, skip wiring for all door tiles except the one that was hit by this wire event.
            for (int y = doorY; y < doorY + 4; ++y)
            {
                Main.tile[doorX, y].type = (ushort)newDoorID;
                if (Main.netMode != NetmodeID.MultiplayerClient && Wiring.running && y != wireHitY)
                    Wiring.SkipWire(doorX, y);
            }

            // Second pass: TileFrame all those positions, which will sync in multiplayer if applicable
            for (int y = doorY; y < doorY + 4; ++y)
                WorldGen.TileFrame(doorX, y);

            // Play the door closing sound (lab doors do not use the door opening sound)
            Main.PlaySound(SoundID.DoorClosed, doorX * 16, doorY * 16);
            return true;
        }
        #endregion
    }
}
