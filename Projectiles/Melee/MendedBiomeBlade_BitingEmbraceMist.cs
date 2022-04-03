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
        public Player Owner => Main.player[Projectile.owner];
        public Color mistColor;
        public int variant = -1;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Glacial Mist");
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = Projectile.height = 34;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 300;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 size = Projectile.Size * Projectile.scale;

            return Collision.CheckAABBvAABBCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center - size / 2f, size);
        }

        public override void AI()
        {
            if (variant == -1)
                variant = Main.rand.Next(3);

            if (Main.rand.Next(15) == 0 && Projectile.alpha <= 140) //only try to spawn your particles if you're not close to dying
            {
                Vector2 particlePosition = Projectile.Center + Main.rand.NextVector2Circular(Projectile.width * Projectile.scale * 0.5f, Projectile.height * Projectile.scale * 0.5f);
                Particle snowflake = new SnowflakeSparkle(particlePosition, Vector2.Zero, Color.White, new Color(75, 177, 250), Main.rand.NextFloat(0.3f, 1.5f), 40, 0.5f);
                GeneralParticleHandler.SpawnParticle(snowflake);
            }

            Projectile.velocity *= 0.85f;
            Projectile.position += Projectile.velocity;
            Projectile.rotation += 0.02f * Projectile.timeLeft / 300f * ((Projectile.velocity.X > 0) ? 1f : -1f);

            if (Projectile.alpha < 165)
            {
                Projectile.scale += 0.05f;
                Projectile.alpha += 2;
            }
            else
            {
                Projectile.scale *= 0.975f;
                Projectile.alpha += 1;
            }
            if (Projectile.alpha >= 170)
                Projectile.Kill();

            mistColor = Color.Lerp(new Color(172, 238, 255), new Color(145, 170, 188), MathHelper.Clamp((float)(Projectile.alpha - 100) / 80, 0f, 1f)) * (255 - Projectile.alpha / 255f);
        }


        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.EnterShaderRegion(BlendState.Additive);

            var tex = GetTexture("CalamityMod/Particles/MediumMist");
            Rectangle frame = tex.Frame(1, 3, 0, variant);
            Main.EntitySpriteDraw(tex, Projectile.position - Main.screenPosition, frame, mistColor * 0.5f * ((255f - Projectile.alpha) / 255f), Projectile.rotation, frame.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            Main.spriteBatch.ExitShaderRegion();
            return false;
        }

    }
}
