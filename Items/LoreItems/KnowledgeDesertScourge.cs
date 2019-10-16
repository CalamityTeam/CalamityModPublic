﻿using Terraria;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class KnowledgeDesertScourge : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Desert Scourge");
            Tooltip.SetDefault("The great sea worm appears to have survived the extreme heat and has even adapted to it.\n" +
                "What used to be a majestic beast swimming through the water has now become a dried-up and\n" +
                "gluttonous husk, constantly on a voracious search for its next meal.\n" +
                "Place in your inventory for an increase to defense while in the desert or sunken sea.");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = 1;
            item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return false;
        }

        public override void UpdateInventory(Player player)
        {
            if (player.ZoneDesert || player.Calamity().ZoneSunkenSea)
            {
                player.Calamity().desertScourgeLore = true;
            }
        }
    }
}
