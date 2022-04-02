using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Pets
{
    public class YharonSonPet : ModProjectile
    {
        public Player player => Main.player[projectile.owner];
        private static int xFrameAmt = 3;
        private static int yFrameAmt = 16;

        public int frameX = 0;
        public int frameY = 0;

        public int CurrentFrame 
        {
            get => frameX * yFrameAmt + frameY;
            set
            {
                frameX = value / yFrameAmt;
                frameY = value % yFrameAmt;
            }
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Son of Yharon");
            Main.projPet[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.netImportant = true;
            projectile.width = 104;
            projectile.height = 82;
            projectile.friendly = true;
            projectile.penetrate = -1;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.frameCounter <= 1)
                return false;
            Texture2D texture = Main.projectileTexture[projectile.type];
            Vector2 origin = texture.Size() / new Vector2((float)xFrameAmt, (float)yFrameAmt) * 0.5f;
            Rectangle frame = texture.Frame(xFrameAmt, yFrameAmt, frameX, frameY);
            SpriteEffects spriteEffects = projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, frame, lightColor, projectile.rotation, origin, projectile.scale, spriteEffects, 0f);
            return false;
        }

        private bool CurrentlySitting() => CurrentFrame == 0 || CurrentFrame == 3 || CurrentFrame == 4 || CurrentFrame >= 19;
        private bool CurrentlyFlying() => CurrentFrame == 1 || CurrentFrame == 2 || CurrentFrame >= 5 && CurrentFrame <= 18;

        private void SittingFrames()
        {
            projectile.frameCounter++;
            if (projectile.frameCounter % 6 == 0)
            {
                if (CurrentlyFlying())
                    CurrentFrame = 3;
                else if (CurrentFrame == 45)
                    CurrentFrame = 0;
                else if (CurrentFrame != 0)
                    CurrentFrame++;
                if (CurrentFrame == 5)
                    CurrentFrame = 0;
                // Roughly 25.97% chance per second for blinking
                if (Main.rand.NextBool(200) && CurrentFrame == 0)
                    CurrentFrame = 46;
                // Roughly 3.9% chance per second for head shaking
                if (Main.rand.NextBool(1500) && CurrentFrame == 0)
                    CurrentFrame = 19;
                if (frameX >= xFrameAmt)
                    CurrentFrame = 0;
            }
        }

        private void FlyingFrames()
        {
            projectile.frameCounter++;
            if (projectile.frameCounter % 6 == 0)
            {
                if (CurrentlySitting())
                    CurrentFrame = 1;
                else
                    CurrentFrame++;
                if (CurrentFrame == 3)
                    CurrentFrame = 5;
                if (CurrentFrame > 18)
                    CurrentFrame = 5;
                // This should never happen at this stage but just in case
                if (frameX >= xFrameAmt)
                    CurrentFrame = 0;
            }
        }

        private void PetDefaults()
        {
            if (!player.active)
            {
                projectile.active = false;
                return;
            }
            CalamityPlayer modPlayer = player.Calamity();
            if (player.dead)
            {
                modPlayer.yharonPet = false;
            }
            if (modPlayer.yharonPet)
            {
                projectile.timeLeft = 2;
            }
        }

        public override void AI()
        {
            // Insert generic pet setup
            PetDefaults();

            // Fly toward the player if they are using Rocket Boots
            if (player.rocketDelay2 > 0)
            {
                projectile.ai[0] = 1f;
            }

            float xDist = player.Center.X - projectile.Center.X;
            float yDist = player.Center.Y - projectile.Center.Y;
            Vector2 playerVector = new Vector2(xDist, yDist);
            float playerDist = playerVector.Length();

            //Teleport to player if too far
            if (playerDist > 2000f)
            {
                projectile.position.X = player.Center.X - projectile.width / 2;
                projectile.position.Y = player.Center.Y - projectile.height / 2;
                projectile.netUpdate = true;
            }
            // Otherwise, make sure we're not going crazy in the y velocity and try to fly back toward the player
            else if (playerDist > (CurrentlySitting() ? 300f : 200f))
            {
                if (yDist > 0f && projectile.velocity.Y < 0f)
                {
                    projectile.velocity.Y = 0f;
                }
                if (yDist < 0f && projectile.velocity.Y > 0f)
                {
                    projectile.velocity.Y = 0f;
                }
                projectile.ai[0] = 1f;
            }

            // Fly toward the player
            if (projectile.ai[0] != 0f)
            {
                FlyingPetAI();
            }
            // Try and sit down
            else
            {
                SitDownAttempt();
            }
            projectile.spriteDirection = projectile.direction;
            DontSitInMidair();
        }

        private void DontSitInMidair()
        {
            if (CurrentlySitting())
            {
                bool noSolidGround = true;
                for (int i = (int)projectile.BottomLeft.X / 16; i < (int)projectile.BottomRight.X / 16; i++)
                {
                    Tile tileBelow = CalamityUtils.ParanoidTileRetrieval(i, (int)(projectile.Bottom.Y / 16)); 
                    if (tileBelow.IsTileSolidGround())
                    {
                        noSolidGround = false;
                        break;
                    }
                }
                if (noSolidGround)
                    projectile.ai[0] = 1f;
            }
        }

        private void FlyingPetAI()
        {
            float passiveMvtFloat = 0.2f;
            projectile.tileCollide = false;
            float range = 200f;
            float xDist = player.Center.X - projectile.Center.X;
            float yDist = player.Center.Y - projectile.Center.Y;
            yDist += Main.rand.NextFloat(-10, 20);
            xDist += Main.rand.NextFloat(-10, 20);
            xDist += 60f * -(float)player.direction;
            yDist -= 60f;
            Vector2 playerVector = new Vector2(xDist, yDist);
            float playerDist = playerVector.Length();
            float returnSpeed = 12f;

            //If player is close enough, try and land
            if (playerDist < range && player.velocity.Y == 0f && projectile.Bottom.Y <= player.Bottom.Y && !Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
            {
                projectile.ai[0] = 0f;
                if (projectile.velocity.Y < -6f)
                {
                    projectile.velocity.Y = -6f;
                }
            }

            if (playerDist < 60f)
            {
                playerVector.X = projectile.velocity.X;
                playerVector.Y = projectile.velocity.Y;
            }
            else
            {
                playerDist = returnSpeed / playerDist;
                playerVector.X *= playerDist;
                playerVector.Y *= playerDist;
            }
            if (projectile.velocity.X < playerVector.X)
            {
                projectile.velocity.X += passiveMvtFloat;
                if (projectile.velocity.X < 0f)
                {
                    projectile.velocity.X += passiveMvtFloat * 1.5f;
                }
            }
            if (projectile.velocity.X > playerVector.X)
            {
                projectile.velocity.X -= passiveMvtFloat;
                if (projectile.velocity.X > 0f)
                {
                    projectile.velocity.X -= passiveMvtFloat * 1.5f;
                }
            }
            if (projectile.velocity.Y < playerVector.Y)
            {
                projectile.velocity.Y += passiveMvtFloat;
                if (projectile.velocity.Y < 0f)
                {
                    projectile.velocity.Y += passiveMvtFloat * 1.5f;
                }
            }
            if (projectile.velocity.Y > playerVector.Y)
            {
                projectile.velocity.Y -= passiveMvtFloat;
                if (projectile.velocity.Y > 0f)
                {
                    projectile.velocity.Y -= passiveMvtFloat * 1.5f;
                }
            }
            if (projectile.velocity.X >= 0.5f)
            {
                projectile.direction = -1;
            }
            else if (projectile.velocity.X < -0.5f)
            {
                projectile.direction = 1;
            }
            else if (player.Center.X < projectile.Center.X)
            {
                projectile.direction = 1;
            }
            else if (player.Center.X > projectile.Center.X)
            {
                projectile.direction = -1;
            }

            //Tilting
            projectile.rotation = projectile.velocity.X * 0.075f;

            // Frames while flying
            FlyingFrames();
        }

        private void SitDownAttempt()
        {
            projectile.tileCollide = true;

            bool tooRight = false;
            bool tooLeft = false;
            bool abovePlayer = false;
            float xAdjust = 0.08f;
            float xVelMax = 6.5f;

            if (player.Center.X < projectile.Center.X - 85f)
            {
                tooRight = true;
            }
            else if (player.Center.X > projectile.Center.X + 85f)
            {
                tooLeft = true;
            }

            if (tooRight)
            {
                if (projectile.velocity.X > -3.5f)
                {
                    projectile.velocity.X -= xAdjust;
                }
                else
                {
                    projectile.velocity.X -= xAdjust * 0.25f;
                }
            }
            else if (tooLeft)
            {
                if (projectile.velocity.X < 3.5f)
                {
                    projectile.velocity.X += xAdjust;
                }
                else
                {
                    projectile.velocity.X += xAdjust * 0.25f;
                }
            }
            else
            {
                projectile.velocity.X *= 0.9f;
                if (projectile.velocity.X >= 0f - xAdjust && projectile.velocity.X <= xAdjust)
                {
                    projectile.velocity.X = 0f;
                }
            }
            projectile.velocity.X *= 0.95f;
            if (projectile.velocity.X > -0.1f && projectile.velocity.X < 0.1f)
            {
                projectile.velocity.X = 0f;
            }

            if (player.Bottom.Y - 8f > projectile.Bottom.Y)
            {
                abovePlayer = true;
            }
            Collision.StepUp(ref projectile.position, ref projectile.velocity, projectile.width, projectile.height, ref projectile.stepSpeed, ref projectile.gfxOffY);
            if (projectile.velocity.Y == 0f)
            {
                if (!abovePlayer && projectile.velocity.X != 0f)
                {
                    int i = (int)projectile.Center.X / 16;
                    int j = (int)projectile.Center.Y / 16 + 1;
                    if (tooRight)
                    {
                        i--;
                    }
                    if (tooLeft)
                    {
                        i++;
                    }
                    WorldGen.SolidTile(i, j);
                }
            }

            // Cap X velocity when attempting to sit
            if (projectile.velocity.X > xVelMax)
            {
                projectile.velocity.X = xVelMax;
            }
            if (projectile.velocity.X < -xVelMax)
            {
                projectile.velocity.X = -xVelMax;
            }

            // Handle frames and sitting direction/rotation
            bool sitting = projectile.position.X == projectile.oldPosition.X;
            if (projectile.velocity.Y == 0f && sitting)
            {
                if (player.Center.X < projectile.Center.X)
                {
                    projectile.direction = 1;
                }
                else if (player.Center.X > projectile.Center.X)
                {
                    projectile.direction = -1;
                }
                projectile.rotation = 0f;
                // Frames while sitting
                SittingFrames();
            }
            else
            {
                if (projectile.velocity.X >= 0.5f)
                {
                    projectile.direction = -1;
                }
                else if (projectile.velocity.X < -0.5f)
                {
                    projectile.direction = 1;
                }
                else if (player.Center.X < projectile.Center.X)
                {
                    projectile.direction = 1;
                }
                else if (player.Center.X > projectile.Center.X)
                {
                    projectile.direction = -1;
                }

                projectile.rotation = projectile.velocity.X * 0.075f;

                // Frames while flying
                FlyingFrames();
            }
        }
    }
}
