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
	public class LifefruitScythe : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Lifehunt Scythe");
			Tooltip.SetDefault("Heals the player on enemy hits");
		}

		public override void SetDefaults()
		{
			item.width = 60;
			item.damage = 115;
			item.melee = true;
			item.useAnimation = 18;
			item.useStyle = 1;
			item.useTime = 18;
			item.useTurn = true;
			item.knockBack = 7.5f;
			item.UseSound = SoundID.Item71;
			item.autoReuse = true;
			item.height = 72;
			item.value = 1250000;
			item.shoot = mod.ProjectileType("LifeScythe");
			item.shootSpeed = 9f;
		}
		
		public override void ModifyTooltips(List<TooltipLine> list)
	    {
	        foreach (TooltipLine line2 in list)
	        {
	            if (line2.mod == "Terraria" && line2.Name == "ItemName")
	            {
	                line2.overrideColor = new Color(0, 255, 200);
	            }
	        }
	    }
	
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "UeliaceBar", 15);
	        recipe.AddTile(TileID.LunarCraftingStation);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}
	
	    public override void MeleeEffects(Player player, Rectangle hitbox)
	    {
	        if (Main.rand.Next(4) == 0)
	        {
	        	int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 75);
	        }
	    }
	    
	    public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
	    {
	    	if (target.type == NPCID.TargetDummy)
			{
				return;
			}
	    	player.statLife += 2;
	    	player.HealEffect(2);
			target.AddBuff(BuffID.OnFire, 200);
			target.AddBuff(BuffID.CursedInferno, 200);
		}
	}
}
