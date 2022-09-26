using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Walls
{
    [LegacyName("ChaoticBrickWall")]
    public class ScoriaBrickWall : ModWall
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            ItemDrop = ModContent.ItemType<Items.Placeables.Walls.ScoriaBrickWall>();
            AddMapEntry(new Color(255, 0, 0));
            DustType = 105;
        }

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
    }
}
