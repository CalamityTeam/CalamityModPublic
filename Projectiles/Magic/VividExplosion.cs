using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
	public class VividExplosion : ModProjectile
	{
		private const float radius = 204.5f;
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Explosion");
		}

		public override void SetDefaults()
		{
			projectile.width = projectile.height = 1; //Uses custom collision, this field is irrelevant
			projectile.friendly = true;
			projectile.ignoreWater = true;
			projectile.tileCollide = false;
			projectile.Calamity().rogue = true;
			projectile.penetrate = -1;
			projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 6;
			projectile.timeLeft = 5;
		}

		public override void AI()
		{

			Lighting.AddLight(projectile.Center, Main.DiscoR * 0.5f / 255f, Main.DiscoG * 0.5f / 255f, Main.DiscoB * 0.5f / 255f);

			float dustSpeed = Main.rand.NextFloat(12f, 35f);
			Vector2 dustVel = CalamityUtils.RandomVelocity(40f, dustSpeed, dustSpeed, 1f);
			int dustType = Utils.SelectRandom(Main.rand, new int[]
			{
				107,
				234,
				269
			});
			int rainbow = Dust.NewDust(projectile.position, projectile.width, projectile.height, dustType, 0f, 0f, 100, default, 2f);
			Dust dust = Main.dust[rainbow];
			dust.noGravity = true;
			dust.position = projectile.Center;
			dust.position.X += (float)Main.rand.Next(-10, 11);
			dust.position.Y += (float)Main.rand.Next(-10, 11);
			dust.velocity = dustVel;
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			float dist1 = Vector2.Distance(projectile.Center, targetHitbox.TopLeft());
			float dist2 = Vector2.Distance(projectile.Center, targetHitbox.TopRight());
			float dist3 = Vector2.Distance(projectile.Center, targetHitbox.BottomLeft());
			float dist4 = Vector2.Distance(projectile.Center, targetHitbox.BottomRight());

			float minDist = dist1;
			if (dist2 < minDist)
				minDist = dist2;
			if (dist3 < minDist)
				minDist = dist3;
			if (dist4 < minDist)
				minDist = dist4;

			return minDist <= radius;
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			target.ExoDebuffs();
		}

		public override void OnHitPvp(Player target, int damage, bool crit)
		{
			target.ExoDebuffs();
		}
	}
}
