using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.FurnitureCosmilite
{
    public class CosmiliteDresser : ModTile
    {
        public override void SetStaticDefaults()
        {
            this.SetUpDresser();
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Cosmilite Dresser");
            AddMapEntry(new Color(191, 142, 111), name);
            TileID.Sets.DisableSmartCursor[Type] = true;
            AdjTiles = new int[] { TileID.Dressers };
            ContainerName.SetDefault("Cosmilite Dresser");
            DresserDrop = ModContent.ItemType<Items.Placeables.FurnitureCosmilite.CosmiliteDresser>();
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 132, 0f, 0f, 1, new Color(255, 255, 255), 1f);
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 134, 0f, 0f, 1, new Color(255, 255, 255), 1f);
            return false;
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

        public override bool RightClick(int i, int j)
        {
            return CalamityUtils.DresserRightClick();
        }

        public override void MouseOverFar(int i, int j)
        {
            CalamityUtils.DresserMouseFar<Items.Placeables.FurnitureCosmilite.CosmiliteDresser>(chest);
        }

        public override void MouseOver(int i, int j)
        {
            CalamityUtils.DresserMouseOver<Items.Placeables.FurnitureCosmilite.CosmiliteDresser>(chest);
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 48, 32, DresserDrop);
            Chest.DestroyChest(i, j);
        }
    }
}
