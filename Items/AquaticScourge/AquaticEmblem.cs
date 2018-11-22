using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.AquaticScourge
{
    public class AquaticEmblem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aquatic Emblem");
            Tooltip.SetDefault("Most ocean enemies become friendly and provides waterbreathing\n" +
                "Being underwater slowly boosts your defense over time but also slows movement speed\n" +
                "The defense boost and movement speed reduction slowly vanish while outside of water\n" +
                "Maximum defense boost is 40, maximum movement speed reduction is 15%");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 26;
            item.value = 500000;
            item.rare = 5;
            item.accessory = true;
            item.expert = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.aquaticEmblem = true;
            player.npcTypeNoAggro[65] = true;
            player.npcTypeNoAggro[220] = true;
            player.npcTypeNoAggro[64] = true;
            player.npcTypeNoAggro[67] = true;
            player.npcTypeNoAggro[221] = true;
            player.gills = true;
        }
    }
}