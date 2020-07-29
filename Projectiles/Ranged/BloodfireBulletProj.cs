using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
	public class BloodfireBulletProj : ModProjectile
	{
		private const int Lifetime = 600;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Bloodfire Bullet");
			/*
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 0;
			ProjectileID.Sets.TrailingMode[projectile.type] = 0;
			*/
		}

		public override void SetDefaults()
		{
			projectile.width = 4;
			projectile.height = 4;
			projectile.friendly = true;
			projectile.ranged = true;
			projectile.aiStyle = 1;
			aiType = ProjectileID.BulletHighVelocity;
			projectile.alpha = 255;
			projectile.extraUpdates = 4;
			projectile.timeLeft = Lifetime;
		}

		// These bullets glow in the dark.
		public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, 140);

		public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection) => damage += OnHitEffect(Main.player[projectile.owner]);
		public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit) => damage += OnHitEffect(Main.player[projectile.owner]);

		// Returns the amount of bonus damage that should be dealt. Boosts life regeneration appropriately as a side effect.
		private int OnHitEffect(Player owner)
		{
			// Adds 3 frames to lifeRegenTime on every hit. This increased value is used for the damage calculation.
			owner.lifeRegenTime += 3;

			// Deals (1.00 + (0.1 * current lifeRegen))% of current lifeRegenTime as flat bonus damage on hit.
			// For example, at 0 life regen, you get 1% of lifeRegenTime as bonus damage.
			// At 10 life regen, you get 2%. At 20 life regen, you get 3%.
			// Negative life regen does not decrease damage.
			int regenForCalc = owner.lifeRegen > 0 ? owner.lifeRegen : 0;
			float regenDamageRatio = 0.01f + 0.001f * regenForCalc;

			// For the sake of bonus damage, life regen time caps at 3600, aka 60 seconds. This is its natural cap in vanilla.
			int regenTimeForCalc = (int)MathHelper.Clamp(owner.lifeRegenTime, 0f, 3600f);
			return (int)(regenDamageRatio * regenTimeForCalc);
		}

		/*
		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);
			Main.PlaySound(0, (int)projectile.Center.X, (int)projectile.Center.Y, 1, 1f, 0f);
			return true;
		}
		*/
	}
}
