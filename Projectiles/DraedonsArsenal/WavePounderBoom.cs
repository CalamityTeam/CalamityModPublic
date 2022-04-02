using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Terraria;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class WavePounderBoom : BaseMassiveExplosionProjectile
    {
        public override int Lifetime => 60;
        public override bool UsesScreenshake => projectile.Calamity().stealthStrike;
        public override float GetScreenshakePower(float pulseCompletionRatio) => CalamityUtils.Convert01To010(pulseCompletionRatio) * 16f;
        public override Color GetCurrentExplosionColor(float pulseCompletionRatio) => Color.Lerp(Color.Yellow * 1.6f, Color.White, MathHelper.Clamp(pulseCompletionRatio * 2.2f, 0f, 1f));
        public override void SetStaticDefaults() => DisplayName.SetDefault("Explosion");

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 2;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.timeLeft = Lifetime;
            projectile.Calamity().rogue = true;
        }

        public override void PostAI() => Lighting.AddLight(projectile.Center, 0.2f, 0.1f, 0f);
    }
}
