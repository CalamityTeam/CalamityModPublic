using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class GaussEnergy : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gauss Energy");
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 240;
        }
        public override void AI()
        {
            for (int i = 0; i < 2; i++)
            {
                Dust dust = Dust.NewDustPerfect(projectile.Center + Main.rand.NextVector2Circular(4f, 4f), 107);
                dust.scale = Main.rand.NextFloat(1.1f, 1.3f);
                dust.noGravity = true;
            }
        }
    }
}
