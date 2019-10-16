using CalamityMod.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Tiles
{
    public class AbyssGravel : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileMergeDirt[Type] = true;

            TileMerge.MergeGeneralTiles(Type);
            TileMerge.MergeAbyssTiles(Type);

            drop = ModContent.ItemType<AbyssGravel>();
            AddMapEntry(new Color(0, 0, 0));
            mineResist = 10f;
            minPick = 65;
            soundType = 21;
            dustType = 33;
        }

        public override bool CanExplode(int i, int j)
        {
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
