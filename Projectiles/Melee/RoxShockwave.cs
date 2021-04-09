using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee
{
    public class RoxShockwave : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            projectile.penetrate = -1;
            projectile.width = 450;
            projectile.height = 450;
            projectile.melee = true;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.timeLeft = 40;
        }
    }
}
