using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class BloomStone : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 54;
            Item.value = CalamityGlobalItem.Rarity7BuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            Lighting.AddLight((int)player.Center.X / 16, (int)player.Center.Y / 16, 0.25f, 0.4f, 0.2f);

            // Provide life benefits if the player is standing on ground and has typical gravity.
            int x = (int)player.Center.X / 16;
            int y = (int)(player.Bottom.Y - 1f) / 16;
            Tile groundTile = CalamityUtils.ParanoidTileRetrieval(x, y + 1);
            bool groundTileIsSolid = groundTile.HasUnactuatedTile && (Main.tileSolid[groundTile.TileType] || Main.tileSolidTop[groundTile.TileType]);
            if (groundTileIsSolid && player.gravDir == 1f)
                modPlayer.BloomStoneRegen = true;

            // Flower boots effects.
            if (player.whoAmI == Main.myPlayer && player.velocity.Y == 0f && player.grappling[0] == -1)
            {
                Tile walkTile = CalamityUtils.ParanoidTileRetrieval(x, y);
                if (!walkTile.HasTile && walkTile.LiquidAmount == 0 && groundTile != null && WorldGen.SolidTile(groundTile))
                {
                    walkTile.TileFrameY = 0;
                    walkTile.Get<TileWallWireStateData>().Slope = SlopeType.Solid;
                    walkTile.Get<TileWallWireStateData>().IsHalfBlock = false;
                    // On dirt blocks, there's a small chance to grow a dye plant.
                    if (groundTile.TileType == TileID.Dirt)
                    {
                        if (Main.rand.NextBool(1000))
                        {
                            walkTile.Get<TileWallWireStateData>().HasTile = true;
                            walkTile.TileType = TileID.DyePlants;
                            walkTile.TileFrameX = (short)(34 * Main.rand.Next(0, 13));
                            while (walkTile.TileFrameX == 144)
                                walkTile.TileFrameX = (short)(34 * Main.rand.Next(0, 13));
                        }

                        if (Main.netMode == NetmodeID.MultiplayerClient)
                            NetMessage.SendTileSquare(-1, x, y, 1, TileChangeType.None);
                    }

                    // On grass, grow flowers.
                    if (groundTile.TileType == TileID.Grass)
                    {
                        if (Main.rand.NextBool())
                        {
                            walkTile.Get<TileWallWireStateData>().HasTile = true;
                            walkTile.TileType = TileID.Plants;
                            walkTile.TileFrameX = (short)(18 * Main.rand.Next(6, 11));

                            while (walkTile.TileFrameX == 144)
                                walkTile.TileFrameX = (short)(18 * Main.rand.Next(6, 11));
                        }
                        else
                        {
                            walkTile.Get<TileWallWireStateData>().HasTile = true;
                            walkTile.TileType = TileID.Plants2;
                            walkTile.TileFrameX = (short)(18 * Main.rand.Next(6, 21));

                            while (walkTile.TileFrameX == 144)
                                walkTile.TileFrameX = (short)(18 * Main.rand.Next(6, 21));
                        }

                        if (Main.netMode == NetmodeID.MultiplayerClient)
                            NetMessage.SendTileSquare(-1, x, y, 1, TileChangeType.None);
                    }

                    // On hallowed grass, grow hallowed flowers.
                    else if (groundTile.TileType == TileID.HallowedGrass)
                    {
                        if (Main.rand.NextBool())
                        {
                            walkTile.Get<TileWallWireStateData>().HasTile = true;
                            walkTile.TileType = TileID.HallowedPlants;
                            walkTile.TileFrameX = (short)(18 * Main.rand.Next(4, 7));
                            while (walkTile.TileFrameX == 90)
                                walkTile.TileFrameX = (short)(18 * Main.rand.Next(4, 7));
                        }
                        else
                        {
                            walkTile.Get<TileWallWireStateData>().HasTile = true;
                            walkTile.TileType = TileID.HallowedPlants2;
                            walkTile.TileFrameX = (short)(18 * Main.rand.Next(2, 8));
                            while (walkTile.TileFrameX == 90)
                                walkTile.TileFrameX = (short)(18 * Main.rand.Next(2, 8));
                        }

                        if (Main.netMode == NetmodeID.MultiplayerClient)
                            NetMessage.SendTileSquare(-1, x, y, 1, TileChangeType.None);
                    }

                    // On jungle grass, grow jungle flowers.
                    else if (groundTile.TileType == TileID.JungleGrass)
                    {
                        walkTile.Get<TileWallWireStateData>().HasTile = true;
                        walkTile.TileType = TileID.JunglePlants2;
                        walkTile.TileFrameX = (short)(18 * Main.rand.Next(9, 17));

                        if (Main.netMode == NetmodeID.MultiplayerClient)
                            NetMessage.SendTileSquare(-1, x, y, 1, TileChangeType.None);
                    }
                }
            }
        }
    }
}
