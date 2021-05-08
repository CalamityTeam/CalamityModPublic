using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.Furniture.CraftingStations
{
    public class DraedonsForge : ModTile //Legacy names used to avoid fucking over people's placed forges
    {

        public override void SetDefaults() 
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = false;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style4x2); //Changed from 3x2 to 4x2 due to sprite, this likely will be a breaking change.
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 18 };
            TileObjectData.addTile(Type);
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Cosmic Anvil");
            AddMapEntry(new Color(0, 255, 0), name);
            disableSmartCursor = true;
            adjTiles = new int[] { TileID.Anvils, TileID.TinkerersWorkbench, TileID.WorkBenches, TileID.LunarCraftingStation,
                TileID.MythrilAnvil, TileID.AdamantiteForge, TileID.DemonAltar }; //Specifically not furnaces
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(i * 16, j * 16, 32, 16, ModContent.ItemType<Items.Placeables.Furniture.CraftingStations.DraedonsForge>());
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = (float)Main.DiscoR / 255f;
            g = (float)Main.DiscoG / 255f;
            b = (float)Main.DiscoB / 255f;
        }
    }
}
