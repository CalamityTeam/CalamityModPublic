using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.SunkenSea
{
	public class WhirlpoolProjectile : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Riptide");
		}

		public override void SetDefaults()
		{
			projectile.width = 16;
			projectile.height = 16;
			projectile.scale = 0.75f;
			projectile.friendly = true;
			projectile.alpha = 150;
			projectile.penetrate = -1;
			projectile.timeLeft = 99;
			projectile.melee = true;
			aiType = ProjectileID.Amarok;
			projectile.CloneDefaults(ProjectileID.CorruptYoyo);
		}

		public override void AI()
		{
			if (Main.rand.Next(1, 51) == 1)
			{
				switch (Main.rand.Next(1, 9))
				{
					case 1:
						Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0, -10, mod.ProjectileType("AquaStream"), 4, 0.0f, projectile.owner, 1.2f/*X Increment*/, 0.2f/*Y Increment*/);
						break;
					case 2:
						Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 5, -5, mod.ProjectileType("AquaStream"), 4, 0.0f, projectile.owner, 0.7f/*X Increment*/, 0.7f/*Y Increment*/);
						break;
					case 3:
						Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 10, 0, mod.ProjectileType("AquaStream"), 4, 0.0f, projectile.owner, 0.2f/*X Increment*/, 1.2f/*Y Increment*/);
						break;
					case 4:
						Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 5, 5, mod.ProjectileType("AquaStream"), 4, 0.0f, projectile.owner, -0.7f/*X Increment*/, 0.7f/*Y Increment*/);
						break;
					case 5:
						Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, -0, 10, mod.ProjectileType("AquaStream"), 4, 0.0f, projectile.owner, -1.2f/*X Increment*/, -0.2f/*Y Increment*/);
						break;
					case 6:
						Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, -5, 5, mod.ProjectileType("AquaStream"), 4, 0.0f, projectile.owner, -0.7f/*X Increment*/, -0.7f/*Y Increment*/);
						break;
					case 7:
						Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, -10, -0, mod.ProjectileType("AquaStream"), 4, 0.0f, projectile.owner, -0.2f/*X Increment*/, -1.2f/*Y Increment*/);
						break;
					case 8:
						Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, -10, -10, mod.ProjectileType("AquaStream"), 4, 0.0f, projectile.owner, 0.7f/*X Increment*/, -0.7f/*Y Increment*/);
						break;
				}
			}
		}
	}
}