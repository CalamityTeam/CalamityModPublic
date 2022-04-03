using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Magic
{
    public class UberBubble : ModProjectile
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
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 1;
            Projectile.alpha = 255;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 30;
        }

        public override void AI()
        {
            Projectile.velocity *= 0.975f;

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
                num745 -= -MathHelper.TwoPi;

            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item96, Projectile.position);
            int num190 = Main.rand.Next(4, 6);
            for (int num191 = 0; num191 < num190; num191++)
            {
                int num192 = Dust.NewDust(Projectile.Center, 0, 0, 171, 0f, 0f, 100, default, 1.4f);
                Main.dust[num192].velocity *= 0.8f;
                Main.dust[num192].position = Vector2.Lerp(Main.dust[num192].position, Projectile.Center, 0.5f);
                Main.dust[num192].noGravity = true;
            }
            if (Projectile.owner == Main.myPlayer)
            {
                for (int numBubbles = 0; numBubbles < 3; numBubbles++)
                    Projectile.NewProjectile(Projectile.Center.X, Projectile.Center.Y, Projectile.velocity.X * (Main.rand.NextFloat() * 2f), Projectile.velocity.Y * (Main.rand.NextFloat() * 2f), ModContent.ProjectileType<BlueBubble>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
        }
    }
}
