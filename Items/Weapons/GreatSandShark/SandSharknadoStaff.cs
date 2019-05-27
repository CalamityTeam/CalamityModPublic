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

namespace CalamityMod.Items.Weapons.GreatSandShark
{
	public class SandSharknadoStaff : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Sand Sharknado Staff");
			Tooltip.SetDefault("Summons a sandnado to fight for you");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 98;
	        item.mana = 10;
	        item.width = 48;
	        item.height = 56;
	        item.useTime = 35;
	        item.useAnimation = 35;
	        item.useStyle = 1;
	        item.noMelee = true;
	        item.knockBack = 2f;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = 8;
	        item.UseSound = SoundID.Item44;
	        item.autoReuse = true;
	        item.shoot = mod.ProjectileType("Sandnado");
	        item.shootSpeed = 10f;
	        item.summon = true;
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

		public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.TempestStaff);
            recipe.AddIngredient(null, "GrandScale");
            recipe.AddIngredient(ItemID.HallowedBar, 10);
            recipe.AddIngredient(ItemID.AncientCloth, 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}