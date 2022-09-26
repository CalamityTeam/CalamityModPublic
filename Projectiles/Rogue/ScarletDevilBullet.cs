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
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.timeLeft = 140;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] <= 60f)
            {
                Projectile.velocity.X *= 0.975f;
                Projectile.velocity.Y *= 0.975f;
            }
            else
            {
                Vector2 center = Projectile.Center;
                float maxDistance = 1000f;
                bool homeIn = false;

                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].CanBeChasedBy(Projectile, false))
                    {
                        float extraDistance = (float)(Main.npc[i].width / 2) + (float)(Main.npc[i].height / 2);

                        if (Vector2.Distance(Main.npc[i].Center, Projectile.Center) < (maxDistance + extraDistance))
                        {
                            center = Main.npc[i].Center;
                            homeIn = true;
                            break;
                        }
                    }
                }

                if (homeIn)
                {
                    Vector2 moveDirection = Projectile.SafeDirectionTo(center, Vector2.UnitY);
                    Projectile.velocity = (Projectile.velocity * 10f + moveDirection * 30f) / (11f);
                }
                else
                {
                    Projectile.velocity.X = 0f;
                    Projectile.velocity.Y = 0f;
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(250, 250, 250);
        }

        public override bool? CanDamage()
        {
            if (Projectile.Calamity().stealthStrike && Projectile.ai[0] < 60f)
                return false;
            return null;
        }
    }
}
