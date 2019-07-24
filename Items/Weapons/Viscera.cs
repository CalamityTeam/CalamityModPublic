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
    public class Viscera : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Viscera");
            Tooltip.SetDefault("Fires a blood beam that heals you on enemy hits\n" +
				"The more tiles and enemies the beam bounces off of or travels through the more healing the beam does");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 153;
            item.magic = true;
            item.mana = 15;
            item.width = 50;
            item.height = 52;
            item.useTime = 14;
            item.useAnimation = 14;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 6f;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = 10;
            item.UseSound = SoundID.Item20;
            item.autoReuse = true;
            item.shoot = mod.ProjectileType("Viscera");
            item.shootSpeed = 6f;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 13;
		}

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "BloodstoneCore", 4);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}