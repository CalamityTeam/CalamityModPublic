using CalamityMod.CalPlayer;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Pets
{
    public class Sparks : ModProjectile
    {
        private int color = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sparks");
            Main.projFrames[projectile.type] = 4;
            Main.projPet[projectile.type] = true;
            ProjectileID.Sets.LightPet[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 44;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft *= 5;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
        }

        private void Pickup()
        {
            int defaultItemGrabRange = 38;
            Player player = Main.player[projectile.owner];

            for (int itemIndex = 0; itemIndex < Main.maxItems; itemIndex++)
            {
                Item item = Main.item[itemIndex];
                if (item.active && item.noGrabDelay == 0 && item.owner == projectile.owner && ItemLoader.CanPickup(item, player))
                {
                    int num = defaultItemGrabRange;//Player.defaultItemGrabRange;

                    if (new Rectangle((int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height).Intersects(new Rectangle((int)item.position.X, (int)item.position.Y, item.width, item.height)))
                    {
                        if (projectile.owner == Main.myPlayer && (player.ActiveItem().type != ItemID.None || player.itemAnimation <= 0))
                        {
                            // TODO, fix this maybe?
                            if (!ItemLoader.OnPickup(item, player))
                            {
                                Main.item[itemIndex] = new Item();
                                if (Main.netMode == NetmodeID.MultiplayerClient)
                                {
                                    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, itemIndex, 0f, 0f, 0f, 0, 0, 0);
                                }
                                continue;
                            }

                            if (ItemID.Sets.NebulaPickup[item.type])
                            {
                                continue;
                                //Main.PlaySound(7, (int)player.position.X, (int)player.position.Y, 1);
                                //int num2 = item.buffType;
                                //Main.item[itemIndex] = new Item();
                                //if (Main.netMode == 1)
                                //{
                                //    NetMessage.SendData(102, -1, -1, "", projectile.owner, (float)num2, player.Center.X, player.Center.Y, 0, 0, 0);
                                //    NetMessage.SendData(21, -1, -1, "", itemIndex, 0f, 0f, 0f, 0, 0, 0);
                                //}
                                //else
                                //{
                                //    player.NebulaLevelup(num2);
                                //}
                            }
                            if (item.type == ItemID.Heart || item.type == ItemID.CandyApple || item.type == ItemID.CandyCane)
                            {
                                continue;
                                //Main.PlaySound(7, (int)player.position.X, (int)player.position.Y, 1);
                                //player.statLife += 20;
                                //if (Main.myPlayer == player.whoAmI)
                                //{
                                //    player.HealEffect(20, true);
                                //}
                                //if (player.statLife > player.statLifeMax2)
                                //{
                                //    player.statLife = player.statLifeMax2;
                                //}
                                //Main.item[itemIndex] = new Item();
                                //if (Main.netMode == 1)
                                //{
                                //    NetMessage.SendData(21, -1, -1, "", itemIndex, 0f, 0f, 0f, 0, 0, 0);
                                //}
                            }
                            else if (item.type == ItemID.Star || item.type == ItemID.SoulCake || item.type == ItemID.SugarPlum)
                            {
                                continue;
                                //Main.PlaySound(7, (int)player.position.X, (int)player.position.Y, 1);
                                //player.statMana += 100;
                                //if (Main.myPlayer == player.whoAmI)
                                //{
                                //    player.ManaEffect(100);
                                //}
                                //if (player.statMana > player.statManaMax2)
                                //{
                                //    player.statMana = player.statManaMax2;
                                //}
                                //Main.item[itemIndex] = new Item();
                                //if (Main.netMode == 1)
                                //{
                                //    NetMessage.SendData(21, -1, -1, "", itemIndex, 0f, 0f, 0f, 0, 0, 0);
                                //}
                            }
                            else
                            {
                                Main.item[itemIndex] = player.GetItem(projectile.owner, item, false, false);
                                if (Main.netMode == NetmodeID.MultiplayerClient)
                                {
                                    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, itemIndex, 0f, 0f, 0f, 0, 0, 0);
                                }
                            }
                        }
                    }
                }
            }

            for (int npcIndex = 0; npcIndex < Main.maxNPCs; npcIndex++)
            {
                NPC npc = Main.npc[npcIndex];
                if (npc.active && (npc.type == NPCID.Butterfly || npc.type == NPCID.GoldButterfly))
                {
                    if (new Rectangle((int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height).Intersects(new Rectangle((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height)))
                    {
                        npc.life = 0;
                        npc.active = false;
                        Main.PlaySound(SoundID.Item2, projectile.position);
                        npc.netUpdate = true;
                    }
                }
            }
        }

        private void PassiveAI()
        {
            Player player = Main.player[projectile.owner];
            float SAImovement = 0.05f;
            for (int index = 0; index < Main.projectile.Length; index++)
            {
                Projectile proj = Main.projectile[index];
                bool flag23 = Main.projPet[proj.type];
                if (index != projectile.whoAmI && proj.active && proj.owner == projectile.owner && flag23 && Math.Abs(projectile.position.X - proj.position.X) + Math.Abs(projectile.position.Y - proj.position.Y) < (float)projectile.width)
                {
                    if (projectile.position.X < proj.position.X)
                    {
                        projectile.velocity.X -= SAImovement;
                    }
                    else
                    {
                        projectile.velocity.X += SAImovement;
                    }
                    if (projectile.position.Y < proj.position.Y)
                    {
                        projectile.velocity.Y -= SAImovement;
                    }
                    else
                    {
                        projectile.velocity.Y += SAImovement;
                    }
                }
            }
            float num16 = 0.5f;
            projectile.tileCollide = false;
            Vector2 vector3 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
            float xDist = player.position.X + (float)(player.width / 2) - vector3.X;
            float yDist = player.position.Y + (float)(player.height / 2) - vector3.Y;
            yDist += (float)Main.rand.Next(-10, 21);
            xDist += (float)Main.rand.Next(-10, 21);
            xDist += (float)(60 * (float)player.direction);
            yDist -= 60f;
            float playerDist = (float)Math.Sqrt((double)(xDist * xDist + yDist * yDist));
            float num21 = 18f;
            if (playerDist < 160f)
                projectile.ai[0] = 0f;
            if (playerDist < 100f && player.velocity.Y == 0f &&
                projectile.position.Y + (float)projectile.height <= player.position.Y + (float)player.height &&
                !Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
            {
                if (projectile.velocity.Y < -6f)
                {
                    projectile.velocity.Y = -6f;
                }
            }
            if (playerDist > 2000f)
            {
                projectile.position.X = player.Center.X - (float)(projectile.width / 2);
                projectile.position.Y = player.Center.Y - (float)(projectile.height / 2);
                projectile.netUpdate = true;
            }
            if (playerDist < 50f)
            {
                if (Math.Abs(projectile.velocity.X) > 2f || Math.Abs(projectile.velocity.Y) > 2f)
                {
                    projectile.velocity *= 0.99f;
                }
                num16 = 0.01f;
            }
            else
            {
                if (playerDist < 100f)
                {
                    num16 = 0.1f;
                }
                if (playerDist > 300f)
                {
                    num16 = 1f;
                }
                playerDist = num21 / playerDist;
                xDist *= playerDist;
                yDist *= playerDist;
            }
            if (projectile.velocity.X < xDist)
            {
                projectile.velocity.X = projectile.velocity.X + num16;
                if (num16 > 0.05f && projectile.velocity.X < 0f)
                {
                    projectile.velocity.X = projectile.velocity.X + num16;
                }
            }
            if (projectile.velocity.X > xDist)
            {
                projectile.velocity.X = projectile.velocity.X - num16;
                if (num16 > 0.05f && projectile.velocity.X > 0f)
                {
                    projectile.velocity.X = projectile.velocity.X - num16;
                }
            }
            if (projectile.velocity.Y < yDist)
            {
                projectile.velocity.Y = projectile.velocity.Y + num16;
                if (num16 > 0.05f && projectile.velocity.Y < 0f)
                {
                    projectile.velocity.Y = projectile.velocity.Y + num16 * 2f;
                }
            }
            if (projectile.velocity.Y > yDist)
            {
                projectile.velocity.Y = projectile.velocity.Y - num16;
                if (num16 > 0.05f && projectile.velocity.Y > 0f)
                {
                    projectile.velocity.Y = projectile.velocity.Y - num16 * 2f;
                }
            }
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();

            //Change color based on health
            if (player.statLife >= (int)((double)player.statLifeMax2 * 0.75))
            {
                if (Main.myPlayer == projectile.owner)
                {
                    color = 0; //gold
                }
            }
            else if (player.statLife >= (int)((double)player.statLifeMax2 * 0.5))
            {
                if (Main.myPlayer == projectile.owner)
                {
                    color = 1; //blue
                }
            }
            else
            {
                if (Main.myPlayer == projectile.owner)
                {
                    color = 2; //green
                }
            }

            //give off light based on Dragonfly color
            if (color == 0) //gold
            {
                Lighting.AddLight(projectile.Center, 1.5f, 1.5f, 0.1f);
            }
            else if (color == 1) //blue
            {
                Lighting.AddLight(projectile.Center, 0.1f, 0.1f, 1.5f);
            }
            else //green
            {
                Lighting.AddLight(projectile.Center, 0.1f, 1.5f, 0.1f);
            }

            //framing and sprite direction
            projectile.spriteDirection = projectile.direction;
            projectile.rotation = projectile.velocity.X * 0.05f;
            projectile.frameCounter++;
            if (projectile.frameCounter > 4)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= 4)
            {
                projectile.frame = 0;
            }
            if ((double)projectile.velocity.X >= 0.25)
            {
                projectile.direction = 1;
            }
            else if ((double)projectile.velocity.X < -0.25)
            {
                projectile.direction = -1;
            }

            if (!player.active)
            {
                projectile.active = false;
                return;
            }
            if (player.dead)
            {
                modPlayer.sparks = false;
            }
            if (modPlayer.sparks)
            {
                projectile.timeLeft = 2;
            }

            Pickup(); //pickup items

            bool flag24 = false;
            if (projectile.ai[0] == 2f)
            {
                projectile.ai[1] += 1f;
                if (projectile.ai[1] > 30f)
                {
                    projectile.ai[1] = 1f;
                    projectile.ai[0] = 0f;
                    projectile.numUpdates = 0;
                    projectile.netUpdate = true;
                }
                else
                {
                    flag24 = true;
                }
            }
            if (flag24)
            {
                return;
            }

            //find a target
            Vector2 targetLocation = projectile.position;
            bool foundFood = false;
            float range = 160f; //10 block range

            for (int itemIndex = 0; itemIndex < Main.maxItems; itemIndex++)
            {
                Item item = Main.item[itemIndex];
                if (item.active && item.noGrabDelay == 0 && item.owner == projectile.owner && ItemLoader.CanPickup(item, Main.player[item.owner]) && Main.player[item.owner].ItemSpace(item))
                {
                    if (ItemID.Sets.NebulaPickup[item.type])
                    {
                        continue;
                    }
                    if (item.type == ItemID.Heart || item.type == ItemID.CandyApple || item.type == ItemID.CandyCane)
                    {
                        continue;
                    }
                    else if (item.type == ItemID.Star || item.type == ItemID.SoulCake || item.type == ItemID.SugarPlum)
                    {
                        continue;
                    }

                    float itemDist = Vector2.Distance(item.Center, projectile.Center);
                    float distanceToPotential = Vector2.Distance(projectile.Center, targetLocation);
                    if (distanceToPotential < itemDist && itemDist < range)
                    {
                        range = itemDist;
                        targetLocation = item.Center;
                        foundFood = true;
                    }
                }
            }
            //prioritize butterflies over items
            for (int npcIndex = 0; npcIndex < Main.maxNPCs; npcIndex++)
            {
                NPC npc = Main.npc[npcIndex];
                if (npc.active && (npc.type == NPCID.Butterfly || npc.type == NPCID.GoldButterfly))
                {
                    float npcDist = Vector2.Distance(npc.Center, projectile.Center);
                    float distanceToPotential = Vector2.Distance(projectile.Center, targetLocation);
                    if (distanceToPotential < npcDist && npcDist < range)
                    {
                        range = npcDist;
                        targetLocation = npc.Center;
                        foundFood = true;
                    }
                }
            }

            float num647 = 100f;
            if (foundFood)
            {
                num647 = 200f;
            }
            if (Vector2.Distance(player.Center, projectile.Center) > num647)
            {
                projectile.ai[0] = 1f;
                projectile.netUpdate = true;
            }
            if (foundFood && projectile.ai[0] == 0f)
            {
                Vector2 vector47 = targetLocation - projectile.Center;
                float num648 = vector47.Length();
                vector47.Normalize();
                if (num648 > 200f)
                {
                    float scaleFactor2 = 18f;
                    vector47 *= scaleFactor2;
                    projectile.velocity = (projectile.velocity * 40f + vector47) / 41f;
                }
                else
                {
                    float num649 = 9f;
                    vector47 *= -num649;
                    projectile.velocity = (projectile.velocity * 40f + vector47) / 41f;
                }
            }
            else
            {
                PassiveAI();
            }
            if (projectile.ai[1] > 0f)
            {
                projectile.ai[1] += (float)Main.rand.Next(1, 3);
            }
            if (projectile.ai[1] > 30f)
            {
                projectile.ai[1] = 0f;
                projectile.netUpdate = true;
            }
            if (projectile.ai[0] == 0f)
            {
                if (projectile.ai[1] == 0f && foundFood && range < 300f)
                {
                    projectile.ai[1] += 1f;
                    if (Main.myPlayer == projectile.owner)
                    {
                        projectile.ai[0] = 2f;
                        Vector2 value20 = targetLocation - projectile.Center;
                        value20.Normalize();
                        projectile.velocity = value20 * 12f;
                        projectile.netUpdate = true;
                    }
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            if (color == 1)
                texture = ModContent.GetTexture("CalamityMod/Projectiles/Pets/SparksBlue");
            if (color == 2)
                texture = ModContent.GetTexture("CalamityMod/Projectiles/Pets/SparksGreen");
            int height = texture.Height / Main.projFrames[projectile.type];
            int frameHeight = height * projectile.frame;
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;
            Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, frameHeight, texture.Width, height)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2((float)texture.Width / 2f, (float)height / 2f), projectile.scale, spriteEffects, 0f);
            return false;
        }
    }
}