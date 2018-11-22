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
	public class AuguroftheElements : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Augur of the Elements");
			Tooltip.SetDefault("Casts a burst of elemental tentacles to spear your enemies");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 74;
	        item.magic = true;
	        item.mana = 6;
	        item.width = 28;
	        item.crit = 3;
	        item.height = 30;
	        item.useTime = 1;
	        item.reuseDelay = 10;
	        item.useAnimation = 10;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 5.5f;
	        item.value = 10000000;
	        item.UseSound = SoundID.Item103;
	        item.autoReuse = true;
	        item.shoot = mod.ProjectileType("ElementTentacle");
	        item.shootSpeed = 30f;
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
			recipe.AddIngredient(null, "EldritchTome");
			recipe.AddIngredient(null, "TomeofFates");
			recipe.AddIngredient(ItemID.ShadowFlameHexDoll);
			recipe.AddIngredient(null, "GalacticaSingularity", 5);
			recipe.AddIngredient(ItemID.LunarBar, 5);
	        recipe.AddTile(TileID.LunarCraftingStation);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}
	    
	    public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
	    	Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
	    	float num78 = (float)Main.mouseX + Main.screenPosition.X - vector2.X;
			float num79 = (float)Main.mouseY + Main.screenPosition.Y - vector2.Y;
	    	Vector2 value2 = new Vector2(num78, num79);
			value2.Normalize();
			Vector2 value3 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
			value3.Normalize();
			value2 = value2 * 6f + value3;
			value2.Normalize();
			value2 *= item.shootSpeed;
			float num91 = (float)Main.rand.Next(10, 50) * 0.001f;
			if (Main.rand.Next(2) == 0)
			{
				num91 *= -1f;
			}
			float num92 = (float)Main.rand.Next(10, 50) * 0.001f;
			if (Main.rand.Next(2) == 0)
			{
				num92 *= -1f;
			}
			Projectile.NewProjectile(vector2.X, vector2.Y, value2.X, value2.Y, type, damage, knockBack, player.whoAmI, num92, num91);
	    	return false;
		}
	}
}