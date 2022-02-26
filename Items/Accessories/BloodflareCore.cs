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
			Tooltip.SetDefault("You lose up to half your defense after taking damage\n" + "Lost defense regenerates over time\n" + "You gain 1 health for every 1 defense gained as it regenerates");
		}

		public override void SetDefaults()
		{
			item.width = 26;
			item.height = 26;
			item.value = CalamityGlobalItem.Rarity12BuyPrice;
			item.rare = ItemRarityID.Purple;
			item.Calamity().customRarity = CalamityRarity.Turquoise;
			item.accessory = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			CalamityPlayer modPlayer = player.Calamity();
			modPlayer.bloodflareCore = true;
		}
	}
}
