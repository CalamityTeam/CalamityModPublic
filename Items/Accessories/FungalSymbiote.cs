using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items
{
    public class FungalSymbiote : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fungal Symbiote");
            Tooltip.SetDefault("True melee weapons emit mushrooms when swung\n" +
                "Boosts true melee damage by 25%");
        }

        public override void SetDefaults()
        {
            item.width = 38;
            item.height = 36;
            item.value = Item.buyPrice(0, 9, 0, 0);
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
