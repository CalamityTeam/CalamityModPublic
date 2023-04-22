using CalamityMod.Dusts.Furniture;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.FurnitureProfaned
{
    public class ProfanedPiano : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = false;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(191, 142, 111), Language.GetText("ItemName.Piano"));
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 246, 0f, 0f, 1, new Color(255, 255, 255), 1f);
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, ModContent.DustType<ProfanedTileRock>(), 0f, 0f, 1, new Color(255, 255, 255), 1f);
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
