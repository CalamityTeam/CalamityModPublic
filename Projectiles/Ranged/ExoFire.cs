using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class ExoFire : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public bool ProducedAcceleration = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fire");
        }

        public override void SetDefaults()
        {
            projectile.width = 24;
            projectile.height = 24;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.ranged = true;
            projectile.penetrate = -1;
            projectile.extraUpdates = 10;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.timeLeft = 180;
        }

        public override void AI()
        {
            float speedX = 1f;
            float speedY = 1f;
            if (!ProducedAcceleration)
            {
                speedX = Main.rand.NextBool(2) ? 1.03f : 0.97f;
                projectile.velocity *= Utils.RandomVector2(Main.rand, 0.97f, 1.03f);
                ProducedAcceleration = true;
            }
            projectile.velocity.X *= speedX;
            projectile.velocity.X *= speedY;
            if (projectile.ai[0] > 7f)
            {
                float num296 = 1f;
                if (projectile.ai[0] == 8f)
                {
                    num296 = 0.25f;
                }
                else if (projectile.ai[0] == 9f)
                {
                    num296 = 0.5f;
                }
                else if (projectile.ai[0] == 10f)
                {
                    num296 = 0.75f;
                }
                projectile.ai[0] += 1f;
                int num297 = Main.rand.NextBool(2) ? 107 : 234;
                if (Main.rand.NextBool(4))
                {
                    num297 = 269;
                }
                if (Main.rand.NextBool(2))
                {
                    for (int num298 = 0; num298 < 2; num298++)
                    {
                        int num299 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, num297, projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f, 100, default, 0.6f);
                        Dust dust = Main.dust[num299];
                        if (Main.rand.NextBool(3))
                        {
                            dust.scale *= 1.5f;
                            dust.velocity.X *= 1.2f;
                            dust.velocity.Y *= 1.2f;
                        }
                        else
                        {
                            dust.scale *= 0.75f;
                        }
                        dust.noGravity = true;
                        dust.velocity.X *= 0.8f;
                        dust.velocity.Y *= 0.8f;
                        dust.scale *= num296;
                        dust.velocity += projectile.velocity;
                    }
                }
            }
            else
            {
                projectile.ai[0] += 1f;
            }
            projectile.rotation += 0.3f * (float)projectile.direction;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			target.ExoDebuffs();
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
			target.ExoDebuffs();
        }
    }
}
