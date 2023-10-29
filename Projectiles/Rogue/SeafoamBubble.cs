using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Rogue
{
    public class SeafoamBubble : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Projectiles/Typeless/CoralBubble";

        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 28;
            Projectile.friendly = true;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.scale = 1f;
            Projectile.localNPCHitCooldown = 30;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override void AI()
        {
            Projectile.ai[0] += 1f;
            if (Projectile.alpha < 50)
            {
                Projectile.alpha = 50;
            }
            else if (Projectile.alpha > 50)
            {
                Projectile.alpha -= 10;
            }
            Projectile.scale += 0.002f;
            if (Projectile.ai[0] % 60 == 0)
            {
                Projectile.damage *= 2;
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item54, Projectile.position);
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 60;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            int inc;
            for (int i = 0; i < 25; i = inc + 1)
            {
                int waterDust = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, 154, 0f, 0f, 0, default, 1f);
                Main.dust[waterDust].position = (Main.dust[waterDust].position + Projectile.position) / 2f;
                Main.dust[waterDust].velocity = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                Main.dust[waterDust].velocity.Normalize();
                Dust dust = Main.dust[waterDust];
                dust.velocity *= (float)Main.rand.Next(1, 30) * 0.1f;
                Main.dust[waterDust].alpha = Projectile.alpha;
                inc = i;
            }
        }
    }
}
