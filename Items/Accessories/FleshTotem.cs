using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Accessories
{
    public class FleshTotem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flesh Totem");
            Tooltip.SetDefault("Halves enemy contact damage\n" +
                "When you take contact damage this effect has a 20 second cooldown");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.rare = ItemRarityID.Yellow;
            item.value = CalamityGlobalItem.Rarity8BuyPrice;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.fleshTotem = true;
        }
    }
}
