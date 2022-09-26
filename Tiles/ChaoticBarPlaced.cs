using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Items.Materials;

namespace CalamityMod.Tiles
{
    public class ChaoticBarPlaced : ModTile
    {
        public override void SetStaticDefaults()
        {
            this.SetUpBar(new Color(255, 165, 0));
            DustType = 87;
            ItemDrop = ModContent.ItemType<ScoriaBar>();
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            type = Main.rand.NextBool() ? 87 : 6;
            return true;
        }
    }
}
