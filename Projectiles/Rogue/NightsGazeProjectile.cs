
using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class NightsGazeProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Night's Gaze");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 300;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            if (projectile.ai[0] == 0f)
            {
                projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 2.355f;
            }

            if (projectile.Calamity().stealthStrike)
            {
                if (Main.rand.Next(8) == 0)
                {
                    int projectileDamage = (int)(projectile.damage * 0.75f);
                    int projectileType = ModContent.ProjectileType<NightsGazeStar>();
                    Vector2 velocity = projectile.velocity;

                    int p = Projectile.NewProjectile(projectile.Center, velocity, projectileType, projectileDamage, 5f, projectile.owner, 1f, 0f);
                    Main.projectile[p].penetrate = 1;
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Nightwither>(), projectile.timeLeft);

            int starCount = 6;
            int starFrequency = 3;
            float spread = 20f;
            for (int i = 0; i < starCount; i++)
            {
                int projectileDamage = (int)(projectile.damage * 0.75f);
                int projectileType = ModContent.ProjectileType<NightsGazeSpark>();
                if (Main.rand.Next(starCount) < starFrequency)
                {
                    projectileType = ModContent.ProjectileType<NightsGazeStar>();
                }
                Vector2 velocity = projectile.oldVelocity.RotateRandom(MathHelper.ToRadians(spread));
                float speed = Main.rand.NextFloat(1.5f, 2f);
                float moveDuration = Main.rand.Next(5, 15);

                Projectile.NewProjectile(projectile.Center, velocity * speed, projectileType, projectileDamage, 5f, projectile.owner, 0f, moveDuration);
            }

            Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 62, 0.6f);
            Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 68, 0.2f);
            Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 122, 0.4f);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
            Main.PlaySound(0, projectile.position);
            projectile.Kill();
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 3);
            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Vector2 origin = new Vector2(projectile.width / 2, projectile.height / 2);
            spriteBatch.Draw(ModContent.GetTexture("CalamityMod/Projectiles/Rogue/NightsGazeProjectileGlow"), projectile.Center - Main.screenPosition, null, Color.White, projectile.rotation, origin, 1f, SpriteEffects.None, 0f);
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                int dustToUse = Main.rand.Next(0, 3);
                int dustType = 0;
                switch (dustToUse)
                {
                    case 0:
                        dustType = 109;
                        break;
                    case 1:
                        dustType = 111;
                        break;
                    case 2:
                        dustType = 132;
                        break;
                }

                int dust = Dust.NewDust(projectile.Center, 1, 1, dustType, projectile.velocity.X / 3f, projectile.velocity.Y / 3f, 0, default, 1.5f);
                Main.dust[dust].noGravity = true;
            }
        }
    }
}
