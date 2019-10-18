using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Tiles.FurnitureVoid
{
    public class VoidDresser : ModTile
    {
        public override void SetDefaults()
        {
            CalamityUtils.SetUpDresser(Type);
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Void Dresser");
            AddMapEntry(new Color(191, 142, 111), name);
            disableSmartCursor = true;
            adjTiles = new int[] { TileID.Dressers };
            dresser = "Void Dresser";
            dresserDrop = ModContent.ItemType<Items.VoidDresser>();
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 180, 0f, 0f, 1, new Color(255, 255, 255), 1f);
            return false;
        }

        public override bool HasSmartInteract()
        {
            return true;
        }

        public override bool NewRightClick(int i, int j)
        {
            return CalamityUtils.DresserRightClick();
        }

        public override void MouseOverFar(int i, int j)
        {
            CalamityUtils.DresserMouseFar("VoidDresser", chest, i, j);
        }

        public override void MouseOver(int i, int j)
        {
            CalamityUtils.DresserMouseOver("VoidDresser", chest, i, j);
        }

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
