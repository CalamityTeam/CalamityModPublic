using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons
{
    public class Celestus : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Celestus");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.damage = 145;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.autoReuse = true;
            item.useAnimation = 10;
            item.useStyle = 1;
            item.useTime = 10;
            item.knockBack = 6f;
            item.UseSound = SoundID.Item1;
            item.thrown = true;
            item.height = 20;
            item.value = 100000000;
            item.shoot = mod.ProjectileType("Celestus");
            item.shootSpeed = 25f;
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

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "AccretionDisk");
            recipe.AddIngredient(null, "Crystalline");
            recipe.AddIngredient(null, "ExecutionersBlade");
            recipe.AddIngredient(null, "Pwnagehammer");
            recipe.AddIngredient(null, "SpearofPaleolith");
            recipe.AddIngredient(null, "NightmareFuel", 5);
            recipe.AddIngredient(null, "EndothermicEnergy", 5);
            recipe.AddIngredient(null, "CosmiliteBar", 5);
            recipe.AddIngredient(null, "DarksunFragment", 5);
            recipe.AddIngredient(null, "HellcasterFragment", 3);
            recipe.AddIngredient(null, "Phantoplasm", 5);
            recipe.AddIngredient(null, "AuricOre", 25);
            recipe.AddTile(null, "DraedonsForge");
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
