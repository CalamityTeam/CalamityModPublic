using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Healing
{
    public class SilvaOrb : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Healing";
        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.extraUpdates = 3;
			Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Projectile.alpha -= 2;
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.scale += 0.05f;
                if (Projectile.scale > 1.2f)
                {
                    Projectile.localAI[0] = 1f;
                }
            }
            else
            {
                Projectile.scale -= 0.05f;
                if (Projectile.scale < 0.8f)
                {
                    Projectile.localAI[0] = 0f;
                }
            }

            Projectile.HealingProjectile((int)Projectile.ai[1], (int)Projectile.ai[0], 6f, 15f);
            return;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(Main.DiscoR, 203, 103, Projectile.alpha);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                int silvaHeal = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 157, 0f, 0f, 0, new Color(Main.DiscoR, 203, 103), 1f);
                Main.dust[silvaHeal].noGravity = true;
                Main.dust[silvaHeal].velocity *= 1.5f;
                Main.dust[silvaHeal].scale = 1.5f;
            }
        }
    }
}
