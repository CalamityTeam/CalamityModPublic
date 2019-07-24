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
    public class CheatTestThing : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("lul");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.value = 1;
            item.rare = 1;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            bool canUse = (player.name == "Fabsol" || player.name == "Totalbiscuit") && player.townNPCs <= 1;
            if (canUse)
            {
                modPlayer.lol = true;
            }
            else if (!player.immune)
            {
                player.KillMe(PlayerDeathReason.ByOther(12), 1000.0, 0, false);
            }
        }
    }
}