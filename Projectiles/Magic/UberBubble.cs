using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Magic
{
    public class UberBubble : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
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

            Vector2 uber = Projectile.ai[0].ToRotationVector2();
            float projRotation = Projectile.velocity.ToRotation();
            float aiRotation = uber.ToRotation();
            double rotationClamp = aiRotation - projRotation;
            if (rotationClamp > MathHelper.Pi)
                rotationClamp -= MathHelper.TwoPi;
            if (rotationClamp < -MathHelper.Pi)
                rotationClamp -= -MathHelper.TwoPi;

            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item96, Projectile.position);
            int randDustAmt = Main.rand.Next(4, 6);
            for (int i = 0; i < randDustAmt; i++)
            {
                int purpleDust = Dust.NewDust(Projectile.Center, 0, 0, 171, 0f, 0f, 100, default, 1.4f);
                Main.dust[purpleDust].velocity *= 0.8f;
                Main.dust[purpleDust].position = Vector2.Lerp(Main.dust[purpleDust].position, Projectile.Center, 0.5f);
                Main.dust[purpleDust].noGravity = true;
            }
            if (Projectile.owner == Main.myPlayer)
            {
                for (int numBubbles = 0; numBubbles < 3; numBubbles++)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, Projectile.velocity.X * (Main.rand.NextFloat() * 2f), Projectile.velocity.Y * (Main.rand.NextFloat() * 2f), ModContent.ProjectileType<BlueBubble>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
        }
    }
}
