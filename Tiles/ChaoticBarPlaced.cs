using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Items.Materials;

namespace CalamityMod.Tiles
{
    public class ChaoticBarPlaced : ModTile
    {
        public override void SetDefaults()
        {
            this.SetUpBar(new Color(255, 165, 0));
            dustType = 87;
            drop = ModContent.ItemType<CruptixBar>();
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            type = Main.rand.NextBool() ? 87 : 6;
            return true;
        }
    }
}
