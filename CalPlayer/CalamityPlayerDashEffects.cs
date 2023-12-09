using CalamityMod.Balancing;
using CalamityMod.CalPlayer.Dashes;
using CalamityMod.EntitySources;
using CalamityMod.Enums;
using CalamityMod.Items.Mounts;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.CalPlayer
{
    public partial class CalamityPlayer : ModPlayer
    {
        public int VerticalGodslayerDashTimer;
        public int VerticalSpeedBlasterDashTimer;

        private string dashID; //private backing variable

        public string DashID
        {
            get
            {
                return (String.IsNullOrEmpty(dashID) && Player.dashType == 0 && CalamityConfig.Instance.DefaultDashEnabled) ? DefaultDash.ID : dashID; //gives default dash ONLY if no custom or vanilla dash.
            }
            set => dashID = value;
        }

    public string DeferredDashID;

        public string LastUsedDashID;
        
        public PlayerDashEffect UsedDash
        {
            get
            {
                PlayerDashManager.FindByID(DashID, out PlayerDashEffect dashEffect);
                return dashEffect;
            }
        }

        public bool HasCustomDash => !string.IsNullOrEmpty(DashID);

        public bool HandleDashDodges()
        {
            bool playerDashing = Player.pulley || (Player.grappling[0] == -1 && !Player.tongued);
            if (playerDashing && DashID == GodslayerArmorDash.ID && Player.dashDelay < 0)
            {
                GodSlayerDodge();
                return true;
            }

            // Neither scarf can be used if either is on cooldown
            // TODO -- Why do the two scarves not use the same cooldown? They're both called "Scarf Cooldown".
            if (playerDashing && DashID == CounterScarfDash.ID && Player.dashDelay < 0 && dodgeScarf && !(Player.HasCooldown(Cooldowns.CounterScarf.ID) || Player.HasCooldown(Cooldowns.EvasionScarf.ID)))
            {
                CounterScarfDodge();
                return true;
            }
            return false;
        }

        public void ModDashMovement()
        {
            if (Player.whoAmI != Main.myPlayer)
                return;

            var source = new ProjectileSource_PlayerDashHit(Player);

            // Handle collision slam-through effects.
            if (HasCustomDash && Player.dashDelay < 0)
            {
                Rectangle hitArea = new Rectangle((int)(Player.position.X + Player.velocity.X * 0.5 - 4f), (int)(Player.position.Y + Player.velocity.Y * 0.5 - 4), Player.width + 8, Player.height + 8);
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC n = Main.npc[i];

                    // Ignore critters with the Guide to Critter Companionship
                    if (Player.dontHurtCritters && NPCID.Sets.CountsAsCritter[n.type])
                        continue;

                    if (n.active && !n.dontTakeDamage && !n.friendly && n.Calamity().dashImmunityTime[Player.whoAmI] <= 0)
                    {
                        if (hitArea.Intersects(n.getRect()) && (n.noTileCollide || Player.CanHit(n)))
                        {
                            DashHitContext hitContext = default;
                            UsedDash.OnHitEffects(Player, n, source, ref hitContext);

                            // Don't bother doing anything if no damage is done.
                            if (hitContext.damageClass is null || hitContext.BaseDamage <= 0)
                                continue;

                            // Duplicated from the way TML edits vanilla ram dash damage (and Shield of Cthulhu)
                            int dashDamage = (int)Player.GetTotalDamage(hitContext.damageClass).ApplyTo(hitContext.BaseDamage);
                            float dashKB = Player.GetTotalKnockback(hitContext.damageClass).ApplyTo(hitContext.BaseKnockback);
                            bool rollCrit = Main.rand.Next(100) < Player.GetTotalCritChance(hitContext.damageClass);

                            Player.ApplyDamageToNPC(n, dashDamage, dashKB, hitContext.HitDirection, rollCrit, hitContext.damageClass, true);
                            if (n.Calamity().dashImmunityTime[Player.whoAmI] < 12)
                                n.Calamity().dashImmunityTime[Player.whoAmI] = 12;

                            Player.GiveImmuneTimeForCollisionAttack(hitContext.PlayerImmunityFrames);
                        }
                    }
                }
            }

            if (Player.dashDelay > 0) //Speed Blaster
            {
                VerticalSpeedBlasterDashTimer = 0;
                LastUsedDashID = string.Empty;
                return;
            }

            if (Player.dashDelay > 0)
            {
                VerticalGodslayerDashTimer = 0;
                LastUsedDashID = string.Empty;
                return;
            }

            if (Player.dashDelay < 0)
            {
                int dashDelayToApply = BalancingConstants.UniversalDashCooldown;
                if (UsedDash.CollisionType == DashCollisionType.ShieldSlam)
                    dashDelayToApply = BalancingConstants.UniversalShieldSlamCooldown;
                else if (UsedDash.CollisionType == DashCollisionType.ShieldBonk)
                    dashDelayToApply = BalancingConstants.UniversalShieldBonkCooldown;
                
                float dashSpeed = 12f;
                float dashSpeedDecelerationFactor = 0.985f;
                float runSpeed = Math.Max(Player.accRunSpeed, Player.maxRunSpeed);
                float runSpeedDecelerationFactor = 0.94f;

                LastUsedDashID = DashID;

                // Handle mid-dash effects.
                UsedDash.MidDashEffects(Player, ref dashSpeed, ref dashSpeedDecelerationFactor, ref runSpeedDecelerationFactor);
                if (UsedDash.IsOmnidirectional && VerticalGodslayerDashTimer < 25)
                {
                    VerticalGodslayerDashTimer++;
                    if (VerticalGodslayerDashTimer >= 25)
                    {
                        Player.dashDelay = dashDelayToApply;
                        // Stop the player from going flying
                        Player.velocity *= 0.2f;
                    }
                }

                if (UsedDash.IsOmnidirectional && VerticalSpeedBlasterDashTimer < 25)
                {
                    VerticalSpeedBlasterDashTimer++;
                    if (VerticalSpeedBlasterDashTimer >= 25)
                    {
                        Player.dashDelay = dashDelayToApply;
                        // Stop the player from going flying
                        Player.velocity *= 0.2f;
                    }
                }

                if (HasCustomDash)
                {
                    Player.vortexStealthActive = false;

                    // Decide the player's facing direction.
                    if (Player.velocity.X != 0f)
                        Player.ChangeDir(Math.Sign(Player.velocity.X));

                    // Handle mid-dash movement.
                    if (UsedDash.IsOmnidirectional)
                    {
                        if (Player.velocity.Length() > dashSpeed)
                        {
                            Player.velocity *= dashSpeedDecelerationFactor;
                            return;
                        }
                        if (Player.velocity.Length() > runSpeed)
                        {
                            Player.velocity *= runSpeedDecelerationFactor;
                            return;
                        }
                    }
                    else
                    {
                        if (Player.velocity.X > dashSpeed || Player.velocity.X < -dashSpeed)
                        {
                            Player.velocity.X *= dashSpeedDecelerationFactor;
                            return;
                        }
                        if (Player.velocity.X > runSpeed || Player.velocity.X < -runSpeed)
                        {
                            Player.velocity.X *= runSpeedDecelerationFactor;
                            return;
                        }
                    }

                    // Dash delay depends on the type of dash used.
                    Player.dashDelay = dashDelayToApply;

                    if (UsedDash.IsOmnidirectional)
                    {
                        if (Player.velocity.Length() < 0f)
                        {
                            Player.velocity.Normalize();
                            Player.velocity *= -runSpeed;
                            return;
                        }
                        if (Player.velocity.Length() > 0f)
                        {
                            Player.velocity.Normalize();
                            Player.velocity *= runSpeed;
                            return;
                        }
                    }
                    else
                    {
                        if (Player.velocity.X < 0f)
                        {
                            Player.velocity.X = -runSpeed;
                            return;
                        }
                        if (Player.velocity.X > 0f)
                        {
                            Player.velocity.X = runSpeed;
                            return;
                        }
                    }
                }
            }

            // Handle first-frame effects.
            else if (HasCustomDash && !Player.mount.Active)
            {
                if (DoADash(UsedDash.CalculateDashSpeed(Player)))
                    UsedDash.OnDashEffects(Player);
            }
        }

        public bool HandleHorizontalDash(out DashDirection direction)
        {
            direction = DashDirection.Directionless;
            bool dashWasExecuted = false;

            // If the manual hotkey is bound, standard Terraria dashes cannot be triggered by double tapping.
            var manualDashHotkeys = CalamityKeybinds.DashHotkey.GetAssignedKeys();
            bool manualHotkeyBound = (manualDashHotkeys?.Count ?? 0) > 0;
            bool pressedManualHotkey = manualHotkeyBound && CalamityKeybinds.DashHotkey.JustPressed;

            int dashDirectionToUse = 0;

            // The manual hotkey is bound. Dashing is controlled solely by this hotkey. Vanilla inputs will not function.
            if (pressedManualHotkey)
            {
                // If you are holding D but not A, then always dash right.
                if (Player.controlRight && !Player.controlLeft)
                    dashDirectionToUse = 1;
                // If you are holding A but not D, then always dash left.
                else if (Player.controlLeft && !Player.controlRight)
                    dashDirectionToUse = -1;

                // If you are holding neither A nor D, or holding both, then dash in the direction the player is moving.
                // If the player is not moving at all, then dash the direction the player is facing.
                else
                {
                    if (MathF.Abs(Player.velocity.X) <= 0.01f)
                        dashDirectionToUse = Player.direction;
                    else
                        dashDirectionToUse = Player.velocity.X > 0f ? 1 : -1;
                }
            }

            // The manual hotkey is not bound. Dashing is controlled via vanilla inputs.
            else if (!manualHotkeyBound)
            {
                // Check whether or not a horizontal dash was declared via vanilla methods this frame.
                bool vanillaLeftDashInput = !manualHotkeyBound && Player.controlLeft && Player.releaseLeft;
                bool vanillaRightDashInput = !manualHotkeyBound && Player.controlRight && Player.releaseRight;
                dashDirectionToUse = vanillaRightDashInput ? 1 : vanillaLeftDashInput ? -1 : 0;
            }


            if (dashDirectionToUse == 1)
            {
                if (dashTimeMod > 0 || pressedManualHotkey)
                {
                    direction = DashDirection.Right;
                    dashWasExecuted = true;
                    dashTimeMod = 0;
                }
                else
                    dashTimeMod = 15;
            }
            else if (dashDirectionToUse == -1)
            {
                if (dashTimeMod < 0 || pressedManualHotkey)
                {
                    direction = DashDirection.Left;
                    dashWasExecuted = true;
                    dashTimeMod = 0;
                }
                else
                    dashTimeMod = -15;
            }
            return dashWasExecuted;
        }

        public bool HandleOmnidirectionalDash(out DashDirection direction)
        {
            direction = DashDirection.Directionless;
            bool justDashed = false;

            if (Player.controlUp && Player.controlLeft)
            {
                if (dashTimeMod < 0)
                {
                    direction = DashDirection.UpLeft;
                    justDashed = true;
                    dashTimeMod = 0;
                }
                else
                    dashTimeMod = -15;
            }
            else if (Player.controlUp && Player.controlRight)
            {
                if (dashTimeMod > 0)
                {
                    direction = DashDirection.UpRight;
                    justDashed = true;
                    dashTimeMod = 0;
                }
                else
                    dashTimeMod = 15;
            }
            else if (Player.controlDown && Player.controlLeft)
            {
                if (dashTimeMod < 0)
                {
                    direction = DashDirection.DownLeft;
                    justDashed = true;
                    dashTimeMod = 0;
                    Player.maxFallSpeed = 50f;
                }
                else
                    dashTimeMod = -15;
            }
            else if (Player.controlDown && Player.controlRight)
            {
                if (dashTimeMod > 0)
                {
                    direction = DashDirection.DownRight;
                    justDashed = true;
                    dashTimeMod = 0;
                    Player.maxFallSpeed = 50f;
                }
                else
                    dashTimeMod = 15;
            }
            else if (Player.controlUp)
            {
                if (dashTimeMod < 0)
                {
                    direction = DashDirection.Up;
                    justDashed = true;
                    dashTimeMod = 0;
                }
                else
                    dashTimeMod = -15;
            }
            else if (Player.controlDown)
            {
                if (dashTimeMod > 0)
                {
                    direction = DashDirection.Down;
                    justDashed = true;
                    dashTimeMod = 0;
                    Player.maxFallSpeed = 50f;
                }
                else
                    dashTimeMod = 15;
            }
            else if (Player.controlLeft)
            {
                if (dashTimeMod < 0)
                {
                    direction = DashDirection.Left;
                    justDashed = true;
                    dashTimeMod = 0;
                }
                else
                    dashTimeMod = -15;
            }
            else if (Player.controlRight)
            {
                if (dashTimeMod > 0)
                {
                    direction = DashDirection.Right;
                    justDashed = true;
                    dashTimeMod = 0;
                }
                else
                    dashTimeMod = 15;
            }
            return justDashed;
        }

        public bool DoADash(float dashSpeed)
        {
            bool justDashed;
            bool omnidirectionalDash = UsedDash?.IsOmnidirectional ?? false;
            DashDirection direction;

            // Have the dash time incrementally move towards its default state of zero.
            if (dashTimeMod != 0)
                dashTimeMod -= (dashTimeMod > 0).ToDirectionInt();

            // Determine dash times.
            if (omnidirectionalDash)
                justDashed = HandleOmnidirectionalDash(out direction);
            else
                justDashed = HandleHorizontalDash(out direction);

            // Make dash movements happen if ready.
            if (justDashed)
            {
                int totalDirections = 8;
                Vector2[] possibleVelocities = new Vector2[totalDirections];
                for (int i = 0; i < totalDirections; i++)
                    possibleVelocities[i] = -Vector2.UnitY.RotatedBy(MathHelper.TwoPi * i / totalDirections) * dashSpeed;

                switch (direction)
                {
                    // Up Left
                    case DashDirection.UpLeft:
                        Player.velocity = possibleVelocities[7];
                        break;

                    // Down Left
                    case DashDirection.DownLeft:
                        Player.velocity = possibleVelocities[5];
                        break;

                    // Up
                    case DashDirection.Up:
                        Player.velocity = possibleVelocities[0];
                        break;

                    // Left
                    case DashDirection.Left:
                        Player.velocity = omnidirectionalDash ? possibleVelocities[6] : new Vector2(possibleVelocities[6].X, Player.velocity.Y);
                        break;

                    // Nothing
                    case DashDirection.Directionless:
                        break;

                    // Right
                    case DashDirection.Right:
                        Player.velocity = omnidirectionalDash ? possibleVelocities[2] : new Vector2(possibleVelocities[2].X, Player.velocity.Y);
                        break;

                    // Down
                    case DashDirection.Down:
                        Player.velocity = possibleVelocities[4];
                        break;

                    // Down Right
                    case DashDirection.DownRight:
                        Player.velocity = possibleVelocities[3];
                        break;

                    // Up Right
                    case DashDirection.UpRight:
                        Player.velocity = possibleVelocities[1];
                        break;
                }

                // Make any dash movements move to a rapid halt if there are any tiles in the way.
                Point upwardTilePoint = (Player.Center + new Vector2(MathHelper.Clamp((int)direction, -1f, 1f) * Player.width / 2 + 2, Player.gravDir * -Player.height / 2f + Player.gravDir * 2f)).ToTileCoordinates();
                Point aheadTilePoint = (Player.Center + new Vector2(MathHelper.Clamp((int)direction, -1f, 1f) * Player.width / 2 + 2, 0f)).ToTileCoordinates();
                if (WorldGen.SolidOrSlopedTile(upwardTilePoint.X, upwardTilePoint.Y) || WorldGen.SolidOrSlopedTile(aheadTilePoint.X, aheadTilePoint.Y))
                    Player.velocity.X /= 2f;

                Player.dashDelay = -1;
            }

            return justDashed;
        }

        public void ModHorizontalMovement()
        {
            if (Player.mount.Active && Player.mount.Type == ModContent.MountType<AlicornMount>() && Math.Abs(Player.velocity.X) > Player.mount.DashSpeed - Player.mount.RunSpeed / 2f)
            {
                Rectangle damageHitbox = Player.getRect();

                if (Player.direction == 1)
                    damageHitbox.Offset(Player.width - 1, 0);

                damageHitbox.Width = 2;
                damageHitbox.Inflate(6, 12);
                float damage = Player.GetTotalDamage<SummonDamageClass>().ApplyTo(800f);
                float knockback = 10f;
                int NPCImmuneTime = 30;
                int playerImmuneTime = 6;
                DoMountDashDamage(damageHitbox, damage, knockback, NPCImmuneTime, playerImmuneTime);
            }

            if (Player.mount.Active && Player.mount.Type == ModContent.MountType<RimehoundMount>() && Math.Abs(Player.velocity.X) > Player.mount.RunSpeed / 2f)
            {
                Rectangle damageHitbox = Player.getRect();

                if (Player.direction == 1)
                    damageHitbox.Offset(Player.width - 1, 0);

                damageHitbox.Width = 2;
                damageHitbox.Inflate(6, 12);
                float damage = Player.GetTotalDamage<SummonDamageClass>().ApplyTo(50f);
                float knockback2 = 8f;
                int NPCImmuneTime = 30;
                int playerImmuneTime = 6;
                DoMountDashDamage(damageHitbox, damage, knockback2, NPCImmuneTime, playerImmuneTime);
            }

            if (Player.mount.Active && Player.mount.Type == ModContent.MountType<OnyxExcavator>() && Math.Abs(Player.velocity.X) > Player.mount.RunSpeed / 2f)
            {
                Rectangle damageHitbox = Player.getRect();

                if (Player.direction == 1)
                    damageHitbox.Offset(Player.width - 1, 0);

                damageHitbox.Width = 2;
                damageHitbox.Inflate(6, 12);
                float damage = Player.GetTotalDamage<SummonDamageClass>().ApplyTo(25f);
                float knockback2 = 5f;
                int NPCImmuneTime = 30;
                int playerImmuneTime = 6;
                DoMountDashDamage(damageHitbox, damage, knockback2, NPCImmuneTime, playerImmuneTime);
            }
        }

        public int DoMountDashDamage(Rectangle myRect, float Damage, float Knockback, int NPCImmuneTime, int PlayerImmuneTime)
        {
            int totalHurtNPCs = 0;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC n = Main.npc[i];

                // Ignore critters with the Guide to Critter Companionship
                if (Player.dontHurtCritters && NPCID.Sets.CountsAsCritter[n.type])
                    continue;

                if (n.active && !n.dontTakeDamage && !n.friendly && n.Calamity().dashImmunityTime[Player.whoAmI] <= 0)
                {
                    Rectangle npcHitbox = n.getRect();
                    if (myRect.Intersects(npcHitbox) && (n.noTileCollide || Collision.CanHit(Player.position, Player.width, Player.height, n.position, n.width, n.height)))
                    {
                        int hitDirection = Math.Sign(Player.velocity.X);

                        // Use the player's facing direction as a fallback if they are not making any horizontal movement.
                        if (hitDirection == 0)
                            hitDirection = Player.direction;

                        // TODO -- This should probably use DirectStrike?
                        if (Player.whoAmI == Main.myPlayer)
                            Player.ApplyDamageToNPC(n, (int)Damage, Knockback, hitDirection, false);

                        n.Calamity().dashImmunityTime[Player.whoAmI] = NPCImmuneTime;
                        Player.GiveIFrames(PlayerImmuneTime, false);
                        totalHurtNPCs++;
                        break;
                    }
                }
            }
            return totalHurtNPCs;
        }
    }
}
