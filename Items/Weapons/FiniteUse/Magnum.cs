using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.FiniteUse
{
    public class Magnum : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Magnum");
			Tooltip.SetDefault("Uses Magnum Rounds\n" +
                "Does more damage to organic enemies\n" +
				"Can be used thrice per boss battle");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 80;
            item.crit += 46;
	        item.width = 46;
	        item.height = 24;
	        item.useTime = 25;
	        item.useAnimation = 25;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 8f;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = 2;
	        item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/Magnum");
	        item.autoReuse = true;
	        item.shootSpeed = 12f;
	        item.shoot = mod.ProjectileType("MagnumRound");
	        item.useAmmo = mod.ItemType("MagnumRounds");
			if (CalamityPlayer.areThereAnyDamnBosses)
			{
				item.GetGlobalItem<CalamityGlobalItem>(mod).timesUsed = 3;
			}
		}

		public override bool OnPickup(Player player)
		{
			if (CalamityPlayer.areThereAnyDamnBosses)
			{
				item.GetGlobalItem<CalamityGlobalItem>(mod).timesUsed = 3;
			}
			return true;
		}

		public override bool CanUseItem(Player player)
		{
			return item.GetGlobalItem<CalamityGlobalItem>(mod).timesUsed < 3;
		}

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-5, 0);
		}

		public override void UpdateInventory(Player player)
		{
			if (!CalamityPlayer.areThereAnyDamnBosses)
			{
				item.GetGlobalItem<CalamityGlobalItem>(mod).timesUsed = 0;
			}
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			if (CalamityPlayer.areThereAnyDamnBosses)
			{
				for (int i = 0; i < 58; i++)
				{
					if (player.inventory[i].type == item.type)
					{
						player.inventory[i].GetGlobalItem<CalamityGlobalItem>(mod).timesUsed++;
					}
				}
			}
			return true;
		}

		public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.FlintlockPistol);
            recipe.AddIngredient(ItemID.IronBar, 10);
            recipe.anyIronBar = true;
            recipe.AddIngredient(ItemID.Diamond, 5);
            recipe.AddTile(TileID.Anvils);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
	}
}
