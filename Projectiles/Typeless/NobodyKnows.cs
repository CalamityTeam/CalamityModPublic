using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Typeless
{
    public class NobodyKnows : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nope");
        }

        public override void SetDefaults()
        {
            projectile.width = 2;
            projectile.height = 2;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 1;
        }
    }
}
