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
	public class SparkSpreader : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Spark Spreader");
			Tooltip.SetDefault("20% chance to not consume gel");
		}

	    public override void SetDefaults()
	    {
			item.damage = 7;
			item.ranged = true;
			item.width = 52;
			item.height = 20;
			item.useTime = 10;
			item.useAnimation = 30;
			item.useStyle = 5;
			item.noMelee = true;
			item.knockBack = 1f;
			item.UseSound = SoundID.Item34;
            item.value = Item.buyPrice(0, 1, 0, 0);
            item.rare = 1;
			item.autoReuse = true;
			item.shoot = mod.ProjectileType("SparkSpreaderFire");
			item.shootSpeed = 5f;
			item.useAmmo = 23;
		}

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-4, 0);
        }

	    public override bool ConsumeAmmo(Player player)
	    {
	    	if (Main.rand.Next(0, 100) < 20)
	    		return false;
	    	return true;
	    }

	    public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(ItemID.FlareGun, 1);
	        recipe.AddIngredient(ItemID.Ruby, 1);
	        recipe.AddIngredient(ItemID.Gel, 12);
	        recipe.AddTile(TileID.Anvils);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
	}
}
