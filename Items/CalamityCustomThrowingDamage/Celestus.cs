using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.CalamityCustomThrowingDamage
{
    public class Celestus : CalamityDamageItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Celestus");
        }

        public override void SafeSetDefaults()
        {
            item.width = 20;
            item.damage = 480;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.autoReuse = true;
            item.useAnimation = 10;
            item.useStyle = 1;
            item.useTime = 10;
            item.knockBack = 6f;
            item.UseSound = SoundID.Item1;
            item.height = 20;
            item.value = Item.buyPrice(2, 50, 0, 0);
            item.rare = 10;
            item.shoot = mod.ProjectileType("Celestus");
            item.shootSpeed = 25f;
			item.GetGlobalItem<CalamityGlobalItem>(mod).rogue = true;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 15;
		}

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "AccretionDisk");
            recipe.AddIngredient(null, "ShatteredSun");
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
