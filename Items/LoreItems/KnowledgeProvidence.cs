using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Items.LoreItems
{
    public class KnowledgeProvidence : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Providence, the Profaned Goddess");
			Tooltip.SetDefault("A core surrounded by stone and flame, a simple origin and a simple goal.\n" +
				"What would have become of us had she not been defeated is a frightening concept to consider.\n" +
				"Place in your inventory to imbue all projectiles with profaned flames, causing them to inflict extra damage.");
		}

		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 20;
			item.rare = 10;
			item.consumable = false;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 12;
		}

		public override bool CanUseItem(Player player)
		{
			return false;
		}

		public override void UpdateInventory(Player player)
		{
			CalamityPlayer modPlayer = player.GetCalamityPlayer();
			modPlayer.providenceLore = true;
		}
	}
}
