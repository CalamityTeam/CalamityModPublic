using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
	public class AstralHelm : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Astral Helm");
			Tooltip.SetDefault("Danger detection");
		}

		public override void SetDefaults()
		{
			item.width = 18;
			item.height = 18;
			item.value = Item.buyPrice(0, 40, 0, 0);
			item.rare = 9;
			item.defense = 17; //63
		}

		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == mod.ItemType("AstralBreastplate") && legs.type == mod.ItemType("AstralLeggings");
		}

		public override void ArmorSetShadows(Player player)
		{
			player.armorEffectDrawShadow = true;
		}

		public override void UpdateArmorSet(Player player)
		{
			player.setBonus = "25% increased movement speed\n" +
				"28% increased damage and 21% increased critical strike chance\n" +
				"Whenever you crit an enemy fallen, hallowed, and astral stars will rain down\n" +
				"This effect has a 1 second cooldown before it can trigger again";
			CalamityPlayer modPlayer = player.GetCalamityPlayer();
			modPlayer.astralStarRain = true;
			player.moveSpeed += 0.25f;
			player.allDamage += 0.28f;
			modPlayer.AllCritBoost(21);
		}

		public override void UpdateEquip(Player player)
		{
			player.dangerSense = true;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "AstralBar", 8);
			recipe.AddTile(TileID.LunarCraftingStation);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
