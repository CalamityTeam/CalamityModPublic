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
	public class PurityAxe : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Axe of Purity");
			Tooltip.SetDefault("Cleanses the evil");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 43;
	        item.melee = true;
	        item.width = 58;
	        item.height = 46;
	        item.useTime = 19;
	        item.useAnimation = 19;
	        item.useTurn = true;
	        item.axe = 25;
	        item.useStyle = 1;
	        item.knockBack = 5f;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
	        item.UseSound = SoundID.Item1;
	        item.autoReuse = true;
			item.shoot = ProjectileID.PurificationPowder;
			item.shootSpeed = 12f;
		}
	
	    public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(null, "FellerofEvergreens");
			recipe.AddIngredient(ItemID.PurificationPowder, 20);
			recipe.AddIngredient(ItemID.PixieDust, 10);
	        recipe.AddIngredient(ItemID.CrystalShard, 5);
	        recipe.AddTile(TileID.Anvils);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
	    
	    public override void MeleeEffects(Player player, Rectangle hitbox)
	    {
	        if (Main.rand.Next(5) == 0)
	        {
	        	int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 58);
	        }
	    }
	}
}