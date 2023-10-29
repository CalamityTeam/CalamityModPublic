using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Magic
{
    public class BlueBubble : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
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
            float projRotate = Projectile.velocity.ToRotation();
            float aiRotation = v2.ToRotation();
            double projAngle = aiRotation - projRotate;
            if (projAngle > MathHelper.Pi)
                projAngle -= MathHelper.TwoPi;
            if (projAngle < -MathHelper.Pi)
                projAngle += MathHelper.TwoPi;

            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;

            if (Projectile.timeLeft < 90)
                CalamityUtils.HomeInOnNPC(Projectile, !Projectile.tileCollide, 400f, 8f, 20f);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item54, Projectile.Center);
            int num190 = Main.rand.Next(5, 9);
            for (int i = 0; i < num190; i++)
            {
                int bubbly = Dust.NewDust(Projectile.Center, 0, 0, 206, 0f, 0f, 100, default, 1.4f);
                Main.dust[bubbly].velocity *= 0.8f;
                Main.dust[bubbly].position = Vector2.Lerp(Main.dust[bubbly].position, Projectile.Center, 0.5f);
                Main.dust[bubbly].noGravity = true;
            }
        }
    }
}
