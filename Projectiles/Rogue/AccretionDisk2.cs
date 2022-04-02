using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
	public class AccretionDisk2 : ModProjectile
	{
		public override string Texture => "CalamityMod/Items/Weapons/Rogue/AccretionDisk";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Accretion Disk");
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
			ProjectileID.Sets.TrailingMode[projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			projectile.width = 56;
			projectile.height = 56;
			projectile.alpha = 120;
			projectile.ignoreWater = true;
			projectile.friendly = true;
			projectile.tileCollide = false;
			projectile.penetrate = -1;
			projectile.aiStyle = 3;
			projectile.timeLeft = 60;
			aiType = ProjectileID.WoodenBoomerang;
			projectile.Calamity().rogue = true;
		}

		public override void AI()
		{
			if (Main.rand.NextBool(10))
			{
				int num250 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 66, projectile.direction * 2, 0f, 150, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 0.5f);
				Main.dust[num250].noGravity = true;
				Main.dust[num250].velocity *= 0f;
			}
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 90);
			target.AddBuff(BuffID.Frostburn, 90);
			target.AddBuff(ModContent.BuffType<HolyFlames>(), 90);
		}

		public override void OnHitPvp(Player target, int damage, bool crit)
		{
			target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 90);
			target.AddBuff(BuffID.Frostburn, 90);
			target.AddBuff(ModContent.BuffType<HolyFlames>(), 90);
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 2);
			return false;
		}
	}
}
