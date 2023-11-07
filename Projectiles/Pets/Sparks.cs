using CalamityMod.CalPlayer;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Pets
{
    public class Sparks : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Pets";
        private int color = 0;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.LightPet[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 44;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft *= 5;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
        }

        private void Pickup()
        {
            int defaultItemGrabRange = 38;
            Player player = Main.player[Projectile.owner];

            for (int itemIndex = 0; itemIndex < Main.maxItems; itemIndex++)
            {
                Item item = Main.item[itemIndex];
                if (item.active && item.noGrabDelay == 0 && item.playerIndexTheItemIsReservedFor == Projectile.owner && ItemLoader.CanPickup(item, player))
                {
                    if (new Rectangle((int)Projectile.position.X, (int)Projectile.position.Y, Projectile.width, Projectile.height).Intersects(new Rectangle((int)item.position.X, (int)item.position.Y, item.width, item.height)))
                    {
                        if (Projectile.owner == Main.myPlayer && (player.ActiveItem().type != ItemID.None || player.itemAnimation <= 0))
                        {
                            // TODO -- fix this maybe? (what is it?)
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
                                //int nebulaBuff = item.buffType;
                                //Main.item[itemIndex] = new Item();
                                //if (Main.netMode == 1)
                                //{
                                //    NetMessage.SendData(102, -1, -1, "", projectile.owner, (float)nebulaBuff, player.Center.X, player.Center.Y, 0, 0, 0);
                                //    NetMessage.SendData(21, -1, -1, "", itemIndex, 0f, 0f, 0f, 0, 0, 0);
                                //}
                                //else
                                //{
                                //    player.NebulaLevelup(nebulaBuff);
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
                                Main.item[itemIndex] = player.GetItem(Projectile.owner, item, new GetItemSettings());
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
                if (npc.active && (npc.type == NPCID.Butterfly || npc.type == NPCID.GoldButterfly || npc.type == NPCID.HellButterfly))
                {
                    if (new Rectangle((int)Projectile.position.X, (int)Projectile.position.Y, Projectile.width, Projectile.height).Intersects(new Rectangle((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height)))
                    {
                        npc.life = 0;
                        npc.active = false;
                        SoundEngine.PlaySound(SoundID.Item2, Projectile.position);
                        npc.netUpdate = true;
                    }
                }
            }
        }

        private void PassiveAI()
        {
            Player player = Main.player[Projectile.owner];
            float SAImovement = 0.05f;
            for (int index = 0; index < Main.projectile.Length; index++)
            {
                Projectile proj = Main.projectile[index];
                bool isPet = Main.projPet[proj.type];
                if (index != Projectile.whoAmI && proj.active && proj.owner == Projectile.owner && isPet && Math.Abs(Projectile.position.X - proj.position.X) + Math.Abs(Projectile.position.Y - proj.position.Y) < (float)Projectile.width)
                {
                    if (Projectile.position.X < proj.position.X)
                    {
                        Projectile.velocity.X -= SAImovement;
                    }
                    else
                    {
                        Projectile.velocity.X += SAImovement;
                    }
                    if (Projectile.position.Y < proj.position.Y)
                    {
                        Projectile.velocity.Y -= SAImovement;
                    }
                    else
                    {
                        Projectile.velocity.Y += SAImovement;
                    }
                }
            }
            float flySpeed = 0.5f;
            Projectile.tileCollide = false;
            Vector2 flyDirection = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
            float xDist = player.position.X + (float)(player.width / 2) - flyDirection.X;
            float yDist = player.position.Y + (float)(player.height / 2) - flyDirection.Y;
            yDist += (float)Main.rand.Next(-10, 21);
            xDist += (float)Main.rand.Next(-10, 21);
            xDist += (float)(60 * (float)player.direction);
            yDist -= 60f;
            float playerDist = (float)Math.Sqrt((double)(xDist * xDist + yDist * yDist));
            if (playerDist < 160f)
                Projectile.ai[0] = 0f;
            if (playerDist < 100f && player.velocity.Y == 0f &&
                Projectile.position.Y + (float)Projectile.height <= player.position.Y + (float)player.height &&
                !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
            {
                if (Projectile.velocity.Y < -6f)
                {
                    Projectile.velocity.Y = -6f;
                }
            }
            if (playerDist > 2000f)
            {
                Projectile.position.X = player.Center.X - (float)(Projectile.width / 2);
                Projectile.position.Y = player.Center.Y - (float)(Projectile.height / 2);
                Projectile.netUpdate = true;
            }
            if (playerDist < 50f)
            {
                if (Math.Abs(Projectile.velocity.X) > 2f || Math.Abs(Projectile.velocity.Y) > 2f)
                {
                    Projectile.velocity *= 0.99f;
                }
                flySpeed = 0.01f;
            }
            else
            {
                if (playerDist < 100f)
                {
                    flySpeed = 0.1f;
                }
                if (playerDist > 300f)
                {
                    flySpeed = 1f;
                }
                playerDist = 18f / playerDist;
                xDist *= playerDist;
                yDist *= playerDist;
            }
            if (Projectile.velocity.X < xDist)
            {
                Projectile.velocity.X = Projectile.velocity.X + flySpeed;
                if (flySpeed > 0.05f && Projectile.velocity.X < 0f)
                {
                    Projectile.velocity.X = Projectile.velocity.X + flySpeed;
                }
            }
            if (Projectile.velocity.X > xDist)
            {
                Projectile.velocity.X = Projectile.velocity.X - flySpeed;
                if (flySpeed > 0.05f && Projectile.velocity.X > 0f)
                {
                    Projectile.velocity.X = Projectile.velocity.X - flySpeed;
                }
            }
            if (Projectile.velocity.Y < yDist)
            {
                Projectile.velocity.Y = Projectile.velocity.Y + flySpeed;
                if (flySpeed > 0.05f && Projectile.velocity.Y < 0f)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y + flySpeed * 2f;
                }
            }
            if (Projectile.velocity.Y > yDist)
            {
                Projectile.velocity.Y = Projectile.velocity.Y - flySpeed;
                if (flySpeed > 0.05f && Projectile.velocity.Y > 0f)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y - flySpeed * 2f;
                }
            }
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();

            //Change color based on health
            if (player.statLife >= (int)((double)player.statLifeMax2 * 0.75))
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    color = 0; //gold
                }
            }
            else if (player.statLife >= (int)((double)player.statLifeMax2 * 0.5))
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    color = 1; //blue
                }
            }
            else
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    color = 2; //green
                }
            }

            //give off light based on Dragonfly color
            if (color == 0) //gold
            {
                Lighting.AddLight(Projectile.Center, 1.5f, 1.5f, 0.1f);
            }
            else if (color == 1) //blue
            {
                Lighting.AddLight(Projectile.Center, 0.1f, 0.1f, 1.5f);
            }
            else //green
            {
                Lighting.AddLight(Projectile.Center, 0.1f, 1.5f, 0.1f);
            }

            //framing and sprite direction
            Projectile.spriteDirection = Projectile.direction;
            Projectile.rotation = Projectile.velocity.X * 0.05f;
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 4)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= 4)
            {
                Projectile.frame = 0;
            }
            if ((double)Projectile.velocity.X >= 0.25)
            {
                Projectile.direction = 1;
            }
            else if ((double)Projectile.velocity.X < -0.25)
            {
                Projectile.direction = -1;
            }

            if (!player.active)
            {
                Projectile.active = false;
                return;
            }
            if (player.dead)
            {
                modPlayer.sparks = false;
            }
            if (modPlayer.sparks)
            {
                Projectile.timeLeft = 2;
            }

            Pickup(); //pickup items

            bool decelerate = false;
            if (Projectile.ai[0] == 2f)
            {
                Projectile.ai[1] += 1f;
                if (Projectile.ai[1] > 30f)
                {
                    Projectile.ai[1] = 1f;
                    Projectile.ai[0] = 0f;
                    Projectile.numUpdates = 0;
                    Projectile.netUpdate = true;
                }
                else
                {
                    decelerate = true;
                }
            }
            if (decelerate)
            {
                return;
            }

            //find a target
            Vector2 targetLocation = Projectile.position;
            bool foundFood = false;
            float range = 160f; //10 block range

            for (int itemIndex = 0; itemIndex < Main.maxItems; itemIndex++)
            {
                Item item = Main.item[itemIndex];
                if (item.active && item.noGrabDelay == 0 && item.playerIndexTheItemIsReservedFor == Projectile.owner && 
                    ItemLoader.CanPickup(item, Main.player[item.playerIndexTheItemIsReservedFor]) && Main.player[item.playerIndexTheItemIsReservedFor].ItemSpace(item).CanTakeItemToPersonalInventory)
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

                    float itemDist = Vector2.Distance(item.Center, Projectile.Center);
                    float distanceToPotential = Vector2.Distance(Projectile.Center, targetLocation);
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
                    float npcDist = Vector2.Distance(npc.Center, Projectile.Center);
                    float distanceToPotential = Vector2.Distance(Projectile.Center, targetLocation);
                    if (distanceToPotential < npcDist && npcDist < range)
                    {
                        range = npcDist;
                        targetLocation = npc.Center;
                        foundFood = true;
                    }
                }
            }

            float separationAnxietyDist = 100f;
            if (foundFood)
            {
                separationAnxietyDist = 200f;
            }
            if (Vector2.Distance(player.Center, Projectile.Center) > separationAnxietyDist)
            {
                Projectile.ai[0] = 1f;
                Projectile.netUpdate = true;
            }
            if (foundFood && Projectile.ai[0] == 0f)
            {
                Vector2 targetDirection = targetLocation - Projectile.Center;
                float targetDist = targetDirection.Length();
                targetDirection.Normalize();
                if (targetDist > 200f)
                {
                    float scaleFactor2 = 18f;
                    targetDirection *= scaleFactor2;
                    Projectile.velocity = (Projectile.velocity * 40f + targetDirection) / 41f;
                }
                else
                {
                    targetDirection *= -9f;
                    Projectile.velocity = (Projectile.velocity * 40f + targetDirection) / 41f;
                }
            }
            else
            {
                PassiveAI();
            }
            if (Projectile.ai[1] > 0f)
            {
                Projectile.ai[1] += (float)Main.rand.Next(1, 3);
            }
            if (Projectile.ai[1] > 30f)
            {
                Projectile.ai[1] = 0f;
                Projectile.netUpdate = true;
            }
            if (Projectile.ai[0] == 0f)
            {
                if (Projectile.ai[1] == 0f && foundFood && range < 300f)
                {
                    Projectile.ai[1] += 1f;
                    if (Main.myPlayer == Projectile.owner)
                    {
                        Projectile.ai[0] = 2f;
                        Vector2 targetDest = targetLocation - Projectile.Center;
                        targetDest.Normalize();
                        Projectile.velocity = targetDest * 12f;
                        Projectile.netUpdate = true;
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            if (color == 1)
                texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Pets/SparksBlue").Value;
            if (color == 2)
                texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Pets/SparksGreen").Value;
            int height = texture.Height / Main.projFrames[Projectile.type];
            int frameHeight = height * Projectile.frame;
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, frameHeight, texture.Width, height)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2((float)texture.Width / 2f, (float)height / 2f), Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
}
