using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
	public class GastricBelcherBubble : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Gastric Bubble");
			ProjectileID.Sets.MinionShot[projectile.type] = true;
		}

		public override void SetDefaults()
		{
			projectile.friendly = true;
			projectile.width = projectile.height = 18;
			projectile.minion = true;
			projectile.timeLeft = 180;
			projectile.aiStyle = 72;
		}

		public override void Kill(int timeLeft)
		{
			Main.PlaySound(SoundID.Item54, projectile.position);
			Vector2 center = projectile.Center;
			for (int dustIndex = 0; dustIndex < 10; ++dustIndex)
			{
				int scalar = (int)(10 * projectile.ai[1]);
				int bubble = Dust.NewDust(projectile.Center - Vector2.One * scalar, scalar * 2, scalar * 2, 212, 0.0f, 0.0f, 0, new Color(), 1f);
				Dust dust = Main.dust[bubble];
				Vector2 dustVec = Vector2.Normalize(dust.position - projectile.Center);
				dust.position = projectile.Center + dustVec * scalar * projectile.scale;
				dust.velocity = dustIndex >= 30 ? dustVec * Main.rand.Next(45, 91) / 10f : dustVec * dust.velocity.Length();
				dust.color = Main.hslToRgb(0.4f + Main.rand.NextFloat() * 0.2f, 0.9f, 0.5f);
				dust.color = Color.Lerp(dust.color, Color.White, 0.3f);
				dust.noGravity = true;
				dust.scale = 0.7f;
			}
		}
	}
}
