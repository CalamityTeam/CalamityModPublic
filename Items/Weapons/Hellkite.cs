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
	public class Hellkite : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Hellkite");
			Tooltip.SetDefault("Contains the power of an ancient drake");
		}

		public override void SetDefaults()
		{
			item.width = 84;
			item.damage = 118;
			item.melee = true;
			item.useAnimation = 22;
			item.useStyle = 1;
			item.useTime = 22;
			item.useTurn = true;
			item.knockBack = 8f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.height = 84;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.rare = 7;
		}
	
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "DraedonBar", 8);
			recipe.AddIngredient(ItemID.FieryGreatsword);
	        recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}
	
	    public override void MeleeEffects(Player player, Rectangle hitbox)
	    {
	        if (Main.rand.Next(4) == 0)
	        {
	        	int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 174);
	        }
	    }
	    
	    public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
	    {
			target.AddBuff(BuffID.OnFire, 1800);
		}
	}
}
