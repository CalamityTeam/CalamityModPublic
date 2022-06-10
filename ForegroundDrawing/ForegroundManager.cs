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
        public static bool drawToScreenLastFrame = false;

        internal static void Load()
        {
            _foregroundElements = new List<Point>(1000);
            _foregroundElementCount = 0;
        }

        public static void AddForegroundDrawingPoint(int x, int y)
        {
            if (drawToScreenLastFrame)
            {
                _foregroundElements.Add(new Point(x, y));
                _foregroundElementCount++;
            }
        }


        public static void DrawTiles()
        {
            drawToScreenLastFrame = Main.drawToScreen;

            for (int i = 0; i < _foregroundElementCount; i++)
            {
                ushort type = Main.tile[_foregroundElements[i]].TileType;
                if (TileLoader.GetTile(type) is IForegroundTile fgTile)
                    fgTile.ForegroundDraw(_foregroundElements[i].X, _foregroundElements[i].Y, Main.spriteBatch);
            }

            if (!Main.drawToScreen) return;
            _foregroundElementCount = 0;
            _foregroundElements.Clear();

        }
    }
}
