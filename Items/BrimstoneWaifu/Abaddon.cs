using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.BrimstoneWaifu
{
    [AutoloadEquip(EquipType.Head)]
    public class Abaddon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Abaddon");
            Tooltip.SetDefault("Makes you immune to Brimstone Flames");
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
            player.buffImmune[mod.BuffType("BrimstoneFlames")] = true;
        }
    }
}