using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class SilvaHornedHelm : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Silva Horned Helm");
            Tooltip.SetDefault("13% increased ranged damage and critical strike chance");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
			item.value = Item.buyPrice(0, 90, 0, 0);
			item.defense = 36; //110
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 15;
		}

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == mod.ItemType("SilvaArmor") && legs.type == mod.ItemType("SilvaLeggings");
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.silvaSet = true;
            modPlayer.silvaRanged = true;
            player.setBonus = "You are immune to almost all debuffs\n" +
                "Reduces all damage taken by 5%, this is calculated separately from damage reduction\n" +
                "All projectiles spawn healing leaf orbs on enemy hits\n" +
                "Max run speed and acceleration boosted by 5%\n" +
                "If you are reduced to 0 HP you will not die from any further damage for 10 seconds\n" +
                "If you get reduced to 0 HP again while this effect is active you will lose 100 max life\n" +
                "This effect only triggers once per life\n" +
                "Your max life will return to normal if you die\n" +
                "Increases your rate of fire with all ranged weapons\n" +
                "During the silva invulnerability time your ranged weapons will do 40% more damage";
        }

        public override void UpdateEquip(Player player)
        {
            player.rangedDamage += 0.13f;
            player.rangedCrit += 13;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "DarksunFragment", 5);
            recipe.AddIngredient(null, "EffulgentFeather", 5);
            recipe.AddIngredient(null, "CosmiliteBar", 5);
            recipe.AddIngredient(null, "NightmareFuel", 14);
            recipe.AddIngredient(null, "EndothermicEnergy", 14);
            recipe.AddTile(null, "DraedonsForge");
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}