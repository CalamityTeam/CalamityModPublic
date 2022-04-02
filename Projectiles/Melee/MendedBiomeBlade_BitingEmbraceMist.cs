using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Projectiles.Melee
{
    public class BitingEmbraceMist : ModProjectile
    {
        public override string Texture => "CalamityMod/Particles/MediumMist";
        public Player Owner => Main.player[projectile.owner];
        public Color mistColor;
        public int variant = -1;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Glacial Mist");
        }
        public override void SetDefaults()
        {
            projectile.melee = true;
            projectile.width = projectile.height = 34;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.penetrate = 2;
            projectile.timeLeft = 300;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 size = projectile.Size * projectile.scale;

            return Collision.CheckAABBvAABBCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center - size / 2f, size);
        }

        public override void AI()
        {
            if (variant == -1)
                variant = Main.rand.Next(3);

            if (Main.rand.Next(15) == 0 && projectile.alpha <= 140) //only try to spawn your particles if you're not close to dying
            {
                Vector2 particlePosition = projectile.Center + Main.rand.NextVector2Circular(projectile.width * projectile.scale * 0.5f, projectile.height * projectile.scale * 0.5f);
                Particle snowflake = new SnowflakeSparkle(particlePosition, Vector2.Zero, Color.White, new Color(75, 177, 250), Main.rand.NextFloat(0.3f, 1.5f), 40, 0.5f);
                GeneralParticleHandler.SpawnParticle(snowflake);
            }

            projectile.velocity *= 0.85f;
            projectile.position += projectile.velocity;
            projectile.rotation += 0.02f * projectile.timeLeft / 300f * ((projectile.velocity.X > 0) ? 1f : -1f);

            if (projectile.alpha < 165)
            {
                projectile.scale += 0.05f;
                projectile.alpha += 2;
            }
            else
            {
                projectile.scale *= 0.975f;
                projectile.alpha += 1;
            }
            if (projectile.alpha >= 170)
                projectile.Kill();

            mistColor = Color.Lerp(new Color(172, 238, 255), new Color(145, 170, 188), MathHelper.Clamp((float)(projectile.alpha - 100) / 80, 0f, 1f)) * (255 - projectile.alpha / 255f);
        }


        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            spriteBatch.EnterShaderRegion(BlendState.Additive);

            var tex = GetTexture("CalamityMod/Particles/MediumMist");
            Rectangle frame = tex.Frame(1, 3, 0, variant);
            spriteBatch.Draw(tex, projectile.position - Main.screenPosition, frame, mistColor * 0.5f * ((255f - projectile.alpha) / 255f), projectile.rotation, frame.Size() * 0.5f, projectile.scale, SpriteEffects.None, 0f);

            spriteBatch.ExitShaderRegion();
            return false;
        }

    }
}
