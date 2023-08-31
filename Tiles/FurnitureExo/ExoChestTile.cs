using CalamityMod.Items.Placeables.FurnitureExo;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.ObjectInteractions;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.FurnitureExo
{
    public class ExoChestTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            this.SetUpChest(ModContent.ItemType<ExoChest>(), true, 2);
            AddMapEntry(new Color(71, 95, 114), CalamityUtils.GetItemName<ExoChest>(), CalamityUtils.GetMapChestName);
        }

        public override bool CanExplode(int i, int j) => false;
        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 107, 0f, 0f, 1, new Color(255, 255, 255), 1f);
            return false;
        }
        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;

        public override LocalizedText DefaultContainerName(int frameX, int frameY) => CalamityUtils.GetItemName<ExoChest>();
        public override void MouseOver(int i, int j) => CalamityUtils.ChestMouseOver<ExoChest>(i, j);
        public override void MouseOverFar(int i, int j) => CalamityUtils.ChestMouseFar<ExoChest>(i, j);
        public override void KillMultiTile(int i, int j, int frameX, int frameY) => Chest.DestroyChest(i, j);
        public override bool RightClick(int i, int j) => CalamityUtils.ChestRightClick(i, j);

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];
            int yOffset = TileObjectData.GetTileData(tile).DrawYOffset;
            Texture2D glowmask = ModContent.Request<Texture2D>("CalamityMod/Tiles/FurnitureExo/ExoChestGlow").Value;
            Vector2 drawOffset = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
            Vector2 drawPosition = new Vector2(i * 16 - Main.screenPosition.X, j * 16 - Main.screenPosition.Y + yOffset) + drawOffset;
            Color drawColour = Color.White;
            Main.spriteBatch.Draw(glowmask, drawPosition, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), drawColour, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
    }
}
