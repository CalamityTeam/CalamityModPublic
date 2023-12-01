using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Tiles.Astral;
using Terraria;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace CalamityMod.World
{
    public class AstralChestGeneration
    {
        public static void PlaceAstralChest()
        {
            // Get dungeon area information.
            int left = GenVars.dMinX + 25;
            int right = GenVars.dMaxX - 25;
            int top = (int)Main.worldSurface;
            int bottom = GenVars.dMaxY - 25;

            // Sanity check: If the dungeon area information is somehow irrevocably broken in a way that will guarantee a crash, terminate this
            // method immediately and leave a log message.
            bool invalidWidthData = left >= right;
            bool invalidHeightData = top >= bottom;
            if (invalidWidthData || invalidHeightData)
            {
                CalamityMod.Instance.Logger.Warn("The generated dungeon was found to have unusable area bounds. As a result, the astral chest could not be generated.");
                return;
            }

            int astralChestItemID = ModContent.ItemType<HeavenfallenStardisk>();
            ushort astralChestID = (ushort)ModContent.TileType<AstralChestLocked>();

            // The Astral Chest generates in style 1, which is locked.
            int chestStyle = 1;

            // Define an undefined chest and attempt counter for the loop below.
            Chest chest = null;
            int attempts = 0;

            // Try 1000 times to place the chest somewhere in the dungeon.
            while (chest is null && attempts < 1000)
            {
                attempts++;

                // Pick a random potential location to generate the chest in, based on the chest placement area.
                int x = WorldGen.genRand.Next(left, right);
                int y = WorldGen.genRand.Next(top, bottom);
                Tile randomTile = Main.tile[x, y];

                // Determine if the randomly selected tile is a valid candidate for chest placement based on whether it has dungeon walls and has no active tile.
                // The placement algorithm ensures that if it tries to appear in midair, it is moved down to the floor.
                bool emptyDungeonWall = Main.wallDungeon[randomTile.WallType] && !randomTile.HasTile;

                // Attempt to place the chest.
                if (emptyDungeonWall)
                    chest = MiscWorldgenRoutines.AddChestWithLoot(x, y, astralChestID, tileStyle: chestStyle);
            }

            // If a chest was placed, force its first item to be the unique Biome Chest weapon.
            if (chest != null)
            {
                chest.item[0].SetDefaults(astralChestItemID);

                // Apply a random prefix (including the possibility of no prefix at all) to the added item in the chest.
                chest.item[0].Prefix(-1);
            }

            // If a chest was NOT placed, that means that the loop somehow failed.
            // Log this.
            else
                CalamityMod.Instance.Logger.Warn("The astral chest loop could not find a valid generation location. As a result, it has not generated.");
        }
    }
}
