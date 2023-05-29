using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Typeless
{
    public class NobodyKnows : ModProjectile, ILocalizedModType
    {
        public string LocalizationCategory => "Projectiles.Typeless";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 1;
        }
    }
}
