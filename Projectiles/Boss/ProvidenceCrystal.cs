using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.World;

namespace CalamityMod.Projectiles.Boss
{
    public class ProvidenceCrystal : ModProjectile
	{
		private float speedX = -21f;
		private float speedY = -3f;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Holy Crystal");
		}

		public override void SetDefaults()
		{
			projectile.width = 160;
			projectile.height = 160;
			projectile.ignoreWater = true;
			projectile.timeLeft = CalamityWorld.death ? 2100 : 3600;
			projectile.alpha = 255;
			projectile.tileCollide = false;
			projectile.penetrate = -1;
			cooldownSlot = 1;
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(speedX);
			writer.Write(speedY);
			writer.Write(projectile.localAI[0]);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			speedX = reader.ReadSingle();
			speedY = reader.ReadSingle();
			projectile.localAI[0] = reader.ReadSingle();
		}

		public override void AI()
		{
			Player player = Main.player[projectile.owner];
			if (player.dead || NPC.CountNPCS(mod.NPCType("Providence")) < 1)
			{
				projectile.active = false;
				projectile.netUpdate = true;
				return;
			}
			projectile.position.X = Main.player[projectile.owner].Center.X - (float)(projectile.width / 2);
			projectile.position.Y = Main.player[projectile.owner].Center.Y - (float)(projectile.height / 2) + Main.player[projectile.owner].gfxOffY - 360f;
			if (Main.player[projectile.owner].gravDir == -1f)
			{
				projectile.position.Y = projectile.position.Y + 400f;
				projectile.rotation = 3.14f;
			}
			else
			{
				projectile.rotation = 0f;
			}
			projectile.position.X = (float)((int)projectile.position.X);
			projectile.position.Y = (float)((int)projectile.position.Y);
			projectile.velocity = Vector2.Zero;
			projectile.alpha -= 5;
			if (projectile.alpha < 0)
			{
				projectile.alpha = 0;
			}
			if (projectile.direction == 0)
			{
				projectile.direction = Main.player[projectile.owner].direction;
			}
			if (projectile.alpha == 0 && Main.rand.NextBool(15))
			{
				Dust dust34 = Main.dust[Dust.NewDust(projectile.Top, 0, 0, 267, 0f, 0f, 100, new Color(255, 200, Main.DiscoB), 1f)];
				dust34.velocity.X = 0f;
				dust34.noGravity = true;
				dust34.fadeIn = 1f;
				dust34.position = projectile.Center + Vector2.UnitY.RotatedByRandom(6.2831854820251465) * (4f * Main.rand.NextFloat() + 26f);
				dust34.scale = 0.5f;
			}
			projectile.localAI[0] += 1f;
			if (projectile.localAI[0] >= 300f)
			{
				projectile.localAI[0] = 0f;
				Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 109);
				projectile.netUpdate = true;
				if (projectile.owner == Main.myPlayer)
				{
					for (int num1083 = 0; num1083 < 14; num1083++)
					{
						float x4 = Main.rgbToHsl(new Color(255, 200, Main.DiscoB)).X;
						Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, speedX, speedY, mod.ProjectileType("ProvidenceCrystalShard"), projectile.damage, projectile.knockBack, projectile.owner, x4, (float)projectile.whoAmI);
						speedX += 3f; // -21, -18, -15, -12, -9, -6, -3, 0, 3, 6, 9, 12, 15, 18, 21
					}
				}
				speedX = -21f;
			}
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return new Color(255 - projectile.alpha, 255 - projectile.alpha, 255 - projectile.alpha, 0);
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Microsoft.Xna.Framework.Color color25 = Lighting.GetColor((int)((double)projectile.position.X + (double)projectile.width * 0.5) / 16, (int)(((double)projectile.position.Y + (double)projectile.height * 0.5) / 16.0));
			Vector2 vector59 = projectile.position + new Vector2((float)projectile.width, (float)projectile.height) / 2f + Vector2.UnitY * projectile.gfxOffY - Main.screenPosition;
			Texture2D texture2D34 = Main.projectileTexture[projectile.type];
			Microsoft.Xna.Framework.Rectangle rectangle17 = texture2D34.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);
			Microsoft.Xna.Framework.Color alpha5 = projectile.GetAlpha(color25);
			Vector2 origin11 = rectangle17.Size() / 2f;
			float scaleFactor5 = (float)Math.Cos((double)(6.28318548f * (projectile.localAI[0] / 60f))) + 3f + 3f;
			for (float num286 = 0f; num286 < 4f; num286 += 1f)
			{
				SpriteBatch arg_F907_0 = Main.spriteBatch;
				Texture2D arg_F907_1 = texture2D34;
				Vector2 arg_F8CE_0 = vector59;
				Vector2 arg_F8BE_0 = Vector2.UnitY;
				double arg_F8BE_1 = (double)(num286 * 1.57079637f);
				Vector2 center = default;
				arg_F907_0.Draw(arg_F907_1, arg_F8CE_0 + arg_F8BE_0.RotatedBy(arg_F8BE_1, center) * scaleFactor5, new Microsoft.Xna.Framework.Rectangle?(rectangle17), alpha5 * 0.2f, projectile.rotation, origin11, projectile.scale, SpriteEffects.None, 0f);
			}
			return false;
		}
	}
}
