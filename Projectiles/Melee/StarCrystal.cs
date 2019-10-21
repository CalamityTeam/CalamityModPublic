using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class StarCrystal : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Star");
        }

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.aiStyle = 29;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.penetrate = 1;
            projectile.alpha = 255;
            projectile.timeLeft = 600;
            aiType = 121;
        }

        public override void Kill(int timeLeft)
        {
            if (projectile.owner == Main.myPlayer)
            {
                for (int k = 0; k < 3; k++)
                {
                    Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 227, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
                    Projectile.NewProjectile(projectile.position.X, projectile.position.Y, (float)Main.rand.Next(-35, 36) * 0.2f, (float)Main.rand.Next(-35, 36) * 0.2f, ModContent.ProjectileType<TinyCrystal>(),
                    (int)((double)projectile.damage * 0.5), projectile.knockBack * 0.15f, Main.myPlayer, 0f, 0f);
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 240);
        }
    }
}
