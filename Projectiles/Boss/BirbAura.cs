using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Boss
{
    public class BirbAura : ModProjectile
    {
        float timer = 135f;
        float timeBeforeVanish = 0f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Draconic Aura");
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

            float num801 = 1f;
            if (timeBeforeVanish == 0f)
                timeBeforeVanish = Projectile.timeLeft <= 900 ? 900f : 1200f;

            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] >= timeBeforeVanish)
            {
                Projectile.Kill();
                return;
            }

            Projectile.scale = (float)Math.Sin(Projectile.localAI[0] * Math.PI / timeBeforeVanish) * 10f * num801;
            if (Projectile.scale > num801)
                Projectile.scale = num801;

            float num804 = Projectile.velocity.ToRotation();
            Projectile.rotation = num804 - MathHelper.PiOver2;
            Projectile.velocity = num804.ToRotationVector2();

            float num805 = 3f; //3f
            float num806 = Projectile.width;

            Vector2 samplingPoint = Projectile.Center;
            if (vector78.HasValue)
                samplingPoint = vector78.Value;

            float laserLength = Projectile.ai[1] - 160f;
            float[] array3 = new float[(int)num805];
            Collision.LaserScan(samplingPoint, Projectile.velocity, num806 * Projectile.scale, laserLength, array3);
            float num807 = 0f;
            for (int num808 = 0; num808 < array3.Length; num808++)
            {
                num807 += array3[num808];
            }
            num807 /= num805;

            num807 = MathHelper.Clamp(num807, 3600f, 4800f);

            float amount = 0.5f;
            Projectile.localAI[1] = MathHelper.Lerp(Projectile.localAI[1], num807, amount);

            DelegateMethods.v3_1 = new Vector3(0.9f, 0.3f, 0.3f);
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * Projectile.localAI[1], Projectile.width * Projectile.scale, DelegateMethods.CastLight);
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            if (timer <= 0f && (Projectile.localAI[0] >= 120f || Projectile.timeLeft <= 900))
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    timer = 15f;
                    Vector2 fireFrom = new Vector2(target.Center.X, target.Center.Y - 900f);
                    SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/LightningStrike"), (int)target.Center.X, (int)target.Center.Y - 300);
                    Vector2 ai0 = target.Center - fireFrom;
                    float ai = Main.rand.Next(100);
                    Vector2 velocity = Vector2.Normalize(ai0.RotatedByRandom(MathHelper.PiOver4)) * 7f;
                    Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), fireFrom.X, fireFrom.Y, velocity.X, velocity.Y, ModContent.ProjectileType<RedLightning>(), damage, 0f, Projectile.owner, ai0.ToRotation(), ai);
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
            Texture2D texture2 = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/BirbAuraStart").Value;
            Texture2D texture3 = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/BirbAuraEnd").Value;
            float num223 = Projectile.localAI[1]; //length of laser
            Color color44 = new Color(128, 128, 128, 0);
            Vector2 vector = Projectile.Center - Main.screenPosition;
            Rectangle? sourceRectangle2 = null;
            Main.EntitySpriteDraw(texture2, vector, sourceRectangle2, color44, Projectile.rotation, texture2.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            num223 -= (texture2.Height / 2 + texture3.Height) * Projectile.scale;
            Vector2 value20 = Projectile.Center;
            value20 += Projectile.velocity * Projectile.scale * texture2.Height / 2f;
            if (num223 > 0f)
            {
                float num224 = 0f;
                Rectangle rectangle7 = new Rectangle(0, 0, texture.Width, texture.Height);
                while (num224 + 1f < num223)
                {
                    if (num223 - num224 < rectangle7.Height)
                    {
                        rectangle7.Height = (int)(num223 - num224);
                    }
                    Main.EntitySpriteDraw(texture, value20 - Main.screenPosition, new Rectangle?(rectangle7), color44, Projectile.rotation, new Vector2(rectangle7.Width / 2, 0f), Projectile.scale, SpriteEffects.None, 0);
                    num224 += rectangle7.Height * Projectile.scale;
                    value20 += Projectile.velocity * rectangle7.Height * Projectile.scale;
                    rectangle7.Y += texture.Height;
                    if (rectangle7.Y + rectangle7.Height > texture.Height)
                    {
                        rectangle7.Y = 0;
                    }
                }
            }
            Vector2 vector2 = value20 - Main.screenPosition;
            sourceRectangle2 = null;
            Main.EntitySpriteDraw(texture3, vector2, sourceRectangle2, color44, Projectile.rotation, texture3.Frame(1, 1, 0, 0).Top(), Projectile.scale, SpriteEffects.None, 0);
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
            float num6 = 0f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity * Projectile.localAI[1], 80f * Projectile.scale, ref num6))
            {
                return true;
            }
            return false;
        }
    }
}
