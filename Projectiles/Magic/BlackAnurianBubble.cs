using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Magic
{
    public class BlackAnurianBubble : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] > 2f)
            {
                Projectile.alpha -= 20;
                if (Projectile.alpha < 100)
                {
                    Projectile.alpha = 100;
                }
            }
            else
            {
                Projectile.localAI[0] += 1f;
            }
            if (Projectile.ai[0] > 30f)
            {
                if (Projectile.velocity.Y > -8f)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y - 0.05f;
                }
                Projectile.velocity.X = Projectile.velocity.X * 0.98f;
            }
            else
            {
                Projectile.ai[0] += 1f;
            }
            Projectile.rotation = Projectile.velocity.X * 0.1f;
            if (Projectile.wet)
            {
                if (Projectile.velocity.Y > 0f)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y * 0.98f;
                }
                if (Projectile.velocity.Y > -8f)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y - 0.2f;
                }
                Projectile.velocity.X = Projectile.velocity.X * 0.94f;
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item54, Projectile.position);
            int inc;
            for (int i = 0; i < 25; i = inc + 1)
            {
                int blackDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 14, 0f, 0f, 0, default, 1f);
                Main.dust[blackDust].position = (Main.dust[blackDust].position + Projectile.position) / 2f;
                Main.dust[blackDust].velocity = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                Main.dust[blackDust].velocity.Normalize();
                Dust dust = Main.dust[blackDust];
                dust.velocity *= (float)Main.rand.Next(1, 30) * 0.1f;
                Main.dust[blackDust].alpha = Projectile.alpha;
                inc = i;
            }
        }
    }
}
