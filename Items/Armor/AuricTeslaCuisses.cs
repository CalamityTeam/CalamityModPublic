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
    public class AuricTeslaCuisses : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Auric Tesla Cuisses");
            Tooltip.SetDefault("50% increased movement speed\n" +
                "12% increased damage and 5% increased critical strike chance\n" +
                "Magic carpet effect");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = 8750000;
            item.defense = 44;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.overrideColor = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB);
                }
            }
        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.5f;
            player.carpet = true;
            player.meleeDamage += 0.12f;
            player.meleeCrit += 5;
            player.rangedDamage += 0.12f;
            player.rangedCrit += 5;
            player.magicDamage += 0.12f;
            player.magicCrit += 5;
            CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.12f;
            CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingCrit += 5;
            player.minionDamage += 0.12f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "SilvaLeggings");
            recipe.AddIngredient(null, "GodSlayerLeggings");
            recipe.AddIngredient(null, "BloodflareCuisses");
            recipe.AddIngredient(null, "TarragonLeggings");
            recipe.AddIngredient(null, "AuricOre", 80);
            recipe.AddIngredient(null, "EndothermicEnergy", 20);
            recipe.AddIngredient(null, "NightmareFuel", 20);
            recipe.AddIngredient(null, "Phantoplasm", 15);
            recipe.AddIngredient(null, "DarksunFragment", 10);
            recipe.AddIngredient(null, "BarofLife", 8);
            recipe.AddIngredient(null, "HellcasterFragment", 6);
            recipe.AddIngredient(null, "CoreofCalamity", 3);
            recipe.AddIngredient(null, "GalacticaSingularity", 2);
            recipe.AddIngredient(ItemID.FlyingCarpet);
            recipe.AddTile(null, "DraedonsForge");
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}