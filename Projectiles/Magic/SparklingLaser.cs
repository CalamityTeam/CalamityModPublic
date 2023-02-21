using CalamityMod.Projectiles.BaseProjectiles;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class SparklingLaser : BaseLaserbeamProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public bool playedSound = false;
        public const int ChargeupTime = 50;

        public Player Owner => Main.player[Projectile.owner];
        public override Color LightCastColor => new Color(204, 204, 255); //#CCCCFF
        public override float Lifetime => 18000f;
        public override float MaxScale => 1f;
        public override float MaxLaserLength => 1600f; //100 tiles
        public override Texture2D LaserBeginTexture => ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/SparklingLaserBegin", AssetRequestMode.ImmediateLoad).Value;
        public override Texture2D LaserMiddleTexture => ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/SparklingLaserMid", AssetRequestMode.ImmediateLoad).Value;
        public override Texture2D LaserEndTexture => ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/SparklingLaserEnd", AssetRequestMode.ImmediateLoad).Value;
        private const float AimResponsiveness = 0.8f; // Last Prism is 0.92f. Lower makes the laser turn faster.

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sparkling Laser");
            Main.projFrames[Projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.hide = true;
            Projectile.timeLeft = 18000;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 20;
        }

        public override void DetermineScale()
        {
            Projectile.scale = Time < ChargeupTime ? 0f : MaxScale;
        }

        public override float DetermineLaserLength()
        {
            return DetermineLaserLength_CollideWithTiles(5);
        }

        public override bool PreAI()
        {
            // Multiplayer support here, only run this code if the client running it is the owner of the projectile
            if (Projectile.owner == Main.myPlayer)
            {
                Vector2 rrp = Owner.RotatedRelativePoint(Owner.MountedCenter, true);
                UpdateAim(rrp);
                Projectile.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;
                Projectile.netUpdate = true;
            }

            int dir = Projectile.direction;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.Center = Owner.Center + Projectile.velocity * 56f; //Distance offset
            Projectile.timeLeft = 18000; //Infinite lifespan
            Owner.ChangeDir(dir);
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.itemRotation = ((Projectile.rotation + MathHelper.PiOver2).ToRotationVector2() * -Owner.direction).ToRotation();

            if (!Owner.channel)
            {
                Projectile.Kill();
                return false;
            }
            // Do we still have enough mana? If not, we kill the projectile because we cannot use it anymore
            if (Owner.miscCounter % 10 == 0 && !Owner.CheckMana(Owner.ActiveItem(), -1, true))
            {
                Projectile.Kill();
                return false;
            }

            if (Time < ChargeupTime)
            {
                // Crate charge-up dust.
                int dustCount = (int)(Time / 20f);
                Vector2 spawnPos = Projectile.Center;
                for (int k = 0; k < dustCount + 1; k++)
                {
                    Dust dust = Dust.NewDustDirect(spawnPos, 1, 1, 226, Projectile.velocity.X / 2f, Projectile.velocity.Y / 2f);
                    dust.position += Main.rand.NextVector2Square(-10f, 10f);
                    dust.velocity = Main.rand.NextVector2Unit() * (10f - dustCount * 2f) / 10f;
                    dust.scale = Main.rand.NextFloat(0.5f, 1f);
                    dust.noGravity = true;
                }
                Time++;
                return false;
            }

            // Play a cool sound when fully charged.
            if (!playedSound)
            {
                SoundEngine.PlaySound(SoundID.Item68, Projectile.position);
                playedSound = true;
            }
            return true;
        }

        public override void PostAI()
        {
            // Determine frames.
            Projectile.frameCounter++;
            if (Projectile.frameCounter % 5f == 4f)
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Projectile.type];
        }

        // Gently adjusts the aim vector of the laser to point towards the mouse.
        private void UpdateAim(Vector2 source)
        {
            Vector2 aimVector = Vector2.Normalize(Main.MouseWorld - source);
            if (aimVector.HasNaNs())
                aimVector = -Vector2.UnitY;
            aimVector = Vector2.Normalize(Vector2.Lerp(aimVector, Vector2.Normalize(Projectile.velocity), AimResponsiveness));

            if (aimVector != Projectile.velocity)
                Projectile.netUpdate = true;
            Projectile.velocity = aimVector;
        }

        public override bool ShouldUpdatePosition() => false;

        public override bool? CanDamage() => Time >= ChargeupTime;

        // Update CutTiles so the laser will cut tiles (like grass).
        public override void CutTiles()
        {
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Vector2 unit = Projectile.velocity;
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + unit * LaserLength, Projectile.width + 16, DelegateMethods.CutTiles);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (target.life <= 0 && target.lifeMax > 5 && Projectile.owner == Main.myPlayer)
            {
                int shardDamage = Projectile.damage / 5;
                int shardAmt = Main.rand.Next(2, 4);
                for (int s = 0; s < shardAmt; s++)
                {
                    Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
                    int shard = Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, velocity, ModContent.ProjectileType<AquashardSplit>(), shardDamage, 0f, Projectile.owner);
                    if (shard.WithinBounds(Main.maxProjectiles))
                        Main.projectile[shard].DamageType = DamageClass.Magic;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // This should never happen, but just in case-
            if (Projectile.velocity == Vector2.Zero || Time < ChargeupTime)
                return false;

            Color beamColor = LaserOverlayColor;
            Rectangle startFrameArea = LaserBeginTexture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            Rectangle middleFrameArea = LaserMiddleTexture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            Rectangle endFrameArea = LaserEndTexture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);

            // Start texture drawing.
            // This is a lot more scuffed than other lasers... for some reason I'll try not to think about right now.
            Vector2 laserBeginCenter = Projectile.Center - Main.screenPosition + Projectile.velocity * Projectile.scale * 50f;
            Main.EntitySpriteDraw(LaserBeginTexture,
                             laserBeginCenter,
                             startFrameArea,
                             beamColor,
                             Projectile.rotation,
                             LaserBeginTexture.Size() / 2f,
                             Projectile.scale,
                             SpriteEffects.None,
                             0);

            // Prepare things for body drawing.
            float laserBodyLength = LaserLength;
            laserBodyLength -= (startFrameArea.Height / 2 + endFrameArea.Height) * Projectile.scale;
            Vector2 centerOnLaser = Projectile.Center - Main.screenPosition;
            centerOnLaser += Projectile.velocity * Projectile.scale * 10.5f;

            // Body drawing.
            if (laserBodyLength > 0f)
            {
                float laserOffset = middleFrameArea.Height * Projectile.scale;
                float incrementalBodyLength = 0f;
                while (incrementalBodyLength + 1f < laserBodyLength)
                {
                    centerOnLaser += Projectile.velocity * laserOffset;
                    incrementalBodyLength += laserOffset;
                    Main.EntitySpriteDraw(LaserMiddleTexture,
                                     centerOnLaser,
                                     middleFrameArea,
                                     beamColor,
                                     Projectile.rotation,
                                     LaserMiddleTexture.Width * 0.5f * Vector2.UnitX,
                                     Projectile.scale,
                                     SpriteEffects.None,
                                     0);
                }
            }

            // End texture drawing.
            if (Math.Abs(LaserLength - DetermineLaserLength()) < 30f)
            {
                Main.EntitySpriteDraw(LaserEndTexture,
                                 centerOnLaser,
                                 endFrameArea,
                                 beamColor,
                                 Projectile.rotation,
                                 LaserEndTexture.Frame(1, 1, 0, 0).Top(),
                                 Projectile.scale,
                                 SpriteEffects.None,
                                 0);
            }
            return false;
        }
    }
}
