using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Healing
{
    public class EmpyreanHealOrb : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Healing";
        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 240;
            Projectile.extraUpdates = 3;
        }

        public override void AI()
        {
            Projectile.HealingProjectile((int)Projectile.ai[1], (int)Projectile.ai[0], 5.5f, 15f);
            float dustX = Projectile.velocity.X * 0.2f;
            float dustY = -(Projectile.velocity.Y * 0.2f);
            if (Projectile.timeLeft % 2 == 0)
            {
                int healDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 62, 0f, 0f, 100, default, 2f);
                Dust dust = Main.dust[healDust];
                dust.noGravity = true;
                dust.position.X -= dustX;
                dust.position.Y -= dustY;
            }
        }
    }
}
