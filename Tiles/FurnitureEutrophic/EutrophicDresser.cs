using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.FurnitureEutrophic
{
    public class EutrophicDresser : ModTile
    {
        public override void SetStaticDefaults()
        {
            this.SetUpDresser();
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Eutrophic Dresser");
            AddMapEntry(new Color(191, 142, 111), name);
            TileID.Sets.DisableSmartCursor[Type] = true;
            AdjTiles = new int[] { TileID.Dressers };
            dresser = "Eutrophic Dresser";
            dresserDrop = ModContent.ItemType<Items.Placeables.FurnitureEutrophic.EutrophicDresser>();
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 51, 0f, 0f, 1, new Color(54, 69, 72), 1f);
            return false;
        }

        public override bool HasSmartInteract()
        {
            return true;
        }

        public override bool RightClick(int i, int j)
        {
            return CalamityUtils.DresserRightClick();
        }

        public override void MouseOverFar(int i, int j)
        {
            CalamityUtils.DresserMouseFar<Items.Placeables.FurnitureEutrophic.EutrophicDresser>(chest);
        }

        public override void MouseOver(int i, int j)
        {
            CalamityUtils.DresserMouseOver<Items.Placeables.FurnitureEutrophic.EutrophicDresser>(chest);
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 48, 32, dresserDrop);
            Chest.DestroyChest(i, j);
        }
    }
}
