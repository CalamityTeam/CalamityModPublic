using CalamityMod.Utilities;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Tiles
{
    public class AuricOre : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileSpelunker[Type] = true;
            Main.tileValue[Type] = 810;

            TileMerge.MergeGeneralTiles(Type);

            dustType = 55;
            drop = ModContent.ItemType<AuricOre>();
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Auric Ore");
            AddMapEntry(new Color(255, 200, 0), name);
            mineResist = 10f;
            minPick = 275;
            soundType = 21;
        }

        public override bool CanExplode(int i, int j)
        {
            return CalamityWorld.downedYharon;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.20f;
            g = 0.16f;
            b = 0.00f;
        }
    }
}
