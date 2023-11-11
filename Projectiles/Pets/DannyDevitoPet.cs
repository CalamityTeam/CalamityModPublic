using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Pets
{
    public class DannyDevitoPet : ModProjectile, ILocalizedModType
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
            Projectile.width = 42;
            Projectile.height = 42;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft *= 5;
            Projectile.tileCollide = true;
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
                modPlayer.trashMan = false;
            }
            if (modPlayer.trashMan)
            {
                Projectile.timeLeft = 2;
            }
            Vector2 vector46 = Projectile.position;
            if (!fly)
            {
                Projectile.rotation = 0;
                Vector2 projCenter = Projectile.Center;
                Vector2 playerDirection = player.Center - projCenter;
                float playerDistance = playerDirection.Length();
                if (Projectile.velocity.Y == 0 && (HoleBelow() || (playerDistance > 110f && Projectile.position.X == Projectile.oldPosition.X)))
                {
                    Projectile.velocity.Y = -5f;
                }
                Projectile.velocity.Y += 0.20f;
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
                        Projectile.velocity.X += 0.10f;
                        if (Projectile.velocity.X > 7f)
                        {
                            Projectile.velocity.X = 7f;
                        }
                    }
                    else
                    {
                        Projectile.velocity.X -= 0.10f;
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
                    Projectile.frame = 0;
                }
                else if (Projectile.velocity.Y > 0.3f && Projectile.position.Y != Projectile.oldPosition.Y)
                {
                    Projectile.frame = 1;
                    Projectile.frameCounter = 0;
                }
                else
                {
                    Projectile.frameCounter++;
                    if (Projectile.frameCounter > 5)
                    {
                        Projectile.frame++;
                        Projectile.frameCounter = 0;
                    }
                    if (Projectile.frame > 6)
                    {
                        Projectile.frame = 2;
                    }
                    if (Projectile.frame < 2)
                    {
                        Projectile.frame = 2;
                    }
                }
            }
            else if (fly)
            {
                float flySpeed = 0.3f;
                Projectile.tileCollide = false;
                Vector2 flyDirection = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
                float horiPos = Main.player[Projectile.owner].position.X + (float)(Main.player[Projectile.owner].width / 2) - flyDirection.X;
                float vertiPos = Main.player[Projectile.owner].position.Y + (float)(Main.player[Projectile.owner].height / 2) - flyDirection.Y;
                vertiPos += (float)Main.rand.Next(-10, 21);
                horiPos += (float)Main.rand.Next(-10, 21);
                horiPos += (float)(60 * -(float)Main.player[Projectile.owner].direction);
                vertiPos -= 60f;
                float playerDistance = (float)Math.Sqrt((double)(horiPos * horiPos + vertiPos * vertiPos));
                if (playerDistance > 1200f)
                {
                    Projectile.position.X = Main.player[Projectile.owner].Center.X - (float)(Projectile.width / 2);
                    Projectile.position.Y = Main.player[Projectile.owner].Center.Y - (float)(Projectile.height / 2);
                    Projectile.netUpdate = true;
                }
                if (playerDistance < 100f)
                {
                    flySpeed = 0.1f;
                    if (player.velocity.Y == 0f)
                    {
                        ++playerStill;
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
                        Projectile.velocity *= 0.90f;
                    }
                    flySpeed = 0.01f;
                }
                else
                {
                    if (playerDistance < 100f)
                    {
                        flySpeed = 0.1f;
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
                Projectile.rotation = Projectile.velocity.X * 0.03f;
                Projectile.frame = 7;
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

        /*public override void PostDraw(Color lightColor)
        {
            if (!Main.gamePaused || Main.gameMenu)
            {
                Vector2 projDirect = projectile.position - Main.screenPosition;
                if ((float)Main.mouseX > projDirect.X && (float)Main.mouseX < projDirect.X + (float)projectile.width && (float)Main.mouseY > projDirect.Y && (float)Main.mouseY < projDirect.Y + (float)projectile.height)
                {
                    int playerTileX = (int)(Main.player[projectile.owner].Center.X / 16f);
                    int playerTileY = (int)(Main.player[projectile.owner].Center.Y / 16f);
                    int projTileX = (int)projectile.Center.X / 16;
                    int projTileY = (int)projectile.Center.Y / 16;
                    int lastTileRangeX = Main.player[projectile.owner].lastTileRangeX;
                    int lastTileRangeY = Main.player[projectile.owner].lastTileRangeY;
                    if (playerTileX >= projTileX - lastTileRangeX && playerTileX <= projTileX + lastTileRangeX + 1 && playerTileY >= projTileY - lastTileRangeY && playerTileY <= projTileY + lastTileRangeY + 1)
                    {
                        Main.player[projectile.owner].noThrow = 2;
                        Main.player[projectile.owner].cursorItemIconEnabled = true;
                        Main.player[projectile.owner].cursorItemIconID = 3213;
                        if (PlayerInput.UsingGamepad)
                        {
                            Main.player[projectile.owner].GamepadEnableGrappleCooldown();
                        }
                        if (Main.mouseRight && Main.mouseRightRelease && Player.StopMoneyTroughFromWorking == 0)
                        {
                            Main.mouseRightRelease = false;
                            if (Main.player[projectile.owner].chest == -2)
                            {
                                Main.PlaySound(SoundID.Item11, -1, -1);
                                Main.player[projectile.owner].chest = -1;
                                Recipe.FindRecipes();
                                return;
                            }
                            Main.player[projectile.owner].GetCalamityPlayer().trashManChest = projectile.type;
                            Main.player[projectile.owner].chest = -2;
                            Main.player[projectile.owner].chestX = (int)(projectile.Center.X / 16f);
                            Main.player[projectile.owner].chestY = (int)(projectile.Center.Y / 16f);
                            Main.player[projectile.owner].talkNPC = -1;
                            Main.npcShop = 0;
                            Main.playerInventory = true;
                            Main.PlaySound(SoundID.Item10, -1, -1);
                            Recipe.FindRecipes();
                        }
                    }
                }
            }
        }*/

        private bool HoleBelow() //pretty much the same as the one used in mantis
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
                    if (Main.tile[x, y].HasTile)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
