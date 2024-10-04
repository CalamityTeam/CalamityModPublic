using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using CalamityMod.Balancing;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer;
using CalamityMod.DataStructures;
using CalamityMod.Events;
using CalamityMod.FluidSimulation;
using CalamityMod.ForegroundDrawing;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Accessories.Vanity;
using CalamityMod.Items.Dyes;
using CalamityMod.Items.Potions.Alcohol;
using CalamityMod.NPCs;
using CalamityMod.NPCs.Astral;
using CalamityMod.NPCs.AstrumAureus;
using CalamityMod.NPCs.Crabulon;
using CalamityMod.NPCs.Ravager;
using CalamityMod.Particles;
using CalamityMod.Projectiles;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.Systems;
using CalamityMod.Tiles.Abyss;
using CalamityMod.Waters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Achievements;
using Terraria.GameContent.Drawing;
using Terraria.GameContent.Events;
using Terraria.GameContent.Liquid;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.Graphics;
using Terraria.Graphics.Capture;
using Terraria.Graphics.Light;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI.Gamepad;
using static Terraria.WaterfallManager;

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

        //private static readonly MethodInfo textureGetValueMethod = typeof(Asset<Texture2D>).GetMethod("get_Value", BindingFlags.Public | BindingFlags.Instance);

        public static event Func<VertexColors, int, Point, VertexColors> ExtraColorChangeConditions;

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

        private static void ApplyDashKeybind(Terraria.On_Player.orig_DoCommonDashHandle orig, Player self, out int dir, out bool dashing, Player.DashStartAction dashStartAction)
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
                if (self.dashTime > 0)
                    self.dashTime--;
                if (self.dashTime < 0)
                    self.dashTime++;

                if ((self.dashTime <= 0 && self.direction == -1) || (self.dashTime >= 0 && self.direction == 1))
                {
                    self.dashTime = 15;
                    return;
                }

                dashing = true;
                self.dashTime = 0;
                self.timeSinceLastDashStarted = 0;
                dashStartAction?.Invoke(dir);
                return;
            }

            if (CalamityKeybinds.DashHotkey.GetAssignedKeys().Count == 0)
                orig(self, out dir, out dashing, dashStartAction);
            else
            {
                dir = 1;
                dashing = false;
            }
        }
        #endregion

        #region Allow Empress to Enrage in Boss Rush
        private static bool AllowEmpressToEnrageInBossRush(Terraria.On_NPC.orig_ShouldEmpressBeEnraged orig)
        {
            if (BossRushEvent.BossRushActive)
                return true;

            return orig();
        }
        #endregion

        #region Enabling of Triggered NPC Platform Fallthrough
        // Why this isn't a mechanism provided by TML itself or vanilla itself is beyond me.
        private static void AllowTriggeredFallthrough(Terraria.On_NPC.orig_ApplyTileCollision orig, NPC self, bool fall, Vector2 cPosition, int cWidth, int cHeight)
        {
            if (self.active && self.type == ModContent.NPCType<FusionFeeder>())
            {
                self.velocity = Collision.AdvancedTileCollision(TileID.Sets.ForAdvancedCollision.ForSandshark, cPosition, self.velocity, cWidth, cHeight, fall, fall, 1);
                return;
            }
            var isNpcValid = self.TryGetGlobalNPC(out CalamityGlobalNPC npc); //why the fuck this errors is anybody's guess, it absolutely shouldn't and yet it does
            if (isNpcValid && self.active && npc.ShouldFallThroughPlatforms)
                fall = true;

            orig(self, fall, cPosition, cWidth, cHeight);
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

        private static void AlterTownNPCSpawnRate(Terraria.On_Main.orig_UpdateTime_SpawnTownNPCs orig)
        {
            double oldWorldRate = Main.desiredWorldTilesUpdateRate;
            Main.desiredWorldTilesUpdateRate *= CalamityConfig.Instance.TownNPCSpawnRateMultiplier;
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
        #endregion

        #region Custom Gate Door Logic
        private static bool OpenDoor_LabDoorOverride(Terraria.On_WorldGen.orig_OpenDoor orig, int i, int j, int direction)
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

        private static bool CloseDoor_LabDoorOverride(Terraria.On_WorldGen.orig_CloseDoor orig, int i, int j, bool forced)
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

        #region Platform Collision Checks for Grounded Bosses
        private static bool EnableCalamityBossPlatformCollision(Terraria.On_NPC.orig_Collision_DecideFallThroughPlatforms orig, NPC self)
        {
            if ((self.type == ModContent.NPCType<AstrumAureus>() || self.type == ModContent.NPCType<Crabulon>() || self.type == ModContent.NPCType<RavagerBody>() ||
                self.type == ModContent.NPCType<RockPillar>() || self.type == ModContent.NPCType<FlamePillar>()) &&
                self.target >= 0 && Main.player[self.target].position.Y > self.position.Y + self.height)
                return true;

            return orig(self);
        }
        #endregion

        #region Incorporate Enchantments in Item Names
        private static string IncorporateEnchantmentInAffix(Terraria.On_Item.orig_AffixName orig, Item self)
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

        #region Hellbound Enchantment Projectile Creation Effects
        private static int IncorporateMinionExplodingCountdown(Terraria.On_Projectile.orig_NewProjectile_IEntitySource_float_float_float_float_int_int_float_int_float_float_float orig, IEntitySource spawnSource, float x, float y, float xSpeed, float ySpeed, int type, int damage, float knockback, int owner, float ai0, float ai1, float ai2)
        {
            // This is unfortunately not something that can be done via SetDefaults since owner is set
            // after that method is called. Doing it directly when the projectile is spawned appears to be the only reasonable way.
            int proj = orig(spawnSource, x, y, xSpeed, ySpeed, type, damage, knockback, owner, ai0, ai1, ai2);
            Projectile projectile = Main.projectile[proj];
            if (projectile.minion)
            {
                Player player = Main.player[projectile.owner];
                if (Main.gameMenu || !player.active)
                    return proj;

                // Do not apply Hellbound effects to minions not spawned by the item itself, if it came out of an item
                // This prevent minions like Luxor's Gift getting it, but minions spawned out of minions such as Temporal Umbrella will work fine
                if (spawnSource is EntitySource_ItemUse && player.ActiveItem().shoot != projectile.type)
                    return proj;

                CalamityPlayer.EnchantHeldItemEffects(player, player.Calamity(), player.ActiveItem());
                if (player.Calamity().explosiveMinionsEnchant)
                    projectile.Calamity().ExplosiveEnchantCountdown = CalamityGlobalProjectile.ExplosiveEnchantTime;
            }
            return proj;
        }
        #endregion

        #region Chaos Stone and Chalice of the Blood God
        private static void ManaSicknessAndChaliceBufferHeal(ILContext il)
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

                if (modPlayer.chaliceOfTheBloodGod && modPlayer.chaliceBleedoutBuffer > 0D)
                {
                    // 20FEB2024: Ozzatron: to prevent abuse, buffer clearing is now 50% of the potion instead of 50% of your buffer
                    float amountOfBleedToClear = ChaliceOfTheBloodGod.HealingPotionRatioForBufferClear * player.GetHealLife(potion, true);

                    // Actually clear the buffer
                    modPlayer.chaliceBleedoutBuffer -= amountOfBleedToClear;

                    // Display text indicating that healing was applied to the bleedout buffer.
                    if (Main.netMode != NetmodeID.Server)
                    {
                        string text = $"(+{amountOfBleedToClear})";
                        Rectangle location = new Rectangle((int)player.position.X + 4, (int)player.position.Y - 3, player.width - 4, player.height - 4);
                        CombatText.NewText(location, ChaliceOfTheBloodGod.BleedoutBufferDamageTextColor, Language.GetTextValue(text), dot: true);
                    }
                }
            });

            //
            // The following section enables Mana Burn for Chaos Stone by conditionally replacing Mana Sickness.
            //

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
        #endregion

        #region Fire Cursor Effect for the Calamity Accessory
        private static void UseCoolFireCursorEffect(Terraria.On_Main.orig_DrawCursor orig, Vector2 bonus, bool smart)
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

        #region Custom Draw Layers
        private static void AdditiveDrawing(ILContext il)
        {
            ILCursor cursor = new(il);
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchCall<MoonlordDeathDrama>("DrawWhite")))
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
        private static void DrawFusableParticles(Terraria.On_Main.orig_SortDrawCacheWorms orig, Main self)
        {
            DeathAshParticle.DrawAll();

            if (Main.LocalPlayer.dye.Any(dyeItem => dyeItem.type == ModContent.ItemType<ProfanedMoonlightDye>()))
                Main.LocalPlayer.Calamity().ProfanedMoonlightAuroraDrawer?.Draw(Main.LocalPlayer.Center - Main.screenPosition, false, Main.GameViewMatrix.TransformationMatrix, Matrix.Identity);

            orig(self);
        }

        private static void DrawForegroundParticles(Terraria.On_Main.orig_DrawInfernoRings orig, Main self)
        {
            GeneralParticleHandler.DrawAllParticles(Main.spriteBatch);
            orig(self);
        }
        #endregion

        #region Lava Style Edits
        //Nine Layers of Hell brought to you by LIONEIGHTCAKE!!
        //All new liquid rendering for lava can be found in Systems/LavaRendering.cs

        //To save on time, a LOT of the edits are from the BiomeLava mod (owned by myself, lion8cake). Obv every edit has been changed to reflect
        //Calamity's style of IL edits and to remove any unnessesary edits not relivant to calamity. In the loading of the detours and edits
        //BiomeLava is a check for whether they load or not. If BiomeLava is active all ModLavaStyles will be turned into ModCalls for BiomeLava.

        #region Lava Rendering
        private void DoDrawLavas(ILContext il)
        {
            ILCursor cursor = new ILCursor(il);
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchLdsfld<Main>("drawToScreen"), i => i.MatchBrfalse(out _), i => i.MatchLdarg0(), i => i.MatchLdcI4(1), i => i.MatchCall<Main>("DrawWaters")))
            {
                LogFailure("DoDraw Lava", "Could not locate the drawing of Background Waters");
                return;
            }
            cursor.EmitDelegate(() => {
                LavaRendering.instance.DrawLavas(isBackground: true);
            });
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchLdsfld<Main>("drawToScreen"), i => i.MatchBrfalse(out _), i => i.MatchLdarg0(), i => i.MatchLdcI4(0), i => i.MatchCall<Main>("DrawWaters")))
            {
                LogFailure("DoDraw Lava", "Could not locate the drawing of Waters");
                return;
            }
            cursor.EmitDelegate(() => {
                LavaRendering.instance.DrawLavas();
            });
        }

        private void RenderLavas(ILContext il)
        {
            ILCursor cursor = new ILCursor(il);
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchLdarg0(), i => i.MatchLdcI4(0), i => i.MatchCall<Main>("DrawWaters")))
            {
                LogFailure("Render Lava", "Could not locate the drawing of Waters");
                return;
            }
            cursor.EmitDelegate(() => {
                LavaRendering.instance.DrawLavas();
            });
        }

        private void RenderLavaBackgrounds(ILContext il)
        {
            ILCursor cursor = new ILCursor(il);
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchLdarg0(), i => i.MatchLdcI4(1), i => i.MatchCall<Main>("DrawWaters")))
            {
                LogFailure("Render Lava Backgroumds", "Could not locate the drawing of Background Waters");
                return;
            }
            cursor.EmitDelegate(() => {
                LavaRendering.instance.DrawLavas(isBackground: true);
            });
        }

        private void DrawLavatoCapture(ILContext il)
        {
            ILCursor cursor = new ILCursor(il);
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchLdsfld<Main>("liquidAlpha"), i => i.MatchCall(out _), i => i.MatchStloc2()))
            {
                LogFailure("Draw lavas to captures", "Could not locate the saving of water alphas");
                return;
            }
            cursor.EmitDelegate(() => {
                LavaRendering.alphaSave = CalamityMod.lavaAlpha.ToArray();
            });
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcI4(0), i => i.MatchStloc(34), i => i.MatchBr(out _), i => i.MatchLdloc(34), i => i.MatchLdcI4(1), i => i.MatchBeq(out _)))
            {
                LogFailure("Draw lavas to captures", "Could not locate the resetting of water alphas");
                return;
            }
            cursor.EmitLdloc(8);
            cursor.EmitDelegate((CaptureBiome biome) => {
                for (int i = 0; i < 1; i++)
                {
                    CalamityMod.lavaAlpha[i] = ((i == CalamityMod.LavaStyle) ? 1f : 0f);
                }
            });
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchLdarg0(), i => i.MatchLdcI4(1), i => i.MatchLdsfld<Main>("waterStyle"), i => i.MatchLdcR4(1), i => i.MatchLdcI4(1), i => i.MatchCall<Main>("DrawLiquid")))
            {
                LogFailure("Draw lavas to captures", "Could not locate the background of liquid capture drawing");
                return;
            }
            cursor.EmitDelegate(() => {
                LavaRendering.instance.DrawLiquid(bg: true, CalamityMod.LavaStyle);
            });
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchLdarg0(), i => i.MatchLdcI4(1), i => i.MatchLdsfld<Main>("bloodMoon"), i => i.MatchBrtrue(out _), i => i.MatchLdloc(8), i => i.MatchLdfld<CaptureBiome>("WaterStyle"), i => i.MatchBr(out _), i => i.MatchLdcI4(9), i => i.MatchLdcR4(1), i => i.MatchLdcI4(1), i => i.MatchCall<Main>("DrawLiquid")))
            {
                LogFailure("Draw lavas to captures", "Could not locate the second background of liquid capture drawing");
                return;
            }
            cursor.EmitDelegate(() => {
                LavaRendering.instance.DrawLiquid(bg: true, CalamityMod.LavaStyle);
            });
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchLdarg0(), i => i.MatchLdcI4(0), i => i.MatchLdsfld<Main>("waterStyle"), i => i.MatchLdcR4(1), i => i.MatchLdcI4(1), i => i.MatchCall<Main>("DrawLiquid")))
            {
                LogFailure("Draw lavas to captures", "Could not locate the liquid capture drawing");
                return;
            }
            cursor.EmitDelegate(() => {
                LavaRendering.instance.DrawLiquid(bg: false, CalamityMod.LavaStyle);
            });
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchLdarg0(), i => i.MatchLdcI4(0), i => i.MatchLdloc(8), i => i.MatchLdfld<CaptureBiome>("WaterStyle"), i => i.MatchLdcR4(1), i => i.MatchLdcI4(1), i => i.MatchCall<Main>("DrawLiquid")))
            {
                LogFailure("Draw lavas to captures", "Could not locate the second liquid capture drawing");
                return;
            }
            cursor.EmitDelegate(() => {
                LavaRendering.instance.DrawLiquid(bg: false, CalamityMod.LavaStyle);
            });
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchLdloc2(), i => i.MatchStsfld<Main>("liquidAlpha")))
            {
                LogFailure("Draw lavas to captures", "Could not locate water style value returner");
                return;
            }
            cursor.EmitDelegate(() => {
                CalamityMod.lavaAlpha = LavaRendering.alphaSave;
            });
        }

        private void AddTileLiquidDrawing(ILContext il)
        {
            ILCursor cursor = new ILCursor(il);
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchLdarg0(), i => i.MatchLdarg1(), i => i.MatchLdcI4(0), i => i.MatchLdarg(out _), i => i.MatchLdloc1(), i => i.MatchLdloc2(), i => i.MatchLdloc(12), i => i.MatchLdloc(13), i => i.MatchLdloc(14), i => i.MatchCall<TileDrawing>("DrawTile_LiquidBehindTile")))
            {
                LogFailure("Tile Lava Drawing", "Could not locate the drawing of Liquid Behind Tile drawing");
                return;
            }
            cursor.EmitLdloc1();
            cursor.EmitLdloc2();
            cursor.EmitLdloc(12);
            cursor.EmitLdloc(13);
            cursor.EmitLdloc(14);
            cursor.EmitDelegate((Microsoft.Xna.Framework.Vector2 unscaledPosition, Microsoft.Xna.Framework.Vector2 vector, int j, int i, Terraria.Tile tile) => {
                LavaRendering.instance.DrawTile_LiquidBehindTile(solidLayer: false, inFrontOfPlayers: false, -1, unscaledPosition, vector, j, i, tile);
            });
        }
        #endregion
        
        #region Lava Blocking
        private void BlockLavaDrawing(ILContext il)
        {
            //This edit to DrawNormalLiquids makes lavas in normal and white lighting draw with an alpha and with new textures
            //If the parameter for the waterstyle is more than the max waterstyles then its subtracted by the max water style count and thats the lava style ID
            ILCursor cursor = new ILCursor(il);

            //Continue if statement, basically
            //if the liquid being drawn is lava and the water style is greater than the max water styles or if the liquid is water and less than the max water styles then the draw code is ran
            //otherwise the loop/s are continued for the next liquid
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchStloc(4), i => i.MatchBr(out _), i => i.MatchLdloc(2), i => i.MatchLdfld(typeof(LiquidRenderer).GetNestedType("LiquidDrawCache", BindingFlags.NonPublic), "IsVisible")))
            {
                LogFailure("Liquid Renderer Drawing", "Could not locate the IsVisible boolean check");
                return;
            }
            cursor.EmitLdarg3();
            cursor.EmitLdloc2(); //Initiated Liquid Draw Cache (needed for the Type parameter)
            cursor.EmitLdfld(typeof(LiquidRenderer).GetNestedType("LiquidDrawCache", BindingFlags.NonPublic).GetRuntimeField("Type"));
            cursor.EmitDelegate<Func<bool, int, int, bool>>((IsVisible, style, type) => IsVisible && ((type == 1 && style >= LavaRendering.instance.WaterStyleMaxCount + 1) || (type != 1 && style <= LavaRendering.instance.WaterStyleMaxCount)));

            //Lava alpha color, if the liquid drawn is lava, multiply num by the water alpha
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdloc(2), i => i.MatchLdfld(typeof(LiquidRenderer).GetNestedType("LiquidDrawCache", BindingFlags.NonPublic).GetRuntimeField("Type")), i => i.MatchStloc(8)))
            {
                LogFailure("Liquid Renderer Drawing", "Could not locate creation of the local variable num2 (the liquid type holder variable)");
                return;
            }
            cursor.EmitLdloc(8);
            cursor.EmitLdloca(7);
            cursor.EmitLdarg(4);
            cursor.EmitDelegate((int num2, ref float num, float globalAlpha) =>
            {
                if (num2 == LiquidID.Lava)
                {
                    num *= globalAlpha;
                }
            });

            //Conditionally replace the liquid texture whether the liquid is lava or water
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchCallvirt(typeof(Asset<Texture2D>).GetMethod("get_Value", BindingFlags.Public | BindingFlags.Instance))))
            {
                LogFailure("Liquid Renderer Drawing", "Could not locate the Texture2D array of liquids");
                return;
            }
            cursor.EmitLdarg3();
            cursor.EmitLdloc2(); //Initiated Liquid Draw Cache (needed for the Type parameter)
            cursor.EmitLdfld(typeof(LiquidRenderer).GetNestedType("LiquidDrawCache", BindingFlags.NonPublic).GetRuntimeField("Type"));
            cursor.EmitDelegate<Func<Texture2D, int, int, Texture2D>>((initialTexture, style, type) => (style >= LavaRendering.instance.WaterStyleMaxCount + 1 && type == LiquidID.Lava) ? CalamityMod.LavaTextures.liquid[style - LavaRendering.instance.WaterStyleMaxCount - 1].Value : initialTexture);
        }

        private void BlockLavaDrawingForSlopes(On_TileDrawing.orig_DrawTile_LiquidBehindTile orig, TileDrawing self, bool solidLayer, bool inFrontOfPlayers, int waterStyleOverride, Vector2 screenPosition, Vector2 screenOffset, int tileX, int tileY, Tile tileCache)
        {
            Tile tile = Main.tile[tileX + 1, tileY];
            Tile tile2 = Main.tile[tileX - 1, tileY];
            Tile tile3 = Main.tile[tileX, tileY - 1];
            Tile tile4 = Main.tile[tileX, tileY + 1];
            if (tileCache.LiquidType == LiquidID.Lava || tile.LiquidType == LiquidID.Lava || tile2.LiquidType == LiquidID.Lava || tile3.LiquidType == LiquidID.Lava || tile4.LiquidType == LiquidID.Lava)
            {
                return;
            }
            orig.Invoke(self, solidLayer, inFrontOfPlayers, waterStyleOverride, screenPosition, screenOffset, tileX, tileY, tileCache);
        }

        private void BlockLavaDrawingForSlopes2(On_TileDrawing.orig_DrawPartialLiquid orig, TileDrawing self, bool behindBlocks, Tile tileCache, ref Vector2 position, ref Rectangle liquidSize, int liquidType, ref VertexColors colors)
        {
            if (liquidType == 1)
            {
                return;
            }
            orig.Invoke(self, behindBlocks, tileCache, ref position, ref liquidSize, liquidType, ref colors);
        }

        private void LavafallRemover(On_WaterfallManager.orig_DrawWaterfall_int_int_int_float_Vector2_Rectangle_Color_SpriteEffects orig, WaterfallManager self, int waterfallType, int x, int y, float opacity, Vector2 position, Rectangle sourceRect, Color color, SpriteEffects effects)
        {
            if (waterfallType == 1)
            {
                return;
            }
            orig.Invoke(self, waterfallType, x, y, opacity, position, sourceRect, color, effects);
        }

        private void BlockRetroLightingLava(ILContext il)
        {
            ILCursor cursor = new ILCursor(il);
            ILLabel target = cursor.DefineLabel();
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchCgt(), i => i.MatchLdarg1(), i => i.MatchOr(), i => i.MatchBrfalse(out target)))
            {
                LogFailure("old Drawing Waters", "Could not locate the if statement that contains the check for liquid types, amounts and wether the liquid has a bg (arg1) or not");
                return;
            }
            if (target == null)
            {
                LogFailure("old Drawing Waters", "The BrFalse returned null, the boolean check getting was unsuccessful");
                return;
            }
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchLdindU1(), i => i.MatchSub(), i => i.MatchConvR4(), i => i.MatchStloc(14)))
            {
                LogFailure("old Drawing Waters", "Could not locate the num3 local variable creation to put the if check after");
                return;
            }
            cursor.EmitLdloc(12);
            cursor.EmitLdloc(11);
            cursor.EmitDelegate((int i, int j) => {
                return Main.tile[i, j].LiquidType == LiquidID.Lava;
            });
            cursor.EmitBrtrue(target);
        }
        #endregion
        
        #region Lava Replacing
        private void LavaBubbleReplacer(ILContext il)
        {
            ILCursor cursor = new ILCursor(il);
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchLdcI4(16), i => i.MatchLdcI4(16), i => i.MatchLdcI4(35)))
            {
                LogFailure("Ambient lava bubble replacer", "Could not locate the bubble newdust parameters");
                return;
            }
            cursor.EmitDelegate<Func<int, int>>(type => LavaRendering.dustLava());
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchLdcI4(16), i => i.MatchLdcI4(8), i => i.MatchLdcI4(35)))
            {
                LogFailure("Ambient lava bubble replacer", "Could not locate the surface bubble newdust parameters");
                return;
            }
            cursor.EmitDelegate<Func<int, int>>(type2 => LavaRendering.dustLava());
        }
        
        private void LavaDropletReplacer(ILContext il)
		{
			ILCursor cursor = new ILCursor(il);
			if (!cursor.TryGotoNext(MoveType.After, i => i.MatchLdarg(out _), i => i.MatchLdcI4(374), i => i.MatchBneUn(out _), i => i.MatchLdcI4(716)))
            {
                LogFailure("Ambient lava droplet replacer", "Could not locate the lava droplet newgore parameters");
                return;
            }
			cursor.EmitDelegate<Func<int, int>>(type => LavaRendering.goreLava());
		}

        private void SplashEntityLava(ILContext il)
        {
            ILCursor cursor = new ILCursor(il);
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchLdfld<Entity>("width"), i => i.MatchLdcI4(12), i => i.MatchAdd(), i => i.MatchLdcI4(24), i => i.MatchLdcI4(35)))
            {
                LogFailure("Entity Lava Splashing (Item, Projectile, NPC, Player)", "Could not locate the first lava bubble splashing");
                return;
            }
            cursor.EmitDelegate<Func<int, int>>(type => LavaRendering.dustLava());
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchLdfld<Entity>("width"), i => i.MatchLdcI4(12), i => i.MatchAdd(), i => i.MatchLdcI4(24), i => i.MatchLdcI4(35)))
            {
                LogFailure("Entity Lava Splashing (Item, Projectile, NPC, Player)", "Could not locate the second lava bubble splashing");
                return;
            }
            cursor.EmitDelegate<Func<int, int>>(type2 => LavaRendering.dustLava());
        }

        private void PlayerDebuffEdit(ILContext il)
        {
            //Injects code directly at the position where the OnFire debuff is handled
            ILCursor cursor = new ILCursor(il);
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdarg0(), i => i.MatchLdcI4(24), i => i.MatchLdloc(161), i => i.MatchLdcI4(1), i => i.MatchLdcI4(0), i => i.MatchCall<Player>("AddBuff")))
            {
                LogFailure("Player Update Lava Debuff", "Could not locate the infliction of the On Fire! debuff inside the Player Update code");
                return;
            }
            cursor.EmitLdarg0();
            cursor.EmitLdloc(161);
            cursor.EmitDelegate((Player player, int onFiretime) =>
            {
                LavaStylesLoader.InflictDebuff(player, CalamityMod.LavaStyle, onFiretime);
            });
        }
        #endregion

        #region Other
        private Color WaterfallGlowmaskEditor(On_WaterfallManager.orig_StylizeColor orig, float alpha, int maxSteps, int waterfallType, int y, int s, Tile tileCache, Color aColor)
        {
            if (CalamityMod.LavaStyle != 0 && !LavaStylesLoader.Get(CalamityMod.LavaStyle).LavafallGlowmask())
            {
                return aColor;
            }
            else
            {
                return orig.Invoke(alpha, maxSteps, waterfallType, y, s, tileCache, aColor);
            }
        }

        private void LavaFallRedrawer(On_WaterfallManager.orig_Draw orig, WaterfallManager self, SpriteBatch spriteBatch)
        {
            orig.Invoke(self, spriteBatch);
            LavaRendering.instance.InitialDrawLavafall(self);
        }
        #endregion

        #endregion

        #region Liquid Visuals
        //Contains all liquid light and liquid alpha (seethrough-ability)
        private void LiquidEmitLight(On_TileLightScanner.orig_ApplyLiquidLight orig, TileLightScanner self, Tile tile, ref Vector3 lightColor)
        {
            orig.Invoke(self, tile, ref lightColor);
            if (tile.LiquidAmount > 0 && !tile.HasTile)
            {
                if (tile.LiquidType == LiquidID.Water)
                {
                    float R = 0f;
                    float G = 0f;
                    float B = 0f;
                    CalamityWaterLoader.ModifyLightSetup(tile.X(), tile.Y(), Main.waterStyle, ref R, ref G, ref B);
                    if (lightColor.X < R)
                    {
                        lightColor.X = R;
                    }
                    if (lightColor.Y < G)
                    {
                        lightColor.Y = G;
                    }
                    if (lightColor.Z < B)
                    {
                        lightColor.Z = B;
                    }
                }
                else if (tile.LiquidType == LiquidID.Lava && CalamityMod.Instance.biomeLava == null)
                {
                    Vector3 lavaLight = new Vector3(0.55f, 0.33f, 0.11f);
                    float R = CalamityMod.LavaStyle == 0 ? lavaLight.X : 0f;
                    float G = CalamityMod.LavaStyle == 0 ? lavaLight.Y : 0f;
                    float B = CalamityMod.LavaStyle == 0 ? lavaLight.Z : 0f;
                    LavaStylesLoader.ModifyLightSetup(tile.X(), tile.Y(), CalamityMod.LavaStyle, ref R, ref G, ref B);
                    for (int j = 0; j < LavaStylesLoader.TotalCount; j++)
                    {
                        if (CalamityMod.lavaAlpha[j] > 0f && j != CalamityMod.LavaStyle)
                        {
                            float r = j == 0 ? lavaLight.X : 0f;
                            float g = j == 0 ? lavaLight.Y : 0f;
                            float b = j == 0 ? lavaLight.Z : 0f;
                            LavaStylesLoader.ModifyLightSetup(tile.X(), tile.Y(), j, ref r, ref g, ref b);
                            float r2 = CalamityMod.LavaStyle == 0 ? lavaLight.X : 0f;
                            float g2 = CalamityMod.LavaStyle == 0 ? lavaLight.Y : 0f;
                            float b2 = CalamityMod.LavaStyle == 0 ? lavaLight.Z : 0f;
                            LavaStylesLoader.ModifyLightSetup(tile.X(), tile.Y(), CalamityMod.LavaStyle, ref r2, ref g2, ref b2);
                            R = Single.Lerp(r, r2, CalamityMod.lavaAlpha[CalamityMod.LavaStyle]);
                            G = Single.Lerp(g, g2, CalamityMod.lavaAlpha[CalamityMod.LavaStyle]);
                            B = Single.Lerp(b, b2, CalamityMod.lavaAlpha[CalamityMod.LavaStyle]);
                        }
                    }
                    if (!(R == 0 && G == 0 && B == 0))
                    {
                        float colorManipulator = (float)(270 - Main.mouseTextColor) / 900f;
                        R += colorManipulator;
                        G += colorManipulator;
                        B += colorManipulator;
                    }
                    if (lightColor.X < R)
                    {
                        lightColor.X = R;
                    }
                    if (lightColor.Y < G)
                    {
                        lightColor.Y = G;
                    }
                    if (lightColor.Z < B)
                    {
                        lightColor.Z = B;
                    }
                }
            }
        }

        private void LavafallLightEditor(On_WaterfallManager.orig_AddLight orig, int waterfallType, int x, int y)
        {
            if (waterfallType == 1)
            {
                float r = 0.55f;
                float g = 0.33f;
                float b = 0.11f;
                LavaStylesLoader.ModifyLightSetup(x, y, CalamityMod.LavaStyle, ref r, ref g, ref b);
                if (!(r == 0 && g == 0 && b == 0))
                {
                    float r8;
                    float num3 = (r8 = (r + (float)(270 - Main.mouseTextColor) / 900f) * 0.4f);
                    float g8 = num3 * g;
                    float b8 = num3 * b;
                    Lighting.AddLight(x, y, r8, g8, b8);
                }
                return;
            }
            orig.Invoke(waterfallType, x, y);
        }

        private static void LiquidDrawColors(ILContext il)
        {
            ILCursor cursor = new ILCursor(il);
            if (!cursor.TryGotoNext(MoveType.Before, c => c.MatchLdarg2(), c => c.MatchLdloc3(), c => c.MatchLdloc(4), c => c.MatchCall<Main>("DrawTileInWater")))
            {
                LogFailure("Liquid Draw Colors", "Could not locate the liquid vertex colors for drawing");
                return;
            }

            cursor.Emit(OpCodes.Ldloc_3);
            cursor.Emit(OpCodes.Ldloc, 4);
            cursor.Emit(OpCodes.Ldloc_2);
            cursor.Emit(OpCodes.Ldfld, typeof(LiquidRenderer).GetNestedType("LiquidDrawCache", BindingFlags.NonPublic).GetRuntimeField("Type"));
            cursor.Emit(OpCodes.Ldloca, 9);

            cursor.EmitDelegate((int x, int y, int liquidType, ref VertexColors initialColor) =>
            {
                if (liquidType == LiquidID.Water)
                {
                    CalamityWaterLoader.DrawColorSetup(x, y, Main.waterStyle, ref initialColor);
                }
                else if (liquidType == LiquidID.Lava && CalamityMod.Instance.biomeLava == null)
                {
                    LavaStylesLoader.DrawColorSetup(x, y, CalamityMod.LavaStyle, ref initialColor);
                }
            });
        }

        private static void LiquidSlopeDrawColors(ILContext il)
        {
            ILCursor cursor = new ILCursor(il);

            if (!cursor.TryGotoNext(MoveType.Before, c => c.MatchLdcI4(0), c => c.MatchStloc(19), c => c.MatchLdloc(11), c => c.MatchBrfalse(out _)))
            {
                LogFailure("Liquid Slope Draw Colors", "Could not locate the liquid slope vertex colors for drawing");
                return;
            }

            cursor.Emit(OpCodes.Ldarg, 6);
            cursor.Emit(OpCodes.Ldarg, 7);
            cursor.Emit(OpCodes.Ldloca, 14);
            cursor.Emit(OpCodes.Ldloc, 11);
            cursor.Emit(OpCodes.Ldloc, 10);

            cursor.EmitDelegate((int x, int y, ref VertexColors initialColor, bool flag6, int num2) =>
            {
                if (flag6)
                {
                    int totalCount = (int)typeof(Loader).GetProperty("TotalCount", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static).GetValue(LoaderManager.Get<WaterStylesLoader>());
                    for (int i = 0; i < totalCount; i++)
                    {
                        Tile tile = Main.tile[x, y];
                        if (i == Main.waterStyle && tile.LiquidType == LiquidID.Water)
                        {
                            CalamityWaterLoader.DrawColorSetup(x, y, Main.waterStyle, ref initialColor, true);
                        }
                        else if ((tile.LiquidType == LiquidID.Lava || i == 1) && CalamityMod.Instance.biomeLava == null)
                        {
                            LavaStylesLoader.DrawColorSetup(x, y, CalamityMod.LavaStyle, ref initialColor);
                        }
                    }
                }
            });
        }
        #endregion

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
                    Array.Resize(ref arr, arr.Length + 1);
                    arr[arr.Length - 1] = (short)ModContent.NPCType<Twinkler>();
                    return arr;
                });

                // hook applied successfully
                return;
            }

            // couldn't find the right place to insert
            throw new Exception("Hook location not found, switch(*) { case 54: ...");
        }
        #endregion

        #region Make Tax Collector Worth it
        private static void MakeTaxCollectorUseful(ILContext il)
        {
            ILCursor cursor = new(il);
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchCall<Item>("buyPrice")))
            {
                LogFailure("Tax Collector Money Boosts", "Could not locate the amount of money to collect per town NPC.");
                return;
            }
            cursor.Emit(OpCodes.Pop);
            cursor.Emit<CalamityGlobalNPC>(OpCodes.Call, "get_TotalTaxesPerNPC");

            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchCall<Item>("buyPrice")))
            {
                LogFailure("Tax Collector Money Boosts", "Could not locate the maximum amount of money to collect.");
                return;
            }
            cursor.Emit(OpCodes.Pop);
            cursor.Emit<CalamityGlobalNPC>(OpCodes.Call, "get_TaxesToCollectLimit");
        }
        #endregion

        #region Foreground tiles drawing
        private static void DrawForegroundStuff(Terraria.On_Main.orig_DrawGore orig, Main self)
        {
            orig(self);
            if (Main.PlayerLoaded && !Main.gameMenu)
                ForegroundManager.DrawTiles();
        }

        private static void ClearForegroundStuff(Terraria.GameContent.Drawing.On_TileDrawing.orig_PreDrawTiles orig, Terraria.GameContent.Drawing.TileDrawing self, bool solidLayer, bool forRenderTargets, bool intoRenderTargets)
        {
            orig(self, solidLayer, forRenderTargets, intoRenderTargets);

            if (!solidLayer && (intoRenderTargets || Lighting.UpdateEveryFrame))
                ForegroundManager.ClearTiles();
        }
        #endregion

        #region Tile ping overlay
        private static void ClearTilePings(Terraria.GameContent.Drawing.On_TileDrawing.orig_Draw orig, Terraria.GameContent.Drawing.TileDrawing self, bool solidLayer, bool forRenderTargets, bool intoRenderTargets, int waterStyleOverride)
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
        private static void CustomGrappleMovementCheck(Terraria.On_Player.orig_GrappleMovement orig, Player self)
        {
            WulfrumPackPlayer mp = self.GetModPlayer<WulfrumPackPlayer>();

            if (mp.GrappleMovementDisabled)
                return;

            orig(self);
        }

        /// <summary>
        /// This is called right before the game decides wether or not to update the players velocity based on "real" physics (aka not tongued or hooked or with a pulley)
        /// </summary>
        private static void CustomGrapplePreDefaultMovement(Terraria.On_Player.orig_UpdatePettingAnimal orig, Player self)
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
        private static void CustomGrapplePreStepUp(Terraria.On_Player.orig_SlopeDownMovement orig, Player self)
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
        /// This is done to put the hook if it was cacehd during the frame instruction.
        /// </summary>
        private static void CustomGrapplePostFrame(Terraria.On_Player.orig_PlayerFrame orig, Player self)
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

        #region Find Calamity Item Dye Shader

        internal static void FindCalamityItemDyeShader(Terraria.On_Player.orig_UpdateItemDye orig, Player self, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem)
        {
            orig(self, isNotInVanitySlot, isSetToHidden, armorItem, dyeItem);
            if (armorItem.type == ModContent.ItemType<Calamity>())
                self.Calamity().CalamityFireDyeShader = GameShaders.Armor.GetShaderFromItemId(dyeItem.type);
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
        internal static void GetDifficultyOverride(Terraria.GameContent.UI.Elements.On_AWorldListItem.orig_GetDifficulty orig, AWorldListItem self, out string expertText, out Color gameModeColor)
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
        public static void ShimmerEffectEdits(Terraria.On_Item.orig_GetShimmered orig, Item self)
        {
            // Don't keep the original stack amount when shimmering Fabsol's Vodka into Crystal Heart Vodka
            if (self.type == ModContent.ItemType<FabsolsVodka>())
            {
                self.SetDefaults(ModContent.ItemType<CrystalHeartVodka>());
                self.shimmered = true;
                self.shimmerWet = true;
                self.wet = true;
                self.velocity *= 0.1f;
                if (Main.netMode == 0)
                {
                    Item.ShimmerEffect(self.Center);
                }
                else
                {
                    NetMessage.SendData(146, -1, -1, null, 0, (int)self.Center.X, (int)self.Center.Y);
                    NetMessage.SendData(145, -1, -1, null, self.whoAmI, 1f);
                }
                AchievementsHelper.NotifyProgressionEvent(27);
            }
            else
            {
                orig(self);
            }
        }
        #endregion
    }
}
