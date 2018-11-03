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
	public class EnergyStaff : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Energy Staff");
			Item.staff[item.type] = true;
		}

	    public override void SetDefaults()
	    {
	        item.damage = 231;
	        item.summon = true;
	        item.sentry = true;
	        item.mana = 25;
	        item.width = 66;
	        item.height = 68;
	        item.useTime = 38;
	        item.useAnimation = 38;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 5f;
	        item.value = 500000;
	        item.autoReuse = true;
	        item.shoot = mod.ProjectileType("ProfanedEnergy");
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
	    
	    public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
	    {
			float num72 = item.shootSpeed;
	    	Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
	    	float num78 = (float)Main.mouseX + Main.screenPosition.X - vector2.X;
			float num79 = (float)Main.mouseY + Main.screenPosition.Y - vector2.Y;
			if (player.gravDir == -1f)
			{
				num79 = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - vector2.Y;
			}
			float num80 = (float)Math.Sqrt((double)(num78 * num78 + num79 * num79));
			float num81 = num80;
			if ((float.IsNaN(num78) && float.IsNaN(num79)) || (num78 == 0f && num79 == 0f))
			{
				num78 = (float)player.direction;
				num79 = 0f;
				num80 = num72;
			}
			else
			{
				num80 = num72 / num80;
			}
	    	num78 = 0f;
			num79 = 0f;
			vector2.X = (float)Main.mouseX + Main.screenPosition.X;
			vector2.Y = (float)Main.mouseY + Main.screenPosition.Y;
            for (int x = 0; x < 1000; x++)
            {
                Projectile projectile = Main.projectile[x];
                if (projectile.active && projectile.owner == player.whoAmI && projectile.type == mod.ProjectileType("ProfanedEnergy"))
                {
                    projectile.Kill();
                }
            }
			Projectile.NewProjectile(vector2.X, vector2.Y, num78, num79, mod.ProjectileType("ProfanedEnergy"), damage, knockBack, player.whoAmI, 0f, 0f);
			return false;
	    }
	}
}