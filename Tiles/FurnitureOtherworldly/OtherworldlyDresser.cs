using CalamityMod.Dusts.Furniture;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.ObjectInteractions;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.FurnitureOtherworldly
{
    [LegacyName("OccultDresser")]
    public class OtherworldlyDresser : ModTile
    {
        public override void SetStaticDefaults()
        {
            this.SetUpDresser(ModContent.ItemType<Items.Placeables.FurnitureOtherworldly.OtherworldlyDresser>());
            AddMapEntry(new Color(191, 142, 111), CalamityUtils.GetItemName<Items.Placeables.FurnitureOtherworldly.OtherworldlyDresser>(), CalamityUtils.GetMapChestName);
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 1, 0f, 0f, 1, new Color(125, 94, 128), 1f);
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, ModContent.DustType<OtherworldlyTileCloth>(), 0f, 0f, 1, new Color(255, 255, 255), 1f);
            return false;
        }
        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;

        public override LocalizedText DefaultContainerName(int frameX, int frameY) => CalamityUtils.GetItemName<Items.Placeables.FurnitureOtherworldly.OtherworldlyDresser>();
        public override void MouseOver(int i, int j) => CalamityUtils.DresserMouseOver<Items.Placeables.FurnitureOtherworldly.OtherworldlyDresser>();
        public override void MouseOverFar(int i, int j) => CalamityUtils.DresserMouseFar<Items.Placeables.FurnitureOtherworldly.OtherworldlyDresser>();
        public override void KillMultiTile(int i, int j, int frameX, int frameY) => Chest.DestroyChest(i, j);
        public override bool RightClick(int i, int j) => CalamityUtils.DresserRightClick();
    }
}
