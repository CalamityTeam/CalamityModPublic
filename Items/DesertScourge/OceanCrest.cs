using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.DesertScourge
{
    public class OceanCrest : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ocean Crest");
            Tooltip.SetDefault("Most ocean enemies become friendly and provides waterbreathing");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 26;
            item.value = Item.buyPrice(0, 6, 0, 0);
            item.rare = 9;
            item.accessory = true;
            item.expert = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.npcTypeNoAggro[65] = true;
            player.npcTypeNoAggro[220] = true;
            player.npcTypeNoAggro[64] = true;
            player.npcTypeNoAggro[67] = true;
            player.npcTypeNoAggro[221] = true;
            player.gills = true;
        }
    }
}
