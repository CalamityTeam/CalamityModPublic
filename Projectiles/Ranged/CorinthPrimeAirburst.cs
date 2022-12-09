using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class CorinthPrimeAirburst : BaseMassiveExplosionProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public override int Lifetime => 60;
        public override bool UsesScreenshake => Projectile.damage > 1;
        public override float GetScreenshakePower(float pulseCompletionRatio) => CalamityUtils.Convert01To010(pulseCompletionRatio) * 16f;
        public override Color GetCurrentExplosionColor(float pulseCompletionRatio) => Color.Lerp(Color.DarkRed * 1.6f, Color.OrangeRed, MathHelper.Clamp(pulseCompletionRatio * 2.2f, 0f, 1f));
        public override void SetStaticDefaults() => DisplayName.SetDefault("Airburst");

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 2;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.timeLeft = Lifetime;
            Projectile.DamageType = DamageClass.Ranged;
        }

        public override void PostAI() => Lighting.AddLight(Projectile.Center, 0.3f, 0f, 0f);
    }
}
