using CalamityMod.Particles.Metaballs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class SpiritDust : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults() => DisplayName.SetDefault("Spirit Dust");

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 60;
        }

        public override void AI()
        {
            float radius = MathHelper.SmoothStep(80f, 48f, 1f - Projectile.timeLeft / 90f);
            Vector2 spawnPosition = Projectile.Center + Main.rand.NextVector2Circular(5f, 5f) * radius / 130f;
            FusableParticleManager.GetParticleSetByType<GruesomeEminenceParticleSet>()?.SpawnParticle(spawnPosition, radius);
        }
    }
}
