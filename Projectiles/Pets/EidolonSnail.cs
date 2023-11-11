using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using System;

namespace CalamityMod.Projectiles.Pets
{
    public class EidolonSnail : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Pets";
        private int playerStill = 0;
        private bool fly = false;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 8;
            Main.projPet[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.width = 54;
            Projectile.height = 38;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft *= 5;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Pets/EidolonSnail").Value;

            int height = texture.Height / Main.projFrames[Projectile.type];
            int frameHeight = height * Projectile.frame;
            SpriteEffects spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY + 2), 
            new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, frameHeight, texture.Width, height)), lightColor, 
            Projectile.rotation, new Vector2(texture.Width / 2f, height / 2f), Projectile.scale, spriteEffects, 0);

            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Pets/EidolonSnailGlow").Value;

            int height = texture.Height / Main.projFrames[Projectile.type];
            int frameHeight = height * Projectile.frame;
            SpriteEffects spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY + 2), 
            new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, frameHeight, texture.Width, height)), Color.White, 
            Projectile.rotation, new Vector2(texture.Width / 2f, height / 2f), Projectile.scale, spriteEffects, 0);
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            Player player = Main.player[Projectile.owner];
            Vector2 projCenter = Projectile.Center;
            Vector2 playerDirection = player.Center - projCenter;
            float playerDistance = playerDirection.Length();
            fallThrough = playerDistance > 200f;
            return true;
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
                modPlayer.eidolonSnailPet = false;
            }
            if (modPlayer.eidolonSnailPet)
            {
                Projectile.timeLeft = 2;
            }

            Vector2 projPos = Projectile.position;
            if (!fly)
            {
                Projectile.rotation = 0;
                Vector2 projCenter = Projectile.Center;
                Vector2 playerDirection = player.Center - projCenter;
                float playerDistance = playerDirection.Length();
                if (Projectile.velocity.Y == 0 && ((HoleBelow() && playerDistance > 150f) || (playerDistance > 150f && Projectile.position.X == Projectile.oldPosition.X)))
                {
                    Projectile.velocity.Y = -8f;
                }
                Projectile.velocity.Y += 0.35f;
                if (Projectile.velocity.Y > 15f)
                {
                    Projectile.velocity.Y = 15f;
                }
                if (playerDistance > 520f)
                {
                    fly = true;
                    Projectile.velocity.X = 0f;
                    Projectile.velocity.Y = 0f;
                }
                if (playerDistance > 100f)
                {
                    if (player.position.X - Projectile.position.X > 0f)
                    {
                        Projectile.velocity.X += 0.12f;
                        if (Projectile.velocity.X > 6f)
                        {
                            Projectile.velocity.X = 6f;
                        }
                    }
                    else
                    {
                        Projectile.velocity.X -= 0.12f;
                        if (Projectile.velocity.X < -6f)
                        {
                            Projectile.velocity.X = -6f;
                        }
                    }
                }
                if (playerDistance < 100f)
                {
                    if (Projectile.velocity.X != 0f)
                    {
                        if (Projectile.velocity.X > 0.8f)
                        {
                            Projectile.velocity.X -= 0.25f;
                        }
                        else if (Projectile.velocity.X < -0.8f)
                        {
                            Projectile.velocity.X += 0.25f;
                        }
                        else if (Projectile.velocity.X < 0.8f && Projectile.velocity.X > -0.8f)
                        {
                            Projectile.velocity.X = 0f;
                        }
                    }
                }
                if (playerDistance < 70f)
                {
                    Projectile.velocity.X *= 0.5f;
                }

                //set frames when idle
                if (Projectile.position.X == Projectile.oldPosition.X && Projectile.position.Y == Projectile.oldPosition.Y && Projectile.velocity.X == 0)
                {
                    Projectile.frame = 0;
                    Projectile.frameCounter = 0;
                }
                //falling frame
                else if (Projectile.velocity.Y > 0.3f && Projectile.position.Y != Projectile.oldPosition.Y)
                {
                    Projectile.frame = 4;
                    Projectile.frameCounter = 0;
                }
                else if (Projectile.velocity.X != 0)
                {
                    //moving animation
                    Projectile.frameCounter++;
                    if (Projectile.frameCounter > 2)
                    {
                        Projectile.frame++;
                        Projectile.frameCounter = 0;
                    }
                    if (Projectile.frame > 4)
                    {
                        Projectile.frame = 1;
                    }
                }

                if (Projectile.velocity.X > 0.8f)
                {
                    Projectile.spriteDirection = 1;
                }
                else if (Projectile.velocity.X < -0.8f)
                {
                    Projectile.spriteDirection = -1;
                }
            }
            else if (fly)
            {
                float flySpeed = 0.5f;
                Projectile.tileCollide = false;
                Vector2 flyDirection = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
                float horiPos = Main.player[Projectile.owner].position.X + (float)(Main.player[Projectile.owner].width / 2) - flyDirection.X;
                float vertiPos = Main.player[Projectile.owner].position.Y + (float)(Main.player[Projectile.owner].height / 2) - flyDirection.Y;
                vertiPos += (float)Main.rand.Next(-10, 21);
                horiPos += (float)Main.rand.Next(-10, 21);
                horiPos += (float)(60 * -(float)player.direction);
                vertiPos -= 60f;
                float playerDistance = (float)Math.Sqrt((double)(horiPos * horiPos + vertiPos * vertiPos));
                if (playerDistance > 1200f)
                {
                    Projectile.position.X = player.Center.X - (float)(Projectile.width / 2);
                    Projectile.position.Y = player.Center.Y - (float)(Projectile.height / 2);
                    Projectile.netUpdate = true;
                }
                if (playerDistance < 100f)
                {
                    flySpeed = 0.5f;
                    if (player.velocity.Y == 0f)
                    {
                        ++playerStill;
                    }
                    else
                    {
                        playerStill = 0;
                    }
                    if (playerStill > 10 && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
                    {
                        fly = false;
                        Projectile.velocity *= 0.2f;
                        Projectile.tileCollide = true;
                    }
                }
                if (playerDistance < 50f)
                {
                    if (Math.Abs(Projectile.velocity.X) > 2f || Math.Abs(Projectile.velocity.Y) > 2f)
                    {
                        Projectile.velocity *= 0.90f;
                    }
                    flySpeed = 0.02f;
                }
                else
                {
                    if (playerDistance < 100f)
                    {
                        flySpeed = 0.35f;
                    }
                    if (playerDistance > 300f)
                    {
                        flySpeed = 1f;
                    }
                    playerDistance = 18f / playerDistance;
                    horiPos *= playerDistance;
                    vertiPos *= playerDistance;
                }
                if (Projectile.velocity.X <= horiPos)
                {
                    Projectile.velocity.X = Projectile.velocity.X + flySpeed;
                    if (flySpeed > 0.05f && Projectile.velocity.X < 0f)
                    {
                        Projectile.velocity.X = Projectile.velocity.X + flySpeed;
                    }
                }
                if (Projectile.velocity.X > horiPos)
                {
                    Projectile.velocity.X = Projectile.velocity.X - flySpeed;
                    if (flySpeed > 0.05f && Projectile.velocity.X > 0f)
                    {
                        Projectile.velocity.X = Projectile.velocity.X - flySpeed;
                    }
                }
                if (Projectile.velocity.Y <= vertiPos)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y + flySpeed;
                    if (flySpeed > 0.05f && Projectile.velocity.Y < 0f)
                    {
                        Projectile.velocity.Y = Projectile.velocity.Y + flySpeed * 2f;
                    }
                }
                if (Projectile.velocity.Y > vertiPos)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y - flySpeed;
                    if (flySpeed > 0.05f && Projectile.velocity.Y > 0f)
                    {
                        Projectile.velocity.Y = Projectile.velocity.Y - flySpeed * 2f;
                    }
                }

                Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * (float)Projectile.direction;

                if (Projectile.Center.X < Main.player[Projectile.owner].Center.X)
                {
                    Projectile.spriteDirection = 1;
                }
                else if (Projectile.Center.X > Main.player[Projectile.owner].Center.X)
                {
                    Projectile.spriteDirection = -1;
                }

                //funny flying animation 
                Projectile.frameCounter++;
                if (Projectile.frameCounter > 3)
                {
                    Projectile.frame++;
                    Projectile.frameCounter = 0;
                }
                if (Projectile.frame > 7)
                {
                    Projectile.frame = 5;
                }
            }
        }

        private bool HoleBelow()
        {
            int tileWidth = 4;
            int tileX = (int)(Projectile.Center.X / 16f) - tileWidth;
            if (Projectile.velocity.X > 0)
            {
                tileX += tileWidth;
            }
            int tileY = (int)((Projectile.position.Y + Projectile.height) / 16f);
            for (int y = tileY; y < tileY + 2; y++)
            {
                for (int x = tileX; x < tileX + tileWidth; x++)
                {
                    if (Main.tile[x, y].HasTile && (Main.tile[x - 1, y].HasTile || Main.tile[x + 1, y].HasTile))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}