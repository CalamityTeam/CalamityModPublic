using CalamityMod.Buffs.Mounts;
using CalamityMod.Items;
using CalamityMod.Projectiles.Summon.AndromedaUI;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Terraria.Audio;
using CalamityMod.Items.Weapons.DraedonsArsenal;

namespace CalamityMod.Projectiles.Summon
{
    public class GiantIbanRobotOfDoom : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public int FrameX = 0;
        public int FrameY = 0;
        public int CurrentFrame
        {
            get => FrameY + FrameX * 7;
            set
            {
                FrameX = value / 7;
                FrameY = value % 7;
            }
        }
        public float ClickCooldown
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public bool LeftBracketActive = false;
        public bool RightBracketActive = true; // This is supposed to be the default bracket, according to Iban. Ask him before changing this.
        public bool BottomBracketActive = false;
        public bool RightIconActive => RightBracketActive || BottomBracketActive;

        public bool LeftIconActive = false;
        public bool TopIconActive = false;

        public int RightIconCooldown = 0;
        public const int RightIconAttackTime = 480; // 8 second wait
        public const int RightIconCooldownMax = RightIconAttackTime * 2; // 16 second wait.
        public const float RightIconLungeSpeed = 28f;

        /// <summary>
        /// This cooldown is set in <see cref="CalamityGlobalItem.PerformAndromedaAttacks"/>
        /// </summary>
        public int LaserCooldown = 0;
        public const int LaserBaseDamage = 4200;
        public const int RegicideBaseDamageSmall = 1897;
        public const int RegicideBaseDamageLarge = 5200;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.NeedsUUID[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 152;
            Projectile.height = 212;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 18000;
            Projectile.timeLeft *= 5;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.DamageType = DamageClass.Summon;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            HandleCooldowns(player);
            ManipulatePlayerValues(player);
            RegisterRightClick(player);
            SetFrames(player);
            SetSpriteDirection(player);
            player.AddBuff(LeftIconActive ? ModContent.BuffType<AndromedaSmallBuff>() : ModContent.BuffType<AndromedaBuff>(), 2);
        }
        public void HandleCooldowns(Player player)
        {
            if (RightIconCooldown > 0 && RightIconActive)
            {
                RightIconCooldown--; // The shooting of the lightning is done in the SetFrames method.
            }
            if (LaserCooldown > 0)
            {
                LaserCooldown--;
                FireLaserBeam(player);
            }
        }
        public void ManipulatePlayerValues(Player player)
        {
            if (!player.active || player.dead)
            {
                player.Calamity().andromedaState = AndromedaPlayerState.Inactive;
                Projectile.Kill();
                return;
            }
            Projectile.Center = player.Center + Vector2.UnitY * (6f + player.gfxOffY);
            player.Calamity().andromedaState = LeftIconActive ? AndromedaPlayerState.SmallRobot : AndromedaPlayerState.LargeRobot;
            player.channel = false;
            if (RightIconCooldown > RightIconAttackTime)
            {
                player.Calamity().andromedaState = AndromedaPlayerState.SpecialAttack;

                for (int i = 0; i < 4; i++)
                {
                    Vector2 spinningPoint = new Vector2(0f, -28f).RotatedBy(RightIconCooldown / 60f * MathHelper.TwoPi)
                        .RotatedBy(Projectile.velocity.ToRotation())
                        .RotatedBy(MathHelper.Lerp(MathHelper.ToRadians(-40f), MathHelper.ToRadians(40f), i / 4f));

                    Vector2 center = player.Center + new Vector2(6f, -2f).RotatedBy(player.velocity.ToRotation());

                    int idx = Dust.NewDust(center, 0, 0, 226, 0f, 0f, 100, default, 0.5f);
                    Main.dust[idx].noGravity = true;
                    Main.dust[idx].position = center + spinningPoint;
                    Main.dust[idx].velocity = Vector2.Zero;
                    spinningPoint *= -1f;

                    idx = Dust.NewDust(center, 0, 0, 226, 0f, 0f, 100, default, 0.5f);
                    Main.dust[idx].noGravity = true;
                    Main.dust[idx].position = center + spinningPoint;
                    Main.dust[idx].velocity = Vector2.Zero;
                }
                // Exit the charge mode early.
                if ((player.velocity.X == 0f || player.velocity.Y == 0f) && RightIconCooldown < RightIconCooldownMax - 30f)
                {
                    ExitChargeModeEarly(player);
                }

                player.velocity = Vector2.Lerp(player.velocity, Projectile.SafeDirectionTo(Main.MouseWorld, Vector2.UnitY) * RightIconLungeSpeed, 0.225f);
                Projectile.rotation = player.velocity.ToRotation() + MathHelper.PiOver2;
            }
            else
                Projectile.rotation = 0f;

            if (player.mount != null) // Kill any mounts
            {
                player.mount.Dismount(player);
            }
        }
        public void RegisterRightClick(Player player)
        {
            if (Main.mouseRight && ClickCooldown <= 0f)
            {
                ClickCooldown = 30f;
                // Exit the charge mode early.
                if (RightIconCooldown > RightIconAttackTime)
                {
                    ExitChargeModeEarly(player);
                    return;
                }
                // If the player has any existing UIs, kill them all.
                if (player.ownedProjectileCounts[ModContent.ProjectileType<AndromedaUI_Background>()] > 0)
                {
                    for (int i = 0; i < Main.projectile.Length; i++)
                    {
                        if (Main.projectile[i].active &&
                            Main.projectile[i].type == ModContent.ProjectileType<AndromedaUI_Background>() &&
                            Main.projectile[i].owner == player.whoAmI)
                        {
                            Main.projectile[i].Kill();
                        }
                    }
                }
                // Otherwise, create one.
                else
                {
                    if (Main.myPlayer == player.whoAmI)
                    {
                        Projectile ui = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), 
                                                 Main.MouseWorld,
                                                 Vector2.Zero,
                                                 ModContent.ProjectileType<AndromedaUI_Background>(),
                                                 0,
                                                 0f,
                                                 player.whoAmI);
                        ui.localAI[0] = Projectile.GetByUUID(Projectile.owner, Projectile.whoAmI);
                        ui.netUpdate = true;
                    }
                }
            }
            else if (ClickCooldown > 0f)
            {
                ClickCooldown--;
            }
        }
        public void SetFrames(Player player)
        {
            Projectile.frameCounter++;
            if (RightIconCooldown <= RightIconAttackTime)
            {
                if (Math.Abs(player.velocity.Y) != 0f)
                {
                    SetFlyingFrames(player);
                }
                else if (Math.Abs(player.velocity.X) > 3f)
                {
                    SetWalkingFrames();
                }
                if (player.velocity == Vector2.Zero)
                {
                    CurrentFrame = 0;
                }
            }
        }
        public void SetFlyingFrames(Player player)
        {
            // Falling
            if (player.velocity.Y > 0f)
            {
                CurrentFrame = 1;
                return;
            }
            // Quickly go to flying frames
            if (CurrentFrame == 0)
            {
                if (Projectile.frameCounter >= 4)
                {
                    CurrentFrame++;
                    Projectile.frameCounter = 0;
                }
            }
            // Flying frames
            else if (Projectile.frameCounter % 4 == 3)
            {
                CurrentFrame++;
                if (CurrentFrame >= 6)
                    CurrentFrame = 2;
            }

            // Dust effect

            Vector2 dustOffset = new Vector2(94f, 58f);
            if (Projectile.spriteDirection == -1)
            {
                dustOffset.X = 214 - dustOffset.X;
            }
            if (!Main.dedServ && player.Calamity().andromedaState == AndromedaPlayerState.LargeRobot)
            {
                for (int i = 0; i < 2; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.position + dustOffset, 263);
                    dust.velocity = Vector2.Normalize(dust.position - Projectile.Top).RotatedByRandom(0.4f) * Main.rand.NextFloat(4f, 7f) + Projectile.velocity;
                    dust.color = Color.SkyBlue;
                    dust.scale = Main.rand.NextFloat(0.9f, 1.35f);
                    dust.noGravity = true;
                }
            }

            // Clamp frames
            if (CurrentFrame >= 6)
                CurrentFrame = 0;
        }
        public void SetWalkingFrames()
        {
            Player player = Main.player[Projectile.owner];
            int walkInterval = (int)MathHelper.Clamp(8 - Math.Abs(player.velocity.X) / 3.5f, 1, 8);
            if (player.velocity.X == 0f)
            {
                CurrentFrame = 0;
            }
            else if (Projectile.frameCounter >= walkInterval)
            {
                Projectile.frameCounter = 0;
                CurrentFrame++;
                if (CurrentFrame >= 13)
                {
                    CurrentFrame = 6;
                }
            }
            // Clamp frames
            if (CurrentFrame >= 14 || CurrentFrame <= 6)
                CurrentFrame = 6;
        }
        public void SetSpriteDirection(Player player)
        {
            if (RightIconCooldown > RightIconAttackTime)
            {
                Projectile.spriteDirection = (Math.Cos(Projectile.rotation) > 0).ToDirectionInt();
                return;
            }
            if (player.velocity.X != 0) // So that the original sprite direction is maintained when there is no X movement.
            {
                Projectile.spriteDirection = (player.velocity.X > 0).ToDirectionInt();
            }
            int slashIndex = -1;

            for (int i = 0; i < Main.projectile.Length; i++)
            {
                if (Main.projectile[i].active &&
                    Main.projectile[i].type == ModContent.ProjectileType<AndromedaRegislash>() &&
                    Main.projectile[i].owner == Projectile.owner)
                {
                    slashIndex = i;
                    break;
                }
            }

            if (slashIndex != -1)
            {
                if (Main.projectile[slashIndex].frameCounter > 0) // To ensure that the starting variables of the blade have been initialized
                {
                    Projectile.spriteDirection = (Math.Cos(Main.projectile[slashIndex].rotation) > 0).ToDirectionInt(); // ai[1] is the blade's starting rotation
                }
            }

            int laserBeamIndex = -1;

            for (int i = 0; i < Main.projectile.Length; i++)
            {
                if (Main.projectile[i].active &&
                    Main.projectile[i].type == ModContent.ProjectileType<AndromedaDeathRay>() &&
                    Main.projectile[i].owner == Projectile.owner)
                {
                    laserBeamIndex = i;
                    break;
                }
            }

            if (laserBeamIndex != -1)
            {
                Projectile.spriteDirection = (Math.Cos(Main.projectile[laserBeamIndex].velocity.ToRotation()) > 0).ToDirectionInt(); // ai[1] is the blade's starting rotation
            }
        }
        public void FireLaserBeam(Player player)
        {
            if (LaserCooldown % (AndromedaDeathRay.TrueTimeLeft - 10) == (AndromedaDeathRay.TrueTimeLeft - 11))
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    SoundEngine.PlaySound(GaussRifle.FireSound, Projectile.Center);
                    int damage = (int)player.GetTotalDamage<SummonDamageClass>().ApplyTo(LaserBaseDamage);
                    Vector2 laserVelocity = (Main.MouseWorld - (Main.player[Projectile.owner].Center + new Vector2(Projectile.spriteDirection == 1 ? 48f : 22f, -28f))).SafeNormalize(Vector2.UnitX * Projectile.spriteDirection);
                    Projectile deathLaser = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(),
                                                                           Projectile.Center,
                                                                           laserVelocity,
                                                                           ModContent.ProjectileType<AndromedaDeathRay>(),
                                                                           damage,
                                                                           8f,
                                                                           Projectile.owner,
                                                                           Projectile.whoAmI);
                    deathLaser.originalDamage = LaserBaseDamage;
                    if (player.HeldItem != null && deathLaser.whoAmI.WithinBounds(Main.maxProjectiles))
                        deathLaser.DamageType = DamageClass.Summon;
                }
            }
        }
        public void ExitChargeModeEarly(Player player)
        {
            RightIconCooldown = RightIconAttackTime;
            SoundEngine.PlaySound(GaussRifle.FireSound, Projectile.Center);
            SpecialAttackExplosionDust(player);
        }
        public void SpecialAttackExplosionDust(Player player)
        {
            if (!Main.dedServ)
            {
                // Petal
                for (int angleInterval = 0; angleInterval < 10; angleInterval++)
                {
                    for (int outwardness = 190; outwardness < 360; outwardness += 8)
                    {
                        for (int speedSign = -1; speedSign <= 1; speedSign += 2)
                        {
                            float angle = MathHelper.Lerp(0f, MathHelper.TwoPi / 10f, (outwardness - 190f) / 170f);
                            Dust dust = Dust.NewDustPerfect(Projectile.Center, 221);
                            dust.noGravity = true;
                            dust.scale = 1.6f;
                            dust.position = player.Center + outwardness * (MathHelper.TwoPi / 10f * angleInterval).ToRotationVector2().RotatedBy(angle);
                            dust.velocity = player.SafeDirectionTo(dust.position) * 8f * speedSign;

                            dust = Dust.NewDustPerfect(Projectile.Center, 221);
                            dust.noGravity = true;
                            dust.scale = 1.6f;
                            dust.position = player.Center + outwardness * (MathHelper.TwoPi / 10f * angleInterval).ToRotationVector2().RotatedBy(-angle);
                            dust.velocity = player.SafeDirectionTo(dust.position) * 8f * speedSign;
                        }
                    }
                }
                // Star
                int pointsOnStar = 6;
                for (int k = 0; k < 2; k++)
                {
                    for (int i = 0; i < pointsOnStar; i++)
                    {
                        float angle = MathHelper.Pi * 1.5f - i * MathHelper.TwoPi / pointsOnStar;
                        float nextAngle = MathHelper.Pi * 1.5f - ((i + 3) % pointsOnStar) * MathHelper.TwoPi / pointsOnStar;
                        if (k == 1)
                            nextAngle = MathHelper.Pi * 1.5f - (i + 2) * MathHelper.TwoPi / pointsOnStar;
                        Vector2 start = angle.ToRotationVector2();
                        Vector2 end = nextAngle.ToRotationVector2();
                        int pointsOnStarSegment = 24;
                        for (int j = 0; j < pointsOnStarSegment; j++)
                        {
                            Dust dust = Dust.NewDustPerfect(player.Center, 221);
                            dust.noGravity = true;
                            dust.scale = 1.9f;
                            dust.velocity = Vector2.Lerp(start, end, j / (float)pointsOnStarSegment) * 13f * new Vector2(1.414f, 1f);
                        }
                    }
                }
            }
        }
        public override bool PreDraw(ref Color lightColor) => false; // Drawing is done completely by the player.
        public override bool OnTileCollide(Vector2 oldVelocity) => false;
        public override bool? CanDamage() => false;
        public override void OnKill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            player.width = 20;
            player.height = 42;
        }
    }
}
