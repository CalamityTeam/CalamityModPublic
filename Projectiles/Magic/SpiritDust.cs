using CalamityMod.Particles;
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
            projectile.width = projectile.height = 10;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 60;
        }

        public override void AI()
        {
            float radius = MathHelper.SmoothStep(80f, 48f, 1f - projectile.timeLeft / 90f);
            Vector2 spawnPosition = projectile.Center + Main.rand.NextVector2Circular(5f, 5f) * radius / 130f;
            FusableParticleManager.GetParticleSetByType<GruesomeEminenceParticleSet>()?.SpawnParticle(spawnPosition, radius);
        }
    }
}
