using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using System;

namespace CalamityMod.Projectiles.Pets
{
    public class SeaSlug : ModProjectile
    {
        private int playerStill = 0;
        private bool fly = false;
        private int idleTimer = 0;

        private Form SeaSlugColor = Form.Normal;
        private enum Form
        {
            Normal,
            Sulphur,
            Abyss,
			Sunken,
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sea Slug");
            Main.projFrames[Projectile.type] = 6;
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

        private void UpdateColor(Player player)
        {
            if (player.InSulphur())
            {
                SeaSlugColor = Form.Sulphur;
            }
            else if (player.InAbyss())
            {
                SeaSlugColor = Form.Abyss;
            }
            else if (player.InSunkenSea())
            {
                SeaSlugColor = Form.Sunken;
            }
            else 
            {
                SeaSlugColor = Form.Normal;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Drawing(lightColor, ModContent.Request<Texture2D>("CalamityMod/Projectiles/Pets/SeaSlug").Value,
            ModContent.Request<Texture2D>("CalamityMod/Projectiles/Pets/SeaSlugSulphur").Value,
            ModContent.Request<Texture2D>("CalamityMod/Projectiles/Pets/SeaSlugAbyss").Value,
            ModContent.Request<Texture2D>("CalamityMod/Projectiles/Pets/SeaSlugSunken").Value);
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Drawing(Color.White, ModContent.Request<Texture2D>("CalamityMod/Projectiles/Pets/SeaSlug_Glow").Value,
            ModContent.Request<Texture2D>("CalamityMod/Projectiles/Pets/SeaSlugSulphur_Glow").Value,
            ModContent.Request<Texture2D>("CalamityMod/Projectiles/Pets/SeaSlugAbyss_Glow").Value,
            ModContent.Request<Texture2D>("CalamityMod/Projectiles/Pets/SeaSlugSunken_Glow").Value);
        }

        private void Drawing(Color color, Texture2D normal, Texture2D sulphur, Texture2D abyss, Texture2D sunken)
        {
            Texture2D texture = normal;
            switch (SeaSlugColor)
            {
                case Form.Normal:
                    texture = normal;
                    break;
                case Form.Sulphur:
                    texture = sulphur;
                    break;
                case Form.Abyss:
                    texture = abyss;
                    break;
                case Form.Sunken:
                    texture = sunken;
                    break;
            }

            int height = texture.Height / Main.projFrames[Projectile.type];
            int frameHeight = height * Projectile.frame;
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, frameHeight, texture.Width, height)), color, Projectile.rotation, new Vector2(texture.Width / 2f, height / 2f), Projectile.scale, spriteEffects, 0);
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
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
                modPlayer.seaSlugPet = false;
            }
            if (modPlayer.seaSlugPet)
            {
                Projectile.timeLeft = 2;
            }

            UpdateColor(player);

            Vector2 vector46 = Projectile.position;
            if (!fly)
            {
                Projectile.rotation = 0;
                Vector2 center2 = Projectile.Center;
                Vector2 vector48 = player.Center - center2;
                float playerDistance = vector48.Length();
                if (Projectile.velocity.Y == 0 && (HoleBelow() || (playerDistance > 110f && Projectile.position.X == Projectile.oldPosition.X)))
                {
                    Projectile.velocity.Y = -8f;
                }
                Projectile.velocity.Y += 0.35f;
                if (Projectile.velocity.Y > 15f)
                {
                    Projectile.velocity.Y = 15f;
                }
                if (playerDistance > 600f)
                {
                    fly = true;
                    Projectile.velocity.X = 0f;
                    Projectile.velocity.Y = 0f;
                }
                if (playerDistance > 100f)
                {
                    if (player.position.X - Projectile.position.X > 0f)
                    {
                        Projectile.velocity.X += 0.10f;
                        if (Projectile.velocity.X > 5f)
                        {
                            Projectile.velocity.X = 5f;
                        }
                    }
                    else
                    {
                        Projectile.velocity.X -= 0.10f;
                        if (Projectile.velocity.X < -5f)
                        {
                            Projectile.velocity.X = -5f;
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
                if (Projectile.position.X == Projectile.oldPosition.X && Projectile.position.Y == Projectile.oldPosition.Y && Projectile.velocity.X == 0)
                {
                    Projectile.frame = 0;
                    Projectile.frameCounter = 0;
                }
                if (Projectile.velocity.Y > 0.3f && Projectile.position.Y != Projectile.oldPosition.Y)
                {
                    Projectile.frame = 4;
                    Projectile.frameCounter = 0;
                }
                else
                {
                    Projectile.frameCounter++;
                    if (Projectile.frameCounter > 3)
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
                float num16 = 0.5f;
                Projectile.tileCollide = false;
                Vector2 vector3 = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
                float horiPos = Main.player[Projectile.owner].position.X + (float)(Main.player[Projectile.owner].width / 2) - vector3.X;
                float vertiPos = Main.player[Projectile.owner].position.Y + (float)(Main.player[Projectile.owner].height / 2) - vector3.Y;
                vertiPos += (float)Main.rand.Next(-10, 21);
                horiPos += (float)Main.rand.Next(-10, 21);
                horiPos += (float)(60 * -(float)player.direction);
                vertiPos -= 60f;
                float playerDistance = (float)Math.Sqrt((double)(horiPos * horiPos + vertiPos * vertiPos));
                float num21 = 18f;
                float num27 = (float)Math.Sqrt((double)(horiPos * horiPos + vertiPos * vertiPos));
                if (playerDistance > 1200f)
                {
                    Projectile.position.X = player.Center.X - (float)(Projectile.width / 2);
                    Projectile.position.Y = player.Center.Y - (float)(Projectile.height / 2);
                    Projectile.netUpdate = true;
                }
                if (playerDistance < 100f)
                {
                    num16 = 0.5f;
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
                    num16 = 0.02f;
                }
                else
                {
                    if (playerDistance < 100f)
                    {
                        num16 = 0.35f;
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

                /*
                if (playerDistance < 100f)
                {
                    Projectile.rotation += 0.2f * (float)Projectile.direction;
                }
                else
                {
                    Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * (float)Projectile.direction;
                }
                */

                Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * (float)Projectile.direction;

                if (Projectile.Center.X < Main.player[Projectile.owner].Center.X)
                {
                    Projectile.spriteDirection = 1;
                }
                else if (Projectile.Center.X > Main.player[Projectile.owner].Center.X)
                {
                    Projectile.spriteDirection = -1;
                }

                Projectile.frame = 5;
                idleTimer = 0;
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
