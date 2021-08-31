using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Accessories
{
    public class FungalCarapace : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fungal Carapace");
            Tooltip.SetDefault("You emit a cloud of mushroom spores when you are hit");
        }

        public override void SetDefaults()
        {
            item.defense = 6;
            item.width = 20;
            item.height = 24;
            item.value = CalamityGlobalItem.Rarity4BuyPrice;
            item.rare = ItemRarityID.LightRed;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.fCarapace = true;
        }
    }
}
