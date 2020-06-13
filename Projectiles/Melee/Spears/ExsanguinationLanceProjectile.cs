using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles.BaseProjectiles;

namespace CalamityMod.Projectiles.Melee.Spears
{
    public class ExsanguinationLanceProjectile : BaseSpearProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lance");
        }

        public override void SetDefaults()
        {
            projectile.width = 95;  //The width of the .png file in pixels divided by 2.
            projectile.aiStyle = 19;
            projectile.melee = true;  //Dictates whether this is a melee-class weapon.
            projectile.timeLeft = 90;
            projectile.height = 95;  //The height of the .png file in pixels divided by 2.
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.ownerHitCheck = true;
            //projectile.Calamity().trueMelee = true;
        }
        public override float InitialSpeed => 3f;
        public override float ReelbackSpeed => 2.4f;
        public override float ForwardSpeed => 0.95f;
        public override void ExtraBehavior()
        {
            if (Main.rand.NextBool(5))
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, Main.rand.NextBool(3) ? 16 : 127, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);

            Vector2 goreVec = new Vector2(projectile.position.X + (float)(projectile.width / 2) + projectile.velocity.X, projectile.position.Y + (float)(projectile.height / 2) + projectile.velocity.Y);
			if (Main.rand.NextBool(8))
			{
				int smoke = Gore.NewGore(goreVec, default, Main.rand.Next(375, 378), 1f);
				Main.gore[smoke].behindTiles = true;
			}
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 6;
            target.AddBuff(BuffID.OnFire, 300);
            if (projectile.owner == Main.myPlayer)
            {
                int boom = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<FuckYou>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0.85f + Main.rand.NextFloat() * 1.15f);
                Main.projectile[boom].Calamity().forceMelee = true;
            }
            if (crit)
            {
                for (int i = 0; i < 2; i++)
                {
                    float xPos = projectile.position.X + 800f * Main.rand.NextBool(2).ToDirectionInt();
                    Vector2 spawnPosition = new Vector2(xPos, projectile.position.Y - Main.rand.Next(-800, 801));
                    float speedX = target.position.X - spawnPosition.X;
                    float speedY = target.position.Y - spawnPosition.Y;
                    float magnitude = (float)Math.Sqrt(speedX * speedX + speedY * speedY);
                    magnitude = 10f / xPos;
                    speedX *= magnitude * 150;
                    speedY *= magnitude * 150;
                    speedX = MathHelper.Clamp(speedX, -15f, 15f);
                    speedY = MathHelper.Clamp(speedY, -15f, 15f);
                    if (projectile.owner == Main.myPlayer)
                        Projectile.NewProjectile(spawnPosition.X, spawnPosition.Y, speedX, speedY, ModContent.ProjectileType<TinyFlare>(), (int)(projectile.damage * 0.5), 2f, projectile.owner);
                }
            }
        }
    }
}
