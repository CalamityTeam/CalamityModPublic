using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;

namespace CalamityMod.Projectiles.Boss
{
    public class AresGaussNukeProjectileBoom : BaseMassiveExplosionProjectile
    {
        public override int Lifetime => 60;
        public override bool UsesScreenshake => true;
        public override float GetScreenshakePower(float pulseCompletionRatio) => CalamityUtils.Convert01To010(pulseCompletionRatio) * 16f;
        public override Color GetCurrentExplosionColor(float pulseCompletionRatio) => Color.Lerp(Color.Yellow * 1.6f, Color.White, MathHelper.Clamp(pulseCompletionRatio * 2.2f, 0f, 1f));
        public override float Fadeout(float completion)
        {
            float opacity;
            //Opacity is high for most of the blast
            if (completion < 0.8f)
                opacity = 1f;

            //It only fades out near the end
            else
                opacity = (float)Math.Cos(((completion - 0.8f) / 0.2f * MathHelper.Pi) / 2f); ;

            return opacity * 0.85f;
        }
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gauss Explosion");
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 10000;
        }

        public override void SetDefaults()
        {
            Projectile.Calamity().DealsDefenseDamage = true;
            Projectile.width = Projectile.height = 2;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = Lifetime;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void PostAI() => Lighting.AddLight(Projectile.Center, 0.2f, 0.1f, 0f);

        public override bool CanHitPlayer(Player target) => CalamityUtils.CircularHitboxCollision(Projectile.Center, CurrentRadius * Projectile.scale * 0.4f, target.Hitbox) && Projectile.timeLeft > 6;

        public override void OnHitPlayer(Player target, int damage, bool crit) => target.AddBuff(BuffID.OnFire, 480);
    }
}
