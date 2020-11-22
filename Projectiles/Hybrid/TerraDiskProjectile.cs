using CalamityMod.Projectiles.Hybrid;
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
			projectile.width = 46;
			projectile.height = 46;
			projectile.friendly = true;
			projectile.tileCollide = false;
			projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 10;
			projectile.penetrate = -1;
			projectile.aiStyle = 3;
			projectile.timeLeft = 180;
			aiType = ProjectileID.WoodenBoomerang;
			projectile.Calamity().rogue = true;
		}

		public override void AI()
		{
			Lighting.AddLight(projectile.Center, 0f, (255 - projectile.alpha) * 0.75f / 255f, 0f);
			if (Main.rand.NextBool(5))
				Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 107, projectile.velocity.X, projectile.velocity.Y);
			int[] array = new int[20];
			int num428 = 0;
			float num429 = 300f;
			bool flag14 = false;
			for (int num430 = 0; num430 < 200; num430++)
			{
				if (Main.npc[num430].CanBeChasedBy(projectile, false))
				{
					float num431 = Main.npc[num430].position.X + Main.npc[num430].width / 2;
					float num432 = Main.npc[num430].position.Y + Main.npc[num430].height / 2;
					float num433 = Math.Abs(projectile.position.X + projectile.width / 2 - num431) + Math.Abs(projectile.position.Y + projectile.height / 2 - num432);
					if (num433 < num429 && Collision.CanHit(projectile.Center, 1, 1, Main.npc[num430].Center, 1, 1))
					{
						if (num428 < 20)
						{
							array[num428] = num430;
							num428++;
						}
						flag14 = true;
					}
				}
			}
			if (flag14)
			{
				int num434 = Main.rand.Next(num428);
				num434 = array[num434];
				float num435 = Main.npc[num434].position.X + Main.npc[num434].width / 2;
				float num436 = Main.npc[num434].position.Y + Main.npc[num434].height / 2;
				projectile.localAI[0] += 1f;
				if (projectile.localAI[0] > 8f)
				{
					projectile.localAI[0] = 0f;
					float num437 = 6f;
					Vector2 value10 = new Vector2(projectile.position.X + projectile.width * 0.5f, projectile.position.Y + projectile.height * 0.5f);
					value10 += projectile.velocity * 4f;
					float num438 = num435 - value10.X;
					float num439 = num436 - value10.Y;
					float num440 = (float)Math.Sqrt(num438 * num438 + num439 * num439);
					num440 = num437 / num440;
					if (Main.rand.NextBool(5))
					{
						if (projectile.owner == Main.myPlayer)
						{
							float spread = 60f * 0.0174f;
							double startAngle = Math.Atan2(projectile.velocity.X, projectile.velocity.Y) - spread / 2;
							double deltaAngle = spread / 6f;
							double offsetAngle;
							int i;
							for (i = 0; i < 3; i++)
							{
								offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
								int proj = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<TerraDiskProjectile2>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
								Main.projectile[proj].Calamity().forceRogue = true;
								int proj2 = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<TerraDiskProjectile2>(), projectile.damage / 2, projectile.knockBack, projectile.owner, 0f, 0f);
								Main.projectile[proj2].Calamity().forceRogue = true;
							}
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
