using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Items.LoreItems
{
    public class KnowledgeAstrumAureus : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Astrum Aureus");
			Tooltip.SetDefault("A titanic cyborg infected by a star-borne disease expelled from the belly of an ancient god.\n" +
                "The destruction of this creature will not prevent the spread of the disease.\n" +
				"Place in your inventory to gain increased jump speed in space.");
		}

		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 20;
			item.rare = 7;
			item.consumable = false;
		}

		public override bool CanUseItem(Player player)
		{
			return false;
		}

		public override void UpdateInventory(Player player)
		{
			CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
			modPlayer.astrumAureusLore = true;
		}
	}
}
