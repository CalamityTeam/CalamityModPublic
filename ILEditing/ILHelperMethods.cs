using System.Collections.Generic;
using System.Linq;
using CalamityMod.Systems;
using CalamityMod.Waters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoMod.Cil;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics;
using Terraria.ID;

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

        public static void SelectSulphuricWaterColor(int x, int y, ref VertexColors initialColor, bool isSlope)
        {
            if (SulphuricWaterSafeZoneSystem.NearbySafeTiles.Count >= 1)
            {
                Color cleanWaterColor = new(10, 62, 193);
                Point closestSafeZone = SulphuricWaterSafeZoneSystem.NearbySafeTiles.Keys.OrderBy(t => t.ToVector2().DistanceSQ(new(x, y))).First();
                List<Vector2> points = new()
                {
                    new Vector2(x - 0.5f, y - 0.5f),
                    new Vector2(x + 0.5f, y - 0.5f),
                    new Vector2(x - 0.5f, y + 0.5f),
                    new Vector2(x + 0.5f, y + 0.5f),
                };

                float lerpAmt = (1f - SulphuricWaterSafeZoneSystem.NearbySafeTiles[closestSafeZone]) * 21f;
                for (int i = 0; i < 4; i++)
                {
                    float distanceToClosest = points[i].Distance(closestSafeZone.ToVector2());
                    float acidicWaterInterpolant = Utils.GetLerpValue(12f, 20.5f, distanceToClosest + lerpAmt, true);
                    switch (i)
                    {
                        case 0:
                            initialColor.TopLeftColor = Color.Lerp(initialColor.TopLeftColor, cleanWaterColor, 1f - acidicWaterInterpolant);
                            break;

                        case 1:
                            initialColor.TopRightColor = Color.Lerp(initialColor.TopRightColor, cleanWaterColor, 1f - acidicWaterInterpolant);
                            break;

                        case 2:
                            initialColor.BottomLeftColor = Color.Lerp(initialColor.BottomLeftColor, cleanWaterColor, 1f - acidicWaterInterpolant);
                            break;

                        case 3:
                            initialColor.BottomRightColor = Color.Lerp(initialColor.BottomRightColor, cleanWaterColor, 1f - acidicWaterInterpolant);
                            break;

                        default:
                            break;
                    }
                }
            }

            if (isSlope)
            {
                initialColor.TopLeftColor *= 1f / 3;
                initialColor.TopRightColor *= 1f / 3;
                initialColor.BottomLeftColor *= 1f / 3;
                initialColor.BottomRightColor *= 1f / 3;
            }
            else
            {
                initialColor.TopLeftColor *= 0.4f;
                initialColor.TopRightColor *= 0.4f;
                initialColor.BottomLeftColor *= 0.4f;
                initialColor.BottomRightColor *= 0.4f;
            }
        }

        public static void DumpToLog(ILContext il) => CalamityMod.Instance.Logger.Debug(il.ToString());
        public static void LogFailure(string name, string reason) => CalamityMod.Instance.Logger.Warn($"IL edit \"{name}\" failed! {reason}");
    }
}
