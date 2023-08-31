using CalamityMod.Items.Placeables.FurnitureSacrilegious;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.ObjectInteractions;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.FurnitureSacrilegious
{
    public class SacrilegiousChestTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            this.SetUpChest(ModContent.ItemType<SacrilegiousChest>(), true);
            AddMapEntry(new Color(43, 19, 42), CalamityUtils.GetItemName<SacrilegiousChest>(), CalamityUtils.GetMapChestName);
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 8, 0f, 0f, 1, new Color(255, 255, 255), 1f);
            return false;
        }
        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;

        public override LocalizedText DefaultContainerName(int frameX, int frameY) => CalamityUtils.GetItemName<SacrilegiousChest>();
        public override void MouseOver(int i, int j) => CalamityUtils.ChestMouseOver<SacrilegiousChest>(i, j);
        public override void MouseOverFar(int i, int j) => CalamityUtils.ChestMouseFar<SacrilegiousChest>(i, j);
        public override void KillMultiTile(int i, int j, int frameX, int frameY) => Chest.DestroyChest(i, j);
        public override bool RightClick(int i, int j) => CalamityUtils.ChestRightClick(i, j);

        // Make the chest brighter the more stuff it has
        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            int seed = 1;
            int itemAmt = 0;
            int index = FindChestIndex(i, j, ref seed);
            if (index >= 0)
                itemAmt = CountItems(Main.chest[index]);

            Tile tile = Main.tile[i, j];
            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Tiles/FurnitureSacrilegious/SacrilegiousChestTileGlow").Value;

            int x = i;
            int y = j;
            switch (seed)
            {
                case 2:
                    y = j - 1;
                    break;
                case 3:
                    x = i - 1;
                    break;
                case 4:
                    x = i - 1;
                    y = j - 1;
                    break;
            }
            ulong seeding = Main.TileFrameSeed ^ (ulong)((long)y << 32 | (long)(uint)x);

            int yOffset = TileObjectData.GetTileData(tile).DrawYOffset;
            Vector2 drawOffset = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
            Vector2 drawPosition = new Vector2(i * 16 - Main.screenPosition.X, j * 16 - Main.screenPosition.Y + yOffset) + drawOffset;

            int alpha = 255 - (int)(MathHelper.Lerp(0, 7, itemAmt / 40f) * 15);
            Color color = new Color(120, 100, 100, alpha);

            int loopAmt = itemAmt > 0 ? (itemAmt - 1) / 10 + 4 : 4;
            float shakeAmt = MathHelper.Lerp(0f, 0.3f, itemAmt / 40f);
            if (itemAmt == 0)
                loopAmt = 0;

            for (int c = 0; c < loopAmt; c++)
            {
                float shakeX = Utils.RandomInt(ref seeding, -5, 5) * shakeAmt;
                float shakeY = Utils.RandomInt(ref seeding, -5, 5) * shakeAmt;
                Vector2 shake = new Vector2(shakeX, shakeY);
                spriteBatch.Draw(texture, drawPosition + shake, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
        }

        private int FindChestIndex(int i, int j, ref int seed)
        {
            int index = Chest.FindChest(i, j);
            if (index < 0 && Main.tile[i, j - 1].TileType == ModContent.TileType<SacrilegiousChestTile>())
            {
                index = Chest.FindChest(i, j - 1);
                seed = 2;
            }
            if (index < 0 && Main.tile[i - 1, j].TileType == ModContent.TileType<SacrilegiousChestTile>())
            {
                index = Chest.FindChest(i - 1, j);
                seed = 3;
            }
            if (index < 0 && Main.tile[i - 1, j - 1].TileType == ModContent.TileType<SacrilegiousChestTile>())
            {
                index = Chest.FindChest(i - 1, j - 1);
                seed = 4;
            }
            return index;
        }

        private int CountItems(Chest chest)
        {
            if (chest is null)
                return -1;
            int amt = 0;
            for (int i = 0; i < Chest.maxItems; i++)
            {
                if (chest.item[i] is null || chest.item[i].IsAir)
                    continue;
                if (chest.item[i].stack > 0)
                    amt++;
            }
            return amt;
        }
        
        byte Average(byte a, byte b) => (byte)((a + b) / 2);
    }
}
