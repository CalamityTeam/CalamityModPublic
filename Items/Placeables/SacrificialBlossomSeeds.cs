using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables
{
    public class SacrificialBlossomSeeds : ModItem
	{
        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Sacrificial Blossom Seeds");
			Tooltip.SetDefault("Places sacrificial blossom grass on scorched remains");
		}

		public override void SetDefaults()
		{
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;
            Item.width = 16;
			Item.height = 16;
			Item.useTime = 15;
			Item.useAnimation = 15;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.maxStack = 999;
		}

        public override bool? UseItem(Player player)
		{
			Tile tile = Framing.GetTileSafely(Player.tileTargetX, Player.tileTargetY);
			Tile tileAbove = Framing.GetTileSafely(Player.tileTargetX, Player.tileTargetY - 1);
			
			if (tile.HasTile && !tileAbove.HasTile && tile.TileType == ModContent.TileType<Tiles.Crags.ScorchedRemains>() && player.IsInTileInteractionRange(Player.tileTargetX, Player.tileTargetY))
			{
				Main.tile[Player.tileTargetX, Player.tileTargetY].TileType = (ushort)ModContent.TileType<Tiles.Crags.ScorchedRemainsGrass>();

				return true;
			}

			return false;
		}
    }
}