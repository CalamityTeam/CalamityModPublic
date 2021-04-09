using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Accessories
{
    public class TheTransformer : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Transformer");
            Tooltip.SetDefault("Taking damage releases a blast of sparks\n" +
                                "Sparks do extra damage in Hardmode\n" +
                                "Immunity to Electrified and you resist all electrical projectile and enemy damage\n" +
                                "Enemy bullets do half damage to you and are reflected back at the enemy for 800% their original damage");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.value = CalamityGlobalItem.Rarity2BuyPrice;
            item.rare = ItemRarityID.Green;
            item.accessory = true;
			item.Calamity().challengeDrop = true;
		}

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.aSparkRare = true;
            modPlayer.aSpark = true;
        }
    }
}
