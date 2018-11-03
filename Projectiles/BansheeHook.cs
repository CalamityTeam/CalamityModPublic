using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles
{
    public class BansheeHook : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Banshee Hook");
		}
    	
        public override void SetDefaults()
        {
			projectile.width = 40;
			projectile.melee = true;
			projectile.timeLeft = 90;
			projectile.height = 40;
			projectile.friendly = true;
			projectile.hostile = false;
			projectile.tileCollide = false;
			projectile.ignoreWater = true;
			projectile.penetrate = -1;
			projectile.ownerHitCheck = true;
			projectile.hide = true;
			projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 2;
			projectile.alpha = 255;
        }

        public override void AI()
        {
        	Player player = Main.player[projectile.owner];
			Vector2 vector = player.RotatedRelativePoint(player.MountedCenter, true);
			projectile.direction = player.direction;
			player.heldProj = projectile.whoAmI;
			projectile.Center = vector;
			if (player.dead)
			{
				projectile.Kill();
				return;
			}
			if (!player.frozen)
			{
				if (Main.player[projectile.owner].itemAnimation < Main.player[projectile.owner].itemAnimationMax / 3)
	        	{
					if (projectile.localAI[0] == 0f && Main.myPlayer == projectile.owner)
					{
						projectile.localAI[0] = 1f;
						Projectile.NewProjectile(projectile.Center.X + (projectile.velocity.X * 0.5f), projectile.Center.Y + (projectile.velocity.Y * 0.5f), 
						                         projectile.velocity.X * 0.8f, projectile.velocity.Y * 0.8f, mod.ProjectileType("BansheeHookScythe"), (int)((double)projectile.damage * 1.75), projectile.knockBack * 0.85f, projectile.owner, 0f, 0f);
					}
	        	}
				projectile.spriteDirection = (projectile.direction = player.direction);
				projectile.alpha -= 127;
				if (projectile.alpha < 0)
				{
					projectile.alpha = 0;
				}
				if (projectile.localAI[0] > 0f)
				{
					projectile.localAI[0] -= 1f;
				}
				float num = (float)player.itemAnimation / (float)player.itemAnimationMax;
				float num2 = 1f - num;
				float num3 = projectile.velocity.ToRotation();
				float num4 = projectile.velocity.Length();
				float num5 = 22f;
				Vector2 spinningpoint = new Vector2(1f, 0f).RotatedBy((double)(3.14159274f + num2 * 6.28318548f), default(Vector2)) * new Vector2(num4, projectile.ai[0]);
				projectile.position += spinningpoint.RotatedBy((double)num3, default(Vector2)) + new Vector2(num4 + num5, 0f).RotatedBy((double)num3, default(Vector2));
				Vector2 destination = vector + spinningpoint.RotatedBy((double)num3, default(Vector2)) + new Vector2(num4 + num5 + 40f, 0f).RotatedBy((double)num3, default(Vector2));
				projectile.rotation = player.AngleTo(destination) + ((float)(Math.PI * 0.25)) * (float)player.direction; //or this
				if (projectile.spriteDirection == -1)
				{
					projectile.rotation += (float)Math.PI; //change this
				}
				player.DirectionTo(projectile.Center);
				Vector2 value = player.DirectionTo(destination);
				Vector2 vector2 = projectile.velocity.SafeNormalize(Vector2.UnitY);
				float num6 = 2f;
				int num7 = 0;
				while ((float)num7 < num6)
				{
					Dust dust = Dust.NewDustDirect(projectile.Center, 14, 14, 60, 0f, 0f, 110, default(Color), 1f);
					dust.velocity = player.DirectionTo(dust.position) * 2f;
					dust.position = projectile.Center + vector2.RotatedBy((double)(num2 * 6.28318548f * 2f + (float)num7 / num6 * 6.28318548f), default(Vector2)) * 10f;
					dust.scale = 1f + 0.6f * Main.rand.NextFloat();
					dust.velocity += vector2 * 3f;
					dust.noGravity = true;
					num7++;
				}
				for (int i = 0; i < 1; i++)
				{
					if (Main.rand.Next(3) == 0)
					{
						Dust dust2 = Dust.NewDustDirect(projectile.Center, 20, 20, 60, 0f, 0f, 110, default(Color), 1f);
						dust2.velocity = player.DirectionTo(dust2.position) * 2f;
						dust2.position = projectile.Center + value * -110f;
						dust2.scale = 0.45f + 0.4f * Main.rand.NextFloat();
						dust2.fadeIn = 0.7f + 0.4f * Main.rand.NextFloat();
						dust2.noGravity = true;
						dust2.noLight = true;
					}
				}
			}
			if (player.itemAnimation == 2)
			{
				projectile.Kill();
				player.reuseDelay = 2;
			}
        }
        
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
        	Microsoft.Xna.Framework.Color color25 = Lighting.GetColor((int)((double)projectile.position.X + (double)projectile.width * 0.5) / 16, (int)(((double)projectile.position.Y + (double)projectile.height * 0.5) / 16.0));
        	Vector2 vector53 = projectile.position + new Vector2((float)projectile.width, (float)projectile.height) / 2f + Vector2.UnitY * projectile.gfxOffY - Main.screenPosition;
        	Texture2D texture2D31 = (projectile.spriteDirection == -1) ? mod.GetTexture("Projectiles/BansheeHookAlt") : Main.projectileTexture[projectile.type];
			Microsoft.Xna.Framework.Color alpha4 = projectile.GetAlpha(color25);
			Vector2 origin8 = new Vector2((float)texture2D31.Width, (float)texture2D31.Height) / 2f;
			origin8 = new Vector2((projectile.spriteDirection == 1) ? ((float)texture2D31.Width - -8f) : -8f, -8f); //-8 -8
			SpriteBatch arg_E055_0 = Main.spriteBatch;
			Vector2 arg_E055_2 = vector53;
			Microsoft.Xna.Framework.Rectangle? sourceRectangle2 = null;
			arg_E055_0.Draw(texture2D31, arg_E055_2, sourceRectangle2, new Microsoft.Xna.Framework.Color(255, 255, 255, 127), projectile.rotation, origin8, projectile.scale, SpriteEffects.None, 0f);
        	return false;
        }
        
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
        	float f2 = projectile.rotation - 0.7853982f * (float)Math.Sign(projectile.velocity.X) + ((projectile.spriteDirection == -1) ? 3.14159274f : 0f);
			float num4 = 0f;
			float scaleFactor = -95f;
			if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center, projectile.Center + f2.ToRotationVector2() * scaleFactor, 23f * projectile.scale, ref num4))
			{
				return true;
			}
			return false;
        }
        
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			if (projectile.owner == Main.myPlayer) 
			{
				Projectile.NewProjectile(target.Center.X, target.Center.Y, 0f, 0f, mod.ProjectileType("BansheeHookBoom"), damage, 10f, projectile.owner, 0f, 0.85f + Main.rand.NextFloat() * 1.15f);
			}
        }
    }
}