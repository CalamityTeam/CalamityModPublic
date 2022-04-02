using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Pets
{
    public class SirenYoung : ModProjectile
    {
        private bool underwater = false;
        private int sleepyTimer = 0;
        private int lightLevel = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ocean Spirit");
            Main.projFrames[projectile.type] = 17;
            Main.projPet[projectile.type] = true;
            ProjectileID.Sets.LightPet[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.netImportant = true;
            projectile.width = 38;
            projectile.height = 58;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft *= 5;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
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
                modPlayer.sirenPet = false;
            }
            if (modPlayer.sirenPet)
            {
                projectile.timeLeft = 2;
            }
            bool sleepy = sleepyTimer >= 180;
            if (!sleepy)
            {
                projectile.frameCounter++;
                if (projectile.frameCounter > 6)
                {
                    projectile.frame++;
                    projectile.frameCounter = 0;
                }
            }
            if (underwater)
            {
                if (projectile.frame >= 8)
                {
                    projectile.frame = 0;
                }
            }
            else
            {
                if (projectile.frame >= 16)
                {
                    projectile.frame = sleepy ? 16 : 8;
                }
            }
            underwater = player.IsUnderwater();
            if (underwater)
            {
                if (projectile.frame == 16)
                    projectile.frame = 0;
                if (sleepyTimer > 0)
                    sleepyTimer--;
                if (projectile.localAI[0] == 0f)
                {
                    lightLevel = 0;
                }
                else
                {
                    lightLevel = 1;
                }
            }
            else
            {
                if (!sleepy)
                {
                    sleepyTimer++;
                    lightLevel = 1;
                }
                else
                {
                    lightLevel = 2;
                    projectile.frame = 16;
                }
            }
            switch (lightLevel)
            {
                case 0:
                    Lighting.AddLight(projectile.Center, 0f, 2f, 2.5f); //4.5
                    break;
                case 1:
                    Lighting.AddLight(projectile.Center, 0f, 1.32f, 1.65f); //3
                    break;
                case 2:
                    Lighting.AddLight(projectile.Center, 0f, 0.5f, 0.7f);
                    break;
            }

            float velAdjustment = 0.2f;
            float speedLimit = 5f;
            Vector2 playerVec = player.Center - projectile.Center;
            playerVec.Y += player.gfxOffY;
            if (player.controlLeft && !sleepy)
            {
                playerVec.X -= 120f;
            }
            else if (player.controlRight && !sleepy)
            {
                playerVec.X += 120f;
            }
            if (player.controlDown && !sleepy)
            {
                playerVec.Y += 120f;
            }
            else
            {
                if (player.controlUp && !sleepy)
                {
                    playerVec.Y -= 120f;
                }
                playerVec.Y -= 60f;
            }

            if (projectile.velocity.X < -0.25f || (player.controlLeft && !sleepy))
            {
                projectile.direction = -1; //face left
            }
            else if (projectile.velocity.X > 0.25f || (player.controlRight && !sleepy))
            {
                projectile.direction = 1; //face right
            }
            projectile.spriteDirection = projectile.direction;

            float playerDist = playerVec.Length();
            if (playerDist > 1000f)
            {
                projectile.position.X += playerVec.X;
                projectile.position.Y += playerVec.Y;
            }
            if (projectile.localAI[0] == 1f)
            {
                if (playerDist < 10f && player.velocity.Length() < speedLimit && player.velocity.Y == 0f)
                {
                    projectile.localAI[0] = 0f;
                }
                speedLimit = 12f;
                if (playerDist < speedLimit)
                {
                    projectile.velocity = playerVec;
                }
                else
                {
                    playerDist = speedLimit / playerDist;
                    projectile.velocity = playerVec * playerDist;
                }
                projectile.rotation = projectile.velocity.X * 0.05f;
                return;
            }
            if (playerDist > 200f)
            {
                projectile.localAI[0] = 1f;
            }
            if (playerDist < 10f)
            {
                projectile.velocity.X = playerVec.X;
                projectile.velocity.Y = playerVec.Y;
                projectile.rotation = projectile.velocity.X * 0.05f;
                if (playerDist < speedLimit)
                {
                    projectile.position += projectile.velocity;
                    projectile.velocity *= 0f;
                    velAdjustment = 0f;
                }
            }
            playerDist = speedLimit / playerDist;
            playerVec *= playerDist;
            if (projectile.velocity.X < playerVec.X)
            {
                projectile.velocity.X += velAdjustment;
                if (projectile.velocity.X < 0f)
                {
                    projectile.velocity.X *= 0.99f;
                }
            }
            if (projectile.velocity.X > playerVec.X)
            {
                projectile.velocity.X -= velAdjustment;
                if (projectile.velocity.X > 0f)
                {
                    projectile.velocity.X *= 0.99f;
                }
            }
            if (projectile.velocity.Y < playerVec.Y)
            {
                projectile.velocity.Y += velAdjustment;
                if (projectile.velocity.Y < 0f)
                {
                    projectile.velocity.Y *= 0.99f;
                }
            }
            if (projectile.velocity.Y > playerVec.Y)
            {
                projectile.velocity.Y -= velAdjustment;
                if (projectile.velocity.Y > 0f)
                {
                    projectile.velocity.Y *= 0.99f;
                }
            }
            if (projectile.velocity.X != 0f || projectile.velocity.Y != 0f)
            {
                projectile.rotation = projectile.velocity.X * 0.05f;
            }
        }
    }
}
