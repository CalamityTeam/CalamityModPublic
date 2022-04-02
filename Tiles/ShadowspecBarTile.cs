using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace CalamityMod.Tiles
{
    public class ShadowspecBarTile : ModTile
    {
        public override void SetDefaults()
        {
            this.SetUpBar(new Color(128, 41, 149));
            dustType = ModContent.DustType<ShadowspecBarDust>();
            drop = ModContent.ItemType<ShadowspecBar>();
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            type = ModContent.DustType<ShadowspecBarDust>();
            return true;
        }
    }
}
