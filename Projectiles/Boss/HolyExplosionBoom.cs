using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace CalamityMod.Projectiles.Boss
{
    public class HolyExplosionBoom : BaseMassiveExplosionProjectile
    {
        public override int Lifetime => 60;
        public override bool UsesScreenshake => true;
        public override float GetScreenshakePower(float pulseCompletionRatio) => CalamityUtils.Convert01To010(pulseCompletionRatio) * 16f;
        public override Color GetCurrentExplosionColor(float pulseCompletionRatio)
        {
            Color explosionColor = Main.dayTime ? Color.Orange : Color.BlueViolet;
            return Color.Lerp(explosionColor * 1.6f, Color.White, MathHelper.Clamp(pulseCompletionRatio * 2.2f, 0f, 1f));
        }
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Explosion");
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
            MaxRadius = 3000f;
            Lighting.AddLight(Projectile.Center, 0.1f, 0.1f, 0.1f);
        }
    }
}
