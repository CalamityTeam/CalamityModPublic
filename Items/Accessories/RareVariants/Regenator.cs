using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Accessories.RareVariants
{
    public class Regenator : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Regenator");
			Tooltip.SetDefault("Reduces max HP by 50% but greatly improves life regeneration");
		}

        public override void SetDefaults()
        {
            item.width = 36;
            item.height = 32;
            item.value = Item.buyPrice(0, 12, 0, 0);
            item.rare = 5;
            item.defense = 6;
            item.accessory = true;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 22;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
			modPlayer.regenator = true;
		}
	}
}