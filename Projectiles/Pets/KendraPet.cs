using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Pets
{
    public class KendraPet : ModProjectile
    {
        private int chosenIdle = 0;
        private int idleTimer = 0;
        private int idleBarkTimer = 0;
        private int playerStill = 0;
        private bool fly = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Kendra");
            Main.projFrames[Projectile.type] = 31;
            Main.projPet[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.width = 46;
            Projectile.height = 46;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft *= 5;
            Projectile.tileCollide = true;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            Player player = Main.player[Projectile.owner];
            Vector2 center2 = Projectile.Center;
            Vector2 vector48 = player.Center - center2;
            float playerDistance = vector48.Length();
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
                modPlayer.kendra = false;
            }
            if (modPlayer.kendra)
            {
                Projectile.timeLeft = 2;
            }
            Vector2 vector46 = Projectile.position;
            if (!fly)
            {
                Projectile.rotation = 0;
                Vector2 center2 = Projectile.Center;
                Vector2 vector48 = player.Center - center2;
                float playerDistance = vector48.Length();
                if (Projectile.velocity.Y == 0f && (HoleBelow() || (playerDistance > 110f && Projectile.position.X == Projectile.oldPosition.X)))
                {
                    Projectile.velocity.Y = -5f;
                }
                Projectile.velocity.Y += 0.2f;
                if (Projectile.velocity.Y > 7f)
                {
                    Projectile.velocity.Y = 7f;
                }
                if (playerDistance > 600f)
                {
                    fly = true;
                    Projectile.velocity.X = 0f;
                    Projectile.velocity.Y = 0f;
                    Projectile.tileCollide = false;
                }
                if (playerDistance > 100f)
                {
                    if (player.position.X - Projectile.position.X > 0f)
                    {
                        Projectile.velocity.X += 0.1f;
                        if (Projectile.velocity.X > 7f)
                        {
                            Projectile.velocity.X = 7f;
                        }
                    }
                    else
                    {
                        Projectile.velocity.X -= 0.1f;
                        if (Projectile.velocity.X < -7f)
                        {
                            Projectile.velocity.X = -7f;
                        }
                    }
                }
                if (playerDistance < 100f)
                {
                    if (Projectile.velocity.X != 0f)
                    {
                        if (Projectile.velocity.X > 0.5f)
                        {
                            Projectile.velocity.X -= 0.15f;
                        }
                        else if (Projectile.velocity.X < -0.5f)
                        {
                            Projectile.velocity.X += 0.15f;
                        }
                        else if (Projectile.velocity.X < 0.5f && Projectile.velocity.X > -0.5f)
                        {
                            Projectile.velocity.X = 0f;
                        }
                    }
                }
                if (Projectile.position.X == Projectile.oldPosition.X && Projectile.position.Y == Projectile.oldPosition.Y && Projectile.velocity.X == 0)
                {
                    Projectile.frameCounter++;
                    switch (chosenIdle)
                    {
                        case 1:
                            if (idleTimer == 0)
                            {
                                Projectile.frame = 0;
                            }
                            idleTimer++;
                            if (Projectile.frameCounter > 6)
                            {
                                Projectile.frame++;
                                Projectile.frameCounter = 0;
                            }
                            if (Projectile.frame > 4)
                            {
                                chosenIdle = 0;
                            }
                            if (Projectile.frame < 1)
                            {
                                Projectile.frame = 1;
                            }
                            break;
                        case 2:
                            if (idleTimer == 0)
                            {
                                Projectile.frame = 0;
                            }
                            idleTimer++;
                            if (Projectile.frameCounter > 6)
                            {
                                Projectile.frame++;
                                Projectile.frameCounter = 0;
                            }
                            if (Projectile.frame > 13)
                            {
                                chosenIdle = 0;
                            }
                            if (Projectile.frame < 5)
                            {
                                Projectile.frame = 5;
                            }
                            break;
                        case 3:
                            if (idleTimer == 0)
                            {
                                Projectile.frame = 0;
                            }
                            idleTimer++;
                            if (Projectile.frameCounter > 6)
                            {
                                Projectile.frame++;
                                Projectile.frameCounter = 0;
                            }
                            if (Projectile.frame > 24)
                            {
                                chosenIdle = 0;
                            }
                            if (Projectile.frame < 19)
                            {
                                Projectile.frame = 19;
                            }
                            break;
                    }
                    if (chosenIdle == 0)
                    {
                        Projectile.frame = 0;
                        Projectile.frameCounter = 6;
                        idleTimer++;
                        if (idleTimer > 360)
                        {
                            chosenIdle = Main.rand.Next(1, 3);
                            idleTimer = 0;
                        }
                        idleBarkTimer++;
                        if (idleBarkTimer > 1080 && Main.rand.NextBool(2))
                        {
                            SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Item/KendraBark"), (int)Projectile.position.X, (int)Projectile.position.Y);
                            chosenIdle = 3;
                            idleBarkTimer = 0;
                            idleTimer = 0;
                        }
                    }
                }
                else if (Projectile.velocity.Y > 0.3f && Projectile.position.Y != Projectile.oldPosition.Y)
                {
                    Projectile.frame = 16;
                    Projectile.frameCounter = 0;
                }
                else
                {
                    Projectile.frameCounter++;
                    if (Projectile.frameCounter > 6)
                    {
                        Projectile.frame++;
                        Projectile.frameCounter = 0;
                    }
                    if (Projectile.frame > 18)
                    {
                        Projectile.frame = 14;
                    }
                    if (Projectile.frame < 14)
                    {
                        Projectile.frame = 14;
                    }
                }
            }
            else if (fly)
            {
                float num16 = 0.3f;
                Projectile.tileCollide = false;
                Vector2 vector3 = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
                float horiPos = Main.player[Projectile.owner].position.X + (float)(Main.player[Projectile.owner].width / 2) - vector3.X;
                float vertiPos = Main.player[Projectile.owner].position.Y + (float)(Main.player[Projectile.owner].height / 2) - vector3.Y;
                vertiPos += (float)Main.rand.Next(-10, 21);
                horiPos += (float)Main.rand.Next(-10, 21);
                horiPos += (float)(60 * -(float)Main.player[Projectile.owner].direction);
                vertiPos -= 60f;
                float playerDistance = (float)Math.Sqrt((double)(horiPos * horiPos + vertiPos * vertiPos));
                float num21 = 18.5f;
                float num27 = (float)Math.Sqrt((double)(horiPos * horiPos + vertiPos * vertiPos));
                if (playerDistance > 2000f)
                {
                    Projectile.position.X = Main.player[Projectile.owner].Center.X - (float)(Projectile.width / 2);
                    Projectile.position.Y = Main.player[Projectile.owner].Center.Y - (float)(Projectile.height / 2);
                    Projectile.netUpdate = true;
                }
                if (playerDistance < 100f)
                {
                    num16 = 0.1f;
                    if (player.velocity.Y == 0f)
                    {
                        playerStill++;
                    }
                    else
                    {
                        playerStill = 0;
                    }
                    if (playerStill > 60 && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
                    {
                        fly = false;
                        Projectile.tileCollide = true;
                    }
                }
                if (playerDistance < 50f)
                {
                    if (Math.Abs(Projectile.velocity.X) > 2f || Math.Abs(Projectile.velocity.Y) > 2f)
                    {
                        Projectile.velocity *= 0.9f;
                    }
                    num16 = 0.01f;
                }
                else
                {
                    if (playerDistance < 100f)
                    {
                        num16 = 0.1f;
                    }
                    if (playerDistance > 300f)
                    {
                        num16 = 1f;
                    }
                    playerDistance = num21 / playerDistance;
                    horiPos *= playerDistance;
                    vertiPos *= playerDistance;
                }
                if (Projectile.velocity.X <= horiPos)
                {
                    Projectile.velocity.X = Projectile.velocity.X + num16;
                    if (num16 > 0.05f && Projectile.velocity.X < 0f)
                    {
                        Projectile.velocity.X = Projectile.velocity.X + num16;
                    }
                }
                if (Projectile.velocity.X > horiPos)
                {
                    Projectile.velocity.X = Projectile.velocity.X - num16;
                    if (num16 > 0.05f && Projectile.velocity.X > 0f)
                    {
                        Projectile.velocity.X = Projectile.velocity.X - num16;
                    }
                }
                if (Projectile.velocity.Y <= vertiPos)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y + num16;
                    if (num16 > 0.05f && Projectile.velocity.Y < 0f)
                    {
                        Projectile.velocity.Y = Projectile.velocity.Y + num16 * 2f;
                    }
                }
                if (Projectile.velocity.Y > vertiPos)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y - num16;
                    if (num16 > 0.05f && Projectile.velocity.Y > 0f)
                    {
                        Projectile.velocity.Y = Projectile.velocity.Y - num16 * 2f;
                    }
                }
                Projectile.rotation = Projectile.velocity.X * 0.015f;
                Projectile.frameCounter++;
                if (Projectile.frameCounter > 6)
                {
                    Projectile.frame++;
                    Projectile.frameCounter = 0;
                }
                if (Projectile.frame > 30)
                {
                    Projectile.frame = 25;
                }
                if (Projectile.frame < 25)
                {
                    Projectile.frame = 25;
                }
            }
            if (Projectile.velocity.X > 0.25f)
            {
                Projectile.spriteDirection = -1;
            }
            else if (Projectile.velocity.X < -0.25f)
            {
                Projectile.spriteDirection = 1;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Texture2D texture2D13 = ModContent.Request<Texture2D>(Texture).Value;
            int num214 = ModContent.Request<Texture2D>(Texture).Value.Height / Main.projFrames[Projectile.type];
            int y6 = num214 * Projectile.frame;
            Main.spriteBatch.Draw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture2D13.Width, num214)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2((float)texture2D13.Width / 2f, (float)num214 / 2f), Projectile.scale, SpriteEffects.None, 0);
            return false;
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
