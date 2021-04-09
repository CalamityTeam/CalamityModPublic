using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Accessories
{
    public class LuxorsGift : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Luxor's Gift");
            Tooltip.SetDefault("Weapons fire unique projectiles based on the damage type they have\n" +
                "Some weapons are unable to receive this bonus");
        }

        public override void SetDefaults()
        {
            item.width = 58;
            item.height = 48;
            item.value = CalamityGlobalItem.Rarity3BuyPrice;
            item.rare = ItemRarityID.Orange;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.luxorsGift = true;
        }
    }
}
