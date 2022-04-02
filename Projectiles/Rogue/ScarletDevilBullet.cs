using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class ScarletDevilBullet : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gungnir Bullet");
        }

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.friendly = true;
            projectile.timeLeft = 140;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            projectile.ai[0] += 1f;
            if (projectile.ai[0] <= 60f)
            {
                projectile.velocity.X *= 0.975f;
                projectile.velocity.Y *= 0.975f;
            }
            else
            {
                Vector2 center = projectile.Center;
                float maxDistance = 1000f;
                bool homeIn = false;

                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].CanBeChasedBy(projectile, false))
                    {
                        float extraDistance = (float)(Main.npc[i].width / 2) + (float)(Main.npc[i].height / 2);

                        if (Vector2.Distance(Main.npc[i].Center, projectile.Center) < (maxDistance + extraDistance))
                        {
                            center = Main.npc[i].Center;
                            homeIn = true;
                            break;
                        }
                    }
                }

                if (homeIn)
                {
                    Vector2 moveDirection = projectile.SafeDirectionTo(center, Vector2.UnitY);
                    projectile.velocity = (projectile.velocity * 10f + moveDirection * 30f) / (11f);
                }
                else
                {
                    projectile.velocity.X = 0f;
                    projectile.velocity.Y = 0f;
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(250, 250, 250);
        }

        public override bool CanDamage()
        {
            if (projectile.Calamity().stealthStrike && projectile.ai[0] < 60f)
                return false;
            return base.CanDamage();
        }
    }
}
