
using CalamityMod.Items.Placeables.Ores;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Tiles.Ores
{
    public class ExodiumOre : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            CalamityUtils.MergeWithGeneral(Type);
            CalamityUtils.MergeWithSet(Type, TileID.LunarOre);

            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Exodium Ore");
            AddMapEntry(new Color(51, 48, 68), name);
            mineResist = 5f;
            minPick = 225;
            SoundType = SoundID.Tink;
            Main.tileValue[Type] = 760;
            Main.tileSpelunker[Type] = true;
            ItemDrop = ModContent.ItemType<ExodiumClusterOre>();
            base.SetDefaults();

            TileID.Sets.ChecksForMerge[Type] = true;
        }

        public override bool CanExplode(int i, int j) => false;

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 2 : 4;
    }
}
