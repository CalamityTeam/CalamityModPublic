using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Enemy
{
    public class HorribleHogRubble : ModProjectile, ILocalizedModType
    {
        public int FrameX;

        public new string LocalizationCategory => "Projectiles.Enemy";

        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.DeerclopsRangedProjectile;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.scale = 0f;
            Projectile.hostile = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.spriteDirection = Main.rand.NextBool().ToDirectionInt();
            FrameX = Main.rand.Next(3);
        }

        public override void AI()
        {
            Projectile.scale = MathHelper.Lerp(Projectile.scale, 1f, 0.25f);
            Projectile.ExpandHitboxBy(Projectile.scale);

            Projectile.rotation += Projectile.velocity.X * 0.035f;
            if (Projectile.velocity.Y < 8f)
                Projectile.velocity.Y += 0.165f;

            if (Projectile.timeLeft <= 250)
                Projectile.velocity.X *= 0.92f;
        }

        public override void OnKill(int timeLeft)
        {
            int dustCloudAmt = Main.rand.Next(3, 6);
            for (int i = 0; i < dustCloudAmt; i++)
            {
                Vector2 spawnPosition = Projectile.Center + Main.rand.NextVector2Circular(8f, 8f);
                Vector2 velocity = Main.rand.NextVector2Circular(3f, 3f);
                Color color = Color.Lerp(Color.SaddleBrown, Color.SandyBrown, Main.rand.NextFloat());
                float rotationSpeed = Main.rand.NextFloat(0.01f, 0.03f) * Main.rand.NextBool().ToDirectionInt();

                SmallSmokeParticle dustCloud = new(spawnPosition, velocity, color, color, Main.rand.NextFloat(1.6f, 1.8f), Main.rand.Next(75, 100), rotationSpeed);
                GeneralParticleHandler.SpawnParticle(dustCloud, true);
            }

            int dustAmt = Main.rand.Next(6, 9);
            for (int i = 0; i < dustAmt; i++)
            {
                Vector2 spawnPosition = Projectile.Center + Main.rand.NextVector2Circular(8f, 8f);
                Vector2 velocity = Main.rand.NextVector2Circular(3f, 3f);
                Dust.NewDust(spawnPosition, 0, 0, DustID.Dirt, velocity.X, velocity.Y, Scale: Main.rand.NextFloat(0.8f, 1.2f));
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Rectangle frame = texture.Frame(3, Main.projFrames[Type], FrameX, Projectile.frame);
            Vector2 origin = frame.Size() * 0.5f;
            SpriteEffects spriteEffects = Projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            if (CalamityClientConfig.Instance.Afterimages)
            {
                for (int i = 0; i < Projectile.oldPos.Length; ++i)
                {
                    Vector2 drawPos = Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
                    float interpolant = ((float)(Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length);
                    Main.spriteBatch.Draw(texture, drawPos, frame, Projectile.GetAlpha(lightColor) * 0.3f * interpolant, Projectile.rotation, origin, Projectile.scale * interpolant, spriteEffects, 0f);
                }
            }

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, spriteEffects, 0f);
            return false;
        }
    }
}
