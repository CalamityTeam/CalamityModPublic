using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class FungalSymbiote : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fungal Symbiote");
            Tooltip.SetDefault("Various melee weapons emit mushrooms in true melee range\n" +
                "True melee strikes deal 15% more damage");
        }

        public override void SetDefaults()
        {
            item.width = 38;
            item.height = 36;
            item.value = CalamityGlobalItem.Rarity3BuyPrice;
            item.rare = 3;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.fungalSymbiote = true;
        }
    }
}
