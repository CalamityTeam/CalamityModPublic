using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.LivingFire
{
    public class LivingBrimstoneFireBlockTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            TileID.Sets.CanPlaceNextToNonSolidTile[Type] = true;
            DustType = (int)CalamityDusts.Brimstone;
            AddMapEntry(new Color(178, 34, 34));
            AnimationFrameHeight = 90;
        }

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY) => offsetY = 2;

        public override void AnimateTile(ref int frame, ref int frameCounter) => frame = Main.tileFrame[TileID.LivingFire];

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 1f;
            g = 0f;
            b = 0f;
        }
    }
}
