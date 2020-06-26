using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class AmidiasSpark : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Amidias' Spark");
            Tooltip.SetDefault("Taking damage releases a blast of sparks\n" +
                               "Sparks deal extra damage in Hardmode");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.value = CalamityGlobalItem.Rarity1BuyPrice;
            item.rare = 1;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.aSpark = true;
        }
    }
}
