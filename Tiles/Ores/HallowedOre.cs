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
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileValue[Type] = 690;

            CalamityUtils.MergeWithGeneral(Type);
            drop = ModContent.ItemType<Items.Placeables.Ores.HallowedOre>();
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Hallowed Ore");
            AddMapEntry(new Color(250, 250, 150), name);
            mineResist = 3f;
            minPick = 180;
            soundType = SoundID.Tink;
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

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.07f;
            g = 0.07f;
            b = 0.04f;
        }
    }
}
