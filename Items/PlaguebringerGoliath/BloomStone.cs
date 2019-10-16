using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class BloomStone : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bloom Stone");
            Tooltip.SetDefault("One of the ancient relics\n" +
                "Enemies that get near you take damage and all damage is increased by 3%\n" +
                "You grow flowers on the grass beneath you, chance to grow very random dye plants on grassless dirt");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(4, 7));
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.value = Item.buyPrice(0, 15, 0, 0);
            item.rare = 5;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            Lighting.AddLight((int)player.Center.X / 16, (int)player.Center.Y / 16, 0.25f, 0.4f, 0.2f);
            player.allDamage += 0.03f;
            int bloomCounter = 0;
            int num = 186;
            float num2 = 150f;
            bool flag = bloomCounter % 60 == 0;
            int num3 = 10;
            int random = Main.rand.Next(10);
            if (player.whoAmI == Main.myPlayer)
            {
                if (random == 0)
                {
                    for (int l = 0; l < 200; l++)
                    {
                        NPC nPC = Main.npc[l];
                        if (nPC.active && !nPC.friendly && nPC.damage > 0 && !nPC.dontTakeDamage && !nPC.buffImmune[num] && Vector2.Distance(player.Center, nPC.Center) <= num2)
                        {
                            if (nPC.FindBuffIndex(num) == -1)
                            {
                                nPC.AddBuff(num, 120, false);
                            }
                            if (flag)
                            {
                                nPC.StrikeNPC(num3, 0f, 0, false, false, false);
                                if (Main.netMode != NetmodeID.SinglePlayer)
                                {
                                    NetMessage.SendData(28, -1, -1, null, l, (float)num3, 0f, 0f, 0, 0, 0);
                                }
                            }
                        }
                    }
                }
            }
            bloomCounter++;
            if (bloomCounter >= 180)
            {
            }
            if (player.whoAmI == Main.myPlayer && player.velocity.Y == 0f && player.grappling[0] == -1)
            {
                int num4 = (int)player.Center.X / 16;
                int num5 = (int)(player.position.Y + (float)player.height - 1f) / 16;
                if (Main.tile[num4, num5] == null)
                {
                    Main.tile[num4, num5] = new Tile();
                }
                if (!Main.tile[num4, num5].active() && Main.tile[num4, num5].liquid == 0 && Main.tile[num4, num5 + 1] != null && WorldGen.SolidTile(num4, num5 + 1))
                {
                    Main.tile[num4, num5].frameY = 0;
                    Main.tile[num4, num5].slope(0);
                    Main.tile[num4, num5].halfBrick(false);
                    if (Main.tile[num4, num5 + 1].type == 0)
                    {
                        if (Main.rand.NextBool(1000))
                        {
                            Main.tile[num4, num5].active(true);
                            Main.tile[num4, num5].type = 227;
                            Main.tile[num4, num5].frameX = (short)(34 * Main.rand.Next(1, 13));
                            while (Main.tile[num4, num5].frameX == 144)
                            {
                                Main.tile[num4, num5].frameX = (short)(34 * Main.rand.Next(1, 13));
                            }
                        }
                        if (Main.netMode == NetmodeID.MultiplayerClient)
                        {
                            NetMessage.SendTileSquare(-1, num4, num5, 1, TileChangeType.None);
                        }
                    }
                    if (Main.tile[num4, num5 + 1].type == 2)
                    {
                        if (Main.rand.NextBool(2))
                        {
                            Main.tile[num4, num5].active(true);
                            Main.tile[num4, num5].type = 3;
                            Main.tile[num4, num5].frameX = (short)(18 * Main.rand.Next(6, 11));
                            while (Main.tile[num4, num5].frameX == 144)
                            {
                                Main.tile[num4, num5].frameX = (short)(18 * Main.rand.Next(6, 11));
                            }
                        }
                        else
                        {
                            Main.tile[num4, num5].active(true);
                            Main.tile[num4, num5].type = 73;
                            Main.tile[num4, num5].frameX = (short)(18 * Main.rand.Next(6, 21));
                            while (Main.tile[num4, num5].frameX == 144)
                            {
                                Main.tile[num4, num5].frameX = (short)(18 * Main.rand.Next(6, 21));
                            }
                        }
                        if (Main.netMode == NetmodeID.MultiplayerClient)
                        {
                            NetMessage.SendTileSquare(-1, num4, num5, 1, TileChangeType.None);
                        }
                    }
                    else if (Main.tile[num4, num5 + 1].type == 109)
                    {
                        if (Main.rand.NextBool(2))
                        {
                            Main.tile[num4, num5].active(true);
                            Main.tile[num4, num5].type = 110;
                            Main.tile[num4, num5].frameX = (short)(18 * Main.rand.Next(4, 7));
                            while (Main.tile[num4, num5].frameX == 90)
                            {
                                Main.tile[num4, num5].frameX = (short)(18 * Main.rand.Next(4, 7));
                            }
                        }
                        else
                        {
                            Main.tile[num4, num5].active(true);
                            Main.tile[num4, num5].type = 113;
                            Main.tile[num4, num5].frameX = (short)(18 * Main.rand.Next(2, 8));
                            while (Main.tile[num4, num5].frameX == 90)
                            {
                                Main.tile[num4, num5].frameX = (short)(18 * Main.rand.Next(2, 8));
                            }
                        }
                        if (Main.netMode == NetmodeID.MultiplayerClient)
                        {
                            NetMessage.SendTileSquare(-1, num4, num5, 1, TileChangeType.None);
                        }
                    }
                    else if (Main.tile[num4, num5 + 1].type == 60)
                    {
                        Main.tile[num4, num5].active(true);
                        Main.tile[num4, num5].type = 74;
                        Main.tile[num4, num5].frameX = (short)(18 * Main.rand.Next(9, 17));
                        if (Main.netMode == NetmodeID.MultiplayerClient)
                        {
                            NetMessage.SendTileSquare(-1, num4, num5, 1, TileChangeType.None);
                        }
                    }
                }
            }
        }
    }
}
