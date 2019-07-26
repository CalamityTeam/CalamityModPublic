using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;
using CalamityMod.Items.CalamityCustomThrowingDamage;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Legs)]
    public class SilvaLeggings : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Silva Leggings");
            Tooltip.SetDefault("45% increased movement speed\n" +
                "12% increased damage and 7% increased critical strike chance");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
			item.value = Item.buyPrice(0, 54, 0, 0);
			item.defense = 39;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 15;
		}

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.45f;
			player.allDamage += 0.12f;
			player.GetModPlayer<CalamityPlayer>().AllCritBoost(7);
		}

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "DarksunFragment", 7);
            recipe.AddIngredient(null, "EffulgentFeather", 7);
            recipe.AddIngredient(null, "CosmiliteBar", 7);
			recipe.AddIngredient(null, "Tenebris", 9);
			recipe.AddIngredient(null, "NightmareFuel", 15);
            recipe.AddIngredient(null, "EndothermicEnergy", 15);
            recipe.AddTile(null, "DraedonsForge");
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
