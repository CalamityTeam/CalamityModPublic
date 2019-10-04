using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Utilities;

namespace CalamityMod.Tiles
{
    public class ExodiumOre : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            TileMerge.MergeGeneralTiles(Type);

            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Exodium Ore");
            AddMapEntry(new Color(51, 48, 68), name);
			mineResist = 5f;
			minPick = 225;
			soundType = 21;
			Main.tileValue[Type] = 760;
            Main.tileSpelunker[Type] = true;
            drop = mod.ItemType("ExodiumClusterOre");
            base.SetDefaults();
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 2 : 4;
        }
    }
}
