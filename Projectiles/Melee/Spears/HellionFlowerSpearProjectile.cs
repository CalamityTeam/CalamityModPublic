using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles.BaseProjectiles;

namespace CalamityMod.Projectiles.Melee.Spears
{
    public class HellionFlowerSpearProjectile : BaseSpearProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spear");
        }

        public override void SetDefaults()
        {
            projectile.width = 40;  //The width of the .png file in pixels divided by 2.
            projectile.aiStyle = 19;
            projectile.melee = true;  //Dictates whether this is a melee-class weapon.
            projectile.timeLeft = 90;
            projectile.height = 40;  //The height of the .png file in pixels divided by 2.
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.ownerHitCheck = true;
            projectile.Calamity().trueMelee = true;
        }

        public override float InitialSpeed => 3f;
        public override float ReelbackSpeed => 2.4f;
        public override float ForwardSpeed => 0.95f;
        public override Action<Projectile> EffectBeforeReelback => (proj) =>
        {
            Projectile.NewProjectile(projectile.Center.X + projectile.velocity.X, projectile.Center.Y + projectile.velocity.Y, projectile.velocity.X * 2.4f, projectile.velocity.Y * 2.4f, ModContent.ProjectileType<HellionSpike>(), (int)(projectile.damage * 0.65), projectile.knockBack * 0.85f, projectile.owner, 0f, 0f);
        };
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 8;
            target.AddBuff(BuffID.Venom, 300);
            if (crit)
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
				{
                    int petal = Projectile.NewProjectile(spawnPosition.X, spawnPosition.Y, speedX, speedY, ProjectileID.FlowerPetal, (int)(projectile.damage * 0.5), 2f, projectile.owner);
					Main.projectile[petal].Calamity().forceMelee = true;
					Main.projectile[petal].localNPCHitCooldown = -1;
				}
            }
        }
    }
}
