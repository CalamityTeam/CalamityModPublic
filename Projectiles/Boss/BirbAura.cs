using System;
using System.IO;
using CalamityMod.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class BirbAura : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";
        float timer = 135f;
        float timeBeforeVanish = 0f;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 10000;
        }

        public override void SetDefaults()
        {
            Projectile.width = 80;
            Projectile.height = 80;
            Projectile.hostile = true;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 1200;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
            writer.Write(Projectile.localAI[1]);
            writer.Write(timer);
            writer.Write(timeBeforeVanish);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
            Projectile.localAI[1] = reader.ReadSingle();
            timer = reader.ReadSingle();
            timeBeforeVanish = reader.ReadSingle();
        }

        public override void AI()
        {
            Vector2? vector78 = null;

            if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
                Projectile.velocity = -Vector2.UnitY;

            Vector2 fireFrom = new Vector2(Projectile.ai[0], Projectile.ai[1]);
            Projectile.position = fireFrom - new Vector2(Projectile.width, Projectile.height) / 2f;

            if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
                Projectile.velocity = -Vector2.UnitY;

            if (timer > 0f)
                timer -= 1f;

            float projScale = 1f;
            if (timeBeforeVanish == 0f)
                timeBeforeVanish = Projectile.timeLeft <= 900 ? 900f : 1200f;

            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] >= timeBeforeVanish)
            {
                Projectile.Kill();
                return;
            }

            Projectile.scale = (float)Math.Sin(Projectile.localAI[0] * Math.PI / timeBeforeVanish) * 10f * projScale;
            if (Projectile.scale > projScale)
                Projectile.scale = projScale;

            float projVelRotation = Projectile.velocity.ToRotation();
            Projectile.rotation = projVelRotation - MathHelper.PiOver2;
            Projectile.velocity = projVelRotation.ToRotationVector2();

            float projWidth = Projectile.width;

            Vector2 samplingPoint = Projectile.Center;
            if (vector78.HasValue)
                samplingPoint = vector78.Value;

            float laserLength = Projectile.ai[1] - 160f;
            float[] array3 = new float[3];
            Collision.LaserScan(samplingPoint, Projectile.velocity, projWidth * Projectile.scale, laserLength, array3);
            float auraLength = 0f;
            for (int j = 0; j < array3.Length; j++)
            {
                auraLength += array3[j];
            }
            auraLength /= 3f;

            auraLength = MathHelper.Clamp(auraLength, 3600f, 4800f);

            Projectile.localAI[1] = MathHelper.Lerp(Projectile.localAI[1], auraLength, 0.5f);

            DelegateMethods.v3_1 = new Vector3(0.9f, 0.3f, 0.3f);
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * Projectile.localAI[1], Projectile.width * Projectile.scale, DelegateMethods.CastLight);
        }

        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            if (timer <= 0f && (Projectile.localAI[0] >= 120f || Projectile.timeLeft <= 900))
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    timer = 15f;
                    Vector2 fireFrom = new Vector2(target.Center.X, target.Center.Y - 900f);
                    SoundEngine.PlaySound(CommonCalamitySounds.LightningSound, target.Center - Vector2.UnitY * 300f);
                    Vector2 ai0 = target.Center - fireFrom;
                    float ai = Main.rand.Next(100);
                    Vector2 velocity = Vector2.Normalize(ai0.RotatedByRandom(MathHelper.PiOver4)) * 7f;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), fireFrom.X, fireFrom.Y, velocity.X, velocity.Y, ModContent.ProjectileType<RedLightning>(), Projectile.damage, 0f, Projectile.owner, ai0.ToRotation(), ai);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.velocity == Vector2.Zero)
            {
                return false;
            }
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D texture2 = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/BirbAuraStart", AssetRequestMode.ImmediateLoad).Value;
            Texture2D texture3 = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/BirbAuraEnd", AssetRequestMode.ImmediateLoad).Value;
            float auraDrawLength = Projectile.localAI[1]; //length of laser
            Color grayColor = new Color(128, 128, 128, 0);
            Vector2 vector = Projectile.Center - Main.screenPosition;
            Rectangle? sourceRectangle2 = null;
            Main.EntitySpriteDraw(texture2, vector, sourceRectangle2, grayColor, Projectile.rotation, texture2.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            auraDrawLength -= (texture2.Height / 2 + texture3.Height) * Projectile.scale;
            Vector2 projCenter = Projectile.Center;
            projCenter += Projectile.velocity * Projectile.scale * texture2.Height / 2f;
            if (auraDrawLength > 0f)
            {
                float auraSegment = 0f;
                Rectangle drawRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
                while (auraSegment + 1f < auraDrawLength)
                {
                    if (auraDrawLength - auraSegment < drawRectangle.Height)
                    {
                        drawRectangle.Height = (int)(auraDrawLength - auraSegment);
                    }
                    Main.EntitySpriteDraw(texture, projCenter - Main.screenPosition, new Rectangle?(drawRectangle), grayColor, Projectile.rotation, new Vector2(drawRectangle.Width / 2, 0f), Projectile.scale, SpriteEffects.None, 0);
                    auraSegment += drawRectangle.Height * Projectile.scale;
                    projCenter += Projectile.velocity * drawRectangle.Height * Projectile.scale;
                    drawRectangle.Y += texture.Height;
                    if (drawRectangle.Y + drawRectangle.Height > texture.Height)
                    {
                        drawRectangle.Y = 0;
                    }
                }
            }
            Vector2 vector2 = projCenter - Main.screenPosition;
            sourceRectangle2 = null;
            Main.EntitySpriteDraw(texture3, vector2, sourceRectangle2, grayColor, Projectile.rotation, texture3.Frame(1, 1, 0, 0).Top(), Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void CutTiles()
        {
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Vector2 unit = Projectile.velocity;
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + unit * Projectile.localAI[1], Projectile.width * Projectile.scale, DelegateMethods.CutTiles);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (timer > 15f)
            {
                return false;
            }
            if (projHitbox.Intersects(targetHitbox))
            {
                return true;
            }
            float useless = 0f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity * Projectile.localAI[1], 80f * Projectile.scale, ref useless))
            {
                return true;
            }
            return false;
        }
    }
}
