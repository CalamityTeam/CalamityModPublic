using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace CalamityMod.Projectiles.Boss
{
    public class DoGDeathBoom : BaseMassiveExplosionProjectile
    {
        public override int Lifetime => 180;
        public override bool UsesScreenshake => true;
        public override float GetScreenshakePower(float pulseCompletionRatio) => CalamityUtils.Convert01To010(pulseCompletionRatio) * 28f;
        public override Color GetCurrentExplosionColor(float pulseCompletionRatio)
        {
            return Color.Lerp(Color.Cyan, Color.Fuchsia, MathHelper.Clamp(pulseCompletionRatio * 1.75f, 0f, 1f));
        }
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults() 
        {
            DisplayName.SetDefault("Cosmic Explosion");
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 10000;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 2;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = Lifetime;
            CooldownSlot = 1;
        }

        public override void PostAI()
        {
            MaxRadius = 4200f;
            Lighting.AddLight(Projectile.Center, 0.1f, 0.1f, 0.1f);
        }
    }
}
