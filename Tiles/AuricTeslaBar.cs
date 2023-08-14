using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace CalamityMod.Tiles
{
    public class AuricTeslaBar : ModTile
    {
        public override void SetStaticDefaults()
        {
            this.SetUpBar(ModContent.ItemType<AuricBar>(), new Color(255, 227, 81));
            DustType = ModContent.DustType<AuricBarDust>();
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            type = ModContent.DustType<AuricBarDust>();
            return true;
        }
    }
}
