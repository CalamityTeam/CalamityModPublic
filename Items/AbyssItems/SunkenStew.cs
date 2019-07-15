using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;

namespace CalamityMod.Items.AbyssItems
{
	public class SunkenStew : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Hadal Stew");
            Tooltip.SetDefault("Only gives 50 (37 with Philosopher's Stone) seconds of Potion Sickness\n" +
                "Grants Well Fed for 60 minutes\n" + "Cannot be used via Quick Buff");
        }
		
		public override void SetDefaults()
		{
			item.width = 28;
			item.height = 18;
			item.useTurn = true;
			item.maxStack = 30;
            item.useAnimation = 17;
			item.useTime = 17;
            item.rare = 3;
			item.useStyle = 2;
			item.UseSound = SoundID.Item3;
			item.consumable = true;
            item.potion = true;
            item.healLife = 120;
            item.healMana = 150;
			item.value = Item.buyPrice(0, 2, 0, 0);
		}

        public override bool CanUseItem(Player player)
        {
            return player.FindBuffIndex(BuffID.PotionSickness) == -1;
        }

        // fixes hardcoded potion sickness duration from quick heal
        public override bool UseItem(Player player)
        {
            player.ClearBuff(BuffID.PotionSickness);
            player.AddBuff(BuffID.PotionSickness, player.pStone ? 2220 : 3000);
            player.AddBuff(BuffID.WellFed, 216000);
            return true;
        }

        public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "DepthCells", 5);
            recipe.AddIngredient(ItemID.GlowingMushroom, 3);
            recipe.AddIngredient(ItemID.Honeyfin);
            recipe.AddIngredient(ItemID.Bowl);
            recipe.AddTile(TileID.CookingPots);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
