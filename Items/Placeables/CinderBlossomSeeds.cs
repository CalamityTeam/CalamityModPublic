using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Items.Placeables
{
    public class CinderBlossomSeeds : ModItem
	{
        public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 25;
		}

		public override void SetDefaults()
		{
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;
            Item.width = 16;
			Item.height = 16;
			Item.useTime = 10;
			Item.useAnimation = 15;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.maxStack = 9999;
		}

        public override bool? UseItem(Player player)
		{
			Tile tile = Framing.GetTileSafely(Player.tileTargetX, Player.tileTargetY);
			Tile tileAbove = Framing.GetTileSafely(Player.tileTargetX, Player.tileTargetY - 1);
			
			if (tile.HasTile && !tileAbove.HasTile && tileAbove.LiquidAmount == 0 && tile.TileType == ModContent.TileType<Tiles.Crags.ScorchedRemains>() && player.IsInTileInteractionRange(Player.tileTargetX, Player.tileTargetY, TileReachCheckSettings.Simple))
			{
				Main.tile[Player.tileTargetX, Player.tileTargetY].TileType = (ushort)ModContent.TileType<Tiles.Crags.ScorchedRemainsGrass>();

				SoundEngine.PlaySound(SoundID.Dig, player.Center);

				return true;
			}

			return false;
		}
    }
}
