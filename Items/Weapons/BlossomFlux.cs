using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;
//using TerrariaOverhaul;

namespace CalamityMod.Items.Weapons 
{
	public class BlossomFlux : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Blossom Flux");
			Tooltip.SetDefault("Legendary Drop\n" +
				"Fires a stream of leaves\n" +
				"Right click to fire a spore orb that explodes into a cloud of spore gas\n" +
                "Revengeance drop");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 22;
	        item.ranged = true;
	        item.width = 40;
	        item.height = 62;
	        item.useTime = 4;
	        item.useAnimation = 16;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 0.15f;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.rare = 7;
            item.UseSound = SoundID.Item5;
	        item.autoReuse = true;
	        item.shoot = mod.ProjectileType("LeafArrow");
	        item.shootSpeed = 10f;
	        item.useAmmo = 40;
	    }

        /*public void OverhaulInit()
        {
            this.SetTag("bow");
        }*/

        public override void ModifyTooltips(List<TooltipLine> list)
	    {
	        foreach (TooltipLine line2 in list)
	        {
	            if (line2.mod == "Terraria" && line2.Name == "ItemName")
	            {
	                line2.overrideColor = new Color(Main.DiscoR, 203, 103);
	            }
	        }
	    }
	    
	    public override bool AltFunctionUse(Player player)
		{
			return true;
		}
	    
	    public override bool CanUseItem(Player player)
		{
			if (player.altFunctionUse == 2)
			{
				item.useTime = 25;
	    		item.useAnimation = 25;
	        	item.UseSound = SoundID.Item77;
			}
			else
			{
				item.useTime = 2;
	        	item.useAnimation = 16;
	        	item.UseSound = SoundID.Item5;
			}
			return base.CanUseItem(player);
		}
		
		public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
	    	if (player.altFunctionUse == 2)
	    	{
	        	Projectile.NewProjectile(position.X, position.Y, speedX, speedY, mod.ProjectileType("SporeBomb"), (int)((double)damage * 6f), (knockBack * 60f), player.whoAmI, 0.0f, 0.0f);
	    		return false;
	    	}
	    	else
	    	{
	        	Projectile.NewProjectile(position.X, position.Y, speedX, speedY, mod.ProjectileType("LeafArrow"), damage, knockBack, player.whoAmI, 0.0f, 0.0f);
	    		return false;
	    	}
		}
	}
}