using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Patreon
{
	public class DarkSpark : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Dark Spark");
			Tooltip.SetDefault("And everything under the sun is in tune,\n" +
                "But the sun is eclipsed by the moon.");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(6, 4));
        }

	    public override void SetDefaults()
	    {
	        item.damage = 100;
	        item.magic = true;
	        item.mana = 10;
	        item.width = 16;
	        item.height = 16;
	        item.useTime = 10;
	        item.useAnimation = 10;
	        item.reuseDelay = 5;
	        item.useStyle = 4;
	        item.UseSound = SoundID.Item13;
	        item.noMelee = true;
	        item.noUseGraphic = true;
			item.channel = true;
	        item.knockBack = 0f;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = 10;
            item.shoot = mod.ProjectileType("DarkSpark");
	        item.shootSpeed = 30f;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 21;
		}

	    public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, mod.ProjectileType("DarkSpark"), damage, knockBack, player.whoAmI, 0.0f, 0.0f);
            return false;
		}

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.LastPrism);
            recipe.AddIngredient(null, "DarkPlasma", 10);
            recipe.AddIngredient(null, "RuinousSoul", 20);
            recipe.AddIngredient(null, "DivineGeode", 30);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
