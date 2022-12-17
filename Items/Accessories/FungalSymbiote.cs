using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Accessories
{
    public class FungalSymbiote : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Fungal Symbiote");
            Tooltip.SetDefault("You grow mushrooms on the grass beneath you\n" +
                "Consuming mushrooms provides the Mushy buff, increasing defense by 6 and life regen by 2\n" +
                "All mushroom-based weapons deal 10% more damage and emit non-damaging mushrooms");
        }

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 36;
            Item.value = CalamityGlobalItem.Rarity3BuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.fungalSymbiote = true;

            // Mushroom growing effects.
            int x = (int)player.Center.X / 16;
            int y = (int)(player.Bottom.Y - 1f) / 16;
            Tile groundTile = CalamityUtils.ParanoidTileRetrieval(x, y + 1);
            if (player.whoAmI == Main.myPlayer && player.velocity.Y == 0f && player.grappling[0] == -1)
            {
                Tile walkTile = CalamityUtils.ParanoidTileRetrieval(x, y);
                if (!walkTile.HasTile && walkTile.LiquidAmount == 0 && groundTile != null && WorldGen.SolidTile(groundTile))
                {
                    walkTile.TileFrameY = 0;
                    walkTile.Get<TileWallWireStateData>().Slope = SlopeType.Solid;
                    walkTile.Get<TileWallWireStateData>().IsHalfBlock = false;

                    // On dirt, grow teal and green mushrooms.
                    if (groundTile.TileType == TileID.Dirt)
                    {
                        if (Main.rand.NextBool(1000))
                        {
                            walkTile.Get<TileWallWireStateData>().HasTile = true;
                            walkTile.TileType = TileID.DyePlants;
                            walkTile.TileFrameX = (short)(Main.rand.NextBool() ? 0 : 34);
                        }

                        if (Main.netMode == NetmodeID.MultiplayerClient)
                            NetMessage.SendTileSquare(-1, x, y, 1, TileChangeType.None);
                    }

                    // On grass, grow mushrooms.
                    else if (groundTile.TileType == TileID.Grass)
                    {
                        walkTile.Get<TileWallWireStateData>().HasTile = true;
                        walkTile.TileType = TileID.Plants;
                        walkTile.TileFrameX = 144;

                        if (Main.netMode == NetmodeID.MultiplayerClient)
                            NetMessage.SendTileSquare(-1, x, y, 1, TileChangeType.None);
                    }

                    // On hallowed grass, grow mushrooms.
                    else if (groundTile.TileType == TileID.HallowedGrass)
                    {
                        walkTile.Get<TileWallWireStateData>().HasTile = true;
                        walkTile.TileType = TileID.HallowedPlants;
                        walkTile.TileFrameX = 144;

                        if (Main.netMode == NetmodeID.MultiplayerClient)
                            NetMessage.SendTileSquare(-1, x, y, 1, TileChangeType.None);
                    }

                    // On corrupt grass, grow vile mushrooms.
                    else if (groundTile.TileType == TileID.CorruptGrass)
                    {
                        walkTile.Get<TileWallWireStateData>().HasTile = true;
                        walkTile.TileType = TileID.CorruptPlants;
                        walkTile.TileFrameX = 144;

                        if (Main.netMode == NetmodeID.MultiplayerClient)
                            NetMessage.SendTileSquare(-1, x, y, 1, TileChangeType.None);
                    }

                    // On crimson grass, grow vicious mushrooms.
                    else if (groundTile.TileType == TileID.CrimsonGrass)
                    {
                        walkTile.Get<TileWallWireStateData>().HasTile = true;
                        walkTile.TileType = TileID.CrimsonPlants;
                        walkTile.TileFrameX = 270;

                        if (Main.netMode == NetmodeID.MultiplayerClient)
                            NetMessage.SendTileSquare(-1, x, y, 1, TileChangeType.None);
                    }

                    // On glowing mushroom grass, grow glowing mushrooms.
                    else if (groundTile.TileType == TileID.MushroomGrass)
                    {
                        walkTile.Get<TileWallWireStateData>().HasTile = true;
                        walkTile.TileType = TileID.MushroomPlants;
                        walkTile.TileFrameX = (short)(Main.rand.Next(5) * 18);

                        if (Main.netMode == NetmodeID.MultiplayerClient)
                            NetMessage.SendTileSquare(-1, x, y, 1, TileChangeType.None);
                    }
                }
            }
        }
    }
}
