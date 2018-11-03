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
	public class VividClarity : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Vivid Clarity");
			Tooltip.SetDefault("Fires 3 beams of elemental energy near the mouse cursor that bounce on tile hits\n" +
			                   "These beams split into more beams when enemies are near\n" +
			                   "The more enemies and tiles the beams hit/pierce the more damage they do");
			Item.staff[item.type] = true;
		}

	    public override void SetDefaults()
	    {
	        item.damage = 165;
	        item.magic = true;
	        item.mana = 150;
	        item.width = 90;
	        item.height = 112;
	        item.useTime = 33;
	        item.useAnimation = 33;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 7.5f;
	        item.value = 100000000;
	        item.UseSound = SoundID.Item60;
	        item.autoReuse = true;
	        item.shoot = mod.ProjectileType("VividClarity");
	        item.shootSpeed = 6f;
	    }
	    
	    public override void ModifyTooltips(List<TooltipLine> list)
	    {
	        foreach (TooltipLine line2 in list)
	        {
	            if (line2.mod == "Terraria" && line2.Name == "ItemName")
	            {
                    line2.overrideColor = new Color(108, 45, 199);
                }
	        }
	    }
	    
	    public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
	    {
			float num72 = item.shootSpeed;
	    	Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
	    	float num78 = (float)Main.mouseX + Main.screenPosition.X + vector2.X;
			float num79 = (float)Main.mouseY + Main.screenPosition.Y + vector2.Y;
			if (player.gravDir == -1f)
			{
				num79 = Main.screenPosition.Y + (float)Main.screenHeight + (float)Main.mouseY + vector2.Y;
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
	    	num78 *= num80;
			num79 *= num80;
	    	int num130 = 3;
			for (int num131 = 0; num131 < num130; num131++)
			{
				vector2 = new Vector2(player.position.X + (float)player.width * 0.5f + (float)(Main.rand.Next(201) * -(float)player.direction) + ((float)Main.mouseX + Main.screenPosition.X - player.position.X), player.MountedCenter.Y);
				vector2.X = (vector2.X + player.Center.X) / 2f + (float)Main.rand.Next(-200, 201);
				vector2.Y -= (float)(100 * num131);
				num78 = (float)Main.mouseX + Main.screenPosition.X - vector2.X;
				num79 = (float)Main.mouseY + Main.screenPosition.Y - vector2.Y;
				if (num79 < 0f)
				{
					num79 *= -1f;
				}
				if (num79 < 20f)
				{
					num79 = 20f;
				}
				num80 = (float)Math.Sqrt((double)(num78 * num78 + num79 * num79));
				num80 = num72 / num80;
				num78 *= num80;
				num79 *= num80;
				float speedX4 = num78 + (float)Main.rand.Next(-40, 41) * 0.02f;
				float speedY5 = num79 + (float)Main.rand.Next(-40, 41) * 0.02f;
				Projectile.NewProjectile(vector2.X, vector2.Y, speedX4, speedY5, mod.ProjectileType("VividClarity"), damage, knockBack, player.whoAmI, 0f, 0f);
			}
			return false;
		}
	    
	    public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(null, "ElementalRay");
	        recipe.AddIngredient(null, "ArchAmaryllis");
	        recipe.AddIngredient(null, "AsteroidStaff");
	        recipe.AddIngredient(null, "HellwingStaff");
	        recipe.AddIngredient(null, "PhantasmalFury");
	        recipe.AddIngredient(null, "ShadowboltStaff");
	        recipe.AddIngredient(null, "VenusianTrident");
	        recipe.AddIngredient(null, "NightmareFuel", 5);
        	recipe.AddIngredient(null, "EndothermicEnergy", 5);
	        recipe.AddIngredient(null, "CosmiliteBar", 5);
            recipe.AddIngredient(null, "DarksunFragment", 5);
            recipe.AddIngredient(null, "HellcasterFragment", 3);
            recipe.AddIngredient(null, "Phantoplasm", 5);
            recipe.AddIngredient(null, "AuricOre", 25);
            recipe.AddTile(null, "DraedonsForge");
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
	}
}