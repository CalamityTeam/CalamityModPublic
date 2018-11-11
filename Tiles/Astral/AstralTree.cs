using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework.Graphics;

namespace CalamityMod.Tiles
{
    public class AstralTree : ModTree
    {
        public override Texture2D GetTopTextures(int i, int j, ref int frame, ref int frameWidth, ref int frameHeight, ref int xOffsetLeft, ref int yOffset)
        {
            frame = (i + j * j) % 3;
            return CalamityMod.Instance.GetTexture("Tiles/Normal/AstralTree_Tops");
        }

        public override Texture2D GetBranchTextures(int i, int j, int trunkOffset, ref int frame)
        {
            return CalamityMod.Instance.GetTexture("Tiles/Normal/AstralTree_Branches");
        }

        public override Texture2D GetTexture()
        {
            return CalamityMod.Instance.GetTexture("Tiles/Normal/AstralTree");
        }

        public override int DropWood()
        {
            return CalamityMod.Instance.ItemType("AstralMonolith");
        }

        public override int CreateDust()
        {
            return CalamityMod.Instance.DustType("AstralBasic");
        }

        public override int GrowthFXGore()
        {
            return -1;
        }
    }
}
