using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class FungiOrb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Orb");
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.penetrate = 1;
            projectile.aiStyle = 1;
            aiType = ProjectileID.Bullet;
            projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.basePointBlankShotDuration;
        }

        public override void AI()
        {
            //Rotation
            projectile.spriteDirection = projectile.direction = (projectile.velocity.X > 0).ToDirectionInt();
            projectile.rotation = projectile.velocity.ToRotation() + (projectile.spriteDirection == 1 ? 0f : MathHelper.Pi) + MathHelper.ToRadians(90) * projectile.direction;

            Lighting.AddLight(projectile.Center, new Vector3(0, 244, 252) * (1.2f / 255));

            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 4f)
            {
                Vector2 dspeed = -projectile.velocity * 0.5f;
                int num469 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 56, 0f, 0f, 100, default, 1f);
                Main.dust[num469].noGravity = true;
                Main.dust[num469].velocity = dspeed;

            }
        }

        public override void Kill(int timeLeft)
        {
            if (projectile.owner == Main.myPlayer)
            {
                for (int f = 0; f < 3; f++)
                {
                    Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
                    Projectile.NewProjectile(projectile.Center, velocity, ModContent.ProjectileType<FungiOrb2>(), (int)(projectile.damage * 0.4), 0f, projectile.owner);
                }
            }
            Main.PlaySound(SoundID.NPCDeath1, projectile.position);
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 56, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
        }
    }
}
