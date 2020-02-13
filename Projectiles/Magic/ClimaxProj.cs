using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class ClimaxProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Climax");
            Main.projFrames[projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            projectile.width = 38;
            projectile.height = 38;
            projectile.friendly = true;
            projectile.light = 0.5f;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 150;
            projectile.magic = true;
        }

        public override void AI()
        {
            projectile.velocity.X *= 0.98f;
            projectile.velocity.Y *= 0.98f;
            if (projectile.velocity.X > 0f)
            {
                projectile.rotation += (Math.Abs(projectile.velocity.Y) + Math.Abs(projectile.velocity.X)) * 0.001f;
            }
            else
            {
                projectile.rotation -= (Math.Abs(projectile.velocity.Y) + Math.Abs(projectile.velocity.X)) * 0.001f;
            }
            projectile.frameCounter++;
            if (projectile.frameCounter > 6)
            {
                projectile.frameCounter = 0;
                projectile.frame++;
                if (projectile.frame > 4)
                {
                    projectile.frame = 0;
                }
            }
            if (Math.Sqrt((double)(projectile.velocity.X * projectile.velocity.X + projectile.velocity.Y * projectile.velocity.Y)) > 2.0)
            {
                projectile.velocity *= 0.98f;
            }

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

			if (projectile.timeLeft < 30)
            {
                flag14 = false;
            }
            if (flag14)
            {
                int num440 = Main.rand.Next(num428);
                num440 = array[num440];
                float num441 = Main.npc[num440].position.X + (float)(Main.npc[num440].width / 2);
                float num442 = Main.npc[num440].position.Y + (float)(Main.npc[num440].height / 2);
                projectile.localAI[0] += 1f;
                if (projectile.localAI[0] > 4f)
                {
                    projectile.localAI[0] = 0f;
                    float num443 = 8f;
                    Vector2 vector33 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
                    vector33 += projectile.velocity * 4f;
                    float num444 = num441 - vector33.X;
                    float num445 = num442 - vector33.Y;
                    float num446 = (float)Math.Sqrt((double)(num444 * num444 + num445 * num445));
                    num446 = num443 / num446;
                    num444 *= num446;
                    num445 *= num446;
                    Projectile.NewProjectile(vector33.X, vector33.Y, num444, num445, ModContent.ProjectileType<ClimaxBeam>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (projectile.timeLeft < 30)
            {
                float num7 = (float)projectile.timeLeft / 30f;
                projectile.alpha = (int)(255f - 255f * num7);
            }
            return new Color(255 - projectile.alpha, 255 - projectile.alpha, 255 - projectile.alpha, 0);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num214 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int y6 = num214 * projectile.frame;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture2D13.Width, num214)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2((float)texture2D13.Width / 2f, (float)num214 / 2f), projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
