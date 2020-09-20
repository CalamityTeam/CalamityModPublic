using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class BloomStone : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bloom Stone");
            Tooltip.SetDefault("One of the ancient relics\n" +
                "Enemies that get near you take damage and all damage is increased by 3%\n" +
                "You grow flowers on the grass beneath you, chance to grow very random dye plants on grassless dirt");
        }

        public override void SetDefaults()
        {
            item.width = 38;
            item.height = 54;
            item.value = CalamityGlobalItem.Rarity8BuyPrice;
            item.rare = 8;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            Lighting.AddLight((int)player.Center.X / 16, (int)player.Center.Y / 16, 0.25f, 0.4f, 0.2f);
            player.allDamage += 0.03f;
            int buffType = BuffID.DryadsWardDebuff;
            float range = 150f;
            bool shouldDealDmg = player.miscCounter % 60 == 0;
            int dmg = (int)(10 * player.AverageDamage());
            if (player.whoAmI == Main.myPlayer)
            {
                if (Main.rand.NextBool(10))
                {
                    for (int l = 0; l < Main.maxNPCs; l++)
                    {
                        NPC npc = Main.npc[l];
                        if (npc.active && !npc.friendly && npc.damage > 0 && !npc.dontTakeDamage && !npc.buffImmune[buffType] && Vector2.Distance(player.Center, npc.Center) <= range)
                        {
                            if (npc.FindBuffIndex(buffType) == -1)
                            {
                                npc.AddBuff(buffType, 120, false);
                            }
                            if (shouldDealDmg)
                            {
                                if (player.whoAmI == Main.myPlayer)
                                {
                                    Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<DirectStrike>(), dmg, 0f, player.whoAmI, l);
                                }
                            }
                        }
                    }
                }
            }

			//Flower Boots code
            if (player.whoAmI == Main.myPlayer && player.velocity.Y == 0f && player.grappling[0] == -1)
            {
                int x = (int)player.Center.X / 16;
                int y = (int)(player.position.Y + (float)player.height - 1f) / 16;
				Tile tile = Main.tile[x, y];
                if (tile == null)
                {
                    tile = new Tile();
                }
                if (!tile.active() && tile.liquid == 0 && Main.tile[x, y + 1] != null && WorldGen.SolidTile(x, y + 1))
                {
                    tile.frameY = 0;
                    tile.slope(0);
                    tile.halfBrick(false);
					//On dirt blocks, there's a small chance to grow a dye plant
                    if (Main.tile[x, y + 1].type == TileID.Dirt)
                    {
                        if (Main.rand.NextBool(1000))
                        {
                            tile.active(true);
                            tile.type = TileID.DyePlants;
                            tile.frameX = (short)(34 * Main.rand.Next(1, 13));
                            while (tile.frameX == 144)
                            {
                                tile.frameX = (short)(34 * Main.rand.Next(1, 13));
                            }
                        }
                        if (Main.netMode == NetmodeID.MultiplayerClient)
                        {
                            NetMessage.SendTileSquare(-1, x, y, 1, TileChangeType.None);
                        }
                    }
					//On grass, grow flowers
                    if (Main.tile[x, y + 1].type == TileID.Grass)
                    {
                        if (Main.rand.NextBool(2))
                        {
                            tile.active(true);
                            tile.type = TileID.Plants;
                            tile.frameX = (short)(18 * Main.rand.Next(6, 11));
                            while (tile.frameX == 144)
                            {
                                tile.frameX = (short)(18 * Main.rand.Next(6, 11));
                            }
                        }
                        else
                        {
                            tile.active(true);
                            tile.type = TileID.Plants2;
                            tile.frameX = (short)(18 * Main.rand.Next(6, 21));
                            while (tile.frameX == 144)
                            {
                                tile.frameX = (short)(18 * Main.rand.Next(6, 21));
                            }
                        }
                        if (Main.netMode == NetmodeID.MultiplayerClient)
                        {
                            NetMessage.SendTileSquare(-1, x, y, 1, TileChangeType.None);
                        }
                    }
					//On hallowed grass, grow hallowed flowers
                    else if (Main.tile[x, y + 1].type == TileID.HallowedGrass)
                    {
                        if (Main.rand.NextBool(2))
                        {
                            tile.active(true);
                            tile.type = TileID.HallowedPlants;
                            tile.frameX = (short)(18 * Main.rand.Next(4, 7));
                            while (tile.frameX == 90)
                            {
                                tile.frameX = (short)(18 * Main.rand.Next(4, 7));
                            }
                        }
                        else
                        {
                            tile.active(true);
                            tile.type = TileID.HallowedPlants2;
                            tile.frameX = (short)(18 * Main.rand.Next(2, 8));
                            while (tile.frameX == 90)
                            {
                                tile.frameX = (short)(18 * Main.rand.Next(2, 8));
                            }
                        }
                        if (Main.netMode == NetmodeID.MultiplayerClient)
                        {
                            NetMessage.SendTileSquare(-1, x, y, 1, TileChangeType.None);
                        }
                    }
					//On jungle grass, grow jungle flowers
                    else if (Main.tile[x, y + 1].type == TileID.JungleGrass)
                    {
                        tile.active(true);
                        tile.type = TileID.JunglePlants2;
                        tile.frameX = (short)(18 * Main.rand.Next(9, 17));
                        if (Main.netMode == NetmodeID.MultiplayerClient)
                        {
                            NetMessage.SendTileSquare(-1, x, y, 1, TileChangeType.None);
                        }
                    }
                }
            }
        }
    }
}
