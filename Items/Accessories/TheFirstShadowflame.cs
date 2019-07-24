using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Accessories
{
    public class TheFirstShadowflame : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The First Shadowflame");
            Tooltip.SetDefault("It is said that in the past, Prometheus descended from the heavens to grant man fire.\n" +
				"If that were true, then it is surely the demons of hell that would have risen from below to do the same.\n" +
				"Minions inflict shadowflame on enemy hits.");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.value = Item.buyPrice(0, 15, 0, 0);
            item.rare = 5;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.shadowMinions = true;
        }
    }
}