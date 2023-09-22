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
    public class MidnightSunBeam : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        // How long this laser can exist before it is deleted.
        public const int TrueTimeLeft = 120;

        public override void SetStaticDefaults()
        {
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
            Projectile.localNPCHitCooldown = 16;
            Projectile.DamageType = DamageClass.Summon;
        }

        // Netcode for sending and receiving shit
        // localAI[0] is the timer, localAI[1] is the laser length

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
            writer.Write(Projectile.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
            Projectile.localAI[1] = reader.ReadSingle();
        }

        public override void AI()
        {
            Projectile body = Main.projectile[(int)Projectile.ai[1]];
            if (body.type != ModContent.ProjectileType<MidnightSunUFO>() || !body.active)
                Projectile.Kill();

            if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
            {
                Projectile.velocity = -Vector2.UnitY;
            }

            if (Main.projectile[(int)Projectile.ai[1]].active)
            {
                Projectile.Center = Main.projectile[(int)Projectile.ai[1]].Center;
            }

            if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
            {
                Projectile.velocity = -Vector2.UnitY;
            }

            // How fat the laser is
            float laserSize = 1f;

            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] >= TrueTimeLeft)
            {
                Projectile.Kill();
                return;
            }

            // Causes the effect where the laser appears to expand/contract at the beginning and end of its life
            Projectile.scale = (float)Math.Sin(Projectile.localAI[0] * MathHelper.Pi / TrueTimeLeft) * 10f * laserSize;
            if (Projectile.scale > laserSize)
            {
                Projectile.scale = laserSize;
            }

            // The heart of the "sweeping rotation" part of the laser
            // Basically converts the velocity to a rotation, increments some value to that rotation,
            // and then converts the rotation to a velocity
            float velocityAsRotation = body.rotation + MathHelper.PiOver2;
            Projectile.rotation = velocityAsRotation - MathHelper.PiOver2;
            Projectile.velocity = velocityAsRotation.ToRotationVector2();

            Vector2 samplingPoint = Projectile.Center;

            float[] samples = new float[3];
            Projectile.localAI[1] = body.ai[1];

            // Draw light blue light across the laser
            DelegateMethods.v3_1 = new Vector3(0.52f, 0.93f, 0.97f);
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * Projectile.localAI[1], Projectile.width * Projectile.scale, DelegateMethods.CastLight);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.velocity == Vector2.Zero)
            {
                return false;
            }
            Texture2D laserTailTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/MidnightSunBeamBegin", AssetRequestMode.ImmediateLoad).Value;
            Texture2D laserBodyTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/MidnightSunBeamMid", AssetRequestMode.ImmediateLoad).Value;
            Texture2D laserHeadTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/MidnightSunBeamEnd", AssetRequestMode.ImmediateLoad).Value;
            float laserLength = Projectile.localAI[1];
            Color drawColor = new Color(1f, 1f, 1f) * 0.9f;

            // Laser tail logic

            Main.EntitySpriteDraw(laserTailTexture, Projectile.Center - Main.screenPosition, null, drawColor, Projectile.rotation, laserTailTexture.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);

            // Laser body logic

            laserLength -= (laserTailTexture.Height / 2 + laserHeadTexture.Height) * Projectile.scale;
            Vector2 centerDelta = Projectile.Center;
            centerDelta += Projectile.velocity * Projectile.scale * (float)laserTailTexture.Height / 2f;
            if (laserLength > 0f)
            {
                float laserLengthDelta = 0f;
                Rectangle sourceRectangle = new Rectangle(0, 16 * (Projectile.timeLeft / 3 % 5), laserBodyTexture.Width, 16);
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

            Main.EntitySpriteDraw(laserHeadTexture, centerDelta - Main.screenPosition, null, drawColor, Projectile.rotation, laserHeadTexture.Frame(1, 1, 0, 0).Top(), Projectile.scale, SpriteEffects.None, 0);
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
