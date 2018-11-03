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
    public class HoneyDew : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Honey Dew");
            Tooltip.SetDefault("10% increased damage reduction, +5 defense, and increased life regen while in the Jungle\n" +
            "Poison and Venom immunity\n" +
            "Honey life regen with no speed penalty\n" +
            "Most bee/hornet enemies and projectiles do 75% damage to you");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.value = 500000;
            item.rare = 7;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.beeResist = true;
            if (player.ZoneJungle)
            {
                player.lifeRegen += 2;
                player.statDefense += 5;
                player.endurance += 0.1f;
            }
            player.buffImmune[70] = true;
            player.buffImmune[20] = true;
            if (!player.honey && player.lifeRegen < 0)
            {
                player.lifeRegen += 4;
                if (player.lifeRegen > 0)
                {
                    player.lifeRegen = 0;
                }
            }
            player.lifeRegenTime += 2;
            player.lifeRegen += 2;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "LivingDew");
            recipe.AddIngredient(ItemID.BottledHoney, 10);
            recipe.AddIngredient(ItemID.BeeWax, 10);
            recipe.AddIngredient(ItemID.Bezoar);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}