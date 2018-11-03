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
    public class HeartofDarkness : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Heart of Darkness");
            Tooltip.SetDefault("Gives 10% increased damage while you have the heart attack debuff\n" +
                "You gain rage over time\n" +
                "Revengeance drop");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(6, 4));
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.value = 1500000;
            item.rare = 10;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.heartOfDarkness = true;
        }
    }
}