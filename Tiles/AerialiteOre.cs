using CalamityMod.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Tiles
{
    public class AerialiteOre : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileValue[Type] = 450;
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;

            TileMerge.MergeGeneralTiles(Type);

            dustType = ModContent.DustType<AHSparkle>();
            drop = ModContent.ItemType<AerialiteOre>();
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Aerialite Ore");
            AddMapEntry(new Color(0, 255, 255), name);
            mineResist = 2f;
            minPick = 65;
            soundType = 21;
            Main.tileSpelunker[Type] = true;
        }

        public override bool CanExplode(int i, int j)
        {
            return NPC.downedBoss2;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
