using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class UltimaBolt : ModProjectile
    {
        public float Bounces
        {
            get => projectile.ai[0];
            set => projectile.ai[0] = value;
        }
        public float DefaultHue => ((float)Math.Sin(Main.GlobalTime * 2.5f) + 1) * 0.5f;
        public const int MaxBounces = 1;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 3;
            DisplayName.SetDefault("Ultima Bolt");
        }

        public override void SetDefaults()
        {
            projectile.width = 18;
            projectile.height = 42;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.arrow = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 300;
            projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.basePointBlankShotDuration;
        }

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public Color GetColorFromHue(float hue) => Main.hslToRgb(hue, 1f, 0.725f) * projectile.Opacity;
        public override Color? GetAlpha(Color lightColor) => GetColorFromHue(DefaultHue);

        public void ProduceExplosionDust(int dustCount)
        {
            if (!Main.dedServ)
            {
                for (int i = 0; i < dustCount; i++)
                {
                    Dust dust = Dust.NewDustPerfect(projectile.Center, 267);
                    dust.color = Main.hslToRgb(Main.rand.NextFloat(), 1f, 0.8f);
                    dust.scale = Main.rand.NextFloat(1f, 1.25f);
                    dust.velocity = Main.rand.NextVector2Circular(7f, 7f);
                    dust.noGravity = true;
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Bounces >= MaxBounces)
            {
                projectile.Kill();
                return false;
            }

            if (projectile.velocity.X != oldVelocity.X)
            {
                projectile.velocity.X = -oldVelocity.X;
            }
            if (projectile.velocity.Y != oldVelocity.Y)
            {
                projectile.velocity.Y = -oldVelocity.Y;
            }
            Bounces++;
            ProduceExplosionDust(24);

            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = ModContent.GetTexture(Texture);
            for (int i = 0; i < projectile.oldPos.Length; i++)
            {
                if (i > 0)
                {
                    CalamityUtils.DistanceClamp(ref projectile.oldPos[i], ref projectile.oldPos[i - 1], 8f); // Ensure the max distance between the old positions isn't too great. It looks weird if it is.
                }
                float completionRatio = i / (float)projectile.oldPos.Length;
                Color color = GetColorFromHue((DefaultHue + completionRatio * 0.5f) % 1f) * (float)Math.Pow(1f - completionRatio, 2);
                spriteBatch.Draw(texture,
                                 projectile.oldPos[i] + texture.Size() * 0.5f - Main.screenPosition,
                                 null,
                                 color,
                                 projectile.rotation,
                                 texture.Size() * 0.5f,
                                 projectile.scale,
                                 SpriteEffects.None,
                                 0f);
            }
            return false;
        }

        public override void Kill(int timeLeft)
        {
            CalamityGlobalProjectile.ExpandHitboxBy(projectile, 90, 90);
            projectile.Damage();
            ProduceExplosionDust(32);
        }
    }
}
