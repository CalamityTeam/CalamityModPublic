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
            Main.projFrames[Projectile.type] = 17;
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.LightPet[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.width = 38;
            Projectile.height = 58;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft *= 5;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (!player.active)
            {
                Projectile.active = false;
                return;
            }
            CalamityPlayer modPlayer = player.Calamity();
            if (player.dead)
            {
                modPlayer.sirenPet = false;
            }
            if (modPlayer.sirenPet)
            {
                Projectile.timeLeft = 2;
            }
            bool sleepy = sleepyTimer >= 180;
            if (!sleepy)
            {
                Projectile.frameCounter++;
                if (Projectile.frameCounter > 6)
                {
                    Projectile.frame++;
                    Projectile.frameCounter = 0;
                }
            }
            if (underwater)
            {
                if (Projectile.frame >= 8)
                {
                    Projectile.frame = 0;
                }
            }
            else
            {
                if (Projectile.frame >= 16)
                {
                    Projectile.frame = sleepy ? 16 : 8;
                }
            }
            underwater = player.IsUnderwater();
            if (underwater)
            {
                if (Projectile.frame == 16)
                    Projectile.frame = 0;
                if (sleepyTimer > 0)
                    sleepyTimer--;
                if (Projectile.localAI[0] == 0f)
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
                    Projectile.frame = 16;
                }
            }
            switch (lightLevel)
            {
                case 0:
                    Lighting.AddLight(Projectile.Center, 0f, 2f, 2.5f); //4.5
                    break;
                case 1:
                    Lighting.AddLight(Projectile.Center, 0f, 1.32f, 1.65f); //3
                    break;
                case 2:
                    Lighting.AddLight(Projectile.Center, 0f, 0.5f, 0.7f);
                    break;
            }

            float velAdjustment = 0.2f;
            float speedLimit = 5f;
            Vector2 playerVec = player.Center - Projectile.Center;
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

            if (Projectile.velocity.X < -0.25f || (player.controlLeft && !sleepy))
            {
                Projectile.direction = -1; //face left
            }
            else if (Projectile.velocity.X > 0.25f || (player.controlRight && !sleepy))
            {
                Projectile.direction = 1; //face right
            }
            Projectile.spriteDirection = Projectile.direction;

            float playerDist = playerVec.Length();
            if (playerDist > 1000f)
            {
                Projectile.position.X += playerVec.X;
                Projectile.position.Y += playerVec.Y;
            }
            if (Projectile.localAI[0] == 1f)
            {
                if (playerDist < 10f && player.velocity.Length() < speedLimit && player.velocity.Y == 0f)
                {
                    Projectile.localAI[0] = 0f;
                }
                speedLimit = 12f;
                if (playerDist < speedLimit)
                {
                    Projectile.velocity = playerVec;
                }
                else
                {
                    playerDist = speedLimit / playerDist;
                    Projectile.velocity = playerVec * playerDist;
                }
                Projectile.rotation = Projectile.velocity.X * 0.05f;
                return;
            }
            if (playerDist > 200f)
            {
                Projectile.localAI[0] = 1f;
            }
            if (playerDist < 10f)
            {
                Projectile.velocity.X = playerVec.X;
                Projectile.velocity.Y = playerVec.Y;
                Projectile.rotation = Projectile.velocity.X * 0.05f;
                if (playerDist < speedLimit)
                {
                    Projectile.position += Projectile.velocity;
                    Projectile.velocity *= 0f;
                    velAdjustment = 0f;
                }
            }
            playerDist = speedLimit / playerDist;
            playerVec *= playerDist;
            if (Projectile.velocity.X < playerVec.X)
            {
                Projectile.velocity.X += velAdjustment;
                if (Projectile.velocity.X < 0f)
                {
                    Projectile.velocity.X *= 0.99f;
                }
            }
            if (Projectile.velocity.X > playerVec.X)
            {
                Projectile.velocity.X -= velAdjustment;
                if (Projectile.velocity.X > 0f)
                {
                    Projectile.velocity.X *= 0.99f;
                }
            }
            if (Projectile.velocity.Y < playerVec.Y)
            {
                Projectile.velocity.Y += velAdjustment;
                if (Projectile.velocity.Y < 0f)
                {
                    Projectile.velocity.Y *= 0.99f;
                }
            }
            if (Projectile.velocity.Y > playerVec.Y)
            {
                Projectile.velocity.Y -= velAdjustment;
                if (Projectile.velocity.Y > 0f)
                {
                    Projectile.velocity.Y *= 0.99f;
                }
            }
            if (Projectile.velocity.X != 0f || Projectile.velocity.Y != 0f)
            {
                Projectile.rotation = Projectile.velocity.X * 0.05f;
            }
        }
    }
}
