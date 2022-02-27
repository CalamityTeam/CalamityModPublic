using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
	[AutoloadEquip(EquipType.Legs)]
	public class TitanHeartBoots : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Titan Heart Boots");
			Tooltip.SetDefault("4% increased rogue damage, 10% increased rogue velocity, and 5% increased rogue knockback");
		}

		public override void SetDefaults()
		{
			item.width = 18;
			item.height = 18;
			item.value = Item.buyPrice(0, 12, 0, 0);
			item.rare = ItemRarityID.LightRed;
			item.defense = 14;
		}

		public override void UpdateEquip(Player player)
		{
			player.Calamity().titanHeartBoots = true;
			player.Calamity().throwingVelocity += 0.1f;
			player.Calamity().throwingDamage += 0.04f;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<AstralMonolith>(), 14);
			recipe.AddIngredient(ModContent.ItemType<TitanHeart>());
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
