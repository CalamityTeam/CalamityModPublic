using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Healing
{
    public class RainHeal : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Healing";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
			Projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (Projectile.timeLeft < 165)
            {
                if (Projectile.timeLeft > 150)
                    Projectile.velocity *= 0.9f;
                else
                    Projectile.HealingProjectile(8, Projectile.owner, 12f, 15f, false);
            }

            float dustX = Projectile.velocity.X * 0.2f;
            float dustY = -(Projectile.velocity.Y * 0.2f);
            int rainbow = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 66, 0f, 0f, 100, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1f);
            Dust dust = Main.dust[rainbow];
            dust.noGravity = true;
            dust.position.X -= dustX;
            dust.position.Y -= dustY;
        }
    }
}
