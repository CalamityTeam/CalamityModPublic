using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories.RareVariants
{
    public class TheTransformer : ModItem
	{
		public override void SetStaticDefaults()
		{
				DisplayName.SetDefault("The Transformer");
				Tooltip.SetDefault("Taking damage releases a blast of sparks\n" +
									"Sparks do extra damage in Hardmode\n" +
									"Immunity to Electrified and you resist all elctrical projectile and enemy damage\n" +
									"Enemy bullets do half damage to you and are reflected back at the enemy for 800% their original damage");
		}

		public override void SetDefaults()
		{
			item.width = 26;
			item.height = 26;
			item.value = Item.buyPrice(0, 3, 0, 0);
			item.rare = 1;
			item.accessory = true;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 22;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
			modPlayer.aSparkRare = true;
			modPlayer.aSpark = true;
		}
	}
}
