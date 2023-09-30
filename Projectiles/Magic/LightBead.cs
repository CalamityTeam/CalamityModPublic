using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class LightBead : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.alpha = 50;
            Projectile.scale = 1.2f;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.5f, 0.5f, 0.5f);
            Projectile.rotation += Projectile.velocity.X * 0.2f;
            Projectile.ai[1] += 1f;
            if (Main.rand.NextBool(5))
            {
                Dust whiteMagic = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 244, 0f, 0f, 0, default, 1f);
                whiteMagic.noGravity = true;
                whiteMagic.velocity *= 0.5f;
                whiteMagic.scale *= 0.9f;
            }

            if (Projectile.ai[1] > 300f)
            {
                Projectile.scale -= 0.05f;
                if (Projectile.scale <= 0.2f)
                {
                    Projectile.scale = 0.2f;
                    Projectile.Kill();
                    return;
                }
            }
            CalamityUtils.HomeInOnNPC(Projectile, !Projectile.tileCollide, 200f, 15f, 15f);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 200, Projectile.alpha);
        }

        public override void OnKill(int timeLeft)
        {
            for (int k = 0; k < 4; k++)
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 244, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);

            int beadAmt = Main.rand.Next(2, 3);
            if (Projectile.owner == Main.myPlayer)
            {
                for (int b = 0; b < beadAmt; b++)
                {
                    Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<LightBeadSplit>(), (int)(Projectile.damage * 0.5), 0f, Projectile.owner, 0f, 0f);
                }
            }
        }
    }
}
