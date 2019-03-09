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
    public class Lacerator : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lacerator");
            Tooltip.SetDefault("Enemies that are hit by the yoyo will have their life drained\n" +
                "Someone thought this was a viable weapon against DoG at one point lol");
        }

        public override void SetDefaults()
        {
            item.CloneDefaults(ItemID.TheEyeOfCthulhu);
            item.damage = 150;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = 5;
            item.channel = true;
            item.melee = true;
            item.knockBack = 7f;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = 10;
            item.autoReuse = true;
            item.shoot = mod.ProjectileType("LaceratorProjectile");
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

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, 0.0f, 0.0f);
            return false;
        }
    }
}