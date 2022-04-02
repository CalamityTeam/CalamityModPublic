using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
	public class XerocOrb : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Healing/XerocHealOrb";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Orb");
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.tileCollide = false;
            projectile.timeLeft = 200;
        }

        public override void AI()
        {
            int num469 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 62, 0f, 0f, 100, default, 2f);
            Main.dust[num469].noGravity = true;
            Main.dust[num469].velocity *= 0f;
			CalamityGlobalProjectile.HomeInOnNPC(projectile, true, 200f, 7f, 20f);
        }
    }
}
