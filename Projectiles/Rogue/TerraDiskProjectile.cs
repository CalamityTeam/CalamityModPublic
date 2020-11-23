using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
	public class TerraDiskProjectile : ModProjectile
	{
		public override string Texture => "CalamityMod/Items/Weapons/Rogue/TerraDisk";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Terra Disk");
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
			ProjectileID.Sets.TrailingMode[projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			projectile.width = projectile.height = 46;
			projectile.friendly = true;
			projectile.tileCollide = false;
			projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 10;
			projectile.penetrate = -1;
			projectile.aiStyle = 3;
			aiType = ProjectileID.WoodenBoomerang;
			projectile.timeLeft = 180;
			projectile.Calamity().rogue = true;
		}

		public override void AI()
		{
			Lighting.AddLight(projectile.Center, 0f, (255 - projectile.alpha) * 0.75f / 255f, 0f);
			if (Main.rand.NextBool(5))
				Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 107, projectile.velocity.X, projectile.velocity.Y);

			float maxDistance = 300f;
			bool homeIn = false;

			for (int i = 0; i < Main.maxNPCs; i++)
			{
				NPC npc = Main.npc[i];
				if (npc.CanBeChasedBy(projectile, false))
				{
					float extraDistance = npc.width / 2 + npc.height / 2;

					bool canHit = true;
					if (extraDistance < maxDistance)
						canHit = Collision.CanHit(projectile.Center, 1, 1, npc.Center, 1, 1);

					if (Vector2.Distance(npc.Center, projectile.Center) < maxDistance + extraDistance && canHit)
					{
						homeIn = true;
						break;
					}
				}
			}

			if (!projectile.friendly)
				homeIn = false;

			if (homeIn)
			{
				projectile.localAI[0] += 1f;
				if (projectile.localAI[0] % 8f == 0f && Main.rand.NextBool(5))
				{
					int splitProj = ModContent.ProjectileType<TerraDiskProjectile2>();
					if (projectile.owner == Main.myPlayer)
					{
						float spread = 60f * 0.0174f;
						double startAngle = Math.Atan2(projectile.velocity.X, projectile.velocity.Y) - spread / 2;
						double deltaAngle = spread / 6f;
						double offsetAngle;
						for (int i = 0; i < 3; i++)
						{
							offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
							Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f), splitProj, projectile.damage / 2, projectile.knockBack / 2f, projectile.owner);
							Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f), splitProj, projectile.damage / 2, projectile.knockBack / 2f, projectile.owner);
						}
					}
				}
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 2);
			return false;
		}
	}
}
