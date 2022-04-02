using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;
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
            projectile.width = 80;
            projectile.height = 80;
            projectile.hostile = true;
            projectile.alpha = 255;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft = 1200;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.localAI[0]);
            writer.Write(projectile.localAI[1]);
            writer.Write(timer);
            writer.Write(timeBeforeVanish);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.localAI[0] = reader.ReadSingle();
            projectile.localAI[1] = reader.ReadSingle();
            timer = reader.ReadSingle();
            timeBeforeVanish = reader.ReadSingle();
        }

        public override void AI()
        {
            Vector2? vector78 = null;

            if (projectile.velocity.HasNaNs() || projectile.velocity == Vector2.Zero)
                projectile.velocity = -Vector2.UnitY;

            Vector2 fireFrom = new Vector2(projectile.ai[0], projectile.ai[1]);
            projectile.position = fireFrom - new Vector2(projectile.width, projectile.height) / 2f;

            if (projectile.velocity.HasNaNs() || projectile.velocity == Vector2.Zero)
                projectile.velocity = -Vector2.UnitY;

            if (timer > 0f)
                timer -= 1f;

            float num801 = 1f;
            if (timeBeforeVanish == 0f)
                timeBeforeVanish = projectile.timeLeft <= 900 ? 900f : 1200f;

            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] >= timeBeforeVanish)
            {
                projectile.Kill();
                return;
            }

            projectile.scale = (float)Math.Sin(projectile.localAI[0] * Math.PI / timeBeforeVanish) * 10f * num801;
            if (projectile.scale > num801)
                projectile.scale = num801;

            float num804 = projectile.velocity.ToRotation();
            projectile.rotation = num804 - MathHelper.PiOver2;
            projectile.velocity = num804.ToRotationVector2();

            float num805 = 3f; //3f
            float num806 = projectile.width;

            Vector2 samplingPoint = projectile.Center;
            if (vector78.HasValue)
                samplingPoint = vector78.Value;

            float laserLength = projectile.ai[1] - 160f;
            float[] array3 = new float[(int)num805];
            Collision.LaserScan(samplingPoint, projectile.velocity, num806 * projectile.scale, laserLength, array3);
            float num807 = 0f;
            for (int num808 = 0; num808 < array3.Length; num808++)
            {
                num807 += array3[num808];
            }
            num807 /= num805;

            num807 = MathHelper.Clamp(num807, 3600f, 4800f);

            float amount = 0.5f;
            projectile.localAI[1] = MathHelper.Lerp(projectile.localAI[1], num807, amount);

            DelegateMethods.v3_1 = new Vector3(0.9f, 0.3f, 0.3f);
            Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity * projectile.localAI[1], projectile.width * projectile.scale, new Utils.PerLinePoint(DelegateMethods.CastLight));
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            if (timer <= 0f && (projectile.localAI[0] >= 120f || projectile.timeLeft <= 900))
            {
                if (projectile.owner == Main.myPlayer)
                {
                    timer = 15f;
                    Vector2 fireFrom = new Vector2(target.Center.X, target.Center.Y - 900f);
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/LightningStrike"), (int)target.Center.X, (int)target.Center.Y - 300);
                    Vector2 ai0 = target.Center - fireFrom;
                    float ai = Main.rand.Next(100);
                    Vector2 velocity = Vector2.Normalize(ai0.RotatedByRandom(MathHelper.PiOver4)) * 7f;
                    Projectile.NewProjectile(fireFrom.X, fireFrom.Y, velocity.X, velocity.Y, ModContent.ProjectileType<RedLightning>(), damage, 0f, projectile.owner, ai0.ToRotation(), ai);
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.velocity == Vector2.Zero)
            {
                return false;
            }
            Texture2D texture = Main.projectileTexture[projectile.type];
            Texture2D texture2 = ModContent.GetTexture("CalamityMod/ExtraTextures/Lasers/BirbAuraStart");
            Texture2D texture3 = ModContent.GetTexture("CalamityMod/ExtraTextures/Lasers/BirbAuraEnd");
            float num223 = projectile.localAI[1]; //length of laser
            Color color44 = new Color(128, 128, 128, 0);
            Vector2 vector = projectile.Center - Main.screenPosition;
            Rectangle? sourceRectangle2 = null;
            spriteBatch.Draw(texture2, vector, sourceRectangle2, color44, projectile.rotation, texture2.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            num223 -= (texture2.Height / 2 + texture3.Height) * projectile.scale;
            Vector2 value20 = projectile.Center;
            value20 += projectile.velocity * projectile.scale * texture2.Height / 2f;
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
                    spriteBatch.Draw(texture, value20 - Main.screenPosition, new Rectangle?(rectangle7), color44, projectile.rotation, new Vector2(rectangle7.Width / 2, 0f), projectile.scale, SpriteEffects.None, 0f);
                    num224 += rectangle7.Height * projectile.scale;
                    value20 += projectile.velocity * rectangle7.Height * projectile.scale;
                    rectangle7.Y += texture.Height;
                    if (rectangle7.Y + rectangle7.Height > texture.Height)
                    {
                        rectangle7.Y = 0;
                    }
                }
            }
            Vector2 vector2 = value20 - Main.screenPosition;
            sourceRectangle2 = null;
            spriteBatch.Draw(texture3, vector2, sourceRectangle2, color44, projectile.rotation, texture3.Frame(1, 1, 0, 0).Top(), projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void CutTiles()
        {
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Vector2 unit = projectile.velocity;
            Utils.PlotTileLine(projectile.Center, projectile.Center + unit * projectile.localAI[1], projectile.width * projectile.scale, new Utils.PerLinePoint(DelegateMethods.CutTiles));
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
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center, projectile.Center + projectile.velocity * projectile.localAI[1], 80f * projectile.scale, ref num6))
            {
                return true;
            }
            return false;
        }
    }
}
