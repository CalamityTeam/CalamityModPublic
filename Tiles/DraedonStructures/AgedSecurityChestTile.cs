using CalamityMod.Items.Placeables.DraedonStructures;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.DraedonStructures
{
    public class AgedSecurityChestTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            this.SetUpChest();
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Aged Security Chest");
            AddMapEntry(new Color(130, 119, 115), name, MapChestName);
            TileID.Sets.DisableSmartCursor[Type] = true;
            AdjTiles = new int[] { TileID.Containers };
            chest = "Aged Security Chest";
            chestDrop = ModContent.ItemType<AgedSecurityChest>();
        }

        public override bool CanExplode(int i, int j) => false;

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 226, 0f, 0f, 1, new Color(255, 255, 255), 1f);
            return false;
        }

        public override bool HasSmartInteract() => true;

        public string MapChestName(string name, int i, int j) => CalamityUtils.GetMapChestName(name, i, j);

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(i * 16, j * 16, 32, 32, chestDrop);
            Chest.DestroyChest(i, j);
        }

        public override bool RightClick(int i, int j) => CalamityUtils.ChestRightClick(i, j);

        public override void MouseOver(int i, int j)
        {
            CalamityUtils.ChestMouseOver<AgedSecurityChest>("Security Chest", i, j);
        }

        public override void MouseOverFar(int i, int j)
        {
            CalamityUtils.ChestMouseFar<AgedSecurityChest>("Security Chest", i, j);
        }
    }
}
