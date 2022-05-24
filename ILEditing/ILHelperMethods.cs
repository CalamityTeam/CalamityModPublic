using CalamityMod.Waters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using Terraria;
using Terraria.ID;
using Terraria.Audio;

namespace CalamityMod.ILEditing
{
    public partial class ILChanges
    {
        private static int FindTopOfDoor(int i, int j, Tile rootTile)
        {
            Tile t = Main.tile[i, j];
            int topY = j;
            while (t != null && t.HasTile && t.TileType == rootTile.TileType)
            {
                // Immediately stop at the top of the world, if you got there somehow.
                if (topY == 0)
                    return topY;
                // Go up one space and re-assign the current tile.
                --topY;
                t = Main.tile[i, topY];
            }

            // The above loop will have gone 1 past the top of the door. Correct for this.
            return ++topY;
        }

        private static bool OpenLabDoor(Tile tile, int i, int j, int openID)
        {
            int topY = FindTopOfDoor(i, j, tile);
            return DirectlyTransformLabDoor(i, topY, openID);
        }

        private static bool CloseLabDoor(Tile tile, int i, int j, int closedID)
        {
            int topY = FindTopOfDoor(i, j, tile);
            return DirectlyTransformLabDoor(i, topY, closedID);
        }

        private static bool DirectlyTransformLabDoor(int doorX, int doorY, int newDoorID, int wireHitY = -1)
        {
            // Transform the door one tile at a time.
            // If applicable, skip wiring for all door tiles except the one that was hit by this wire event.
            for (int y = doorY; y < doorY + 4; ++y)
            {
                Main.tile[doorX, y].TileType = (ushort)newDoorID;
                if (Main.netMode != NetmodeID.MultiplayerClient && Wiring.running && y != wireHitY)
                    Wiring.SkipWire(doorX, y);
            }

            // Second pass: TileFrame all those positions, which will sync in multiplayer if applicable
            for (int y = doorY; y < doorY + 4; ++y)
                WorldGen.TileFrame(doorX, y);

            // Play the door closing sound (lab doors do not use the door opening sound)
            SoundEngine.PlaySound(SoundID.DoorClosed, new Vector2(doorX * 16, doorY * 16));
            return true;
        }

        private static Texture2D SelectLavaTexture(Texture2D initialTexture, LiquidTileType type)
        {
            // Use the initial texture if it isn't lava.
            if (initialTexture != CustomLavaManagement.LavaTexture &&
                initialTexture != CustomLavaManagement.LavaBlockTexture &&
                initialTexture != CustomLavaManagement.LavaSlopeTexture)
                return initialTexture;

            foreach (CustomLavaStyle lavaStyle in CustomLavaManagement.CustomLavaStyles)
            {
                if (lavaStyle.ChooseLavaStyle())
                {
                    switch (type)
                    {
                        case LiquidTileType.Block:
                            return lavaStyle.BlockTexture;
                        case LiquidTileType.Waterflow:
                            return lavaStyle.LavaTexture;
                        case LiquidTileType.Slope:
                            return lavaStyle.SlopeTexture;
                    }
                }
            }

            return initialTexture;
        }

        private static Color SelectLavaColor(Texture2D initialTexture, Color initialLightColor)
        {
            // Use the initial color if it isn't lava.
            if (initialTexture != CustomLavaManagement.LavaTexture && 
                initialTexture != CustomLavaManagement.LavaBlockTexture &&
                initialTexture != CustomLavaManagement.LavaSlopeTexture)
                return initialLightColor;

            foreach (CustomLavaStyle lavaStyle in CustomLavaManagement.CustomLavaStyles)
            {
                if (lavaStyle.ChooseLavaStyle())
                {
                    lavaStyle.SelectLightColor(ref initialLightColor);
                    return initialLightColor;
                }
            }

            return initialLightColor;
        }

        public static void DumpToLog(ILContext il) => CalamityMod.Instance.Logger.Debug(il.ToString());
        public static void LogFailure(string name, string reason) => CalamityMod.Instance.Logger.Warn($"IL edit \"{name}\" failed! {reason}");
    }
}
