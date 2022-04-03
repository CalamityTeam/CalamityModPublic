using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Accessories
{
    public class GravistarSabaton : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gravistar Sabaton");
            Tooltip.SetDefault("Tap the DOWN key to increase your fall speed for 5 seconds\n" +
                               "This has an 8 second cooldown\n" +
                               "Striking the ground with increased fall speed will cause an astral explosion");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 22;
            Item.value = CalamityGlobalItem.Rarity7BuyPrice;
            Item.accessory = true;
            Item.rare = ItemRarityID.Lime;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Calamity().gSabaton = true;
        }
    }
}
