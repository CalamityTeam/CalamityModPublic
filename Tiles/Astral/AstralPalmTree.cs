using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework.Graphics;

namespace CalamityMod.Tiles
{
    public class AstralPalmTree : ModPalmTree
    {
        public override Texture2D GetTexture()
        {
            return CalamityMod.Instance.GetTexture("Tiles/Astral/AstralPalmTree");
        }

        public override Texture2D GetTopTextures()
        {
            return CalamityMod.Instance.GetTexture("Tiles/Astral/AstralPalmTree_Tops");
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
