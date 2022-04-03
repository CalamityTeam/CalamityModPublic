using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace CalamityMod.Tiles
{
    public class CosmiliteBarTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            this.SetUpBar(new Color(229, 141, 246));
            dustType = ModContent.DustType<CosmiliteBarDust>();
            drop = ModContent.ItemType<CosmiliteBar>();
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            type = ModContent.DustType<CosmiliteBarDust>();
            return true;
        }
    }
}
