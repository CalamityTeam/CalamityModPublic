using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;

namespace CalamityMod.ForegroundDrawing
{
    interface IForegroundTile
    {
        void ForegroundDraw(int x, int y, SpriteBatch spriteBatch);
    }

    public static class ForegroundManager
    {
        private static List<Point> _foregroundElements;
        private static int _foregroundElementCount;

        internal static void Load()
        {
            _foregroundElements = new List<Point>(1000);
            _foregroundElementCount = 0;
        }

        private static bool DrawToScreen() => Lighting.UpdateEveryFrame || Main.drawToScreen;

        public static void AddForegroundDrawingPoint(int x, int y)
        {
            _foregroundElements.Add(new Point(x, y));
            _foregroundElementCount++;
        }


        public static void DrawTiles()
        {
            for (int i = 0; i < _foregroundElementCount; i++)
            {
                ushort type = Main.tile[_foregroundElements[i]].TileType;
                if (TileLoader.GetTile(type) is IForegroundTile fgTile)
                    fgTile.ForegroundDraw(_foregroundElements[i].X, _foregroundElements[i].Y, Main.spriteBatch);
            }
        }

        public static void ClearTiles()
        {
            _foregroundElements.Clear();
            _foregroundElementCount = 0;
        }
    }
}
