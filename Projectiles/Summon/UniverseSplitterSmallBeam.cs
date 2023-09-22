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
    public class UniverseSplitterSmallBeam : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        // Pretty self explanatory
        public const int FadeinTime = 25;
        public const int MovementTime = 60;
        public const int TimeLeft = 120;
        public const float TotalSweeps = 4f;
        public const float MaximumLength = 3000f;
        // How fat the laser is
        public const float LaserSize = 1.2f;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 12000;
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = TimeLeft;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 6;
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

        public static float CosineInterpolation(float start, float end, float time)
        {
            float cosineTime = (1f - (float)Math.Cos(MathHelper.Pi * time)) * 0.5f;
            return start * (1 - cosineTime) + end * cosineTime;
        }

        public override bool ShouldUpdatePosition() => false;

        public override void AI()
        {
            if (Projectile.localAI[0] <= MovementTime)
            {
                Projectile.velocity = (Projectile.ai[0] + (float)Math.Cos(Projectile.localAI[0] / MovementTime * TotalSweeps) *
                                       CosineInterpolation(1f, 0f, Projectile.localAI[0] / MovementTime) * UniverseSplitterField.SmallBeamAngleMax).ToRotationVector2();
            }
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;

            Projectile.localAI[0] += 1f;

            // Fadein/out. The timeleft varies, so this must be done manually.
            if (Projectile.localAI[0] < FadeinTime)
            {
                Projectile.scale = MathHelper.Lerp(0.01f, LaserSize, Projectile.localAI[0] / FadeinTime);
            }
            if (Projectile.localAI[0] > MovementTime &&
                Projectile.localAI[0] <= MovementTime + 10f)
            {
                Projectile.scale = MathHelper.Lerp(LaserSize, 3.25f, (Projectile.localAI[0] - MovementTime) / 10f);
            }
            if (Projectile.localAI[0] > TimeLeft - 15f)
            {
                Projectile.scale = MathHelper.Lerp(3.25f, 0.01f, (Projectile.localAI[0] - (TimeLeft - 15f)) / 15f);
            }

            Projectile.localAI[1] = MathHelper.Lerp(Projectile.localAI[1], MaximumLength, 0.5f);
            Vector2 beamEndPosiiton = Projectile.Center + Projectile.velocity * (Projectile.localAI[1] - 6f);

            // Draw white light across the laser
            DelegateMethods.v3_1 = Color.White.ToVector3();
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * Projectile.localAI[1], Projectile.width * Projectile.scale, DelegateMethods.CastLight);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.velocity == Vector2.Zero)
            {
                return false;
            }
            Texture2D laserTailTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/UniverseSplitterSmallBeamBegin", AssetRequestMode.ImmediateLoad).Value;
            Texture2D laserBodyTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/UniverseSplitterSmallBeamMid", AssetRequestMode.ImmediateLoad).Value;
            Texture2D laserHeadTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/UniverseSplitterSmallBeamEnd", AssetRequestMode.ImmediateLoad).Value;
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
                Rectangle sourceRectangle = new Rectangle(0, 14 * (Projectile.timeLeft / 3 % 5), laserBodyTexture.Width, 16);
                while (laserLengthDelta + 1f < laserLength)
                {
                    if (laserLength - laserLengthDelta < sourceRectangle.Height)
                    {
                        sourceRectangle.Height = (int)(laserLength - laserLengthDelta);
                    }
                    Main.EntitySpriteDraw(laserBodyTexture, centerDelta - Main.screenPosition, new Rectangle?(sourceRectangle), drawColor, Projectile.rotation, new Vector2(sourceRectangle.Width / 2f, 0f), Projectile.scale, SpriteEffects.None, 0);
                    laserLengthDelta += sourceRectangle.Height * Projectile.scale;
                    centerDelta += Projectile.velocity * sourceRectangle.Height * Projectile.scale;
                    sourceRectangle.Y += 14;
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
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.Electrified, 180);
    }
}
