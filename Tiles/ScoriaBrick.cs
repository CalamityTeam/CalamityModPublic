using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Metadata;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Tiles
{
    [LegacyName("ChaoticBrick")]
    public class ScoriaBrick : ModTile
    {
        int subsheetHeight = 72;
        int subsheetWidth = 216;
        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            DustType = 105;
            ItemDrop = ModContent.ItemType<Items.Placeables.ScoriaBrick>();
            AddMapEntry(new Color(255, 0, 0));
            HitSound = SoundID.Tink;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.04f;
            g = 0.00f;
            b = 0.00f;
        }

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            int xPos = i % 1;
            int yPos = j % 2;

            frameXOffset = xPos * subsheetWidth;
            frameYOffset = yPos * subsheetHeight;
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            TileFraming.CompactFraming(i, j, resetFrame);
            return false;
        }
    }
}
