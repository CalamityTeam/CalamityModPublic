using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.Enums;
using Terraria.ModLoader;
using ReLogic.Content;

namespace CalamityMod.Projectiles.Summon
{
    // TODO -- Make this use BaseLaserbeamProjectile
    public class AndromedaDeathRay : ModProjectile
    {
        // How long this laser can exist before it is deleted.
        public const int TrueTimeLeft = 25;

        // Pretty self explanatory
        private const float maximumLength = 2000f;

        public float AngularMultiplier = 0f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Beam");
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = TrueTimeLeft;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 4;
            Projectile.DamageType = DamageClass.Summon;
        }

        // Netcode for sending and receiving shit
        // localAI[0] is the timer, localAI[1] is the laser length

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
            writer.Write(Projectile.localAI[1]);
            writer.Write(AngularMultiplier);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
            Projectile.localAI[1] = reader.ReadSingle();
            AngularMultiplier = reader.ReadSingle();
        }

        public override void AI()
        {
            Projectile owner = Main.projectile[(int)Projectile.ai[0]];
            Player player = Main.player[Projectile.owner];

            if (owner.type != ModContent.ProjectileType<GiantIbanRobotOfDoom>() || !owner.active)
                Projectile.Kill();
            if (owner.active)
            {
                Projectile.Center = player.Center + new Vector2(owner.spriteDirection == 1 ? 48f : 22f, player.gravDir * -28f);
                if (player.Calamity().andromedaState == AndromedaPlayerState.SmallRobot)
                {
                    Projectile.Center = player.Center + new Vector2(owner.spriteDirection == 1 ? 24f : 2f, 0f);
                }
            }

            // How fat the laser is
            float laserSize = 0.6f + (float)Math.Sin(Projectile.localAI[0] / 7f) * 0.025f; // The sine gives a pulsating effect to the laser

            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] >= TrueTimeLeft)
            {
                Projectile.Kill();
                return;
            }

            // Causes the effect where the laser appears to expand/contract at the beginning and end of its life
            Projectile.scale = (float)Math.Sin(Projectile.localAI[0] * MathHelper.Pi / TrueTimeLeft) * 5f * laserSize;
            if (Projectile.scale > laserSize)
            {
                Projectile.scale = laserSize;
            }

            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;

            // Generate dust form the beginning of the ray, giving an illusion of electricity escaping the laser cannon.
            if (!Main.dedServ)
            {
                for (int i = 0; i < 4; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, 133);
                    dust.velocity = Projectile.velocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(2f, 3.5f) + Main.player[Projectile.owner].velocity * 1.5f;
                    dust.fadeIn = Main.rand.NextFloat(0.6f, 0.85f);
                    if (player.Calamity().andromedaState == AndromedaPlayerState.SmallRobot)
                    {
                        dust.fadeIn = 0.05f;
                        dust.scale = 0.8f;
                    }
                    dust.noGravity = true;
                }
            }

            Vector2 samplingPoint = Projectile.Center;

            float[] samples = new float[3];

            float determinedLength = 0f;
            Collision.LaserScan(samplingPoint, Projectile.velocity, Projectile.width * Projectile.scale, maximumLength, samples);
            for (int i = 0; i < samples.Length; i++)
            {
                determinedLength += samples[i];
            }
            determinedLength /= samples.Length;

            Projectile.localAI[1] = MathHelper.Lerp(Projectile.localAI[1], determinedLength, 0.5f);
            Vector2 beamEndPosiiton = Projectile.Center + Projectile.velocity * Projectile.localAI[1];
            // Dust at the end of the beam, if there's a tile there.
            if (Collision.SolidCollision(beamEndPosiiton, 16, 16) && !Main.dedServ)
            {
                if (AngularMultiplier == 0f)
                    AngularMultiplier = Main.rand.NextFloat(1f, 4f);
                float angle = Projectile.localAI[0] / 18f;
                float x = (float)Math.Sin(angle * AngularMultiplier) * (float)Math.Cos(angle);
                float y = (float)Math.Cos(angle * AngularMultiplier) * (float)Math.Sin(angle);
                Vector2 velocity = new Vector2(x * 4.5f, y * 2f);

                for (int i = 0; i < 3; i++)
                {
                    Dust dust = Dust.NewDustPerfect(beamEndPosiiton + angle.ToRotationVector2() * 8f, 133);
                    dust.velocity = velocity.RotatedBy(MathHelper.TwoPi / 3f * i);
                    dust.scale = (float)Math.Cos(angle) + 1.2f;
                    dust.noGravity = true;
                }
            }

            // Draw light blue light across the laser
            DelegateMethods.v3_1 = Color.SkyBlue.ToVector3();
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * Projectile.localAI[1], Projectile.width * Projectile.scale, DelegateMethods.CastLight);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.velocity == Vector2.Zero)
            {
                return false;
            }
            Texture2D laserTailTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/AndromedaDeathrayBegin", AssetRequestMode.ImmediateLoad).Value;
            Texture2D laserBodyTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/AndromedaDeathrayMid", AssetRequestMode.ImmediateLoad).Value;
            Texture2D laserHeadTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/AndromedaDeathrayEnd", AssetRequestMode.ImmediateLoad).Value;
            float laserLength = Projectile.localAI[1];
            Color drawColor = new Color(1f, 1f, 1f) * 0.9f;

            // Laser tail logic

            Main.EntitySpriteDraw(laserTailTexture, Projectile.Center - Main.screenPosition, null, drawColor, Projectile.rotation, laserTailTexture.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);

            // Laser body logic

            laserLength -= (laserTailTexture.Height / 2 + laserHeadTexture.Height) * Projectile.scale;
            Vector2 centerDelta = Projectile.Center;
            centerDelta += Projectile.velocity * Projectile.scale * laserTailTexture.Height / 2f;
            if (laserLength > 0f)
            {
                float laserLengthDelta = 0f;
                Rectangle sourceRectangle = new Rectangle(0, 0, laserBodyTexture.Width, laserBodyTexture.Height);
                while (laserLengthDelta + 1f < laserLength)
                {
                    if (laserLength - laserLengthDelta < sourceRectangle.Height)
                    {
                        sourceRectangle.Height = (int)(laserLength - laserLengthDelta);
                    }
                    Main.EntitySpriteDraw(laserBodyTexture, centerDelta - Main.screenPosition, new Rectangle?(sourceRectangle), drawColor, Projectile.rotation, new Vector2(sourceRectangle.Width / 2f, 0f), Projectile.scale, SpriteEffects.None, 0);
                    laserLengthDelta += sourceRectangle.Height * Projectile.scale;
                    centerDelta += Projectile.velocity * sourceRectangle.Height * Projectile.scale;
                    sourceRectangle.Y += 16;
                    if (sourceRectangle.Y + sourceRectangle.Height > laserBodyTexture.Height)
                    {
                        sourceRectangle.Y = 0;
                    }
                }
            }

            // Laser head logic

            Main.EntitySpriteDraw(laserHeadTexture, centerDelta - Main.screenPosition, null, drawColor, Projectile.rotation, laserHeadTexture.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void CutTiles()
        {
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Vector2 unit = Projectile.velocity;
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + unit * Projectile.localAI[1], (float)Projectile.width * Projectile.scale, DelegateMethods.CutTiles);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox))
            {
                return true;
            }
            float value = 0f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity * Projectile.localAI[1], 22f * Projectile.scale, ref value))
            {
                return true;
            }
            return false;
        }
    }
}
