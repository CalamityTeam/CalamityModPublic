using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class VitalJelly : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Vital Jelly");
			Tooltip.SetDefault("10% increased movement speed\n" +
				"200% increased jump speed");
		}

		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 24;
            item.value = Item.buyPrice(0, 12, 0, 0);
            item.rare = 5;
			item.accessory = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.moveSpeed += 0.1f;
        	player.jumpSpeedBoost += 2.0f;
		}
	}
}
