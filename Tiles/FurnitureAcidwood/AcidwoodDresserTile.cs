using CalamityMod.Items.Placeables.FurnitureAcidwood;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.FurnitureAcidwood
{
    public class AcidwoodDresserTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            this.SetUpDresser();
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Acidwood Dresser");
            AddMapEntry(new Color(191, 142, 111), name);
            TileID.Sets.DisableSmartCursor[Type] = true;
            AdjTiles = new int[] { TileID.Dressers };
            dresser = "Acidwood Dresser";
            dresserDrop = ModContent.ItemType<AcidwoodDresser>();
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 7, 0f, 0f, 1, new Color(255, 255, 255), 1f);
            return false;
        }

        public override bool HasSmartInteract() => true;

        public override bool RightClick(int i, int j) => CalamityUtils.DresserRightClick();

        public override void MouseOverFar(int i, int j) => CalamityUtils.DresserMouseFar<AcidwoodDresser>(chest);

        public override void MouseOver(int i, int j) => CalamityUtils.DresserMouseOver<AcidwoodDresser>(chest);

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(i * 16, j * 16, 48, 32, dresserDrop);
            Chest.DestroyChest(i, j);
        }
    }
}
