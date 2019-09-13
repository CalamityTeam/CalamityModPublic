using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Projectiles.Melee
{
    public class Cyclone : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cyclone");
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
			ProjectileID.Sets.TrailingMode[projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			projectile.width = 40;
			projectile.height = 40;
			projectile.alpha = 255;
			projectile.friendly = true;
			projectile.melee = true;
			projectile.timeLeft = 300;
			projectile.extraUpdates = 2;
			projectile.penetrate = 2;
			projectile.ignoreWater = true;
			projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 10;
		}

		public override void AI()
		{
			projectile.rotation += 2.5f;
			projectile.alpha -= 5;
			if (projectile.alpha < 50)
			{
				projectile.alpha = 50;
			}
			float num472 = projectile.Center.X;
			float num473 = projectile.Center.Y;
			float npcCenterX = 0f;
			float npcCenterY = 0f;
			float num474 = 600f;
			for (int num475 = 0; num475 < 200; num475++)
			{
				NPC npc = Main.npc[num475];
				if (npc.CanBeChasedBy(projectile, false) && Collision.CanHit(projectile.Center, 1, 1, npc.Center, 1, 1) && !CalamityPlayer.areThereAnyDamnBosses)
				{
					npcCenterX = npc.position.X + (float)(npc.width / 2);
					npcCenterY = npc.position.Y + (float)(npc.height / 2);
					float num478 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - npcCenterX) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - npcCenterY);
					if (num478 < num474)
					{
						if (npc.position.X < num472)
						{
							npc.velocity.X += 0.05f;
						}
						else
						{
							npc.velocity.X -= 0.05f;
						}
						if (npc.position.Y < num473)
						{
							npc.velocity.Y += 0.05f;
						}
						else
						{
							npc.velocity.Y -= 0.05f;
						}
					}
				}
			}
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return new Color(204, 255, 255, projectile.alpha);
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
			return false;
		}
	}
}
