using CalamityMod.Items.Fishing.SulphurCatches;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.Abyss
{
    [LegacyName("AbyssalCrateTile")]
    public class SulphurousCrateTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolidTop[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileTable[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileWaterDeath[Type] = false;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(47, 79, 79), CalamityUtils.GetItemName<SulphurousCrate>()); // dark slate gray
            DustType = 33;
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            if (Main.rand.NextBool())
                type = 44;
            else
                type = 33;

            return true;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
