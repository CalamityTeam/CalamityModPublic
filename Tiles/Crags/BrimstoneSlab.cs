using CalamityMod.Tiles.Crags;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Tiles.Crags
{
    public class BrimstoneSlab : ModTile
    {
        private int sheetWidth = 450;
        private int sheetHeight = 270;

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            CalamityUtils.MergeWithGeneral(Type);
            CalamityUtils.MergeWithHell(Type);

            ItemDrop = ModContent.ItemType<Items.Placeables.BrimstoneSlab>();
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Brimstone Slab");
            AddMapEntry(new Color(79, 55, 70), name);
            MineResist = 3f;
            MinPick = 100;
            HitSound = SoundID.Tink;
            DustType = 235;
        }

        public override bool CanExplode(int i, int j)
        {
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            frameXOffset = i % 2 * sheetWidth;
            frameYOffset = j % 2 * sheetHeight;
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            return TileFraming.BrimstoneFraming(i, j, resetFrame);
        }
    }
}
