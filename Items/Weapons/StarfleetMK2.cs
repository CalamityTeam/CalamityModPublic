using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons
{
	public class StarfleetMK2 : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Starmada");
			Tooltip.SetDefault("Fires a barrage of stars and plasma blasts");
		}

	    public override void SetDefaults()
	    {
            item.damage = 308;
            item.knockBack = 15f;
            item.shootSpeed = 16f;
            item.useStyle = 5;
            item.useAnimation = 27;
            item.useTime = 27;
            item.reuseDelay = 0;
            item.width = 122;
            item.height = 50;
            item.UseSound = SoundID.Item92;
            item.shoot = mod.ProjectileType("StarfleetMK2");
            item.value = Item.buyPrice(1, 80, 0, 0);
            item.rare = 10;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.ranged = true;
            item.channel = true;
            item.useTurn = false;
            item.useAmmo = 75;
            item.autoReuse = true;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 14;
		}

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, 0);
        }

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			Projectile.NewProjectile(position.X, position.Y, speedX, speedY, mod.ProjectileType("StarfleetMK2"), 0, 0f, player.whoAmI);
            return false;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "Starfleet");
            recipe.AddIngredient(null, "CosmiliteBar", 10);
            recipe.AddIngredient(null, "ExodiumClusterOre", 15);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
		}
	}
}
