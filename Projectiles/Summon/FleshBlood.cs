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
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = LifeTime;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override bool? CanHitNPC(NPC target) => Projectile.timeLeft < LifeTime - 30 && target.CanBeChasedBy(Projectile);

        public override void AI()
        {
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > 4f)
            {
                for (int i = 0; i < 2; i++)
                {
                    int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, 0f, 0f, 100, default, 1f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= 0f;
                }
            }

            if (Projectile.timeLeft < LifeTime - 30)
                CalamityGlobalProjectile.HomeInOnNPC(Projectile, !Projectile.tileCollide, 450f, 6f, 20f);
        }
    }
}
