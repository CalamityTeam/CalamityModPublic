using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Tiles
{
    public class CryonicBarTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            this.SetUpBar(new Color(138, 43, 226));
            DustType = 44;
            ItemDrop = ModContent.ItemType<Items.Materials.CryonicBar>();
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            type = Main.rand.NextBool() ? 56 : 73;
            return true;
        }
    }
}
