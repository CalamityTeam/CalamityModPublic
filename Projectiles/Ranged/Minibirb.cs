using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class Minibirb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Minibirb");
            Main.projFrames[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
            projectile.light = 0.25f;
            projectile.extraUpdates = 1;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);
            Main.PlaySound(3, (int)projectile.position.X, (int)projectile.position.Y, 51, 1f, 0f);
            return true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Main.PlaySound(3, (int)projectile.position.X, (int)projectile.position.Y, 51, 1f, 0f);
        }

		public override void AI()
		{
            projectile.frameCounter++;
            if (projectile.frameCounter > 4)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 1)
            {
                projectile.frame = 0;
            }
            if (Main.rand.Next(5) == 0)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 244, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }
            projectile.spriteDirection = projectile.direction = (projectile.velocity.X > 0).ToDirectionInt();
            projectile.rotation = projectile.velocity.ToRotation() + (projectile.spriteDirection == 1 ? MathHelper.Pi : 0f);

			float pcx = projectile.Center.X;
			float pcy = projectile.Center.Y;
			float var1 = 800f;
			bool flag = false;
			for (int npcvar = 0; npcvar < 200; npcvar++)
			{
				if (Main.npc[npcvar].CanBeChasedBy(projectile, false) && Collision.CanHit(projectile.Center, 1, 1, Main.npc[npcvar].Center, 1, 1))
				{
					float var2 = Main.npc[npcvar].position.X + (Main.npc[npcvar].width / 2);
					float var3 = Main.npc[npcvar].position.Y + (Main.npc[npcvar].height / 2);
					float var4 = Math.Abs(projectile.position.X + (projectile.width / 2) - var2) + Math.Abs(projectile.position.Y + (projectile.height / 2) - var3);
					if (var4 < var1)
					{
						var1 = var4;
						pcx = var2;
						pcy = var3;
						flag = true;
					}
				}
			}
			if (flag)
			{
				float homingstrenght = 10f;
				Vector2 vector1 = new Vector2(projectile.position.X + projectile.width * 0.5f, projectile.position.Y + projectile.height * 0.5f);
				float var6 = pcx - vector1.X;
				float var7 = pcy - vector1.Y;
				float var8 = (float)Math.Sqrt(var6 * var6 + var7 * var7);
				var8 = homingstrenght / var8;
				var6 *= var8;
				var7 *= var8;
				projectile.velocity.X = (projectile.velocity.X * 20f + var6) / 21f;
				projectile.velocity.Y = (projectile.velocity.Y * 20f + var7) / 21f;
			}
        }
    }
}
