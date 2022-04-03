using CalamityMod.Dusts.Furniture;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Tiles.FurnitureSilva
{
    public class SilvaBathtub : ModTile
    {
        public override void SetStaticDefaults()
        {
            this.SetUpBathtub(true);
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Bathtub");
            AddMapEntry(new Color(191, 142, 111), name);
            AnimationFrameHeight = 54;
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, ModContent.DustType<SilvaTileGold>(), 0f, 0f, 1, new Color(255, 255, 255), 1f);
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 157, 0f, 0f, 1, new Color(255, 255, 255), 1f);
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(i * 16, j * 16, 16, 32, ModContent.ItemType<Items.Placeables.FurnitureSilva.SilvaBathtub>());
        }
    }
}
