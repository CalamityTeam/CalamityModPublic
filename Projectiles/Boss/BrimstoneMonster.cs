using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.World;

namespace CalamityMod.Projectiles.Boss
{
    public class BrimstoneMonster : ModProjectile
	{
		private float speedAdd = 0f;
		private float speedLimit = 0f;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Brimstone Monster");
		}

		public override void SetDefaults()
		{
			projectile.width = 320;
			projectile.height = 320;
			projectile.hostile = true;
			projectile.ignoreWater = true;
			projectile.tileCollide = false;
			projectile.penetrate = -1;
			projectile.timeLeft = 36000;
			projectile.alpha = 50;
			cooldownSlot = 1;
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(speedAdd);
			writer.Write(projectile.localAI[0]);
			writer.Write(speedLimit);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			speedAdd = reader.ReadSingle();
			projectile.localAI[0] = reader.ReadSingle();
			speedLimit = reader.ReadSingle();
		}

		public override void AI()
		{
			if (!CalamityPlayer.areThereAnyDamnBosses)
			{
				projectile.active = false;
				projectile.netUpdate = true;
				return;
			}
			int choice = (int)projectile.ai[1];
			if (projectile.localAI[0] == 0f)
			{
				projectile.localAI[0] += 1f;
				if (choice == 0)
				{
					speedLimit = 10f;
				}
				else if (choice == 1)
				{
					speedLimit = 20f;
				}
				else if (choice == 2)
				{
					speedLimit = 30f;
				}
				else
				{
					speedLimit = 40f;
				}
			}
			if (speedAdd < speedLimit)
			{
				speedAdd += 0.04f;
			}
			bool revenge = CalamityWorld.revenge;
			Lighting.AddLight(projectile.Center, ((255 - projectile.alpha) * 3f) / 255f, ((255 - projectile.alpha) * 0f) / 255f, ((255 - projectile.alpha) * 0f) / 255f);
			float num953 = (revenge ? 5f : 4.5f) + speedAdd; //100
			float scaleFactor12 = (revenge ? 1.5f : 1.35f) + (speedAdd * 0.25f); //5
			float num954 = 40f;
			if (projectile.timeLeft > 30 && projectile.alpha > 0)
			{
				projectile.alpha -= 25;
			}
			if (projectile.timeLeft > 30 && projectile.alpha < 128 && Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
			{
				projectile.alpha = 128;
			}
			if (projectile.alpha < 0)
			{
				projectile.alpha = 0;
			}
			int num959 = (int)projectile.ai[0];
			if (num959 >= 0 && Main.player[num959].active && !Main.player[num959].dead)
			{
				if (projectile.Distance(Main.player[num959].Center) > num954)
				{
					Vector2 vector102 = projectile.DirectionTo(Main.player[num959].Center);
					if (vector102.HasNaNs())
					{
						vector102 = Vector2.UnitY;
					}
					projectile.velocity = (projectile.velocity * (num953 - 1f) + vector102 * scaleFactor12) / num953;
				}
			}
			else
			{
				if (projectile.ai[0] != -1f)
				{
					projectile.ai[0] = -1f;
					projectile.netUpdate = true;
				}
			}
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			float dist1 = Vector2.Distance(projectile.Center, targetHitbox.TopLeft());
			float dist2 = Vector2.Distance(projectile.Center, targetHitbox.TopRight());
			float dist3 = Vector2.Distance(projectile.Center, targetHitbox.BottomLeft());
			float dist4 = Vector2.Distance(projectile.Center, targetHitbox.BottomRight());

			float minDist = dist1;
			if (dist2 < minDist) minDist = dist2;
			if (dist3 < minDist) minDist = dist3;
			if (dist4 < minDist) minDist = dist4;

			return minDist <= 170f;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Texture2D tex = Main.projectileTexture[projectile.type];
			spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
			return false;
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return new Color(250, 50, 50, projectile.alpha);
		}

		public override void OnHitPlayer(Player target, int damage, bool crit)
		{
			target.AddBuff(mod.BuffType("AbyssalFlames"), 900);
			target.AddBuff(mod.BuffType("VulnerabilityHex"), 300, true);
		}
	}
}
