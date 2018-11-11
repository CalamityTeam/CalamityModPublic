using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables
{
    public class AstralStone : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astral Stone");
        }

        public override void SetDefaults()
        {
            item.createTile = mod.TileType("AstralStone");
            item.useStyle = 1;
            item.useTurn = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.autoReuse = true;
            item.consumable = true;
            item.width = 16;
            item.height = 16;
            item.maxStack = 999;
        }
    }
}
