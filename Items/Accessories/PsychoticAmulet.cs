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
    public class PsychoticAmulet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Psychotic Amulet");
            Tooltip.SetDefault("Boosts melee and ranged damage and critical strike chance by 5%\n" +
                               "Grants a massive boost to these stats if you aren't moving");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.value = Item.buyPrice(0, 15, 0, 0);
            item.rare = 6;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.pAmulet = true;
            player.shroomiteStealth = true;
            player.meleeDamage += 0.05f;
            player.meleeCrit += 5;
            player.rangedDamage += 0.05f;
            player.rangedCrit += 5;
        }
    }
}