using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace CalamityMod.Tiles
{
    public class ShadowspecBarTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            this.SetUpBar(ModContent.ItemType<ShadowspecBar>(), new Color(128, 41, 149));
            DustType = ModContent.DustType<ShadowspecBarDust>();
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            type = ModContent.DustType<ShadowspecBarDust>();
            return true;
        }
    }
}
