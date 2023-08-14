using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Tiles
{
    // TODO -- this name is inconsistent with all other placed bars, and needs to be updated for Scoria
    public class ChaoticBarPlaced : ModTile
    {
        public override void SetStaticDefaults()
        {
            this.SetUpBar(ModContent.ItemType<ScoriaBar>(), new Color(255, 165, 0));
            DustType = 87;
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            type = Main.rand.NextBool() ? 87 : 6;
            return true;
        }
    }
}
