using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Ores
{
    public class CryonicOre : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileOreFinderPriority[Type] = 675;

            CalamityUtils.MergeWithGeneral(Type);
            CalamityUtils.MergeWithSnow(Type);

            ItemDrop = ModContent.ItemType<Items.Placeables.Ores.CryonicOre>();
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Cryonic Ore");
            AddMapEntry(new Color(0, 0, 150), name);
            MineResist = 3f;
            MinPick = 180;
            HitSound = SoundID.Tink;
            Main.tileSpelunker[Type] = true;
        }

        public override bool CanExplode(int i, int j)
        {
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            TileFraming.CustomMergeFrame(i, j, Type, TileID.SnowBlock);
            return false;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.02f;
            g = 0.02f;
            b = 0.06f;
        }
    }
}
