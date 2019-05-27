using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.GameContent;
using Terraria.IO;
using Terraria.ObjectData;
using Terraria.Utilities;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons.Perforators
{
	public class BloodClotStaff : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Blood Clot Staff");
			Tooltip.SetDefault("Summons a blood clot to fight for you");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 7;
	        item.mana = 10;
	        item.width = 58;
	        item.height = 64;
	        item.useTime = 36;
	        item.useAnimation = 36;
	        item.useStyle = 1;
	        item.noMelee = true;
	        item.knockBack = 2.25f;
            item.value = Item.buyPrice(0, 4, 0, 0);
            item.rare = 3;
	        item.UseSound = SoundID.Item44;
	        item.autoReuse = true;
	        item.shoot = mod.ProjectileType("BloodClotMinion");
	        item.shootSpeed = 10f;
	        item.summon = true;
	    }
	    
	    public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(ItemID.Vertebrae, 4);
	        recipe.AddIngredient(ItemID.CrimtaneBar, 5);
	        recipe.AddIngredient(null, "BloodSample", 10);
	        recipe.AddTile(TileID.DemonAltar);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }

		public override bool AltFunctionUse(Player player)
		{
			return true;
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			if (player.altFunctionUse != 2)
			{
				position = Main.MouseWorld;
				speedX = 0;
				speedY = 0;
				Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI);
			}
			return false;
		}

		public override bool UseItem(Player player)
		{
			if (player.altFunctionUse == 2)
			{
				player.MinionNPCTargetAim();
			}
			return base.UseItem(player);
		}
	}
}