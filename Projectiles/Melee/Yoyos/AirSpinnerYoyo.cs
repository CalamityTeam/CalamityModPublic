using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee.Yoyos
{
    public class AirSpinnerYoyo : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Air Spinner");
            ProjectileID.Sets.YoyosLifeTimeMultiplier[projectile.type] = 8f;
            ProjectileID.Sets.YoyosMaximumRange[projectile.type] = 300f;
            ProjectileID.Sets.YoyosTopSpeed[projectile.type] = 10.5f;

            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.aiStyle = 99;
            projectile.width = 16;
            projectile.height = 16;
            projectile.scale = 1.05f;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.penetrate = -1;
            projectile.MaxUpdates = 2;
        }

        public override void AI()
        {
			int[] array = new int[20];
			int num428 = 0;
			float distance = 300f;
			bool flag14 = false;
			for (int num430 = 0; num430 < 200; num430++)
			{
				if (Main.npc[num430].CanBeChasedBy(projectile, false))
				{
					float extraDistance = (float)(Main.npc[num430].width / 2) + (float)(Main.npc[num430].height / 2);

					bool useCollisionDetection = extraDistance < distance;
					bool canHit = true;
					if (useCollisionDetection)
						canHit = Collision.CanHit(projectile.Center, 1, 1, Main.npc[num430].Center, 1, 1);

					if (Vector2.Distance(Main.npc[num430].Center, projectile.Center) < (distance + extraDistance) && canHit)
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
                if (projectile.localAI[0] > 60f)
                {
                    projectile.localAI[0] = 0f;
                    float num437 = 6f;
                    Vector2 value10 = new Vector2(projectile.position.X + projectile.width * 0.5f, projectile.position.Y + projectile.height * 0.5f);
                    value10 += projectile.velocity * 4f;
                    float num438 = num435 - value10.X;
                    float num439 = num436 - value10.Y;
                    float num440 = (float)Math.Sqrt(num438 * num438 + num439 * num439);
                    num440 = num437 / num440;
                    num438 *= num440;
                    num439 *= num440;
                    if (projectile.owner == Main.myPlayer)
                        Projectile.NewProjectile(value10.X, value10.Y, num438, num439, ModContent.ProjectileType<Feather>(), projectile.damage / 4, 0f, projectile.owner, 0f, 0f);
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
            return false;
        }
    }
}
