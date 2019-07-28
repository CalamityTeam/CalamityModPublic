using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.ShrineItems
{
    public class TrinketofChi : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Trinket of Chi");
			Tooltip.SetDefault("After 2 seconds of standing still and not attacking you gain a buff\n" +
				"This buff boosts your damage by 50% and decreases damage taken by 15%\n" +
				"The buff deactivates after you move or attack once");
		}

		public override void SetDefaults()
		{
			item.width = 34;
			item.height = 32;
			item.value = Item.buyPrice(0, 9, 0, 0);
			item.rare = 3;
			item.accessory = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
			modPlayer.trinketOfChi = true;
		}
	}
}
