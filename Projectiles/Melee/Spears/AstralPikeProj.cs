using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Projectiles.BaseProjectiles;

namespace CalamityMod.Projectiles.Melee.Spears
{
    public class AstralPikeProj : BaseSpearProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pike");
        }

        public override void SetDefaults()
        {
            projectile.width = 40;  //The width of the .png file in pixels divided by 2.
            projectile.aiStyle = 19;
            projectile.melee = true;  //Dictates whether projectile is a melee-class weapon.
            projectile.timeLeft = 90;
            projectile.height = 40;  //The height of the .png file in pixels divided by 2.
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
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, ModContent.DustType<AstralOrange>(), projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 120);
            target.immune[projectile.owner] = 6;
            if (crit)
            {
                for (int i = 0; i < 3; i++)
                {
                    float xPos = projectile.position.X + 800f * Main.rand.NextBool(2).ToDirectionInt();
                    Vector2 spawnPosition = new Vector2(xPos, projectile.position.Y - Main.rand.Next(-800, 801));
                    float speedX = target.position.X - spawnPosition.X;
                    float speedY = target.position.Y - spawnPosition.Y;
                    float magnitude = (float)Math.Sqrt(speedX * speedX + speedY * speedY);
                    magnitude = 10f / xPos;
                    speedX *= magnitude * 150f;
                    speedY *= magnitude * 150f;
                    speedX = MathHelper.Clamp(speedX, -15f, 15f);
                    speedY = MathHelper.Clamp(speedY, -15f, 15f);
                    if (projectile.owner == Main.myPlayer)
                    {
                        int proj = Projectile.NewProjectile(spawnPosition.X, spawnPosition.Y, speedX, speedY, ModContent.ProjectileType<AstralStar>(), (int)(projectile.damage * 0.4), 1f, projectile.owner, 3f, 0f);
                        Main.projectile[proj].Calamity().forceMelee = true;
                    }
                }
            }
        }
    }
}
