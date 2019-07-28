using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items
{
    public class MushroomPlasmaRoot : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Mushroom Plasma Root");
			Tooltip.SetDefault("Permanently makes Rage Mode do 15% (60% in Death Mode) more damage\n" +
                "Revengeance drop");
		}

		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 20;
			item.useAnimation = 30;
			item.rare = 2;
			item.useTime = 30;
			item.useStyle = 4;
			item.UseSound = SoundID.Item122;
			item.consumable = true;
		}

		public override bool CanUseItem(Player player)
		{
			CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
			if (modPlayer.rageBoostOne)
			{
				return false;
			}
			return true;
		}

		public override bool UseItem(Player player)
		{
			if (player.itemAnimation > 0 && player.itemTime == 0)
			{
				player.itemTime = item.useTime;
				CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
				modPlayer.rageBoostOne = true;
			}
			return true;
		}
	}
}
