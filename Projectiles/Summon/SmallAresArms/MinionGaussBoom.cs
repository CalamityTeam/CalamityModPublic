using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon.SmallAresArms
{
    public class MinionGaussBoom : BaseMassiveExplosionProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override int Lifetime => 60;

        // No screenshake is used for the sake of gameplay convenience.
        public override bool UsesScreenshake => false;
        public override float GetScreenshakePower(float pulseCompletionRatio) => 0f;
        public override Color GetCurrentExplosionColor(float pulseCompletionRatio) => Color.Lerp(Color.Yellow * 1.6f, Color.White, MathHelper.Clamp(pulseCompletionRatio * 2.2f, 0f, 1f));

        public override void SetStaticDefaults() => ProjectileID.Sets.MinionShot[Projectile.type] = true;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 2;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = Lifetime;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 12;
        }

        public override void PostAI() => Lighting.AddLight(Projectile.Center, 0.2f, 0.1f, 0f);
    }
}
