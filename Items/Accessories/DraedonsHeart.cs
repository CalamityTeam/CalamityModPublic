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
    public class DraedonsHeart : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Draedon's Heart");
            Tooltip.SetDefault("Gives 10% increased damage while you have the heart attack debuff\n" +
                "You gain rage over time\n" +
                "Gives immunity to the horror debuff\n" +
                "Standing still regenerates your life quickly and boosts your defense by 50\n" +
                "Nanomachines, son");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(5, 7));
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.value = 5000000;
            item.accessory = true;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.overrideColor = new Color(108, 45, 199);
                }
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.draedonsHeart = true;
            player.buffImmune[mod.BuffType("Horror")] = true;
            modPlayer.draedonsStressGain = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "HeartofDarkness");
            recipe.AddIngredient(null, "StressPills");
            recipe.AddIngredient(null, "Laudanum");
            recipe.AddIngredient(ItemID.LunarBar, 30);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}