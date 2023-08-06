using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.GameContent.ObjectInteractions;
using Microsoft.Xna.Framework;

namespace CalamityMod.Tiles.Abyss
{
	public class AbyssTreasureChest : ModTile
	{
		public override void SetStaticDefaults() 
		{
			this.SetUpChest();
			AddMapEntry(new Color(71, 49, 41), CalamityUtils.GetItemName<Items.Placeables.Furniture.AbyssTreasureChest>(), CalamityUtils.GetMapChestName);
			DustType = 33;
		}

		public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;
		public override void NumDust(int i, int j, bool fail, ref int num) => num = 1;

		public override LocalizedText DefaultContainerName(int frameX, int frameY) => CalamityUtils.GetItemName<Items.Placeables.Furniture.AbyssTreasureChest>();
		public override void MouseOver(int i, int j) => CalamityUtils.ChestMouseOver<Items.Placeables.Furniture.AbyssTreasureChest>(i, j);
		public override void MouseOverFar(int i, int j) => CalamityUtils.ChestMouseFar<Items.Placeables.Furniture.AbyssTreasureChest>(i, j);
		public override void KillMultiTile(int i, int j, int frameX, int frameY) => Chest.DestroyChest(i, j);

		// Locked Chest stuff
		public override bool IsLockedChest(int i, int j) => Main.tile[i, j].TileFrameX / 36 == 1;
		public override bool UnlockChest(int i, int j, ref short frameXAdjustment, ref int dustType, ref bool manual) => true;

		public override bool RightClick(int i, int j) 
		{
			Tile tile = Main.tile[i, j];

			int left = i;
			int top = j;

			if (tile.TileFrameX % 36 != 0) 
				left--;
			if (tile.TileFrameY != 0) 
				top--;

			return CalamityUtils.LockedChestRightClick(IsLockedChest(left, top), left, top, i, j);
		}
	}
}
