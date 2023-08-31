using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.FurnitureAbyss
{
    [LegacyName("AbyssWorkbench")]
    public class AbyssWorkBench : ModTile
    {
        public override void SetStaticDefaults() => this.SetUpWorkBench(ModContent.ItemType<Items.Placeables.FurnitureAbyss.AbyssWorkBench>(), true);

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 1, 0f, 0f, 1, new Color(100, 130, 150), 1f);
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
