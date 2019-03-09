using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Armor;
using CalamityMod.Items.CalamityCustomThrowingDamage;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class SilvaArmor : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Silva Armor");
            Tooltip.SetDefault("+80 max life and mana\n" +
                       "20% increased movement speed\n" +
                       "12% increased damage and 8% increased critical strike chance");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
			item.value = Item.buyPrice(0, 72, 0, 0);
			item.defense = 44;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 15;
		}

        public override void UpdateEquip(Player player)
        {
            player.statLifeMax2 += 80;
            player.statManaMax2 += 80;
            player.moveSpeed += 0.2f;
            player.meleeDamage += 0.12f;
            player.meleeCrit += 8;
            player.rangedDamage += 0.12f;
            player.rangedCrit += 8;
            player.magicDamage += 0.12f;
            player.magicCrit += 8;
            CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.12f;
            CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingCrit += 8;
            player.minionDamage += 0.12f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "DarksunFragment", 10);
            recipe.AddIngredient(null, "EffulgentFeather", 10);
            recipe.AddIngredient(null, "CosmiliteBar", 10);
            recipe.AddIngredient(null, "NightmareFuel", 16);
            recipe.AddIngredient(null, "EndothermicEnergy", 16);
            recipe.AddIngredient(null, "LeadCore");
            recipe.AddTile(null, "DraedonsForge");
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}