using CalamityMod.Items.Placeables;
using CalamityMod.Items.Fishing.BrimstoneCragCatches;
using CalamityMod.World;
using System.Collections.Generic;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions
{
	public class SunkenStew : ModItem
	{
		public static int BuffType = BuffID.WellFed;
		public static int BuffDuration = 216000;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Hadal Stew");
            Tooltip.SetDefault("Only gives 50 seconds of Potion Sickness\n" +
               "Grants Well Fed\n" +
               "60 minute duration");
		}

		public override void SetDefaults()
		{
			item.width = 28;
			item.height = 18;
			item.useTurn = true;
			item.maxStack = 30;
			item.useAnimation = 17;
			item.useTime = 17;
			item.value = Item.buyPrice(0, 2, 0, 0);
			item.rare = ItemRarityID.Green;
			item.useStyle = ItemUseStyleID.EatingUsing;
			item.UseSound = SoundID.Item3;
			item.consumable = true;
			item.potion = true;
			item.healLife = 120;
			item.healMana = 150;
		}

        public override void ModifyTooltips(List<TooltipLine> list)
        {
			if (Main.LocalPlayer.pStone)
			{
				foreach (TooltipLine line2 in list)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
					{
						line2.text = "Only gives 37 seconds of Potion Sickness";
					}
				}
			}
        }

        public override bool CanUseItem(Player player)
        {
            return player.potionDelay <= 0 && player.Calamity().potionTimer <= 0;
        }

		public override bool UseItem(Player player)
		{
            player.AddBuff(BuffType, BuffDuration);
			// fixes hardcoded potion sickness duration from quick heal (see CalamityPlayerMiscEffects.cs)
			player.Calamity().potionTimer = 2;
			return true;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<AbyssGravel>(), 10);
			recipe.AddIngredient(ModContent.ItemType<CoastalDemonfish>());
			recipe.AddIngredient(ItemID.Honeyfin);
			recipe.AddIngredient(ItemID.Bowl, 2);
			recipe.AddTile(TileID.CookingPots);
			recipe.SetResult(this, 2);
			recipe.AddRecipe();
			recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<Voidstone>(), 10);
			recipe.AddIngredient(ModContent.ItemType<CoastalDemonfish>());
			recipe.AddIngredient(ItemID.Honeyfin);
			recipe.AddIngredient(ItemID.Bowl, 2);
			recipe.AddTile(TileID.CookingPots);
			recipe.SetResult(this, 2);
			recipe.AddRecipe();
		}
	}
}
