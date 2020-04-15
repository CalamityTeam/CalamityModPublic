using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class HellionSpike : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spike");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.aiStyle = 27;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.penetrate = 3;
            projectile.timeLeft = 600;
            aiType = ProjectileID.SporeCloud;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0f, 0.25f, 0f);
            if (Main.rand.NextBool(3))
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 44, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }
        }

        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 44, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
			if (projectile.timeLeft > 595)
				return false;

			CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 2);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 8;
            target.AddBuff(BuffID.Venom, 300);
            if (crit)
            {
                float xPos = projectile.position.X + 800 * Main.rand.NextBool(2).ToDirectionInt();
                Vector2 spawnPosition = new Vector2(xPos, projectile.position.Y - Main.rand.Next(600, 801));
                float num80 = xPos;
                float speedX = (float)target.position.X - spawnPosition.X;
                float speedY = (float)target.position.Y - spawnPosition.Y;
                float dir = (float)Math.Sqrt((double)(speedX * speedX + speedY * speedY));
                dir = 10 / num80;
                speedX *= dir * 150;
                speedY *= dir * 150;
                speedX = MathHelper.Clamp(speedX, -15f, 15f);
                speedY = MathHelper.Clamp(speedY, -15f, 15f);
                if (projectile.owner == Main.myPlayer)
                {
                    int petal = Projectile.NewProjectile(spawnPosition, new Vector2(speedX, speedY), ProjectileID.FlowerPetal, (int)(projectile.damage * 0.5), 2f, projectile.owner);
					Main.projectile[petal].Calamity().forceMelee = true;
					Main.projectile[petal].localNPCHitCooldown = -1;
                }
            }
        }
    }
}
