using CalamityMod.Items.Placeables.Furniture;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Abyss
{
    public class RustyChestTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            this.SetUpChest();
            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(113, 90, 71), CalamityUtils.GetItemName<RustyChest>(), CalamityUtils.GetMapChestName);
            TileID.Sets.DisableSmartCursor[Type] = true;
            AdjTiles = new int[] { TileID.Containers };
        }

        public override LocalizedText DefaultContainerName(int frameX, int frameY) => CalamityUtils.GetItemName<RustyChest>();

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 180, 0f, 0f, 1, new Color(255, 255, 255), 1f);
            return false;
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Chest.DestroyChest(i, j);
        }

        public override bool RightClick(int i, int j)
        {
            return CalamityUtils.ChestRightClick(i, j);
        }

        public override void MouseOver(int i, int j)
        {
            CalamityUtils.ChestMouseOver<RustyChest>(i, j);
        }

        public override void MouseOverFar(int i, int j)
        {
            CalamityUtils.ChestMouseFar<RustyChest>(i, j);
        }
    }
}
