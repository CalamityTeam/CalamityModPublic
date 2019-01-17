using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons 
{
	public class BrinyBaron : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Briny Baron");
			Tooltip.SetDefault("Legendary Drop\n" +
				"Striking an enemy with the blade causes a briny typhoon to appear\n" +
				"Right click to fire a razorwind aqua blade\n" +
                "Revengeance drop");
		}

		public override void SetDefaults()
		{
			item.width = 62;
			item.damage = 66;
			item.melee = true;
			item.useAnimation = 20;
			item.useTime = 20;
			item.useTurn = true;
			item.useStyle = 1;
			item.knockBack = 4f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.height = 62;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = 8;
            item.shootSpeed = 4f;
			item.shoot = mod.ProjectileType("NobodyKnows");
		}
		
		public override void ModifyTooltips(List<TooltipLine> list)
	    {
	        foreach (TooltipLine line2 in list)
	        {
	            if (line2.mod == "Terraria" && line2.Name == "ItemName")
	            {
	                line2.overrideColor = new Color(53, Main.DiscoG, 255);
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
				item.noMelee = true;
	        	item.noUseGraphic = true;
	    		item.useTime = 15;
	    		item.useAnimation = 15;
	        	item.UseSound = SoundID.Item84;
			}
			else
			{
				item.noMelee = false;
	        	item.noUseGraphic = false;
	    		item.useTime = 20;
	    		item.useAnimation = 20;
	        	item.UseSound = SoundID.Item1;
			}
			return base.CanUseItem(player);
		}
		
		public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
	    	if (player.altFunctionUse == 2)
	    	{
	        	Projectile.NewProjectile(position.X, position.Y, speedX, speedY, mod.ProjectileType("Razorwind"), damage, knockBack, player.whoAmI, 0.0f, 0.0f);
	    		return false;
	    	}
	    	else
	    	{
	        	Projectile.NewProjectile(position.X, position.Y, speedX, speedY, mod.ProjectileType("NobodyKnows"), damage, knockBack, player.whoAmI, 0.0f, 0.0f);
	    		return false;
	    	}
		}
	
	    public override void MeleeEffects(Player player, Rectangle hitbox)
	    {
	        if (Main.rand.Next(3) == 0)
	        {
	        	int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 187, 0f, 0f, 100, new Color(53, Main.DiscoG, 255));
	        }
	    }
	    
	    public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
	    {
	    	Projectile.NewProjectile(target.Center.X, target.Center.Y, 0f, 0f, mod.ProjectileType("BrinyTyphoonBubble"), (int)((float)item.damage * player.meleeDamage), knockback, player.whoAmI);
		}
	}
}
