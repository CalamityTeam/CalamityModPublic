using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Amidias
{
	public class UrchinBall : ModProjectile
	{
		public bool spike = false;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("UrchinBall");
		}

		public override void SetDefaults()
		{
			projectile.width = 34;
			projectile.height = 34;
			projectile.friendly = true;
			projectile.ignoreWater = true;
			projectile.penetrate = -1;
			projectile.melee = true;
			projectile.alpha = 255;
			projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 3;
		}

		public override void AI()
		{
			Vector2 vector62 = Main.player[projectile.owner].Center - projectile.Center;
			projectile.rotation = vector62.ToRotation() - 1.57f;
			if (Main.player[projectile.owner].dead)
			{
				projectile.Kill();
				return;
			}
			Main.player[projectile.owner].itemAnimation = 10;
			Main.player[projectile.owner].itemTime = 10;
			float arg_1DC8F_0 = vector62.X;
			if (vector62.X < 0f)
			{
				Main.player[projectile.owner].ChangeDir(1);
				projectile.direction = 1;
			}
			else
			{
				Main.player[projectile.owner].ChangeDir(-1);
				projectile.direction = -1;
			}
			Main.player[projectile.owner].itemRotation = (vector62 * -1f * (float)projectile.direction).ToRotation();
			projectile.spriteDirection = ((vector62.X > 0f) ? -1 : 1);
			if (projectile.ai[0] == 0f && vector62.Length() > 400f)
			{
				projectile.ai[0] = 1f;
			}
			if (projectile.ai[0] == 1f || projectile.ai[0] == 2f)
			{
				if (spike)
				{
					spike = false;
					Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, projectile.velocity.X, projectile.velocity.Y, mod.ProjectileType("UrchinBallSpike"), (int)((double)projectile.damage * 0.5), 0f, Main.myPlayer, 0f, 0f);
				}
				projectile.usesLocalNPCImmunity = false;
				float num693 = vector62.Length();
				if (num693 > 1500f)
				{
					projectile.Kill();
					return;
				}
				if (num693 > 600f)
				{
					projectile.ai[0] = 2f;
				}
				projectile.tileCollide = false;
				float num694 = 20f;
				if (projectile.ai[0] == 2f)
				{
					num694 = 40f;
				}
				projectile.velocity = Vector2.Normalize(vector62) * num694;
				if (vector62.Length() < num694)
				{
					projectile.Kill();
					return;
				}
			}
			float[] var_2_1DE21_cp_0 = projectile.ai;
			int var_2_1DE21_cp_1 = 1;
			float num73 = var_2_1DE21_cp_0[var_2_1DE21_cp_1];
			var_2_1DE21_cp_0[var_2_1DE21_cp_1] = num73 + 1f;
			if (projectile.ai[1] > 5f)
			{
				projectile.alpha = 0;
			}
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);
			projectile.ai[0] = 1f;
			projectile.netUpdate = true;
			Main.PlaySound(0, (int)projectile.position.X, (int)projectile.position.Y, 1, 1f, 0f);
			return false;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Vector2 mountedCenter = Main.player[projectile.owner].MountedCenter;
			Microsoft.Xna.Framework.Color transparent = Microsoft.Xna.Framework.Color.Transparent;
			Texture2D texture2D2 = mod.GetTexture("ExtraTextures/Chains/UrchinFlailChain");
			Vector2 vector17 = projectile.Center;
			Microsoft.Xna.Framework.Rectangle? sourceRectangle = null;
			Vector2 origin = new Vector2((float)texture2D2.Width * 0.5f, (float)texture2D2.Height * 0.5f);
			float num91 = (float)texture2D2.Height;
			Vector2 vector18 = mountedCenter - vector17;
			float rotation15 = (float)Math.Atan2((double)vector18.Y, (double)vector18.X) - 1.57f;
			bool flag13 = true;
			if (float.IsNaN(vector17.X) && float.IsNaN(vector17.Y))
			{
				flag13 = false;
			}
			if (float.IsNaN(vector18.X) && float.IsNaN(vector18.Y))
			{
				flag13 = false;
			}
			while (flag13)
			{
				if (vector18.Length() < num91 + 1f)
				{
					flag13 = false;
				}
				else
				{
					Vector2 value2 = vector18;
					value2.Normalize();
					vector17 += value2 * num91;
					vector18 = mountedCenter - vector17;
					Microsoft.Xna.Framework.Color color17 = Lighting.GetColor((int)vector17.X / 16, (int)(vector17.Y / 16f));
					Main.spriteBatch.Draw(texture2D2, vector17 - Main.screenPosition, sourceRectangle, color17, rotation15, origin, 1f, SpriteEffects.None, 0f);
				}
			}
			return true;
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			spike = true;
			projectile.ai[0] = 1f;
			projectile.netUpdate = true;
			target.AddBuff(BuffID.Venom, 300);
		}
	}
}