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
	public class ElephantKiller : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Elephant Killer");
			Tooltip.SetDefault("Uses Magnum Rounds\n" +
                "Does more damage to organic enemies");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 2000;
            item.crit += 66;
	        item.width = 46;
	        item.height = 26;
	        item.useTime = 19;
	        item.useAnimation = 19;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 8f;
            item.value = Item.buyPrice(1, 20, 0, 0);
            item.rare = 10;
	        item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/Magnum");
	        item.autoReuse = true;
	        item.shootSpeed = 12f;
	        item.shoot = mod.ProjectileType("MagnumRound");
	        item.useAmmo = mod.ItemType("MagnumRounds");
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

        public override Vector2? HoldoutOffset()
		{
			return new Vector2(-5, 0);
		}
	
	    public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "LightningHawk");
            recipe.AddIngredient(ItemID.LunarBar, 30);
            recipe.AddIngredient(ItemID.IllegalGunParts);
            recipe.AddTile(TileID.LunarCraftingStation);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
	}
}