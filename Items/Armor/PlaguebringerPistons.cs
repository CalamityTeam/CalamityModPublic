using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Legs)]
    public class PlaguebringerPistons : ModItem
    {
        public int counter = 0;
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Plaguebringer Pistons");
            Tooltip.SetDefault("13% increased minion damage and 15% increased movement speed\n" +
                "You grow flowers on the grass beneath you, chance to grow very random dye plants on grassless dirt\n" +
                "You spawn bees while sprinting or dashing");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.defense = 8;
            Item.value = CalamityGlobalItem.Rarity8BuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.Calamity().donorItem = true;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Summon) += 0.13f;
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
                if (!tile.HasTile && tile.LiquidAmount == 0 && Main.tile[x, y + 1] != null && WorldGen.SolidTile(x, y + 1))
                {
                    tile.TileFrameY = 0;
                    tile.Get<TileWallWireStateData>().Slope = SlopeType.Solid;
                    tile.Get<TileWallWireStateData>().IsHalfBlock = false;
                    //On dirt blocks, there's a small chance to grow a dye plant
                    if (Main.tile[x, y + 1].TileType == TileID.Dirt)
                    {
                        if (Main.rand.NextBool(1000))
                        {
                            tile.Get<TileWallWireStateData>().HasTile = true;
                            tile.TileType = TileID.DyePlants;
                            tile.TileFrameX = (short)(34 * Main.rand.Next(1, 13));
                            while (tile.TileFrameX == 144)
                            {
                                tile.TileFrameX = (short)(34 * Main.rand.Next(1, 13));
                            }
                        }
                        if (Main.netMode == NetmodeID.MultiplayerClient)
                        {
                            NetMessage.SendTileSquare(-1, x, y, 1, TileChangeType.None);
                        }
                    }
                    //On grass, grow flowers
                    if (Main.tile[x, y + 1].TileType == TileID.Grass)
                    {
                        if (Main.rand.NextBool(2))
                        {
                            tile.Get<TileWallWireStateData>().HasTile = true;
                            tile.TileType = TileID.Plants;
                            tile.TileFrameX = (short)(18 * Main.rand.Next(6, 11));
                            while (tile.TileFrameX == 144)
                            {
                                tile.TileFrameX = (short)(18 * Main.rand.Next(6, 11));
                            }
                        }
                        else
                        {
                            tile.Get<TileWallWireStateData>().HasTile = true;
                            tile.TileType = TileID.Plants2;
                            tile.TileFrameX = (short)(18 * Main.rand.Next(6, 21));
                            while (tile.TileFrameX == 144)
                            {
                                tile.TileFrameX = (short)(18 * Main.rand.Next(6, 21));
                            }
                        }
                        if (Main.netMode == NetmodeID.MultiplayerClient)
                        {
                            NetMessage.SendTileSquare(-1, x, y, 1, TileChangeType.None);
                        }
                    }
                    //On hallowed grass, grow hallowed flowers
                    else if (Main.tile[x, y + 1].TileType == TileID.HallowedGrass)
                    {
                        if (Main.rand.NextBool(2))
                        {
                            tile.Get<TileWallWireStateData>().HasTile = true;
                            tile.TileType = TileID.HallowedPlants;
                            tile.TileFrameX = (short)(18 * Main.rand.Next(4, 7));
                            while (tile.TileFrameX == 90)
                            {
                                tile.TileFrameX = (short)(18 * Main.rand.Next(4, 7));
                            }
                        }
                        else
                        {
                            tile.Get<TileWallWireStateData>().HasTile = true;
                            tile.TileType = TileID.HallowedPlants2;
                            tile.TileFrameX = (short)(18 * Main.rand.Next(2, 8));
                            while (tile.TileFrameX == 90)
                            {
                                tile.TileFrameX = (short)(18 * Main.rand.Next(2, 8));
                            }
                        }
                        if (Main.netMode == NetmodeID.MultiplayerClient)
                        {
                            NetMessage.SendTileSquare(-1, x, y, 1, TileChangeType.None);
                        }
                    }
                    //On jungle grass, grow jungle flowers
                    else if (Main.tile[x, y + 1].TileType == TileID.JungleGrass)
                    {
                        tile.Get<TileWallWireStateData>().HasTile = true;
                        tile.TileType = TileID.JunglePlants2;
                        tile.TileFrameX = (short)(18 * Main.rand.Next(9, 17));
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
            CreateRecipe().
                AddIngredient(ItemID.BeeGreaves).
                AddIngredient(ItemID.FlowerBoots).
                AddIngredient<PlagueCellCluster>(5).
                AddIngredient<InfectedArmorPlating>(5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
