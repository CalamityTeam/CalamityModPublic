using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class GhastlyExplosionShard : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ghastly Fragment");
        }

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.timeLeft = 80;
            Projectile.penetrate = 1;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override bool? CanHitNPC(NPC target) => Projectile.timeLeft < 50 && target.CanBeChasedBy(Projectile);

        public override void AI()
        {
            int num332 = (int)Projectile.ai[0];

            Projectile.ai[1] += 1f;
            float num333 = (120f - Projectile.ai[1]) / 120f;
            if (Projectile.ai[1] > 80f)
                Projectile.Kill();

            Projectile.velocity.Y += 0.2f;
            if (Projectile.velocity.Y > 18f)
                Projectile.velocity.Y = 18f;

            Projectile.velocity.X *= 0.98f;

            int num3;
            for (int num334 = 0; num334 < 2; num334 = num3 + 1)
            {
                int num335 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, num332, Projectile.velocity.X, Projectile.velocity.Y, 50, default, 1.1f);
                Main.dust[num335].position = (Main.dust[num335].position + Projectile.Center) / 2f;
                Main.dust[num335].noGravity = true;
                Dust dust = Main.dust[num335];
                dust.velocity *= 0.3f;
                dust = Main.dust[num335];
                dust.scale *= num333;
                num3 = num334;
            }

            for (int num336 = 0; num336 < 1; num336 = num3 + 1)
            {
                int num335 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, num332, Projectile.velocity.X, Projectile.velocity.Y, 50, default, 0.6f);
                Main.dust[num335].position = (Main.dust[num335].position + Projectile.Center * 5f) / 6f;
                Dust dust = Main.dust[num335];
                dust.velocity *= 0.1f;
                Main.dust[num335].noGravity = true;
                Main.dust[num335].fadeIn = 0.9f * num333;
                dust = Main.dust[num335];
                dust.scale *= num333;
                num3 = num336;
            }

            if (Projectile.timeLeft < 50)
                CalamityGlobalProjectile.HomeInOnNPC(Projectile, !Projectile.tileCollide, 600f, 12f, 20f);
        }

        public override void Kill(int timeLeft)
        {
            int num3;
            for (int num114 = 0; num114 < 10; num114 = num3 + 1)
            {
                int num115 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, (int)Projectile.ai[0], Projectile.velocity.X * 0.1f, Projectile.velocity.Y * 0.1f, 0, default, 0.5f);
                Dust dust;
                Main.dust[num115].scale = 1f + Main.rand.Next(-10, 11) * 0.01f;
                Main.dust[num115].noGravity = true;
                dust = Main.dust[num115];
                dust.velocity *= 1.25f;
                dust = Main.dust[num115];
                dust.velocity -= Projectile.oldVelocity / 10f;
                num3 = num114;
            }
        }
    }
}
