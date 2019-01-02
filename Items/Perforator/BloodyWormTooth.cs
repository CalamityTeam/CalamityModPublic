using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Perforator
{
    public class BloodyWormTooth : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bloody Worm Tooth");
            Tooltip.SetDefault("5% increased damage reduction and increased melee stats\n" +
                               "10% increased damage reduction and melee stats when below 50% life");
        }

        public override void SetDefaults()
        {
            item.width = 12;
            item.height = 15;
            item.value = Item.buyPrice(0, 9, 0, 0);
            item.expert = true;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.statLife < (int)((double)player.statLifeMax2 * 0.5))
            {
                player.meleeDamage += 0.1f;
                player.meleeSpeed += 0.1f;
                player.endurance += 0.1f;
            }
            else
            {
                player.meleeDamage += 0.05f;
                player.meleeSpeed += 0.05f;
                player.endurance += 0.05f;
            }
        }
    }
}