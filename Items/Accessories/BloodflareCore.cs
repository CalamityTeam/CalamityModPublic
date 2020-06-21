using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
	public class BloodflareCore : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Bloodflare Core");
			Tooltip.SetDefault("When below 50% life you gain 5% increased damage reduction and 10% increased damage\n" +
				"When below 15% life you gain 10% increased damage reduction and 20% increased damage\n" +
				"When below 100 defense you gain 15% increased damage");
		}

		public override void SetDefaults()
		{
			item.width = 26;
			item.height = 26;
			item.value = CalamityGlobalItem.Rarity13BuyPrice;
			item.expert = true;
			item.rare = ItemRarityID.Red;
			item.accessory = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			CalamityPlayer modPlayer = player.Calamity();
			modPlayer.bloodflareCore = true;
		}
	}
}
