using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class EmpyreanMarble : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Healing/EmpyreanHealOrb";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Empyrean Marble");
        }

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 200;
        }

        public override void AI()
        {
            int num469 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 62, 0f, 0f, 100, default, 2f);
            Main.dust[num469].noGravity = true;
            Main.dust[num469].velocity *= 0f;
            CalamityUtils.HomeInOnNPC(Projectile, true, 200f, 7f, 20f);
        }
    }
}
