using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
	public class ReaverSpore : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Spore");
		}

		public override void SetDefaults()
		{
			projectile.width = projectile.height = 16;
			projectile.friendly = true;
			projectile.penetrate = 1;
			projectile.alpha = 255;
			projectile.timeLeft = 3600;
			projectile.tileCollide = false;
			projectile.ignoreWater = true;
		}

		public override void AI()
		{
			projectile.SporeSacAI();
		}

		public override void Kill(int timeLeft)
		{
			if (projectile.owner == Main.myPlayer)
			{
				Vector2 velocity = CalamityUtils.RandomVelocity(100f, 1f, 1f, 0.3f);
				int gas = Projectile.NewProjectile(projectile.Center, velocity, ProjectileID.SporeGas + Main.rand.Next(3), projectile.damage, 0f, projectile.owner);
				Main.projectile[gas].usesLocalNPCImmunity = true;
				Main.projectile[gas].localNPCHitCooldown = 30;
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Texture2D texture;
			switch (projectile.ai[0])
			{
				case 1f: texture = ModContent.GetTexture("CalamityMod/Projectiles/Typeless/ReaverSpore2");
					break;
				default: texture = Main.projectileTexture[projectile.type];
					break;
			}
			Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Rectangle?(new Rectangle(0, 0, texture.Width, texture.Height)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2(texture.Width / 2f, texture.Height / 2f), projectile.scale, SpriteEffects.None, 0f);
			return false;
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			target.AddBuff(BuffID.Poisoned, 60 * Main.rand.Next(5, 11));
		}

		public override void OnHitPvp(Player target, int damage, bool crit)
		{
			target.AddBuff(BuffID.Poisoned, 60 * Main.rand.Next(5, 11));
		}
	}
}
