using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class EradicatorProjectile : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Eradicator");
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
			ProjectileID.Sets.TrailingMode[projectile.type] = 1;
		}

        public override void SetDefaults()
        {
            projectile.width = 58;
            projectile.height = 58;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.aiStyle = 3;
            projectile.timeLeft = 180;
            aiType = 52;
            projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 3;
			projectile.GetGlobalProjectile<CalamityGlobalProjectile>(mod).rogue = true;
		}

        public override void AI()
        {
        	Lighting.AddLight(projectile.Center, 0.35f, 0f, 0.25f);
        	int[] array = new int[20];
			int num428 = 0;
			float num429 = 300f;
			bool flag14 = false;
			for (int num430 = 0; num430 < 200; num430++)
			{
				if (Main.npc[num430].CanBeChasedBy(projectile, false))
				{
					float num431 = Main.npc[num430].position.X + (float)(Main.npc[num430].width / 2);
					float num432 = Main.npc[num430].position.Y + (float)(Main.npc[num430].height / 2);
					float num433 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num431) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num432);
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
				float num435 = Main.npc[num434].position.X + (float)(Main.npc[num434].width / 2);
				float num436 = Main.npc[num434].position.Y + (float)(Main.npc[num434].height / 2);
				projectile.localAI[0] += 1f;
				if (projectile.localAI[0] > 8f)
				{
					projectile.localAI[0] = 0f;
					float num437 = 6f;
					Vector2 value10 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
					value10 += projectile.velocity * 4f;
					float num438 = num435 - value10.X;
					float num439 = num436 - value10.Y;
					float num440 = (float)Math.Sqrt((double)(num438 * num438 + num439 * num439));
					num440 = num437 / num440;
					num438 *= num440;
					num439 *= num440;
					if (Main.rand.Next(2) == 0)
					{
						if (projectile.owner == Main.myPlayer)
						{
							int proj = Projectile.NewProjectile(value10.X, value10.Y, num438, num439, mod.ProjectileType("NebulaShot"), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
							Main.projectile[proj].GetGlobalProjectile<CalamityGlobalProjectile>(mod).forceRogue = true;
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

		public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Vector2 origin = new Vector2(31f, 29f);
			spriteBatch.Draw(mod.GetTexture("Projectiles/Rogue/EradicatorProjectileGlow"), projectile.Center - Main.screenPosition, null, Color.White, projectile.rotation, origin, 1f, SpriteEffects.None, 0f);
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			target.AddBuff(BuffID.OnFire, 600);
			target.AddBuff(BuffID.Frostburn, 600);
		}
	}
}
