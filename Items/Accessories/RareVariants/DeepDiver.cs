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
    public class DeepDiver : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Deep Diver");
            Tooltip.SetDefault("15% increased damage, defense, and movement speed when underwater\n" +
								"While underwater you gain the ability to dash great distances");
        }

        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 28;
            item.value = Item.buyPrice(0, 3, 0, 0);
            item.rare = 2;
            item.defense = 2;
            item.accessory = true;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 22;
		}

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (Collision.DrownCollision(player.position, player.width, player.height, player.gravDir))
            {
				CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
				modPlayer.deepDiver = true;
				modPlayer.dashMod = 5;
            }
        }
	}
}