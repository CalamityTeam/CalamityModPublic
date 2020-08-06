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
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Hadal Stew");
			Tooltip.SetDefault(@"Only gives 50 (37 with Philosopher's Stone) seconds of Potion Sickness
Grants Well Fed");
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
			item.useStyle = ItemUseStyleID.EatingUsing;
			item.UseSound = SoundID.Item3;
			item.consumable = true;
			item.potion = true;
			item.buffType = BuffID.WellFed;
			item.buffTime = 216000;
			item.healLife = 120;
			item.healMana = 150;
			item.value = Item.buyPrice(0, 2, 0, 0);
		}

		public override bool CanUseItem(Player player) => player.potionDelay <= 0 && player.Calamity().potionTimer <= 0;

        public override bool CanUseItem(Player player)
        {
            if (player.Calamity().bloodPactBoost)
            {
                item.healLife = 180;
            }
            else
            {
                item.healLife = 120;
            }
            return base.CanUseItem(player);
        }

		public override bool UseItem(Player player)
		{
			if (PlayerInput.Triggers.JustPressed.QuickBuff)
			{
				int healAmt = CalamityWorld.ironHeart ? 0 : item.healLife;
				player.statLife += healAmt;
				player.statMana += item.healMana;
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
					if (!CalamityWorld.ironHeart)
						player.HealEffect(healAmt, true);
					player.ManaEffect(item.healMana);
				}
			}

			// fixes hardcoded potion sickness duration from quick heal (see CalamityPlayerMiscEffects.cs)
			player.Calamity().potionTimer = 2;
			player.Calamity().potionTimerR = 2;
			player.AddBuff(BuffID.WellFed, item.buffTime);
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
