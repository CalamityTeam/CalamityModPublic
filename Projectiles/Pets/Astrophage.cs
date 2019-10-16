using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Projectiles
{
    public class Astrophage : ModProjectile
    {
        private bool fly = false;
		
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astrophage");
            Main.projFrames[projectile.type] = 5;
            Main.projPet[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.netImportant = true;
            projectile.width = 35;
            projectile.height = 35;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft *= 5;
            projectile.tileCollide = true;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            Player player = Main.player[projectile.owner];
            Vector2 center2 = projectile.Center;
            Vector2 vector48 = player.Center - center2;
            float playerDistance = vector48.Length();
            fallThrough = playerDistance > 200f;
            return true;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            if (!player.active)
            {
                projectile.active = false;
                return;
            }
            CalamityPlayer modPlayer = player.Calamity();
            if (player.dead)
            {
                modPlayer.astrophage = false;
            }
            if (modPlayer.astrophage)
            {
                projectile.timeLeft = 2;
            }
            if (projectile.position.X == projectile.oldPosition.X && projectile.position.Y == projectile.oldPosition.Y && projectile.velocity.X == 0)
            {
                projectile.frame = 0;
            }
            else if (projectile.velocity.Y > 0.3f && projectile.position.Y != projectile.oldPosition.Y)
            {
                projectile.frame = 1;
                projectile.frameCounter = 0;
            }
            else
			{
				projectile.frameCounter++;
				if (projectile.frameCounter > 6)
				{
					projectile.frame++;
					projectile.frameCounter = 0;
				}
				if (projectile.frame > 4)
				{
					projectile.frame = 0;
				}
			}
            Vector2 vector46 = projectile.position;
            if (!fly)
            {
                projectile.rotation = 0;
                Vector2 center2 = projectile.Center;
                Vector2 vector48 = player.Center - center2;
                float playerDistance = vector48.Length();
                if (projectile.velocity.Y == 0 && (HoleBelow() || (playerDistance > 110f && projectile.position.X == projectile.oldPosition.X)))
                {
                    projectile.velocity.Y = -5f;
                }
                projectile.velocity.Y += 0.20f;
                if (projectile.velocity.Y > 7f)
                {
                    projectile.velocity.Y = 7f;
                }
                if (playerDistance > 600f)
                {
                    fly = true;
                    projectile.velocity.X = 0f;
                    projectile.velocity.Y = 0f;
                }
                if (playerDistance > 100f)
                {
                    if (player.position.X - projectile.position.X > 0f)
                    {
                        projectile.velocity.X += 0.10f;
                        if (projectile.velocity.X > 7f)
                        {
                            projectile.velocity.X = 7f;
                        }
                    }
                    else
                    {
                        projectile.velocity.X -= 0.10f;
                        if (projectile.velocity.X < -7f)
                        {
                            projectile.velocity.X = -7f;
                        }
                    }
                }
                if (playerDistance < 100f)
                {
                    if (projectile.velocity.X != 0f)
                    {
                        if (projectile.velocity.X > 0.5f)
                        {
                            projectile.velocity.X -= 0.15f;
                        }
                        else if (projectile.velocity.X < -0.5f)
                        {
                            projectile.velocity.X += 0.15f;
                        }
                        else if (projectile.velocity.X < 0.5f && projectile.velocity.X > -0.5f)
                        {
                            projectile.velocity.X = 0f;
                        }
                    }
                }
            }
            else if (fly)
            {
				projectile.alpha +=15;
				if (projectile.alpha >= 255)
				{
					projectile.position.X = player.position.X;
					projectile.position.Y = player.position.Y;
					fly = false;
					projectile.alpha = 0;
				}
            }
            if (projectile.velocity.X > 0.25f)
            {
                projectile.spriteDirection = -1;
            }
            else if (projectile.velocity.X < -0.25f)
            {
                projectile.spriteDirection = 1;
            }
        }

        private bool HoleBelow() //pretty much the same as the one used in mantis
        {
            int tileWidth = 4;
            int tileX = (int)(projectile.Center.X / 16f) - tileWidth;
            if (projectile.velocity.X > 0)
            {
                tileX += tileWidth;
            }
            int tileY = (int)((projectile.position.Y + projectile.height) / 16f);
            for (int y = tileY; y < tileY + 2; y++)
            {
                for (int x = tileX; x < tileX + tileWidth; x++)
                {
                    if (Main.tile[x, y].active())
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
