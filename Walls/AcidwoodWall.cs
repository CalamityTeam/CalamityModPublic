using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables.Walls;
using Terraria;
using Terraria.ID;

namespace CalamityMod.Walls
{
    public class AcidwoodWall : ModWall
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            ItemDrop = ModContent.ItemType<AcidwoodWallItem>();
            HitSound = SoundID.Dig;
            AddMapEntry(new Color(96, 69, 39));
        }

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 7, 0f, 0f, 1, new Color(255, 255, 255), 1f);
            return false;
        }
    }
}
