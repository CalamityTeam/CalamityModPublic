using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class NanoPurgeLaser : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        private const float LaserLength = 40f;
        private const float LaserLengthChangeRate = 1.5f;

        public override string Texture => "CalamityMod/Projectiles/LaserProj";

        public override void SetDefaults()
        {
            Projectile.width = 5;
            Projectile.height = 5;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 2;
            Projectile.timeLeft = 600;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            // Very rapidly fade into existence.
            if (Projectile.alpha > 0)
                Projectile.alpha -= 25;
            if (Projectile.alpha < 0)
                Projectile.alpha = 0;

            // Emit light.
            Lighting.AddLight(Projectile.Center, 0f, 0.7f, 0.1f);

            // Laser length shenanigans. If the laser is still growing, increase localAI 0 to indicate it is getting longer.
            if (Projectile.ai[1] == 0f)
            {
                Projectile.localAI[0] += LaserLengthChangeRate;

                // Cap it at max length.
                if (Projectile.localAI[0] > LaserLength)
                    Projectile.localAI[0] = LaserLength;
            }

            // Otherwise it's shrinking. Once it reaches zero length it dies for good.
            else
            {
                Projectile.localAI[0] -= LaserLengthChangeRate;
                if (Projectile.localAI[0] <= 0f)
                    Projectile.Kill();
            }
        }

        public override Color? GetAlpha(Color lightColor) => new Color(96, 255, 96, 0);

        public override bool PreDraw(ref Color lightColor) => Projectile.DrawBeam(LaserLength, 2f, lightColor);

        public override void OnKill(int timeLeft)
        {
            int dustID = 107;
            int dustAmt = Main.rand.Next(3, 7);
            Vector2 dustPos = Projectile.Center - Projectile.velocity / 2f;
            for (int i = 0; i < dustAmt; ++i)
            {
                Dust d = Dust.NewDustDirect(dustPos, 0, 0, dustID, 0f, 0f, Scale: 0.8f);
                d.velocity *= 1.15f;
                d.noGravity = true;
            }
        }
    }
}
