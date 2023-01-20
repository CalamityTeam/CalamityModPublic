using CalamityMod.Tiles.Ores;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Tiles.Crags
{
    public class ScorchedBone : ModTile
    {
        private int sheetWidth = 450;
        private int sheetHeight = 270;

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            CalamityUtils.MergeWithGeneral(Type);
            CalamityUtils.MergeWithHell(Type);
            CalamityUtils.SetMerge(Type, ModContent.TileType<BrimstoneSlag>());

            DustType = 155;
            HitSound = SoundID.Dig;
            MineResist = 1f;
            MinPick = 100;
            ItemDrop = ModContent.ItemType<Items.Placeables.ScorchedBone>();
            AddMapEntry(new Color(87, 62, 67));
        }

        public override bool CanExplode(int i, int j)
        {
            return false;
        }

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            frameXOffset = i % 3 * sheetWidth;
            frameYOffset = j % 3 * sheetHeight;
        }
        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            return TileFraming.BrimstoneFraming(i, j, resetFrame);
        }
    }
}
