using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class FleshBlood : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public const int LifeTime = 300;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blood");
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.penetrate = 1;
            projectile.timeLeft = LifeTime;
        }

        public override bool? CanHitNPC(NPC target) => projectile.timeLeft < LifeTime - 30 && target.CanBeChasedBy(projectile);

        public override void AI()
        {
            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 4f)
            {
                for (int i = 0; i < 2; i++)
                {
                    int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Blood, 0f, 0f, 100, default, 1f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= 0f;
                }
            }

            if (projectile.timeLeft < LifeTime - 30)
                CalamityGlobalProjectile.HomeInOnNPC(projectile, !projectile.tileCollide, 450f, 6f, 20f);
        }
    }
}
