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
    public class CrownJewel : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crown Jewel");
            Tooltip.SetDefault("Boosts life regen even while under the effects of a damaging debuff\n" +
                "While under the effects of a damaging debuff you will gain 10 defense\n" +
                "Revengeance drop");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.value = 50000;
            item.rare = 1;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.crownJewel = true;
        }
    }
}