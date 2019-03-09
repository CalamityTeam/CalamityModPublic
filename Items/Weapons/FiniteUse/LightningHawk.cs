using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons.FiniteUse
{
	public class LightningHawk : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Lightning Hawk");
			Tooltip.SetDefault("Uses Magnum Rounds\n" +
                "Does more damage to organic enemies");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 400;
            item.crit += 56;
	        item.width = 50;
	        item.height = 28;
	        item.useTime = 21;
	        item.useAnimation = 21;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 8f;
            item.value = Item.buyPrice(0, 48, 0, 0);
            item.rare = 6;
	        item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/Magnum");
	        item.autoReuse = true;
	        item.shootSpeed = 12f;
	        item.shoot = mod.ProjectileType("MagnumRound");
	        item.useAmmo = mod.ItemType("MagnumRounds");
	    }
	    
	    public override Vector2? HoldoutOffset()
		{
			return new Vector2(-5, 0);
		}
	
	    public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "Magnum");
            recipe.AddIngredient(ItemID.SoulofMight, 20);
            recipe.AddIngredient(ItemID.SoulofSight, 20);
            recipe.AddIngredient(ItemID.SoulofFright, 20);
            recipe.AddIngredient(ItemID.IllegalGunParts);
            recipe.AddIngredient(ItemID.HallowedBar, 10);
            recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
	}
}