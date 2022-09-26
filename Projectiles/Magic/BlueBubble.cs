using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Magic
{
    public class BlueBubble : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bubble");
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 1;
            Projectile.alpha = 255;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 120;
        }

        public override bool? CanHitNPC(NPC target) => Projectile.timeLeft < 90 && target.CanBeChasedBy(Projectile);

        public override void AI()
        {
            Projectile.velocity *= 0.99f;

            Projectile.scale += 0.005f;

            if (Projectile.alpha > 0)
                Projectile.alpha -= 30;
            if (Projectile.alpha < 0)
                Projectile.alpha = 0;

            Vector2 v2 = Projectile.ai[0].ToRotationVector2();
            float num743 = Projectile.velocity.ToRotation();
            float num744 = v2.ToRotation();
            double num745 = num744 - num743;
            if (num745 > MathHelper.Pi)
                num745 -= MathHelper.TwoPi;
            if (num745 < -MathHelper.Pi)
                num745 += MathHelper.TwoPi;

            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;

            if (Projectile.timeLeft < 90)
                CalamityUtils.HomeInOnNPC(Projectile, !Projectile.tileCollide, 400f, 8f, 20f);
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item54, Projectile.Center);
            int num190 = Main.rand.Next(5, 9);
            for (int num191 = 0; num191 < num190; num191++)
            {
                int num192 = Dust.NewDust(Projectile.Center, 0, 0, 206, 0f, 0f, 100, default, 1.4f);
                Main.dust[num192].velocity *= 0.8f;
                Main.dust[num192].position = Vector2.Lerp(Main.dust[num192].position, Projectile.Center, 0.5f);
                Main.dust[num192].noGravity = true;
            }
        }
    }
}
