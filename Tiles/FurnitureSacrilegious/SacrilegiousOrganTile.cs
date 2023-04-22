using CalamityMod.Items.Placeables.FurnitureSacrilegious;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.FurnitureSacrilegious
{
    public class SacrilegiousOrganTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = false;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4); //This piano has bookcase dimensions
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(191, 142, 111), CalamityUtils.GetText("Tiles.PipeOrgan"));
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 8, 0f, 0f, 1, new Color(255, 255, 255), 1f);
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
