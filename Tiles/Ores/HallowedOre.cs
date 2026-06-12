using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Ores
{
    public class HallowedOre : ModTile
    {

        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileOreFinderPriority[Type] = 690;

            TileID.Sets.Ore[Type] = true;
            TileID.Sets.OreMergesWithMud[Type] = true;

            Main.tileShine[Type] = 2000;
            Main.tileShine2[Type] = true;

            CalamityUtils.MergeWithGeneral(Type);
            AddMapEntry(new Color(250, 250, 150), CreateMapEntryName());
            MineResist = 2f;
            MinPick = 180;
            HitSound = SoundID.Tink;
            Main.tileSpelunker[Type] = true;

            this.RegisterBlendMergeWith(TileID.Pearlstone);
        }

        public override bool CanExplode(int i, int j)
        {
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 224f / 600f;
            g = 219f / 600f;
            b = 124f / 600f;
        }
    }
}
