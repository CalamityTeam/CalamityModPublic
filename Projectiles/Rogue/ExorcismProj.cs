using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class ExorcismProj : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/Exorcism";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Exorcism");
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.alpha = 0;
            projectile.penetrate = 1;
            projectile.tileCollide = true;
            projectile.timeLeft = 600;
            projectile.Calamity().rogue = true;
            projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            projectile.velocity.Y += 0.1f;
            projectile.rotation += 0.05f * projectile.direction;

            // Damage Scaling
            if (projectile.velocity.Y > 0 && projectile.ai[0] < 2f)
            {
                projectile.ai[0] += 0.015f;
            }
            if (projectile.ai[0] > 2f)
            {
                projectile.ai[0] = 2f;
            }

            projectile.damage = (int)(projectile.ai[1] * projectile.ai[0]);

            // Dust Effects
            Vector2 dustLeft = (new Vector2(-1, 0)).RotatedBy(projectile.rotation);
            Vector2 dustRight = (new Vector2(1, 0)).RotatedBy(projectile.rotation);
            Vector2 dustUp = (new Vector2(0, -1)).RotatedBy(projectile.rotation);
            Vector2 dustDown = (new Vector2(0, 1) * 2f).RotatedBy(projectile.rotation);

            float minSpeed = 1.5f;
            float maxSpeed = 5f;
            float minScale = 0.8f;
            float maxScale = 1.4f;

            int dustType = 175;
            int dustCount = (int)(5 * (projectile.ai[0] - 1f));

            for (int i = 0; i < dustCount; i++)
            {
                int left = Dust.NewDust(projectile.Center, 1, 1, dustType, 0f, 0f);
                Main.dust[left].noGravity = true;
                Main.dust[left].position = projectile.Center;
                Main.dust[left].velocity = dustLeft * Main.rand.NextFloat(minSpeed, maxSpeed) + projectile.velocity;
                Main.dust[left].scale = Main.rand.NextFloat(minScale, maxScale);

                int right = Dust.NewDust(projectile.Center, 1, 1, dustType, 0f, 0f);
                Main.dust[right].noGravity = true;
                Main.dust[right].position = projectile.Center;
                Main.dust[right].velocity = dustRight * Main.rand.NextFloat(minSpeed, maxSpeed) + projectile.velocity;
                Main.dust[right].scale = Main.rand.NextFloat(minScale, maxScale);

                int up = Dust.NewDust(projectile.Center, 1, 1, dustType, 0f, 0f);
                Main.dust[up].noGravity = true;
                Main.dust[up].position = projectile.Center;
                Main.dust[up].velocity = dustUp * Main.rand.NextFloat(minSpeed, maxSpeed) + projectile.velocity;
                Main.dust[up].scale = Main.rand.NextFloat(minScale, maxScale);

                int down = Dust.NewDust(projectile.Center, 1, 1, dustType, 0f, 0f);
                Main.dust[down].noGravity = true;
                Main.dust[down].position = projectile.Center;
                Main.dust[down].velocity = dustDown * Main.rand.NextFloat(minSpeed, maxSpeed) + projectile.velocity;
                Main.dust[down].scale = Main.rand.NextFloat(minScale, maxScale);
            }
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
            Main.PlaySound(SoundID.Dig, projectile.Center);
            projectile.Kill();
            return true;
        }

        public override void Kill(int timeLeft)
        {
            //Crystal smash sound
            Main.PlaySound(SoundID.Item27, projectile.Center);
            // Light burst
            int p = Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<ExorcismShockwave>(), (int)(projectile.damage * 0.8f), 0, projectile.owner, projectile.ai[0] - 1f, 0);
            Main.projectile[p].rotation = projectile.rotation;
            // Stars
            if (projectile.Calamity().stealthStrike)
            {
                int numStars = Main.rand.Next(4, 7);
                for (int i = 0; i < numStars; i++)
                {
                    Vector2 pos = new Vector2(projectile.Center.X + (float)projectile.width * 0.5f + (float)Main.rand.Next(-201, 201), Main.screenPosition.Y - 600f - Main.rand.Next(50));
                    float speedX = (projectile.Center.X - pos.X) / 20f;
                    float speedY = (projectile.Center.Y - pos.Y) / 20f;
                    Projectile.NewProjectile(pos.X, pos.Y, speedX, speedY, ModContent.ProjectileType<ExorcismStar>(), projectile.damage / 2, 3, projectile.owner, Main.rand.NextFloat(-3f, 3f), 0f);
                }
            }
        }
    }
}
