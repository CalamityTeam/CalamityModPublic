using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class AnomalysNanogunMPFBBoom : BaseMassiveExplosionProjectile
    {
        public static readonly SoundStyle MPFBExplosion = new("CalamityMod/Sounds/Item/AnomalysNanogunMPFBExplosion");
        public override int Lifetime => 40;
        public override bool UsesScreenshake => true;
        public override float GetScreenshakePower(float pulseCompletionRatio) => CalamityUtils.Convert01To010(pulseCompletionRatio) * 3f;
        public override Color GetCurrentExplosionColor(float pulseCompletionRatio) => Color.Lerp(Color.Blue, Color.CornflowerBlue, MathHelper.Clamp(pulseCompletionRatio * 2.2f, 0f, 1f));
        public override void SetStaticDefaults() => DisplayName.SetDefault("MPFB Explosion");

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

        public override void PostAI() => Lighting.AddLight(Projectile.Center, 0, Projectile.Opacity * 0.7f / 255f, Projectile.Opacity);
    }
}
