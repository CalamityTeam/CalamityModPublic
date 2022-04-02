using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Projectiles.Ranged
{
    public class AquashardSplit : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Ranged/Aquashard";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aquashard");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 180;
            projectile.aiStyle = 1;
        }

        public override bool? CanHitNPC(NPC target) => projectile.timeLeft < 150 && target.CanBeChasedBy(projectile);

        public override void AI()
        {
            projectile.velocity.X *= 0.9995f;
            projectile.velocity.Y += 0.01f;

            if (projectile.timeLeft < 150)
                CalamityGlobalProjectile.HomeInOnNPC(projectile, !projectile.tileCollide, 450f, 6f, 20f);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item, (int)projectile.position.X, (int)projectile.position.Y, 27);
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 154, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
        }
    }
}
