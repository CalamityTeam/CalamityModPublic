using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace CalamityMod.Tiles
{
    public class AuricTeslaBar : ModTile
    {
        public override void SetDefaults()
        {
            this.SetUpBar(new Color(255, 227, 81));
            dustType = ModContent.DustType<AuricBarDust>();
            drop = ModContent.ItemType<AuricBar>();
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            type = ModContent.DustType<AuricBarDust>();
            return true;
        }
    }
}
