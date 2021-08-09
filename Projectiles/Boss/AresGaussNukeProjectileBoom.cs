using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
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
		public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

		public override void SafeSetStaticDefaults() => DisplayName.SetDefault("Gauss Explosion");

		public override void SetDefaults()
		{
			projectile.Calamity().canBreakPlayerDefense = true;
			projectile.width = projectile.height = 2;
			projectile.hostile = true;
			projectile.ignoreWater = true;
			projectile.tileCollide = false;
			projectile.penetrate = -1;
			projectile.timeLeft = Lifetime;
			cooldownSlot = 1;
		}

		public override void PostAI() => Lighting.AddLight(projectile.Center, 0.2f, 0.1f, 0f);

		public override bool CanHitPlayer(Player target) => CalamityUtils.CircularHitboxCollision(projectile.Center, CurrentRadius * projectile.scale * 0.5f, target.Hitbox);

		public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit) => target.Calamity().lastProjectileHit = projectile;

		public override void OnHitPlayer(Player target, int damage, bool crit) => target.AddBuff(BuffID.OnFire, 480);
	}
}
