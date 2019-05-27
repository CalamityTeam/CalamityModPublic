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
    public class BloodflareBodyArmor : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bloodflare Body Armor");
            Tooltip.SetDefault("12% increased damage and 8% increased critical strike chance\n" +
                       "You regenerate life quickly and gain +30 defense while in lava\n" +
                       "+40 max life");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
			item.value = Item.buyPrice(0, 48, 0, 0);
			item.defense = 35;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 13;
		}

        public override void UpdateEquip(Player player)
        {
            player.statLifeMax2 += 40;
            player.meleeDamage += 0.12f;
            player.meleeCrit += 8;
            player.magicDamage += 0.12f;
            player.magicCrit += 8;
            player.rangedDamage += 0.12f;
            player.rangedCrit += 8;
            CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.12f;
            CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingCrit += 8;
            player.minionDamage += 0.12f;
            if (player.lavaWet)
            {
                player.statDefense += 30;
                player.lifeRegen += 10;
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "BloodstoneCore", 16);
            recipe.AddIngredient(null, "RuinousSoul", 4);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}