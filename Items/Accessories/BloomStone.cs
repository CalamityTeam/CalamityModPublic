using CalamityMod.CalPlayer;
using Terraria;
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
                "You quickly regenerate life while on the ground\n" +
                "This effect works best during daytime\n" +
                "Flowers grow if you are standing on grass\n" +
                "Random dye plants will grow while standing on grassless dirt");
        }

        public override void SetDefaults()
        {
            item.width = 38;
            item.height = 54;
            item.value = CalamityGlobalItem.Rarity7BuyPrice;
            item.rare = ItemRarityID.Lime;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            Lighting.AddLight((int)player.Center.X / 16, (int)player.Center.Y / 16, 0.25f, 0.4f, 0.2f);

            // Provide life benefits if the player is standing on ground and has typical gravity.
            int x = (int)player.Center.X / 16;
            int y = (int)(player.Bottom.Y - 1f) / 16;
            Tile groundTile = CalamityUtils.ParanoidTileRetrieval(x, y + 1);
            bool groundTileIsSolid = groundTile.nactive() && (Main.tileSolid[groundTile.type] || Main.tileSolidTop[groundTile.type]);
            if (groundTileIsSolid && player.gravDir == 1f)
                modPlayer.BloomStoneRegen = true;

            // Flower boots effects.
            if (player.whoAmI == Main.myPlayer && player.velocity.Y == 0f && player.grappling[0] == -1)
            {
                Tile walkTile = CalamityUtils.ParanoidTileRetrieval(x, y);
                if (!walkTile.active() && walkTile.liquid == 0 && groundTile != null && WorldGen.SolidTile(groundTile))
                {
                    walkTile.frameY = 0;
                    walkTile.slope(0);
                    walkTile.halfBrick(false);
                    // On dirt blocks, there's a small chance to grow a dye plant.
                    if (groundTile.type == TileID.Dirt)
                    {
                        if (Main.rand.NextBool(1000))
                        {
                            walkTile.active(true);
                            walkTile.type = TileID.DyePlants;
                            walkTile.frameX = (short)(34 * Main.rand.Next(1, 13));
                            while (walkTile.frameX == 144)
                                walkTile.frameX = (short)(34 * Main.rand.Next(1, 13));
                        }

                        if (Main.netMode == NetmodeID.MultiplayerClient)
                            NetMessage.SendTileSquare(-1, x, y, 1, TileChangeType.None);
                    }

                    // On grass, grow flowers.
                    if (groundTile.type == TileID.Grass)
                    {
                        if (Main.rand.NextBool(2))
                        {
                            walkTile.active(true);
                            walkTile.type = TileID.Plants;
                            walkTile.frameX = (short)(18 * Main.rand.Next(6, 11));

                            while (walkTile.frameX == 144)
                                walkTile.frameX = (short)(18 * Main.rand.Next(6, 11));
                        }
                        else
                        {
                            walkTile.active(true);
                            walkTile.type = TileID.Plants2;
                            walkTile.frameX = (short)(18 * Main.rand.Next(6, 21));

                            while (walkTile.frameX == 144)
                                walkTile.frameX = (short)(18 * Main.rand.Next(6, 21));
                        }

                        if (Main.netMode == NetmodeID.MultiplayerClient)
                            NetMessage.SendTileSquare(-1, x, y, 1, TileChangeType.None);
                    }

                    // On hallowed grass, grow hallowed flowers.
                    else if (groundTile.type == TileID.HallowedGrass)
                    {
                        if (Main.rand.NextBool(2))
                        {
                            walkTile.active(true);
                            walkTile.type = TileID.HallowedPlants;
                            walkTile.frameX = (short)(18 * Main.rand.Next(4, 7));
                            while (walkTile.frameX == 90)
                                walkTile.frameX = (short)(18 * Main.rand.Next(4, 7));
                        }
                        else
                        {
                            walkTile.active(true);
                            walkTile.type = TileID.HallowedPlants2;
                            walkTile.frameX = (short)(18 * Main.rand.Next(2, 8));
                            while (walkTile.frameX == 90)
                                walkTile.frameX = (short)(18 * Main.rand.Next(2, 8));
                        }

                        if (Main.netMode == NetmodeID.MultiplayerClient)
                            NetMessage.SendTileSquare(-1, x, y, 1, TileChangeType.None);
                    }

                    // On jungle grass, grow jungle flowers.
                    else if (groundTile.type == TileID.JungleGrass)
                    {
                        walkTile.active(true);
                        walkTile.type = TileID.JunglePlants2;
                        walkTile.frameX = (short)(18 * Main.rand.Next(9, 17));

                        if (Main.netMode == NetmodeID.MultiplayerClient)
                            NetMessage.SendTileSquare(-1, x, y, 1, TileChangeType.None);
                    }
                }
            }
        }
    }
}
