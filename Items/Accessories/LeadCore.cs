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
    public class LeadCore : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lead Core");
            Tooltip.SetDefault("Grants immunity to the irradiated debuff");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.rare = 10;
            item.value = Item.buyPrice(0, 30, 0, 0);
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[mod.BuffType("Irradiated")] = true;
        }
    }
}