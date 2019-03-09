using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Placeables
{
    public class PrismShard : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Prism Shard");
        }

        public override void SetDefaults()
        {
            item.createTile = mod.TileType("SeaPrismCrystals");
            item.useStyle = 1;
            item.useTurn = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.autoReuse = true;
            item.consumable = true;
            item.width = 26;
            item.height = 26;
            item.maxStack = 999;
			item.value = Item.buyPrice(0, 0, 50, 0);
			item.rare = 2;
        }
    }
}