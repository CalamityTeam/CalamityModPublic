using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class GiantTortoiseShell : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Giant Tortoise Shell");
			Tooltip.SetDefault("10% reduced movement speed\n" +
				"Enemies take damage when they hit you");
		}

		public override void SetDefaults()
		{
			item.defense = 8;
			item.width = 20;
			item.height = 24;
            item.value = Item.buyPrice(0, 9, 0, 0);
            item.rare = 5;
			item.accessory = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
			player.moveSpeed -= 0.1f;
			player.thorns = 0.25f;
		}
	}
}
