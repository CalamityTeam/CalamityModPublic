using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Magic
{
    public class BlackAnurianBubble : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bubble");
        }

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

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item54, Projectile.position);
            int num3;
            for (int num246 = 0; num246 < 25; num246 = num3 + 1)
            {
                int num247 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 14, 0f, 0f, 0, default, 1f);
                Main.dust[num247].position = (Main.dust[num247].position + Projectile.position) / 2f;
                Main.dust[num247].velocity = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                Main.dust[num247].velocity.Normalize();
                Dust dust = Main.dust[num247];
                dust.velocity *= (float)Main.rand.Next(1, 30) * 0.1f;
                Main.dust[num247].alpha = Projectile.alpha;
                num3 = num246;
            }
        }
    }
}
