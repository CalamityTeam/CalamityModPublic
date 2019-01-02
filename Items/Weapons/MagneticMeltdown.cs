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
	public class MagneticMeltdown : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Magnetic Meltdown");
			Tooltip.SetDefault("Fires a spread of magnetic orbs");
			Item.staff[item.type] = true;
		}

	    public override void SetDefaults()
	    {
	        item.damage = 135;
	        item.magic = true;
	        item.mana = 25;
	        item.width = 78;
	        item.height = 78;
	        item.useTime = 27;
	        item.useAnimation = 27;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 5f;
	        item.value = 1000000;
	        item.UseSound = SoundID.Item20;
	        item.autoReuse = true;
	        item.shoot = mod.ProjectileType("MagneticOrb");
	        item.shootSpeed = 12f;
	    }
	    
	    public override void ModifyTooltips(List<TooltipLine> list)
	    {
	        foreach (TooltipLine line2 in list)
	        {
	            if (line2.mod == "Terraria" && line2.Name == "ItemName")
	            {
	                line2.overrideColor = new Color(43, 96, 222);
	            }
	        }
	    }
	    
	    public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
	    {
	        for (int index = 0; index < 4; ++index)
	        {
	            float SpeedX = speedX + (float) Main.rand.Next(-50, 51) * 0.05f;
	            float SpeedY = speedY + (float) Main.rand.Next(-50, 51) * 0.05f;
	            Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI, 1.0f, 0.0f);
	        }
	        return false;
	    }
	    
	    public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(null, "CosmiliteBar", 10);
	        recipe.AddIngredient(ItemID.SpectreStaff);
	        recipe.AddIngredient(ItemID.MagnetSphere);
	        recipe.AddTile(TileID.LunarCraftingStation);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
	}
}