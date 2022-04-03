
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Tiles.FurnitureAbyss
{
    public class SmoothAbyssGravel : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;

            CalamityUtils.MergeWithGeneral(Type);
            CalamityUtils.MergeSmoothTiles(Type);
            CalamityUtils.MergeDecorativeTiles(Type);
            CalamityUtils.MergeWithAbyss(Type);

            SoundType = SoundID.Tink;
            MineResist = 2.1f;
            ItemDrop = ModContent.ItemType<Items.Placeables.FurnitureAbyss.SmoothAbyssGravel>();
            AddMapEntry(new Color(49, 56, 77));
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 1, 0f, 0f, 1, new Color(100, 130, 150), 1f);
            return false;
        }
    }
}
