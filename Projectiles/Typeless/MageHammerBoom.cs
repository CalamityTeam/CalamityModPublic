using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Typeless
{
    public class MageHammerBoom : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Explosion");
        }

        public override void SetDefaults()
        {
            projectile.width = 500;
            projectile.height = 500;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 5;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 1;
        }
    }
}
