using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class DarkEnergyBall : ModProjectile
	{
		private double timeElapsed = 0.0;
		private double circleSize = 1.0;
		private double circleGrowth = 0.02;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Dark Energy");
			Main.projFrames[projectile.type] = 6;
		}

		public override void SetDefaults()
		{
			projectile.width = 80;
			projectile.height = 80;
			projectile.hostile = true;
			projectile.tileCollide = false;
			projectile.timeLeft = 600;
			cooldownSlot = 1;
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(timeElapsed);
			writer.Write(circleSize);
			writer.Write(circleGrowth);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			timeElapsed = reader.ReadDouble();
			circleSize = reader.ReadDouble();
			circleGrowth = reader.ReadDouble();
		}

		public override void AI()
		{
			timeElapsed += 0.02;
			projectile.velocity.X = (float)(Math.Sin(timeElapsed * (double)(0.5f * projectile.ai[0])) * circleSize);
			projectile.velocity.Y = (float)(Math.Cos(timeElapsed * (double)(0.5f * projectile.ai[0])) * circleSize);
			circleSize += circleGrowth;
			projectile.frameCounter++;
			if (projectile.frameCounter > 4)
			{
				projectile.frame++;
				projectile.frameCounter = 0;
			}
			if (projectile.frame > 5)
			{
				projectile.frame = 0;
			}
		}

		public override void Kill(int timeLeft)
		{
			for (int k = 0; k < 5; k++)
			{
				Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 90, 0f, 0f);
			}
		}
	}
}
