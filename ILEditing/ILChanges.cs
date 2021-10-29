using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer;
using CalamityMod.Events;
using CalamityMod.NPCs;
using CalamityMod.NPCs.AstrumAureus;
using CalamityMod.NPCs.Crabulon;
using CalamityMod.NPCs.Ravager;
using CalamityMod.Particles;
using CalamityMod.Projectiles;
using CalamityMod.Tiles.DraedonStructures;
using CalamityMod.Waters;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.GameContent.Liquid;
using Terraria.GameInput;
using Terraria.Graphics;
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

        #region Load / Unload
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

            // Mechanics / features
            On.Terraria.NPC.ApplyTileCollision += AllowTriggeredFallthrough;
            IL.Terraria.Main.UpdateTime += PermitNighttimeTownNPCSpawning;
            On.Terraria.Main.UpdateTime_SpawnTownNPCs += AlterTownNPCSpawnRate;
            IL.Terraria.Player.Hurt += RemoveRNGFromBlackBelt;
            On.Terraria.WorldGen.OpenDoor += OpenDoor_LabDoorOverride;
            On.Terraria.WorldGen.CloseDoor += CloseDoor_LabDoorOverride;
            On.Terraria.Wiring.Teleport += DisableTeleporters; // only applies in boss rush
            IL.Terraria.Main.DrawInterface_40_InteractItemIcon += MakeMouseHoverItemsSupportAnimations;
            On.Terraria.Item.AffixName += IncorporateEnchantmentInAffix;
            On.Terraria.Projectile.NewProjectile_float_float_float_float_int_int_float_int_float_float += IncorporateMinionExplodingCountdown;
            On.Terraria.Main.DrawCursor += UseCoolFireCursorEffect;
            IL.Terraria.Player.QuickHeal += ApplyManaBurnIfNeeded;
            IL.Terraria.Player.QuickMana += ApplyManaBurnIfNeeded;
            IL.Terraria.Player.ItemCheck += ApplyManaBurnIfNeeded;
            IL.Terraria.Player.AddBuff += AllowBuffTimeStackingForManaBurn;
			IL.Terraria.Main.DoDraw += DrawFusableParticles;
            IL.Terraria.Main.DrawTiles += DrawCustomLava;
            IL.Terraria.GameContent.Liquid.LiquidRenderer.InternalDraw += DrawCustomLava2;
            IL.Terraria.WaterfallManager.DrawWaterfall += DrawCustomLavafalls;

            // Ravager platform fall fix
            On.Terraria.NPC.Collision_DecideFallThroughPlatforms += EnableCalamityBossPlatformCollision;

            // Damage and health balance
            IL.Terraria.Main.DamageVar += AdjustDamageVariance;
            IL.Terraria.NPC.scaleStats += RemoveExpertHardmodeScaling;
            IL.Terraria.Projectile.Damage += RemoveAerialBaneDamageBoost;
            IL.Terraria.Projectile.AI_001 += AdjustChlorophyteBullets;

			// Movement speed balance
			IL.Terraria.Player.Update += MaxRunSpeedAdjustment;
            IL.Terraria.Player.Update += RunSpeedAdjustments;
            IL.Terraria.Player.Update += ReduceWingHoverVelocities;

			// Mana regen balance
			IL.Terraria.Player.Update += ManaRegenDelayAdjustment;
			IL.Terraria.Player.UpdateManaRegen += ManaRegenAdjustment;

            // World generation
            IL.Terraria.WorldGen.Pyramid += ReplacePharaohSetInPyramids;
            IL.Terraria.WorldGen.MakeDungeon += PreventDungeonHorizontalCollisions;
            IL.Terraria.WorldGen.DungeonHalls += PreventDungeonHallCollisions;
            IL.Terraria.WorldGen.GrowLivingTree += BlockLivingTreesNearOcean;
            On.Terraria.WorldGen.SmashAltar += PreventSmashAltarCode;
            IL.Terraria.WorldGen.hardUpdateWorld += AdjustChlorophyteSpawnRate;
            IL.Terraria.WorldGen.Chlorophyte += AdjustChlorophyteSpawnLimits;

            // Removal of vanilla stupidity
            IL.Terraria.Item.Prefix += RelaxPrefixRequirements;
            On.Terraria.NPC.SlimeRainSpawns += PreventBossSlimeRainSpawns;
            IL.Terraria.NPC.SpawnNPC += MakeVoodooDemonDollWork;

            // Fix vanilla bugs exposed by Calamity mechanics
            On.Terraria.Main.InitLifeBytes += BossRushLifeBytes;
            IL.Terraria.NPC.NPCLoot += FixSplittingWormBannerDrops;
        }

		/// <summary>
		/// Unloads all IL Editing changes in the mod.
		/// </summary>
		internal static void Unload()
        {
            VanillaSpawnTownNPCs = null;
            labDoorOpen = labDoorClosed = aLabDoorOpen = aLabDoorClosed = -1;

            // Mechanics / features
            On.Terraria.NPC.ApplyTileCollision -= AllowTriggeredFallthrough;
            IL.Terraria.Main.UpdateTime -= PermitNighttimeTownNPCSpawning;
            On.Terraria.Main.UpdateTime_SpawnTownNPCs -= AlterTownNPCSpawnRate;
            IL.Terraria.Player.Hurt -= RemoveRNGFromBlackBelt;
            On.Terraria.WorldGen.OpenDoor -= OpenDoor_LabDoorOverride;
            On.Terraria.WorldGen.CloseDoor -= CloseDoor_LabDoorOverride;
            On.Terraria.Wiring.Teleport -= DisableTeleporters;
            IL.Terraria.Main.DrawInterface_40_InteractItemIcon -= MakeMouseHoverItemsSupportAnimations;
            On.Terraria.Item.AffixName -= IncorporateEnchantmentInAffix;
            On.Terraria.Projectile.NewProjectile_float_float_float_float_int_int_float_int_float_float -= IncorporateMinionExplodingCountdown;
            On.Terraria.Main.DrawCursor -= UseCoolFireCursorEffect;
            IL.Terraria.Player.QuickHeal -= ApplyManaBurnIfNeeded;
            IL.Terraria.Player.QuickMana -= ApplyManaBurnIfNeeded;
            IL.Terraria.Player.ItemCheck -= ApplyManaBurnIfNeeded;
            IL.Terraria.Player.AddBuff -= AllowBuffTimeStackingForManaBurn;
            IL.Terraria.Main.DoDraw -= DrawFusableParticles;
            IL.Terraria.Main.DrawTiles -= DrawCustomLava;
            IL.Terraria.GameContent.Liquid.LiquidRenderer.InternalDraw -= DrawCustomLava2;
            IL.Terraria.WaterfallManager.DrawWaterfall -= DrawCustomLavafalls;

            // Ravager platform fall fix
            On.Terraria.NPC.Collision_DecideFallThroughPlatforms -= EnableCalamityBossPlatformCollision;

			// Damage and health balance
			IL.Terraria.Main.DamageVar -= AdjustDamageVariance;
            IL.Terraria.NPC.scaleStats -= RemoveExpertHardmodeScaling;
            IL.Terraria.Projectile.Damage -= RemoveAerialBaneDamageBoost;
            IL.Terraria.Projectile.AI_001 -= AdjustChlorophyteBullets;

			// Movement speed balance
			IL.Terraria.Player.Update -= MaxRunSpeedAdjustment;
			IL.Terraria.Player.Update -= RunSpeedAdjustments;
            IL.Terraria.Player.Update -= ReduceWingHoverVelocities;

			// Mana regen balance
			IL.Terraria.Player.Update -= ManaRegenDelayAdjustment;
			IL.Terraria.Player.UpdateManaRegen -= ManaRegenAdjustment;

			// World generation
			IL.Terraria.WorldGen.Pyramid -= ReplacePharaohSetInPyramids;
            IL.Terraria.WorldGen.MakeDungeon -= PreventDungeonHorizontalCollisions;
            IL.Terraria.WorldGen.DungeonHalls -= PreventDungeonHallCollisions;
            IL.Terraria.WorldGen.GrowLivingTree -= BlockLivingTreesNearOcean;
            On.Terraria.WorldGen.SmashAltar -= PreventSmashAltarCode;
            IL.Terraria.WorldGen.hardUpdateWorld -= AdjustChlorophyteSpawnRate;
            IL.Terraria.WorldGen.Chlorophyte -= AdjustChlorophyteSpawnLimits;

            // Removal of vanilla stupidity
            IL.Terraria.Item.Prefix -= RelaxPrefixRequirements;
            On.Terraria.NPC.SlimeRainSpawns -= PreventBossSlimeRainSpawns;
            IL.Terraria.NPC.SpawnNPC -= MakeVoodooDemonDollWork;

            // Fix vanilla bugs exposed by Calamity mechanics
            On.Terraria.Main.InitLifeBytes -= BossRushLifeBytes;
            IL.Terraria.NPC.NPCLoot -= FixSplittingWormBannerDrops;
        }
        #endregion

        #region IL Editing Routines and Injections

        #region Mechanics / features

        // Why this isn't a mechanism provided by TML itself or vanilla itself is beyond me.
        private static void AllowTriggeredFallthrough(On.Terraria.NPC.orig_ApplyTileCollision orig, NPC self, bool fall, Vector2 cPosition, int cWidth, int cHeight)
        {
            if (self.active && self.Calamity().ShouldFallThroughPlatforms)
                fall = true;
            orig(self, fall, cPosition, cWidth, cHeight);
        }

        private static void PermitNighttimeTownNPCSpawning(ILContext il)
        {
            // Don't do town NPC spawning at the end (which lies after a !Main.dayTime return).
            // Do it at the beginning, without the arbitrary time restriction.
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
        }

        private static void AlterTownNPCSpawnRate(On.Terraria.Main.orig_UpdateTime_SpawnTownNPCs orig)
        {
            int oldWorldRate = Main.worldRate;
            Main.worldRate *= CalamityConfig.Instance.TownNPCSpawnRateMultiplier;
            orig();
            Main.worldRate = oldWorldRate;
        }

        private static void RemoveRNGFromBlackBelt(ILContext il)
        {
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

            // Emit a delegate which places the player's Calamity dodge cooldown onto the stack.
            cursor.EmitDelegate<Func<Player, int>>((Player p) => p.Calamity().dodgeCooldownTimer);

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
            cursor.EmitDelegate<Action<Player>>((Player p) =>
            {
                CalamityPlayer calPlayer = p.Calamity();
                calPlayer.dodgeCooldownTimer = CalamityPlayer.BeltDodgeCooldown;
                if (Main.netMode == NetmodeID.MultiplayerClient)
                    calPlayer.SyncDodgeCooldown(false);
            });
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

		private static bool EnableCalamityBossPlatformCollision(On.Terraria.NPC.orig_Collision_DecideFallThroughPlatforms orig, NPC self)
		{
			if ((self.type == ModContent.NPCType<AstrumAureus>() || self.type == ModContent.NPCType<CrabulonIdle>() || self.type == ModContent.NPCType<RavagerBody>()) &&
				self.target >= 0 && Main.player[self.target].position.Y > self.position.Y + self.height)
				return true;

			return orig(self);
		}

        private static void DisableTeleporters(On.Terraria.Wiring.orig_Teleport orig)
        {
            if (CalamityPlayer.areThereAnyDamnBosses)
                return;

            orig();
        }

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

        private static string IncorporateEnchantmentInAffix(On.Terraria.Item.orig_AffixName orig, Item self)
        {
            string result = orig(self);
            if (!self.IsAir && self.Calamity().AppliedEnchantment.HasValue)
                result = $"{self.Calamity().AppliedEnchantment.Value.Name} {result}";
            return result;
        }

        private static int IncorporateMinionExplodingCountdown(On.Terraria.Projectile.orig_NewProjectile_float_float_float_float_int_int_float_int_float_float orig, float x, float y, float xSpeed, float ySpeed, int type, int damage, float knockback, int owner, float ai0, float ai1)
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
        }

        private static void ApplyManaBurnIfNeeded(ILContext il)
        {
            ILCursor cursor = new ILCursor(il);

            if (!cursor.TryGotoNext(c => c.MatchLdcI4(BuffID.ManaSickness)))
            {
                LogFailure("Mana Burn Application", "Could not locate the mana sickness buff ID.");
                return;
            }

            cursor.Remove();
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.EmitDelegate<Func<Player, int>>(player =>
            {
                if (!player.active || !player.Calamity().ChaosStone)
                    return BuffID.ManaSickness;
                return ModContent.BuffType<ManaBurn>();
            });
        }

        private static void AllowBuffTimeStackingForManaBurn(ILContext il)
        {
            ILCursor cursor = new ILCursor(il);

            // Get a label that points to the final return before doing anything else.
            cursor.GotoFinalRet();

            ILLabel finalReturn = cursor.DefineLabel();
            cursor.MarkLabel(finalReturn);

            // After the label has been created go back to the beginning of the method.
            cursor.Goto(0);

            if (!cursor.TryGotoNext(MoveType.Before, c => c.MatchStloc(4)))
            {
                LogFailure("Mana Burn Time Stacking", "Could not locate the buff loop incremental variable.");
                return;
            }

            int startOfBuffTimeLogic = cursor.Index - 1;

            if (!cursor.TryGotoNext(MoveType.Before, c => c.MatchLdsfld<Main>("vanityPet")))
            {
                LogFailure("Mana Burn Time Stacking", "Could not locate the Main.vanityPet load.");
                return;
            }

            // Clear away the vanilla logic and re-add it with a delegate.
            // The alternative is a mess of various labels and branches.
            int endOfBuffTimeLogic = cursor.Index;
            cursor.Goto(startOfBuffTimeLogic);
            cursor.RemoveRange(endOfBuffTimeLogic - startOfBuffTimeLogic);

            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldarg_1);
            cursor.Emit(OpCodes.Ldloc_0);
            cursor.EmitDelegate<Func<Player, int, int, bool>>((player, type, buffTime) =>
            {
                for (int j = 0; j < Player.MaxBuffs; j++)
                {
                    if (player.buffType[j] == type)
                    {
                        if (!BuffLoader.ReApply(type, player, buffTime, j))
                        {
                            if (type == BuffID.ManaSickness || type == ModContent.BuffType<ManaBurn>())
                            {
                                player.buffTime[j] += buffTime;
                                if (player.buffTime[j] > Player.manaSickTimeMax)
                                    player.buffTime[j] = Player.manaSickTimeMax;

                            }
                            else if (player.buffTime[j] < buffTime)
                                player.buffTime[j] = buffTime;
                        }
                        return true;
                    }
                }
                return false;
            });
            cursor.Emit(OpCodes.Brtrue, finalReturn);
        }

        #region Fire Cursor
        private static void UseCoolFireCursorEffect(On.Terraria.Main.orig_DrawCursor orig, Vector2 bonus, bool smart)
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
        }
        #endregion

        private static void DrawFusableParticles(ILContext il)
        {
            ILCursor cursor = new ILCursor(il);

            // Over NPCs but before Projectiles.
            if (!cursor.TryGotoNext(c => c.MatchCallOrCallvirt<Main>("SortDrawCacheWorms")))
            {
                LogFailure("Fusable Particle Rendering", "Could not locate the SortDrawCacheWorms reference method to attach to.");
                return;
            }
            cursor.EmitDelegate<Action>(() => FusableParticleManager.RenderAllFusableParticles(FusableParticleRenderLayer.OverNPCsBeforeProjectiles));

            // Over Players.
            if (!cursor.TryGotoNext(MoveType.After, c => c.MatchCallOrCallvirt<Main>("DrawPlayers")))
            {
                LogFailure("Fusable Particle Rendering", "Could not locate the DrawPlayers reference method to attach to.");
                return;
            }
            cursor.EmitDelegate<Action>(() => FusableParticleManager.RenderAllFusableParticles(FusableParticleRenderLayer.OverPlayers));

            // Over Water.
            if (!cursor.TryGotoNext(c => c.MatchCallOrCallvirt<MoonlordDeathDrama>("DrawWhite")))
            {
                LogFailure("Fusable Particle Rendering", "Could not locate the DrawWhite reference method to attach to.");
                return;
            }
            cursor.EmitDelegate<Action>(() => FusableParticleManager.RenderAllFusableParticles(FusableParticleRenderLayer.OverWater));
        }

        private static void DrawCustomLava(ILContext il)
        {
            ILCursor cursor = new ILCursor(il);

            if (!cursor.TryGotoNext(c => c.MatchLdsfld<Main>("liquidTexture")))
            {
                LogFailure("Custom Lava Drawing", "Could not locate the liquid texture array load.");
                return;
            }

            cursor.Index += 3;
            cursor.EmitDelegate<Func<Texture2D, Texture2D>>(initialTexture => SelectLavaTexture(initialTexture, true));

            if (!cursor.TryGotoNext(MoveType.After, c => c.MatchLdloc(155)))
            {
                LogFailure("Custom Lava Drawing", "Could not locate the liquid light color.");
                return;
            }

            // Pass the texture in so that the method can ensure it is not messing around with non-lava textures.
            cursor.Emit(OpCodes.Ldsfld, typeof(Main).GetField("liquidTexture"));
            cursor.Emit(OpCodes.Ldloc, 151);
            cursor.Emit(OpCodes.Ldelem_Ref);
            cursor.EmitDelegate<Func<Color, Texture2D, Color>>((initialColor, initialTexture) => SelectLavaColor(initialTexture, initialColor));
        }

        private static void DrawCustomLava2(ILContext il)
        {
            ILCursor cursor = new ILCursor(il);

            if (!cursor.TryGotoNext(c => c.MatchLdfld<LiquidRenderer>("_liquidTextures")))
            {
                LogFailure("Custom Lava Drawing", "Could not locate the liquid texture array load.");
                return;
            }

            cursor.Index += 3;
            cursor.EmitDelegate<Func<Texture2D, Texture2D>>(initialTexture => SelectLavaTexture(initialTexture, false));
            
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
            cursor.Emit(OpCodes.Ldloc, 12);
            cursor.EmitDelegate<Func<int, int>>(initialWaterfallStyle => CustomLavaManagement.SelectLavafallStyle(initialWaterfallStyle));
            cursor.Emit(OpCodes.Stloc, 12);

            cursor.Emit(OpCodes.Ldloc, 12);
            cursor.Emit(OpCodes.Ldloc, 51);
            cursor.EmitDelegate<Func<int, Color, Color>>((initialWaterfallStyle, initialLavafallColor) => CustomLavaManagement.SelectLavafallColor(initialWaterfallStyle, initialLavafallColor));
            cursor.Emit(OpCodes.Stloc, 51);
        }

        #endregion

        #region Damage and health balance
        private static void AdjustDamageVariance(ILContext il)
        {
            // Change the damage variance from +-15% to +-5%.
            var cursor = new ILCursor(il);
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcI4(-15))) // The -15% lower bound of the variance.
            {
                LogFailure("+/-5% Damage Variance", "Could not locate the lower bound.");
                return;
            }
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_I4, -5); // Increase to -5%.

            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcI4(16))) // The 15% upper bound of the variance.
            {
                LogFailure("+/-5% Damage Variance", "Could not locate the upper bound.");
                return;
            }
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_I4_6); // Decrease to +5%.
        }

        private static void RemoveExpertHardmodeScaling(ILContext il)
        {
            // Completely disable the weak enemy scaling that occurs when Hardmode is active in Expert Mode.
            var cursor = new ILCursor(il);
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcI4(1000))) // The less than 1000 HP check in order for the scaling to take place.
            {
                LogFailure("Expert Hardmode Scaling Removal", "Could not locate the HP check.");
                return;
            }
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_I4_M1); // Replace the 1000 with -1, no NPC can have less than -1 HP on spawn, so it fails to run.
        }

        private static void RemoveAerialBaneDamageBoost(ILContext il)
        {
            var cursor = new ILCursor(il);
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcI4(ProjectileID.DD2BetsyArrow))) // The ID of Aerial Bane projectiles.
            {
                LogFailure("Aerial Bane Nerf", "Could not locate the arrow ID.");
                return;
            }
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(1.5f))) // The damage multiplier.
            {
                LogFailure("Aerial Bane Nerf", "Could not locate the damage multiplier.");
                return;
            }
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_R4, 1f); // Multiplying by 1 means no damage bonus.
        }

        private static void AdjustChlorophyteBullets(ILContext il)
        {
            // Reduce dust from 10 to 5 and homing range.
            var cursor = new ILCursor(il);
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcI4(ProjectileID.ChlorophyteBullet)))
            {
                LogFailure("Chlorophyte Bullet AI", "Could not locate the bullet ID.");
                return;
            }
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcI4(10))) // The number of dust spawned by the bullet.
            {
                LogFailure("Chlorophyte Bullet AI", "Could not locate the dust quantity.");
                return;
            }
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_I4_5); // Decrease dust to 5.

            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(300f))) // The 300 unit distance required to home in.
            {
                LogFailure("Chlorophyte Bullet AI", "Could not locate the homing range.");
                return;
            }
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_R4, 150f); // Reduce homing range by 50%.
        }
		#endregion

		#region Movement speed balance
		private static void MaxRunSpeedAdjustment(ILContext il)
		{
			// Increase the base max run speed of the player to make early game less of a slog.
			var cursor = new ILCursor(il);
			if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(3f))) // The maxRunSpeed variable is set to this specific value before anything else occurs.
			{
				LogFailure("Base Max Run Speed Buff", "Could not locate the max run speed variable.");
				return;
			}
			cursor.Remove();
			cursor.Emit(OpCodes.Ldc_R4, 4.5f); // Increase by 50%.
		}

		private static void RunSpeedAdjustments(ILContext il)
		{
            var cursor = new ILCursor(il);
            float horizontalSpeedCap = 3f; // +200%, aka triple speed. Vanilla caps at +60%
            float asphaltTopSpeedMultiplier = 1.75f; // +75%. Vanilla is +250%
            float asphaltSlowdown = 1f; // Vanilla is 2f. This should actually make asphalt faster.

            // Multiplied by 0.6 on frozen slime, for +26% acceleration
            // Multiplied by 0.7 on ice, for +47% acceleration
            float iceSkateAcceleration = 2.1f;
            float iceSkateTopSpeed = 1f; // no boost at all

            //
            // HORIZONTAL MOVEMENT SPEED CAP
            //
            {
                // Find the +60% horizontal movement speed cap. This is loaded as a double.
                if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR8(1.6D)))
                {
                    LogFailure("Run Speed Adjustments", "Could not locate the horizontal movement speed cap.");
                    return;
                }

                // Replace it with the new, much higher cap.
                cursor.Remove();
                cursor.Emit(OpCodes.Ldc_R8, (double)horizontalSpeedCap);

                // Find the "replace your speed bonus with this if you're over the cap". This is loaded as a float.
                if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(1.6f)))
                {
                    LogFailure("Run Speed Adjustments", "Could not locate the horizontal movement speed cap replacement.");
                    return;
                }

                // Replace it with the new, much higher, cap.
                cursor.Remove();
                cursor.Emit(OpCodes.Ldc_R4, horizontalSpeedCap);
            }

            //
            // ASPHALT
            //
            {
                // Find the top speed multiplier of Asphalt.
                if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(3.5f)))
                {
                    LogFailure("Run Speed Adjustments", "Could not locate Asphalt's top speed multiplier.");
                    return;
                }

                // Massively reduce the increased speed cap of Asphalt.
                cursor.Remove();
                cursor.Emit(OpCodes.Ldc_R4, asphaltTopSpeedMultiplier);

                // Find the run slowdown multiplier of Asphalt.
                if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(2f)))
                {
                    LogFailure("Run Speed Adjustments", "Could not locate Asphalt's run slowdown multiplier.");
                    return;
                }

                // Reducing the slowdown actually makes the (slower) Asphalt more able to reach its top speed.
                cursor.Remove();
                cursor.Emit(OpCodes.Ldc_R4, asphaltSlowdown);
            }

            //
            // ICE SKATES + FROZEN SLIME BLOCKS
            //
            {
                // Find the acceleration multiplier of Ice Skates on Frozen Slime Blocks.
                if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(3.5f)))
                {
                    LogFailure("Run Speed Adjustments", "Could not locate Ice Skates + Frozen Slime Block acceleration multiplier.");
                    return;
                }

                // Massively reduce the acceleration bonus of Ice Skates on Frozen Slime Blocks.
                cursor.Remove();
                cursor.Emit(OpCodes.Ldc_R4, iceSkateAcceleration);

                // Find the top speed multiplier of Ice Skates on Frozen Slime Blocks.
                if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(1.25f)))
                {
                    LogFailure("Run Speed Adjustments", "Could not locate Ice Skates + Frozen Slime Block top speed multiplier.");
                    return;
                }

                // Make Ice Skates give no top speed boost whatsoever on Frozen Slime Blocks.
                cursor.Remove();
                cursor.Emit(OpCodes.Ldc_R4, iceSkateTopSpeed);
            }

            //
            // ICE SKATES + ICE BLOCKS
            //
            {
                // Find the acceleration multiplier of Ice Skates on Ice Blocks.
                if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(3.5f)))
                {
                    LogFailure("Run Speed Adjustments", "Could not locate Ice Skates + Ice Block acceleration multiplier.");
                    return;
                }

                // Massively reduce the acceleration bonus of Ice Skates on Ice Blocks.
                cursor.Remove();
                cursor.Emit(OpCodes.Ldc_R4, iceSkateAcceleration);

                // Find the top speed multiplier of Ice Skates on Ice Blocks.
                if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(1.25f)))
                {
                    LogFailure("Run Speed Adjustments", "Could not locate Ice Skates + Ice Block top speed multiplier.");
                    return;
                }

                // Make Ice Skates give no top speed boost whatsoever on Ice Blocks.
                cursor.Remove();
                cursor.Emit(OpCodes.Ldc_R4, iceSkateTopSpeed);
            }
        }

        private static void ReduceWingHoverVelocities(ILContext il)
        {
            // Reduce wing hover horizontal velocities. Hoverboard is fine because both stats are at 10.
            var cursor = new ILCursor(il);
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(6.25f))) // The accRunSpeed variable is set to this specific value before hover adjustments occur.
            {
                LogFailure("Wing Hover Nerfs", "Could not locate the base speed variable.");
                return;
            }
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(12f))) // The accRunSpeed for Vortex Booster and Nebula Mantle.
            {
                LogFailure("Wing Hover Nerfs", "Could not locate the vortex booster/nebula mantle speed variable.");
                return;
            }
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_R4, 10.8f); // Reduce by 10%.

            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(12f))) // The runAcceleration for Vortex Booster and Nebula Mantle.
            {
                LogFailure("Wing Hover Nerfs", "Could not locate the vortex booster/nebula mantle speed variable.");
                return;
            }
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_R4, 10.8f); // Reduce by 10%.

            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(12f))) // The accRunSpeed for Betsy Wings.
            {
                LogFailure("Wing Hover Nerfs", "Could not locate the betsy's wings speed variable.");
                return;
            }
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_R4, 10.8f); // Reduce by 10%.

            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(12f))) // The runAcceleration for Betsy Wings.
            {
                LogFailure("Wing Hover Nerfs", "Could not locate the betsy's wings speed variable.");
                return;
            }
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_R4, 10.8f); // Reduce by 10%.
        }
		#endregion

		#region Mana regen balance
		private static void ManaRegenDelayAdjustment(ILContext il)
		{
			// Decrease the max mana regen delay so that mage is less annoying to play without mana regen buffs.
			// Decreases the max mana regen delay from a range of 31.5 - 199.5 to 4 - 52.
			var cursor = new ILCursor(il);
			if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(45f))) // The flat amount added to max regen delay in the formula.
			{
				LogFailure("Max Mana Regen Delay Reduction", "Could not locate the max mana regen flat variable.");
				return;
			}
			cursor.Remove();
			cursor.Emit(OpCodes.Ldc_R4, 20f); // Decrease to 20f.

			if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(0.7f))) // The multiplier for max mana regen delay.
			{
				LogFailure("Max Mana Regen Delay Reduction", "Could not locate the max mana regen delay multiplier variable.");
				return;
			}
			cursor.Remove();
			cursor.Emit(OpCodes.Ldc_R4, 0.2f); // Decrease to 0.2f.
		}

		private static void ManaRegenAdjustment(ILContext il)
		{
			// Increase the base mana regen so that mage is less annoying to play without mana regen buffs.
			var cursor = new ILCursor(il);
			if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(0.8f))) // The multiplier for the mana regen formula: (float)statMana / (float)statManaMax2 * 0.8f + 0.2f.
			{
				LogFailure("Mana Regen Buff", "Could not locate the mana regen multiplier variable.");
				return;
			}
			cursor.Remove();
			cursor.Emit(OpCodes.Ldc_R4, 0.25f); // Decrease to 0.25f.

			if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(0.2f))) // The flat added mana regen amount.
			{
				LogFailure("Mana Regen Buff", "Could not locate the flat mana regen variable.");
				return;
			}
			cursor.Remove();
			cursor.Emit(OpCodes.Ldc_R4, 0.75f); // Increase to 0.75f.
		}
		#endregion

		#region World generation

		// Note: There is no need to replace the other Pharaoh piece, due to how the vanilla code works.
		private static void ReplacePharaohSetInPyramids(ILContext il)
        {
            var cursor = new ILCursor(il);

            // Determine the area which determines the held item.
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchStloc(36)))
            {
                LogFailure("Pharaoh Set Pyramid Replacement", "Could not locate the pyramid item selector value.");
                return;
            }

            int startOfItemSelection = cursor.Index;
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdloc(34)))
            {
                LogFailure("Pharaoh Set Pyramid Replacement", "Could not locate the pyramid loot room left position.");
                return;
            }
            int endOfItemSelection = cursor.Index;

            // And delete it completely with intent to replace it.
            // Nuances with compliation appear to make simple a load + remove by constant not work.
            cursor.Index = startOfItemSelection;
            cursor.RemoveRange(endOfItemSelection - startOfItemSelection);

            // And select the item type directly.
            cursor.Emit(OpCodes.Ldloc, 36);
            cursor.EmitDelegate<Func<int, int>>(choice =>
            {
                switch (choice)
                {
                    case 0:
                        return ItemID.SandstorminaBottle;

                    // TODO - Replace this with an amber hook in 1.4 to make more sense thematically.
                    case 1:
                        return ItemID.RubyHook;
                    case 2:
                    default:
                        return ItemID.FlyingCarpet;
                }
            });
            cursor.Emit(OpCodes.Stloc, 36);
        }

        private static void PreventDungeonHorizontalCollisions(ILContext il)
        {
            // Prevent the Dungeon from appearing near the Sulph sea.
            var cursor = new ILCursor(il);
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchStsfld<WorldGen>("dungeonY")))
            {
                LogFailure("Dungeon/Abyss Collision Avoidance (Starting Position)", "Could not locate the dungeon's vertical position.");
                return;
            }

            cursor.EmitDelegate<Action>(() =>
            {
                WorldGen.dungeonX = Utils.Clamp(WorldGen.dungeonX, SulphurousSea.BiomeWidth + 100, Main.maxTilesX - SulphurousSea.BiomeWidth - 100);

                // Adjust the Y position of the dungeon to accomodate for the X shift.
                WorldUtils.Find(new Point(WorldGen.dungeonX, WorldGen.dungeonY), Searches.Chain(new Searches.Down(9001), new Conditions.IsSolid()), out Point result);
                WorldGen.dungeonY = result.Y - 10;
            });
        }

        private static void PreventDungeonHallCollisions(ILContext il)
        {
            // Prevent the Dungeon's halls from getting anywhere near the Abyss.
            var cursor = new ILCursor(il);

            // Forcefully clamp the X position of the new hall end.
            // This prevents a hall, and as a result, the dungeon, from ever impeding on the Abyss/Sulph Sea.
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchStloc(6)))
            {
                LogFailure("Dungeon/Abyss Collision Avoidance (Halls)", "Could not locate the hall horizontal position.");
                return;
            }

            cursor.Emit(OpCodes.Ldloc, 6);
            cursor.EmitDelegate<Func<Vector2, Vector2>>(unclampedValue =>
            {
                unclampedValue.X = MathHelper.Clamp(unclampedValue.X, SulphurousSea.BiomeWidth + 25, Main.maxTilesX - SulphurousSea.BiomeWidth - 25);
                return unclampedValue;
            });
            cursor.Emit(OpCodes.Stloc, 6);
        }

        private static void BlockLivingTreesNearOcean(ILContext il)
        {
            var cursor = new ILCursor(il);
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.EmitDelegate<Func<int, int>>(x => Utils.Clamp(x, 560, Main.maxTilesX - 560));
            cursor.Emit(OpCodes.Starg, 0);
        }

        private static void PreventSmashAltarCode(On.Terraria.WorldGen.orig_SmashAltar orig, int i, int j)
        {
            if (CalamityConfig.Instance.EarlyHardmodeProgressionRework)
                return;

            orig(i, j);
        }

        private static void AdjustChlorophyteSpawnRate(ILContext il)
        {
            var cursor = new ILCursor(il);
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcI4(300))) // 1 in 300 genRand call used to generate Chlorophyte in mud tiles near jungle grass.
            {
                LogFailure("Chlorophyte Spread Rate", "Could not locate the update chance.");
                return;
            }
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_I4, 150); // Increase the chance to 1 in 150.
        }

        private static void AdjustChlorophyteSpawnLimits(ILContext il)
        {
            var cursor = new ILCursor(il);
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcI4(40))) // Find the 40 Chlorophyte tile limit. This limit is checked within a 71x71-tile square, with the reference tile as the center.
            {
                LogFailure("Chlorophyte Spread Limit", "Could not locate the lower limit.");
                return;
            }
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_I4, 60); // Increase the limit to 60.

            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcI4(130))) // Find the 130 Chlorophyte tile limit. This limit is checked within a 171x171-tile square, with the reference tile as the center.
            {
                LogFailure("Chlorophyte Spread Limit", "Could not locate the upper limit.");
                return;
            }
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_I4, 200); // Increase the limit to 200.
        }
        #endregion

        #region Removal of vanilla stupidity
        private static void RelaxPrefixRequirements(ILContext il)
        {
            var cursor = new ILCursor(il);

            // Search for the first instance of Math.Round, which is used to round damage.
            // This one isn't edited, but hitting the Round function is the easiest way to get to the relevant part of the method.
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchCall("System.Math", "Round")))
            {
                LogFailure("Prefix Requirements", "Could not locate the damage Math.Round call.");
                return;
            }

            // Search for the second instance of Math.Round, which is used to round use time.
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchCall("System.Math", "Round")))
            {
                LogFailure("Prefix Requirements", "Could not locate the use time Math.Round call.");
                return;
            }

            // Search for the branch-if-not-equal which checks whether the use time change rounds to nothing.
            // If the change rounds to nothing, then it's equal, so the branch is NOT taken.
            // The branch skips over the "fail this prefix" code.
            ILLabel passesUseTimeCheck = null;
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchBneUn(out passesUseTimeCheck)))
            {
                LogFailure("Prefix Requirements", "Could not locate use time rounding equality branch.");
                return;
            }

            // To allow use-time affecting prefixes even on super fast weapons where they would round to nothing,
            // add another branch which skips over the "fail this prefix" code, given a custom condition.

            // Load the item itself onto the stack so that it becomes an argument for the following delegate.
            cursor.Emit(OpCodes.Ldarg_0);

            // Emit a delegate which returns whether the item's use time is 2, 3, 4 or 5.
            cursor.EmitDelegate<Func<Item, bool>>((Item i) => i.useAnimation >= 2 && i.useAnimation <= 5);

            cursor.Emit(OpCodes.Brtrue_S, passesUseTimeCheck);

            // Search for the branch-if-not-equal which checks whether the mana change rounds to nothing.
            ILLabel passesManaCheck = null;
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchBneUn(out passesManaCheck)))
            {
                LogFailure("Prefix Requirements", "Could not locate mana prefix failure branch.");
                return;
            }

            // Emit an unconditional branch which skips the mana check failure.
            cursor.Emit(OpCodes.Br_S, passesManaCheck);

            // Search for the instance field load which retrieves the item's knockback.
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdfld<Item>("knockBack")))
            {
                LogFailure("Prefix Requirements", "Could not locate knockback load instruction.");
                return;
            }

            // Search for the immediately-following constant load which pulls in 0.0.
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(0f)))
            {
                LogFailure("Prefix Requirements", "Could not locate zero knockback comparison constant.");
                return;
            }

            // Completely nullify the knockback computation by replacing the check against 0 with a check against negative one million.
            // If you absolutely need to block knockback reforges for some reason, you can set your knockback to this value.
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_R4, -1000000f);
        }

        private static void PreventBossSlimeRainSpawns(On.Terraria.NPC.orig_SlimeRainSpawns orig, int plr)
        {
            if (!Main.player[plr].Calamity().bossZen)
                orig(plr);
        }

        // This may seem absolutely obscene, but vanilla spawn behavior does not occur within the spawn pool that TML provides, only modded
        // spawns do. Pretty much everything else is simply a fuckton of manual conditional NPC.NewNPC calls. As such, the only way to bypass
        // vanilla spawn behaviors is the IL edit them out of existence. Here, simply replacing the voodoo demon ID with an empty one is performed.
        // Something cleaner could probably be done, such as getting rid of the entire NPC.NewNPC call, but this is the easiest solution I can come up with.
        private static void MakeVoodooDemonDollWork(ILContext il)
        {
            var cursor = new ILCursor(il);

            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcI4(NPCID.VoodooDemon)))
            {
                LogFailure("Voodoo Demon Doll Mechanic", "Could not locate the Voodoo Demon ID.");
                return;
            }

            cursor.Remove();
            cursor.Emit(OpCodes.Ldloc, 6);
            cursor.EmitDelegate<Func<int, int>>(spawnPlayerIndex =>
            {
                if (Main.player[spawnPlayerIndex].active && Main.player[spawnPlayerIndex].Calamity().disableVoodooSpawns)
                    return NPCID.None;

                return NPCID.VoodooDemon;
            });
        }
        #endregion

        #region Vanilla bugs exposed by Calamity mechanics
        private static void BossRushLifeBytes(On.Terraria.Main.orig_InitLifeBytes orig)
        {
            orig();
            foreach (int npcType in NeedsFourLifeBytes)
                Main.npcLifeBytes[npcType] = 4;
        }

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
        #endregion

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

        private static Texture2D SelectLavaTexture(Texture2D initialTexture, bool blockTexture)
        {
            // Use the initial texture if it isn't lava.
            if (initialTexture != CustomLavaManagement.LavaTexture && initialTexture != CustomLavaManagement.LavaBlockTexture)
                return initialTexture;

            foreach (CustomLavaStyle lavaStyle in CustomLavaManagement.CustomLavaStyles)
            {
                if (lavaStyle.ChooseLavaStyle())
                    return blockTexture ? lavaStyle.BlockTexture : lavaStyle.LavaTexture;
            }

            return initialTexture;
        }

        private static Color SelectLavaColor(Texture2D initialTexture, Color initialLightColor)
        {
            // Use the initial color if it isn't lava.
            if (initialTexture != CustomLavaManagement.LavaTexture && initialTexture != CustomLavaManagement.LavaBlockTexture)
                return initialLightColor;

            foreach (CustomLavaStyle lavaStyle in CustomLavaManagement.CustomLavaStyles)
            {
                if (lavaStyle.ChooseLavaStyle())
                {
                    lavaStyle.SelectLightColor(ref initialLightColor);
                    return initialLightColor;
                }
            }

            return initialLightColor;
        }

        public static void DumpToLog(ILContext il) => CalamityMod.Instance.Logger.Debug(il.ToString());
        public static void LogFailure(string name, string reason) => CalamityMod.Instance.Logger.Warn($"IL edit \"{name}\" failed! {reason}");
        #endregion
    }
}
