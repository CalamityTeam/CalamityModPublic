using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons 
{
	public class NeptunesBounty : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Neptune's Bounty");
            Tooltip.SetDefault("Hitting enemies will cause the crush depth debuff\n" +
				"The lower the enemies' defense the more damage they take from this debuff");
        }

		public override void SetDefaults()
		{
			item.width = 80;
			item.damage = 540;
			item.melee = true;
			item.useAnimation = 17;
			item.useTime = 17;
			item.useTurn = true;
			item.useStyle = 1;
			item.knockBack = 9f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.height = 80;
            item.value = Item.buyPrice(1, 80, 0, 0);
            item.rare = 10;
            item.shoot = mod.ProjectileType("NeptuneOrb");
			item.shootSpeed = 25f;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 14;
		}
		
		public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(null, "AbyssBlade");
	        recipe.AddIngredient(null, "CosmiliteBar", 5);
	        recipe.AddIngredient(null, "Phantoplasm", 5);
            recipe.AddIngredient(null, "DepthCells", 15);
            recipe.AddIngredient(null, "Lumenite", 15);
            recipe.AddIngredient(null, "Tenebris", 5);
            recipe.AddTile(null, "DraedonsForge");
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
	
	    public override void MeleeEffects(Player player, Rectangle hitbox)
	    {
	        if (Main.rand.Next(3) == 0)
	        {
	        	int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 33);
	        }
	    }
	    
	    public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
	    {
	    	target.AddBuff(mod.BuffType("CrushDepth"), 600);
		}
	}
}
