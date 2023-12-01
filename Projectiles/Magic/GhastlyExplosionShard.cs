using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class GhastlyExplosionShard : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.timeLeft = 120;
            Projectile.penetrate = 1;
            Projectile.MaxUpdates = 3;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override bool? CanHitNPC(NPC target) => Projectile.timeLeft < 90 && target.CanBeChasedBy(Projectile);

        public override void AI()
        {
            int dustType = (int)Projectile.ai[0];

            Projectile.ai[1] += 1f;
            float dustScale = (120f - Projectile.ai[1]) / 120f;
            if (Projectile.ai[1] > 120f)
                Projectile.Kill();

            Projectile.velocity.Y += 0.2f;
            if (Projectile.velocity.Y > 18f)
                Projectile.velocity.Y = 18f;

            Projectile.velocity.X *= 0.98f;

            int inc;
            for (int i = 0; i < 2; i = inc + 1)
            {
                int explodeDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dustType, Projectile.velocity.X, Projectile.velocity.Y, 50, default, 1.1f);
                Main.dust[explodeDust].position = (Main.dust[explodeDust].position + Projectile.Center) / 2f;
                Main.dust[explodeDust].noGravity = true;
                Dust dust = Main.dust[explodeDust];
                dust.velocity *= 0.3f;
                dust = Main.dust[explodeDust];
                dust.scale *= dustScale;
                inc = i;
            }

            for (int j = 0; j < 1; j = inc + 1)
            {
                int explodeDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dustType, Projectile.velocity.X, Projectile.velocity.Y, 50, default, 0.6f);
                Main.dust[explodeDust].position = (Main.dust[explodeDust].position + Projectile.Center * 5f) / 6f;
                Dust dust = Main.dust[explodeDust];
                dust.velocity *= 0.1f;
                Main.dust[explodeDust].noGravity = true;
                Main.dust[explodeDust].fadeIn = 0.9f * dustScale;
                dust = Main.dust[explodeDust];
                dust.scale *= dustScale;
                inc = j;
            }

            if (Projectile.timeLeft < 90)
                CalamityUtils.HomeInOnNPC(Projectile, !Projectile.tileCollide, 600f, 12f, 20f);
        }

        public override void OnKill(int timeLeft)
        {
            int inc;
            for (int i = 0; i < 10; i = inc + 1)
            {
                int killDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, (int)Projectile.ai[0], Projectile.velocity.X * 0.1f, Projectile.velocity.Y * 0.1f, 0, default, 0.5f);
                Dust dust;
                Main.dust[killDust].scale = 1f + Main.rand.Next(-10, 11) * 0.01f;
                Main.dust[killDust].noGravity = true;
                dust = Main.dust[killDust];
                dust.velocity *= 1.25f;
                dust = Main.dust[killDust];
                dust.velocity -= Projectile.oldVelocity / 10f;
                inc = i;
            }
        }
    }
}
