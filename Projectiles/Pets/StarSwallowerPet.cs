using CalamityMod.CalPlayer;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Pets
{
    public class StarSwallowerPet : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Pets";
        private int playerStill = 0;
        private bool idleAnimation = false;
        private bool fly = false;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 14;
            Main.projPet[Projectile.type] = true;

            ProjectileID.Sets.CharacterPreviewAnimations[Projectile.type] = ProjectileID.Sets.SimpleLoop(1, 8, 6)
            .WithOffset(-18f, 0f).WithSpriteDirection(1).WhenNotSelected(0, 0);
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.width = 54;
            Projectile.height = 48;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft *= 5;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 scale = Vector2.One;
            if (fly)
            {
                float animRate = Main.GlobalTimeWrappedHourly * 20;
                float scaleMod = 0.2f;
                scale += new Vector2(MathF.Cos(animRate), MathF.Sin(animRate)) * scaleMod;
            }
            SpriteEffects fx = fly ? (SpriteEffects.FlipVertically | (Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None)) : Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2(texture.Width / 2f, texture.Height / 2 / Main.projFrames[Type]), Projectile.scale * scale, fx, 0);

            return false;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            Player player = Main.player[Projectile.owner];
            float heightDif = player.Center.Y - Projectile.Center.Y;
            fallThrough = fly || heightDif >= 160f;
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
                modPlayer.starSwallowerPetFroge = false;
            }
            if (modPlayer.starSwallowerPetFroge)
            {
                Projectile.timeLeft = 2;
            }

            Vector2 projPos = Projectile.position;
            Vector2 mouthPos = Projectile.Center + new Vector2(-20 * Projectile.spriteDirection, -6f).RotatedBy(Projectile.rotation);
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
                if (playerDistance > 480f)
                {
                    fly = true;
                    Projectile.velocity.X = 0f;
                    Projectile.velocity.Y = 0f;
                }
                if (playerDistance > 100f)
                {
                    if (player.position.X - Projectile.position.X > 0f)
                    {
                        Projectile.velocity.X += 0.125f;
                        if (Projectile.velocity.X > 6.75f)
                        {
                            Projectile.velocity.X = 6.75f;
                        }
                    }
                    else
                    {
                        Projectile.velocity.X -= 0.125f;
                        if (Projectile.velocity.X < -6.75f)
                        {
                            Projectile.velocity.X = -6.75f;
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
                    // Idle animation where it opens its mouth and lets out some plasma mist
                    if (Main.rand.NextBool(600) && !idleAnimation)
                    {
                        idleAnimation = true;
                        Projectile.frame = 8;
                        Projectile.frameCounter = 0;
                    }

                    if (idleAnimation)
                    {
                        Projectile.frameCounter++;
                        if (Projectile.frameCounter > 8)
                        {
                            Projectile.frame += (Projectile.ai[0] != -1).ToDirectionInt();
                            Projectile.frameCounter = 0;
                        }
                        if (Projectile.ai[0] != -1)
                        {
                            if (Projectile.frame >= 12)
                            {
                                Projectile.ai[0] = -1;
                                Projectile.frame = 11;
                            }
                        }
                        else
                        {
                            if (Projectile.frame <= 7)
                            {
                                Projectile.ai[0] = 0;
                                Projectile.frame = 0;
                                idleAnimation = false;
                            }
                        }
                        if (Projectile.frame > 8)
                        {
                            GeneralParticleHandler.SpawnParticle(new MediumMistParticle(mouthPos, (Vector2.UnitX * -Projectile.spriteDirection).RotatedByRandom(MathHelper.ToRadians(10)) * Main.rand.NextFloat(4, 8), new Color(201, 234, 71), new Color(115, 192, 58), Main.rand.NextFloat(0.8f, 1.1f), 170f, Main.rand.NextFloat(0.1f, 0.2f) * Main.rand.NextFloatDirection()));
                        }
                    }
                    else
                    {
                        Projectile.frame = 0;
                        Projectile.frameCounter = 0;
                    }
                }
                //falling frame
                else if (Projectile.velocity.Y > 0.3f && Projectile.position.Y != Projectile.oldPosition.Y)
                {
                    Projectile.frame = 7;
                    Projectile.frameCounter = 0;
                }
                else if (Projectile.velocity.X != 0)
                {
                    //moving animation
                    Projectile.frameCounter++;
                    if (Projectile.frameCounter >= 6)
                    {
                        Projectile.frame++;
                        Projectile.frameCounter = 0;
                    }
                    if (Projectile.frame > 7)
                    {
                        Projectile.frame = 0;
                    }
                }

                if (Projectile.velocity.X > 0.8f)
                {
                    Projectile.spriteDirection = -1;
                }
                else if (Projectile.velocity.X < -0.8f)
                {
                    Projectile.spriteDirection = 1;
                }
            }
            else if (fly)
            {
                idleAnimation = false;
                Projectile.ai[0] = 0;
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
                    if (playerStill > 10 && Projectile.Center.Y <= player.Center.Y && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
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

                Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == -1 ? MathHelper.Pi : 0);

                if (Projectile.Center.X < Main.player[Projectile.owner].Center.X)
                {
                    Projectile.spriteDirection = -1;
                }
                else if (Projectile.Center.X > Main.player[Projectile.owner].Center.X)
                {
                    Projectile.spriteDirection = 1;
                }

                Projectile.frame = 12;

                // fly anim particles
                GeneralParticleHandler.SpawnParticle(new MediumMistParticle(mouthPos, mouthPos.DirectionTo(Projectile.oldPosition).RotatedByRandom(MathHelper.ToRadians(20)).RotatedBy(MathHelper.ToRadians(10 * Projectile.spriteDirection)) * Main.rand.NextFloat(4, 8), new Color(201, 234, 71), new Color(115, 192, 58), Main.rand.NextFloat(0.7f, 1f), 200f, Main.rand.NextFloat(0.1f, 0.2f) * Main.rand.NextFloatDirection()));
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
