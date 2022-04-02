using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Projectiles.Rogue
{
    public class NychthemeronProjectile : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/Nychthemeron";

        public static int lifetime = 300;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nychthemeron");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 4;
            projectile.timeLeft = lifetime;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            if (projectile.ai[0] == 0f)
            {
                // Thrown out and floating in the air
                projectile.velocity *= 0.99f;
            }
            else
            {
                Player owner = Main.player[projectile.owner];

                projectile.tileCollide = false;

                // Recall to the player
                Vector2 toPlayer = owner.Center - projectile.Center;
                toPlayer.Normalize();
                toPlayer *= projectile.ai[0];
                projectile.velocity = toPlayer;

                projectile.ai[0] += 0.5f;
                projectile.extraUpdates = 1;
                if (projectile.ai[0] > 20f)
                {
                    projectile.ai[0] = 20f;
                }

                // Delete the projectile if it touches its owner.
                if (Main.myPlayer == projectile.owner && projectile.Hitbox.Intersects(owner.Hitbox))
                {
                    projectile.Kill();
                }
            }
            projectile.rotation += 0.4f * projectile.direction * ((float)projectile.timeLeft / (float)lifetime);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
            Main.PlaySound(SoundID.Dig, projectile.position);

            float minScale = 0.9f;
            float maxScale = 1.1f;
            int numDust = 2;
            for (int i = 0; i < numDust; i++)
            {
                Dust.NewDust(projectile.position, 4, 4, 236, projectile.velocity.X, projectile.velocity.Y, 0, default, Main.rand.NextFloat(minScale, maxScale));
                Dust.NewDust(projectile.position, 4, 4, 240, projectile.velocity.X, projectile.velocity.Y, 0, default, Main.rand.NextFloat(minScale, maxScale));
            }

            projectile.Kill();
            return false;
        }

        public override void Kill(int timeLeft)
        {
            if (timeLeft <= 0)
            {
                float minScale = 0.9f;
                float maxScale = 1.1f;
                int numDust = 2;
                for (int i = 0; i < numDust; i++)
                {
                    Dust.NewDust(projectile.position, 4, 4, 236, projectile.velocity.X, projectile.velocity.Y, 0, default, Main.rand.NextFloat(minScale, maxScale));
                    Dust.NewDust(projectile.position, 4, 4, 240, projectile.velocity.X, projectile.velocity.Y, 0, default, Main.rand.NextFloat(minScale, maxScale));
                }
            }
        }
    }
}
