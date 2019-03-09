using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Patreon
{
	public class MelterAmp : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Melter Amp");
			Main.projFrames[projectile.type] = 3;
		}

		public override void SetDefaults()
		{
			projectile.width = 34;
			projectile.height = 38;
			projectile.friendly = true;
			projectile.magic = true;
			projectile.netImportant = true;
			projectile.penetrate = -1;
			projectile.timeLeft = 6000;
			projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 20;
			projectile.tileCollide = false;
			projectile.ignoreWater = true;
		}

		public override void AI()
		{
			bool flag64 = projectile.type == mod.ProjectileType("MelterAmp");
			Player player = Main.player[projectile.owner];
			if (flag64)
			{
				if (player.dead)
				{
					projectile.active = false;
					return;
				}
				if (player.ownedProjectileCounts[mod.ProjectileType("MelterAmp")] > 1)
				{
					projectile.active = false;
					return;
				}
				if (!player.inventory[player.selectedItem].magic || player.inventory[player.selectedItem].shoot != mod.ProjectileType("MelterNote1"))
				{
					projectile.active = false;
					return;
				}
			}
			Lighting.AddLight(projectile.Center, 0.75f, 0.75f, 0.75f);
			if (projectile.ai[0] > 0f)
			{
				projectile.ai[0] += 1f;
				if (projectile.ai[0] > 6f)
				{
					projectile.ai[0] = 0f;
				}
			}
			if (Main.myPlayer == projectile.owner && projectile.ai[0] == 0f)
			{
				projectile.ai[0] = 1f;
				int Damage = projectile.damage;
				int type;
				projectile.netUpdate = true;
				float num127 = 20f;
				Vector2 vector11 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
				float num128 = (float)Main.mouseX + Main.screenPosition.X - vector11.X;
				float num129 = (float)Main.mouseY + Main.screenPosition.Y - vector11.Y;
				if (player.gravDir == -1f)
				{
					num129 = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - vector11.Y;
				}
				float num130 = (float)Math.Sqrt((double)(num128 * num128 + num129 * num129));
				if (num130 == 0f)
				{
					vector11 = new Vector2(player.position.X + (float)(player.width / 2), player.position.Y + (float)(player.height / 2));
					num128 = projectile.position.X + (float)projectile.width * 0.5f - vector11.X;
					num129 = projectile.position.Y + (float)projectile.height * 0.5f - vector11.Y;
					num130 = (float)Math.Sqrt((double)(num128 * num128 + num129 * num129));
				}
				num130 = num127 / num130;
				num128 *= num130;
				num129 *= num130;
				float VelocityX = num128;
				float VelocityY = num129;
				int note = Main.rand.Next(0, 2);
				if (note == 0)
				{
					Damage = (int)(projectile.damage * 1.5f);
					type = mod.ProjectileType("MelterNote1");
				}
				else
				{
					VelocityX *= 1.5f;
					VelocityY *= 1.5f;
					type = mod.ProjectileType("MelterNote2");
				}
				Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, VelocityX, VelocityY, type, Damage, projectile.knockBack, projectile.owner, 0.0f, 0.0f);
			}
			projectile.frameCounter++;
			if (projectile.frameCounter > 5)
			{
				projectile.frame++;
				projectile.frameCounter = 0;
			}
			if (projectile.frame > 2)
			{
				projectile.frame = 0;
			}
		}
	}
}