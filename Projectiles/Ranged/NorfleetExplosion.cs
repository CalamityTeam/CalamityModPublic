using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class NorfleetExplosion : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Explosion");
        }

        public override void SetDefaults()
        {
            projectile.width = 500;
            projectile.height = 500;
            projectile.friendly = true;
            projectile.ignoreWater = false;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 30;
            projectile.ranged = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 0.75f / 255f, (255 - projectile.alpha) * 0.5f / 255f, (255 - projectile.alpha) * 0.01f / 255f);
            Vector2 dustVel = CalamityUtils.RandomVelocity(120f, 36f, 108f, 1f);
            int idx = Dust.NewDust(projectile.position, projectile.width, projectile.height, Main.rand.NextBool(2) ? 221 : 244, 0f, 0f, 100, default, 2f);
            Dust dust = Main.dust[idx];
            dust.noGravity = true;
            dust.position.X = projectile.Center.X;
            dust.position.Y = projectile.Center.Y;
            dust.position.X += Main.rand.NextFloat(-10f, 10f);
            dust.position.Y += Main.rand.NextFloat(-10f, 10f);
            dust.velocity = dustVel;
        }
    }
}
