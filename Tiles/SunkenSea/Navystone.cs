using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.SunkenSea
{
    public class Navystone : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            CalamityUtils.MergeWithGeneral(Type);
            CalamityUtils.MergeWithDesert(Type);
            CalamityUtils.SetMerge(Type, ModContent.TileType<EutrophicSand>());

            TileID.Sets.ChecksForMerge[Type] = true;
            DustType = 96;
            ItemDrop = ModContent.ItemType<Items.Placeables.Navystone>();
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Navystone");
            AddMapEntry(new Color(0, 90, 90), name);
            MineResist = 2f;
            HitSound = SoundID.Tink;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            return TileFraming.BrimstoneFraming(i, j, resetFrame);
        }
    }
}
