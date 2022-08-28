using CalamityMod.Items.Placeables.FurnitureSacrilegious;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.FurnitureSacrilegious
{
    public class SacrilegiousChestTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            this.SetUpChest(true);
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Sacrilegious Chest");
            AddMapEntry(new Color(43, 19, 42), name, MapChestName);
            TileID.Sets.DisableSmartCursor[Type] = true;
            AdjTiles = new int[] { TileID.Containers };
            ContainerName.SetDefault("Sacrilegious Chest");
            ChestDrop = ModContent.ItemType<SacrilegiousChest>();
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 7, 0f, 0f, 1, new Color(255, 255, 255), 1f);
            return false;
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

        public string MapChestName(string name, int i, int j) => CalamityUtils.GetMapChestName(name, i, j);

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 32, ChestDrop);
            Chest.DestroyChest(i, j);
        }

        public override bool RightClick(int i, int j) => CalamityUtils.ChestRightClick(i, j);

        public override void MouseOver(int i, int j) => CalamityUtils.ChestMouseOver<SacrilegiousChest>("Sacrilegious Chest", i, j);

        public override void MouseOverFar(int i, int j) => CalamityUtils.ChestMouseFar<SacrilegiousChest>("Sacrilegious Chest", i, j);

		// Make the chest brighter the more stuff it has
        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
			int brightness = 0;
			int itemAmt = 0;
			int index = Chest.FindChest(i, j);
			if (index < 0 && Main.tile[i, j - 1].TileType == ModContent.TileType<SacrilegiousChestTile>())
				index = Chest.FindChest(i, j - 1);
			if (index < 0 && Main.tile[i - 1, j].TileType == ModContent.TileType<SacrilegiousChestTile>())
				index = Chest.FindChest(i - 1, j);
			if (index < 0 && Main.tile[i - 1, j - 1].TileType == ModContent.TileType<SacrilegiousChestTile>())
				index = Chest.FindChest(i - 1, j - 1);

			if (index >= 0)
			{
				itemAmt = CountItems(Main.chest[index]);
			}

			if (itemAmt > 0)
				brightness = (itemAmt - 1) / 5;

            Tile tile = Main.tile[i, j];
            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Tiles/FurnitureSacrilegious/SacrilegiousChestTileGlow").Value;
            int yOffset = TileObjectData.GetTileData(tile).DrawYOffset;
            Vector2 drawOffset = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
            Vector2 drawPosition = new Vector2(i * 16 - Main.screenPosition.X, j * 16 - Main.screenPosition.Y + yOffset) + drawOffset;
			int alpha = brightness * 14;
			Color color = new Color(alpha, 100, 100, 0);
            for (int c = 0; c < brightness; c++)
            {
				spriteBatch.Draw(texture, drawPosition, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
			}
		}

		private int CountItems(Chest chest)
		{
			if (chest is null)
				return -1;
			int amt = 0;
            for (int i = 0; i < Chest.maxItems; i++)
            {
				if (chest.item[i].IsAir)
					continue;
				if (chest.item[i].stack > 0)
					amt++;
			}
			return amt;
		}
    }
}
