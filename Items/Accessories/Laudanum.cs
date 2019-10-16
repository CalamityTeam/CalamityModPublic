﻿using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class Laudanum : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Laudanum");
            Tooltip.SetDefault("Boosts your damage by 6%,\n" +
                               "defense by 6, and max movement speed and acceleration by 5%\n" +
                               "Makes you immune to The Horror debuff\n" +
                               "Revengeance drop");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.value = Item.buyPrice(0, 9, 0, 0);
            item.rare = 3;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.laudanum = true;
            player.buffImmune[ModContent.BuffType<Horror>()] = true;
        }
    }
}
