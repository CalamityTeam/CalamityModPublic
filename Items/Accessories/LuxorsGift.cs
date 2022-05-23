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
            SacrificeTotal = 1;
            DisplayName.SetDefault("Luxor's Gift");
            Tooltip.SetDefault("Weapons fire unique projectiles based on the damage type they have\n" +
                "Some weapons are unable to receive this bonus");
        }

        public override void SetDefaults()
        {
            Item.width = 58;
            Item.height = 48;
            Item.value = CalamityGlobalItem.Rarity3BuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.luxorsGift = true;
        }
    }
}
