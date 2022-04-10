using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Pets
{
    public class YharonSonPet : ModProjectile
    {
        public Player player => Main.player[Projectile.owner];
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
            Main.projPet[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.width = 104;
            Projectile.height = 82;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.frameCounter <= 1)
                return false;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 origin = texture.Size() / new Vector2((float)xFrameAmt, (float)yFrameAmt) * 0.5f;
            Rectangle frame = texture.Frame(xFrameAmt, yFrameAmt, frameX, frameY);
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, lightColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            return false;
        }

        private bool CurrentlySitting() => CurrentFrame == 0 || CurrentFrame == 3 || CurrentFrame == 4 || CurrentFrame >= 19;
        private bool CurrentlyFlying() => CurrentFrame == 1 || CurrentFrame == 2 || CurrentFrame >= 5 && CurrentFrame <= 18;

        private void SittingFrames()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter % 6 == 0)
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
            Projectile.frameCounter++;
            if (Projectile.frameCounter % 6 == 0)
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
                Projectile.active = false;
                return;
            }
            CalamityPlayer modPlayer = player.Calamity();
            if (player.dead)
            {
                modPlayer.yharonPet = false;
            }
            if (modPlayer.yharonPet)
            {
                Projectile.timeLeft = 2;
            }
        }

        public override void AI()
        {
            // Insert generic pet setup
            PetDefaults();

            // Fly toward the player if they are using Rocket Boots
            if (player.rocketDelay2 > 0)
            {
                Projectile.ai[0] = 1f;
            }

            float xDist = player.Center.X - Projectile.Center.X;
            float yDist = player.Center.Y - Projectile.Center.Y;
            Vector2 playerVector = new Vector2(xDist, yDist);
            float playerDist = playerVector.Length();

            //Teleport to player if too far
            if (playerDist > 2000f)
            {
                Projectile.position.X = player.Center.X - Projectile.width / 2;
                Projectile.position.Y = player.Center.Y - Projectile.height / 2;
                Projectile.netUpdate = true;
            }
            // Otherwise, make sure we're not going crazy in the y velocity and try to fly back toward the player
            else if (playerDist > (CurrentlySitting() ? 300f : 200f))
            {
                if (yDist > 0f && Projectile.velocity.Y < 0f)
                {
                    Projectile.velocity.Y = 0f;
                }
                if (yDist < 0f && Projectile.velocity.Y > 0f)
                {
                    Projectile.velocity.Y = 0f;
                }
                Projectile.ai[0] = 1f;
            }

            // Fly toward the player
            if (Projectile.ai[0] != 0f)
            {
                FlyingPetAI();
            }
            // Try and sit down
            else
            {
                SitDownAttempt();
            }
            Projectile.spriteDirection = Projectile.direction;
            DontSitInMidair();
        }

        private void DontSitInMidair()
        {
            if (CurrentlySitting())
            {
                bool noSolidGround = true;
                for (int i = (int)Projectile.BottomLeft.X / 16; i < (int)Projectile.BottomRight.X / 16; i++)
                {
                    Tile tileBelow = CalamityUtils.ParanoidTileRetrieval(i, (int)(Projectile.Bottom.Y / 16));
                    if (tileBelow.IsTileSolidGround())
                    {
                        noSolidGround = false;
                        break;
                    }
                }
                if (noSolidGround)
                    Projectile.ai[0] = 1f;
            }
        }

        private void FlyingPetAI()
        {
            float passiveMvtFloat = 0.2f;
            Projectile.tileCollide = false;
            float range = 200f;
            float xDist = player.Center.X - Projectile.Center.X;
            float yDist = player.Center.Y - Projectile.Center.Y;
            yDist += Main.rand.NextFloat(-10, 20);
            xDist += Main.rand.NextFloat(-10, 20);
            xDist += 60f * -(float)player.direction;
            yDist -= 60f;
            Vector2 playerVector = new Vector2(xDist, yDist);
            float playerDist = playerVector.Length();
            float returnSpeed = 12f;

            //If player is close enough, try and land
            if (playerDist < range && player.velocity.Y == 0f && Projectile.Bottom.Y <= player.Bottom.Y && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
            {
                Projectile.ai[0] = 0f;
                if (Projectile.velocity.Y < -6f)
                {
                    Projectile.velocity.Y = -6f;
                }
            }

            if (playerDist < 60f)
            {
                playerVector.X = Projectile.velocity.X;
                playerVector.Y = Projectile.velocity.Y;
            }
            else
            {
                playerDist = returnSpeed / playerDist;
                playerVector.X *= playerDist;
                playerVector.Y *= playerDist;
            }
            if (Projectile.velocity.X < playerVector.X)
            {
                Projectile.velocity.X += passiveMvtFloat;
                if (Projectile.velocity.X < 0f)
                {
                    Projectile.velocity.X += passiveMvtFloat * 1.5f;
                }
            }
            if (Projectile.velocity.X > playerVector.X)
            {
                Projectile.velocity.X -= passiveMvtFloat;
                if (Projectile.velocity.X > 0f)
                {
                    Projectile.velocity.X -= passiveMvtFloat * 1.5f;
                }
            }
            if (Projectile.velocity.Y < playerVector.Y)
            {
                Projectile.velocity.Y += passiveMvtFloat;
                if (Projectile.velocity.Y < 0f)
                {
                    Projectile.velocity.Y += passiveMvtFloat * 1.5f;
                }
            }
            if (Projectile.velocity.Y > playerVector.Y)
            {
                Projectile.velocity.Y -= passiveMvtFloat;
                if (Projectile.velocity.Y > 0f)
                {
                    Projectile.velocity.Y -= passiveMvtFloat * 1.5f;
                }
            }
            if (Projectile.velocity.X >= 0.5f)
            {
                Projectile.direction = -1;
            }
            else if (Projectile.velocity.X < -0.5f)
            {
                Projectile.direction = 1;
            }
            else if (player.Center.X < Projectile.Center.X)
            {
                Projectile.direction = 1;
            }
            else if (player.Center.X > Projectile.Center.X)
            {
                Projectile.direction = -1;
            }

            //Tilting
            Projectile.rotation = Projectile.velocity.X * 0.075f;

            // Frames while flying
            FlyingFrames();
        }

        private void SitDownAttempt()
        {
            Projectile.tileCollide = true;

            bool tooRight = false;
            bool tooLeft = false;
            bool abovePlayer = false;
            float xAdjust = 0.08f;
            float xVelMax = 6.5f;

            if (player.Center.X < Projectile.Center.X - 85f)
            {
                tooRight = true;
            }
            else if (player.Center.X > Projectile.Center.X + 85f)
            {
                tooLeft = true;
            }

            if (tooRight)
            {
                if (Projectile.velocity.X > -3.5f)
                {
                    Projectile.velocity.X -= xAdjust;
                }
                else
                {
                    Projectile.velocity.X -= xAdjust * 0.25f;
                }
            }
            else if (tooLeft)
            {
                if (Projectile.velocity.X < 3.5f)
                {
                    Projectile.velocity.X += xAdjust;
                }
                else
                {
                    Projectile.velocity.X += xAdjust * 0.25f;
                }
            }
            else
            {
                Projectile.velocity.X *= 0.9f;
                if (Projectile.velocity.X >= 0f - xAdjust && Projectile.velocity.X <= xAdjust)
                {
                    Projectile.velocity.X = 0f;
                }
            }
            Projectile.velocity.X *= 0.95f;
            if (Projectile.velocity.X > -0.1f && Projectile.velocity.X < 0.1f)
            {
                Projectile.velocity.X = 0f;
            }

            if (player.Bottom.Y - 8f > Projectile.Bottom.Y)
            {
                abovePlayer = true;
            }
            Collision.StepUp(ref Projectile.position, ref Projectile.velocity, Projectile.width, Projectile.height, ref Projectile.stepSpeed, ref Projectile.gfxOffY);
            if (Projectile.velocity.Y == 0f)
            {
                if (!abovePlayer && Projectile.velocity.X != 0f)
                {
                    int i = (int)Projectile.Center.X / 16;
                    int j = (int)Projectile.Center.Y / 16 + 1;
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
            if (Projectile.velocity.X > xVelMax)
            {
                Projectile.velocity.X = xVelMax;
            }
            if (Projectile.velocity.X < -xVelMax)
            {
                Projectile.velocity.X = -xVelMax;
            }

            // Handle frames and sitting direction/rotation
            bool sitting = Projectile.position.X == Projectile.oldPosition.X;
            if (Projectile.velocity.Y == 0f && sitting)
            {
                if (player.Center.X < Projectile.Center.X)
                {
                    Projectile.direction = 1;
                }
                else if (player.Center.X > Projectile.Center.X)
                {
                    Projectile.direction = -1;
                }
                Projectile.rotation = 0f;
                // Frames while sitting
                SittingFrames();
            }
            else
            {
                if (Projectile.velocity.X >= 0.5f)
                {
                    Projectile.direction = -1;
                }
                else if (Projectile.velocity.X < -0.5f)
                {
                    Projectile.direction = 1;
                }
                else if (player.Center.X < Projectile.Center.X)
                {
                    Projectile.direction = 1;
                }
                else if (player.Center.X > Projectile.Center.X)
                {
                    Projectile.direction = -1;
                }

                Projectile.rotation = Projectile.velocity.X * 0.075f;

                // Frames while flying
                FlyingFrames();
            }
        }
    }
}
