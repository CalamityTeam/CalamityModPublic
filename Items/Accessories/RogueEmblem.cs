using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.CalamityCustomThrowingDamage;

namespace CalamityMod.Items.Accessories
{
    public class RogueEmblem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rogue Emblem");
            Tooltip.SetDefault("15% increased rogue damage");
        }

        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 24;
            item.value = 100000;
            item.rare = 4;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityCustomThrowingDamagePlayer modPlayer2 = CalamityCustomThrowingDamagePlayer.ModPlayer(player);
            modPlayer2.throwingDamage += 0.15f;
        }
    }
}