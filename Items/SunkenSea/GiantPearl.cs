using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.SunkenSea
{
    public class GiantPearl : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Giant Pearl");
            Tooltip.SetDefault("You have a light aura around you\n" +
                "Enemies within the aura are slowed down");
        }

        public override void SetDefaults()
        {
            item.width = 42;
            item.height = 32;
			item.value = Item.buyPrice(0, 3, 0, 0);
			item.rare = 2;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.giantPearl = true;
			Lighting.AddLight((int)(player.position.X + (float)(player.width / 2)) / 16, (int)(player.position.Y + (float)(player.height / 2)) / 16, 0.45f, 0.8f, 0.8f);
        }
    }
}