using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class EmpyreanMarble : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        public override string Texture => "CalamityMod/Projectiles/Healing/EmpyreanHealOrb";

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
            int ourpleDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 62, 0f, 0f, 100, default, 2f);
            Main.dust[ourpleDust].noGravity = true;
            Main.dust[ourpleDust].velocity *= 0f;
            CalamityUtils.HomeInOnNPC(Projectile, true, 200f, 7f, 20f);
        }
    }
}
