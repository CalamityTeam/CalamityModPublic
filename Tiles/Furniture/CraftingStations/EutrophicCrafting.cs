using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.Furniture.CraftingStations
{
    public class EutrophicCrafting : ModTile
    {
        //
        // Note from Ozzatron:
        // It's highly recommended you don't touch this code unless absolutely necessary.
        // Even slightly changing the way that this tile initializes could re-introduce the Ancients Awakened worldgen bug.
        //
        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.StyleHorizontal = true;
            // Remove the bottom anchor entirely, this thing hangs from the wall.
            TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
            TileObjectData.newTile.AnchorRight = new AnchorData(AnchorType.SolidTile, 2, 1);

            // When this Direction property set executes, newTile._tileObjectBase gets separated from Style3x3._tileObjectBase.
            // As such, no data corruption occurs.
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            // Add a wall anchor on the opposite side for the alternate style, and remove the wall anchor that was on the first side.
            TileObjectData.newAlternate.AnchorLeft = new AnchorData(AnchorType.SolidTile, 2, 1);
            TileObjectData.newAlternate.AnchorRight = AnchorData.Empty;

            // This line used to corrupt TileObjectData.Style3x3.Direction to PlaceRight. This has now been fixed.
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.addAlternate(1);
            TileObjectData.addTile(Type);
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Eutrophic Shelf");
            AddMapEntry(new Color(191, 142, 111), name);
            AnimationFrameHeight = 54;
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 51, 0f, 0f, 1, new Color(54, 69, 72), 1f);
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.5f;
            g = 1f;
            b = 1f;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 16, 32, ModContent.ItemType<Items.Placeables.Furniture.CraftingStations.EutrophicCrafting>());
        }
    }
}
