using CalamityMod.Items.Placeables.FurnitureAcidwood;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.ObjectInteractions;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.FurnitureAcidwood
{
    public class AcidwoodChestTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            this.SetUpChest(ModContent.ItemType<AcidwoodChest>(), true);
            AddMapEntry(new Color(191, 142, 111), CalamityUtils.GetItemName<AcidwoodChest>(), CalamityUtils.GetMapChestName);
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 7, 0f, 0f, 1, new Color(255, 255, 255), 1f);
            return false;
        }
        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;

        public override LocalizedText DefaultContainerName(int frameX, int frameY) => CalamityUtils.GetItemName<AcidwoodChest>();
        public override void MouseOver(int i, int j) => CalamityUtils.ChestMouseOver<AcidwoodChest>(i, j);
        public override void MouseOverFar(int i, int j) => CalamityUtils.ChestMouseFar<AcidwoodChest>(i, j);
        public override void KillMultiTile(int i, int j, int frameX, int frameY) => Chest.DestroyChest(i, j);
        public override bool RightClick(int i, int j) => CalamityUtils.ChestRightClick(i, j);
    }
}
