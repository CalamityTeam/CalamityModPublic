using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class FungiOrb2 : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Ranged/FungiOrb";

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
            projectile.timeLeft = 180;
        }

        public override bool? CanHitNPC(NPC target) => projectile.timeLeft < 150 && target.CanBeChasedBy(projectile);

        public override void AI()
        {
            //Rotation
            projectile.spriteDirection = projectile.direction = (projectile.velocity.X > 0).ToDirectionInt();
            projectile.rotation = projectile.velocity.ToRotation() + (projectile.spriteDirection == 1 ? 0f : MathHelper.Pi) + MathHelper.ToRadians(90) * projectile.direction;

            Lighting.AddLight(projectile.Center, new Vector3(0, 244, 252) * (1.2f / 255));

            projectile.velocity.Y += 0.1f;
            projectile.velocity.X *= 0.95f;

            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 4f)
            {
                Vector2 dspeed = -projectile.velocity * 0.5f;
                int num469 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 56, 0f, 0f, 100, default, 1f);
                Main.dust[num469].noGravity = true;
                Main.dust[num469].velocity = dspeed;
            }

            if (projectile.timeLeft < 150)
                CalamityGlobalProjectile.HomeInOnNPC(projectile, !projectile.tileCollide, 450f, 6f, 20f);
        }

        public override void Kill(int timeLeft)
        {
            Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 56, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
        }
    }
}
