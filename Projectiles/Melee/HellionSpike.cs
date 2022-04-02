using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 2);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 8;
            OnHitEffects(target.Center, crit);
            target.AddBuff(BuffID.Venom, 180);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            OnHitEffects(target.Center, crit);
            target.AddBuff(BuffID.Venom, 180);
        }

        private void OnHitEffects(Vector2 targetPos, bool crit)
        {
            if (crit)
            {
                if (projectile.owner == Main.myPlayer)
                {
                    Projectile petal = CalamityUtils.ProjectileBarrage(projectile.Center, targetPos, Main.rand.NextBool(), 800f, 800f, 0f, 800f, 10f, ProjectileID.FlowerPetal, (int)(projectile.damage * 0.5), projectile.knockBack * 0.5f, projectile.owner, true);
                    if (petal.whoAmI.WithinBounds(Main.maxProjectiles))
                    {
                        petal.Calamity().forceMelee = true;
                        petal.localNPCHitCooldown = -1;
                    }
                }
            }
        }
    }
}
