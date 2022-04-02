using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Legs)]
    public class PlaguebringerPistons : ModItem
    {
        public int counter = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plaguebringer Pistons");
            Tooltip.SetDefault("13% increased minion damage and 15% increased movement speed\n" +
                "You grow flowers on the grass beneath you, chance to grow very random dye plants on grassless dirt\n" +
                "You spawn bees while sprinting or dashing");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.defense = 8;
            item.value = CalamityGlobalItem.Rarity8BuyPrice;
            item.rare = ItemRarityID.Yellow;
            item.Calamity().donorItem = true;
        }

        public override void UpdateEquip(Player player)
        {
            player.minionDamage += 0.13f;
            player.moveSpeed += 0.15f;
            player.Calamity().plaguebringerPistons = true;

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

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.BeeGreaves);
            recipe.AddIngredient(ItemID.FlowerBoots);
            recipe.AddIngredient(ModContent.ItemType<PlagueCellCluster>(), 5);
            recipe.AddIngredient(ModContent.ItemType<InfectedArmorPlating>(), 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
