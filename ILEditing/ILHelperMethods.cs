﻿using CalamityMod.Waters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using CalamityMod.Systems;
using Terraria.Graphics;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;

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

            if (cachedLavaStyle == default)
                return initialTexture;
            
            switch (type)
            {
                case LiquidTileType.Block:
                    return cachedLavaStyle.BlockTexture;
                case LiquidTileType.Waterflow:
                    return cachedLavaStyle.LavaTexture;
                case LiquidTileType.Slope:
                    return cachedLavaStyle.SlopeTexture;
            }

            return initialTexture;
        }

        private static VertexColors SelectLavaQuadColor(Texture2D initialTexture, ref VertexColors initialColor, bool forceTrue = false)
        {
            // We should handle the 'forceTrue' flag at this level to prevent us from checking the same thing four times.
            if (!forceTrue)
            {
                if (initialTexture != CustomLavaManagement.LavaTexture &&
                    initialTexture != CustomLavaManagement.LavaBlockTexture &&
                    initialTexture != CustomLavaManagement.LavaSlopeTexture)
                    return initialColor;
            }

            // No lava style to draw? Then skip.
            if (cachedLavaStyle == default)
                return initialColor;
            
            cachedLavaStyle.SelectLightColor(ref initialColor.TopLeftColor);
            cachedLavaStyle.SelectLightColor(ref initialColor.TopRightColor);
            cachedLavaStyle.SelectLightColor(ref initialColor.BottomLeftColor);
            cachedLavaStyle.SelectLightColor(ref initialColor.BottomRightColor);
            return initialColor;
        }

        private static Color SelectLavaColor(Texture2D initialTexture, Color initialLightColor, bool forceTrue = false)
        {
            // Use the initial color if it isn't lava.
            if (!forceTrue)
            {
                if (initialTexture != CustomLavaManagement.LavaTexture &&
                    initialTexture != CustomLavaManagement.LavaBlockTexture &&
                    initialTexture != CustomLavaManagement.LavaSlopeTexture)
                    return initialLightColor;
            }

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

        private static void SelectSulphuricWaterColor(int x, int y, ref VertexColors initialColor)
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

            initialColor.TopLeftColor *= 0.4f;
            initialColor.TopRightColor *= 0.4f;
            initialColor.BottomLeftColor *= 0.4f;
            initialColor.BottomRightColor *= 0.4f;
        }

        public static void DumpToLog(ILContext il) => CalamityMod.Instance.Logger.Debug(il.ToString());
        public static void LogFailure(string name, string reason) => CalamityMod.Instance.Logger.Warn($"IL edit \"{name}\" failed! {reason}");
    }
}
