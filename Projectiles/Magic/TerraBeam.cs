using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class TerraBeam : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bolt");
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.friendly = true;
            projectile.magic = true;
			projectile.ignoreWater = true;
			projectile.penetrate = 10;
            projectile.extraUpdates = 100;
            projectile.timeLeft = 150;
        }

        public override void AI()
        {
            projectile.localAI[1] += 1f;
            if (projectile.localAI[1] >= 21f && projectile.owner == Main.myPlayer)
            {
                projectile.localAI[1] = 0f;
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<TerraOrb>(), (int)(projectile.damage * 0.7), projectile.knockBack, projectile.owner, 0f, 0f);
            }

            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 9f)
            {
                for (int num447 = 0; num447 < 3; num447++)
                {
                    Vector2 vector33 = projectile.position;
                    vector33 -= projectile.velocity * ((float)num447 * 0.25f);
                    int num448 = Dust.NewDust(vector33, 1, 1, 107, 0f, 0f, 0, default, 1.25f);
                    Main.dust[num448].position = vector33;
                    Main.dust[num448].scale = (float)Main.rand.Next(70, 110) * 0.013f;
					Main.dust[num448].velocity *= 0.1f;
				}
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 8;
        }
    }
}
