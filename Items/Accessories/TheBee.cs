using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Accessories
{
    public class TheBee : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Bee");
            Tooltip.SetDefault("When at full HP, your damage is increased based on your damage reduction\n" +
                            "Damage taken at full HP is halved\n" +
                            "This has a 10 second cooldown");
        }

        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 28;
            item.value = CalamityGlobalItem.Rarity3BuyPrice;
            item.rare = ItemRarityID.Orange;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.theBee = true;
        }
    }
}
