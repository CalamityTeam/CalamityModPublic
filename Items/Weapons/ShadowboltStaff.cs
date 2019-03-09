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
    public class ShadowboltStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shadowbolt Staff");
            Tooltip.SetDefault("The more tiles and enemies the beam bounces off of or travels through the more damage the beam does");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 170;
            item.magic = true;
            item.mana = 20;
            item.width = 58;
            item.height = 56;
            item.useTime = 14;
            item.useAnimation = 14;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 8f;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = 10;
            item.UseSound = SoundID.Item72;
            item.autoReuse = true;
            item.shoot = mod.ProjectileType("Shadowbolt");
            item.shootSpeed = 6f;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 13;
		}

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "ArmoredShell", 3);
            recipe.AddIngredient(null, "RuinousSoul", 2);
            recipe.AddIngredient(ItemID.ShadowbeamStaff);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}