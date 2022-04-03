using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Items.Materials;

namespace CalamityMod.Tiles
{
    public class PerennialBar : ModTile
    {
        public override void SetStaticDefaults()
        {
            this.SetUpBar(new Color(157, 255, 0));
            dustType = 44;
            drop = ModContent.ItemType<DraedonBar>();
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            type = Main.rand.NextBool() ? 44 : 157;
            return true;
        }
    }
}
