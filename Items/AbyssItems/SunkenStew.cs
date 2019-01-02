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
			DisplayName.SetDefault("Sunken Stew");
            Tooltip.SetDefault("Causes potion sickness for 50 (37 with Philosopher's Stone effect) seconds instead of 60\n" +
                "Restores 120 life and 150 mana");
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
            item.buffType = BuffID.WellFed;
            item.buffTime = 216000;
            item.value = 10000;
		}

        public override bool CanUseItem(Player player)
        {
            return player.FindBuffIndex(BuffID.PotionSickness) == -1;
        }

        public override bool ConsumeItem(Player player)
        {
            return player.FindBuffIndex(BuffID.PotionSickness) == -1;
        }

        public override void OnConsumeItem(Player player)
        {
            player.statLife += 120;
            player.statMana += 150;
            if (player.statLife > player.statLifeMax2)
            {
                player.statLife = player.statLifeMax2;
            }
            if (player.statMana > player.statManaMax2)
            {
                player.statMana = player.statManaMax2;
            }
            player.AddBuff(BuffID.ManaSickness, Player.manaSickTime, true);
            if (Main.myPlayer == player.whoAmI)
            {
                player.HealEffect(120, true);
                player.ManaEffect(150);
            }
            player.AddBuff(BuffID.WellFed, 216000);
            player.AddBuff(BuffID.PotionSickness, (player.pStone ? 2220 : 3000));
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