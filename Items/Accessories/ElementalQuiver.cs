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
    public class ElementalQuiver : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Elemental Quiver");
            Tooltip.SetDefault("Ranged projectiles have a chance to split\n" +
                "Ranged weapons have a chance to instantly kill normal enemies\n" +
                "10% increased ranged damage and 5% increased ranged critical strike chance\n" +
				"20% reduced ammo usage and increased life regen, minion knockback, defense, and pick speed");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 32;
            item.value = Item.buyPrice(0, 90, 0, 0);
            item.accessory = true;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 20;
		}

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.eQuiver = true;
            player.rangedDamage += 0.1f;
            player.rangedCrit += 5;
            player.ammoCost80 = true;
            player.lifeRegen += 2;
            player.statDefense += 5;
            player.pickSpeed -= 0.15f;
            player.minionKB += 0.5f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.MagicQuiver);
            recipe.AddIngredient(null, "DaedalusEmblem");
            recipe.AddIngredient(null, "Phantoplasm", 20);
            recipe.AddIngredient(null, "NightmareFuel", 20);
            recipe.AddIngredient(null, "EndothermicEnergy", 20);
            recipe.AddTile(null, "DraedonsForge");
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
