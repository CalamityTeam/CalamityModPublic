using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Healing
{
    public class ShinobiHealOrb : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Healing";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.MaxUpdates = 5;
            Projectile.timeLeft = 600;
        }

        public override void AI()
        {
            Projectile.HealingProjectile((int)Projectile.ai[1], Projectile.owner, 6f, 15f);

            for (int i = 0; i < 3; i++)
            {
                int magicHeal = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 15, 0f, 0f, 100, default, 1.3f);
                Main.dust[magicHeal].noGravity = true;
                Main.dust[magicHeal].velocity *= 0f;
            }
        }
    }
}
