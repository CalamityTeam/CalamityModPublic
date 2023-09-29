using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class NorfleetExplosion : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 500;
            Projectile.height = 500;
            Projectile.friendly = true;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 30;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0.75f / 255f, (255 - Projectile.alpha) * 0.5f / 255f, (255 - Projectile.alpha) * 0.01f / 255f);
            Vector2 dustVel = CalamityUtils.RandomVelocity(120f, 36f, 108f, 1f);
            int idx = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, Main.rand.NextBool() ? 221 : 244, 0f, 0f, 100, default, 2f);
            Dust dust = Main.dust[idx];
            dust.noGravity = true;
            dust.position.X = Projectile.Center.X;
            dust.position.Y = Projectile.Center.Y;
            dust.position.X += Main.rand.NextFloat(-10f, 10f);
            dust.position.Y += Main.rand.NextFloat(-10f, 10f);
            dust.velocity = dustVel;
        }
    }
}
