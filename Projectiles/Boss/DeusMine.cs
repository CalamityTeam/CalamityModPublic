using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class DeusMine : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Astral Mine");
		}

		public override void SetDefaults()
		{
			projectile.width = 26;
			projectile.height = 26;
			projectile.hostile = true;
			projectile.alpha = 100;
			projectile.penetrate = -1;
			projectile.tileCollide = false;
			projectile.timeLeft = 1200;
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(projectile.localAI[0]);
			writer.Write(projectile.localAI[1]);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			projectile.localAI[0] = reader.ReadSingle();
			projectile.localAI[1] = reader.ReadSingle();
		}

		public override void AI()
		{
			projectile.velocity.X *= 0.985f;
			projectile.velocity.Y *= 0.985f;
			if (projectile.ai[1] == 0f)
			{
				projectile.ai[1] = 1f;
				Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 33);
			}
		}

		public override bool CanDamage()
		{
			if (projectile.timeLeft > 1100)
			{
				return false;
			}
			return true;
		}

		public override Color? GetAlpha(Color lightColor)
		{
			if (projectile.timeLeft > 1115)
			{
				projectile.localAI[1] += 1f;
				byte b2 = (byte)(((int)projectile.localAI[1]) * 3);
				byte a2 = (byte)((float)projectile.alpha * ((float)b2 / 255f));
				return new Color((int)b2, (int)b2, (int)b2, (int)a2);
			}
			if (projectile.timeLeft < 85)
			{
				byte b2 = (byte)(projectile.timeLeft * 3);
				byte a2 = (byte)((float)projectile.alpha * ((float)b2 / 255f));
				return new Color((int)b2, (int)b2, (int)b2, (int)a2);
			}
			return new Color(255, 255, 255, projectile.alpha);
		}

		public override void Kill(int timeLeft)
		{
			Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 14);
			projectile.position.X = projectile.position.X + (float)(projectile.width / 2);
			projectile.position.Y = projectile.position.Y + (float)(projectile.height / 2);
			projectile.width = (projectile.height = 96);
			projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
			projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
			for (int num621 = 0; num621 < 30; num621++)
			{
				int num622 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 173, 0f, 0f, 100, default(Color), 1.2f);
				Main.dust[num622].velocity *= 3f;
				if (Main.rand.Next(2) == 0)
				{
					Main.dust[num622].scale = 0.5f;
					Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
				}
			}
			for (int num623 = 0; num623 < 60; num623++)
			{
				int num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, mod.DustType("AstralOrange"), 0f, 0f, 100, default(Color), 1.7f);
				Main.dust[num624].noGravity = true;
				Main.dust[num624].velocity *= 1.5f;
				Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, mod.DustType("AstralOrange"), 0f, 0f, 100, default(Color), 1f);
			}
			projectile.Damage();
		}

		public override void OnHitPlayer(Player target, int damage, bool crit)
		{
			target.AddBuff(mod.BuffType("AstralInfectionDebuff"), 300);
		}
	}
}
