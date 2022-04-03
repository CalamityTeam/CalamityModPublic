using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Accessories
{
    public class ScuttlersJewel : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Scuttler's Jewel");
            Tooltip.SetDefault("Stealth strike projectiles spawn a jewel spike when destroyed");
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.value = CalamityGlobalItem.Rarity1BuyPrice;
            Item.rare = ItemRarityID.Blue;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.scuttlersJewel = true;
        }
    }
}
