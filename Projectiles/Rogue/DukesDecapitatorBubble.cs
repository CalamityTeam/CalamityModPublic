using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Rogue
{
    public class DukesDecapitatorBubble : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Projectiles/Typeless/CoralBubble";

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
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
            int rando = Main.rand.Next(5, 9);
            for (int i = 0; i < rando; i++)
            {
                int dust = Dust.NewDust(Projectile.Center, 0, 0, 206, 0f, 0f, 100, default, 1.4f);
                Main.dust[dust].velocity *= 0.8f;
                Main.dust[dust].position = Vector2.Lerp(Main.dust[dust].position, Projectile.Center, 0.5f);
                Main.dust[dust].noGravity = true;
            }
        }
    }
}
