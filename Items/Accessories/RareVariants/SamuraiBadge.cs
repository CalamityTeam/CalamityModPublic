using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Items.Accessories.RareVariants
{
    public class SamuraiBadge : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Samurai Badge");
            Tooltip.SetDefault("20% increased melee damage and speed\n" +
								"Reduces max life by 25%");
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 30;
            item.value = Item.buyPrice(0, 21, 0, 0);
            item.accessory = true;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 22;
		}

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.GetCalamityPlayer();
			modPlayer.badgeOfBraveryRare = true;
        }
	}
}
