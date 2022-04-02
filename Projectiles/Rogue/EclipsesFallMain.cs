using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
	public class EclipsesFallMain : ModProjectile
	{
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/EclipsesFall";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Eclipse's Fall");
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
			ProjectileID.Sets.TrailingMode[projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			projectile.width = 26;
			projectile.height = 26;
			projectile.friendly = true;
			projectile.penetrate = -1;
			projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 10;
			projectile.extraUpdates = 1;
			projectile.ignoreWater = true;
			projectile.tileCollide = false;
			projectile.timeLeft = 300;
			projectile.Calamity().rogue = true;
		}

		public override void AI()
		{
			if (Main.rand.NextBool(8))
			{
				Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 138, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
			}
			projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver4;
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			SpawnSpears(target.Center);
		}

		public override void OnHitPvp(Player target, int damage, bool crit)
		{
			SpawnSpears(target.Center);
		}

		private void SpawnSpears(Vector2 targetPos)
		{
			int spearAmt = Main.rand.Next(3, 6); //3 to 5 spears
			for (int n = 0; n < spearAmt; n++)
			{
				float dmgMult = 0.08f * Main.rand.NextFloat(4f, 7f);
				float kBMult = 0.1f * Main.rand.NextFloat(7f, 10f);
				CalamityUtils.ProjectileRain(targetPos, 400f, 100f, 500f, 800f, 29f, ModContent.ProjectileType<EclipsesSmol>(), (int)(projectile.damage * dmgMult), projectile.knockBack * kBMult, projectile.owner);
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
			return false;
		}
	}
}
