using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.DataStructures;

namespace CalamityMod.Items.Accessories
{
    public class UnstablePrism : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Unstable Prism");
            Tooltip.SetDefault("Three sparks are released on critical hits");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(7, 5));
        }

        public override void SetDefaults()
        {
            item.width = 40;
            item.height = 44;
            item.value = CalamityGlobalItem.Rarity3BuyPrice;
            item.rare = ItemRarityID.Orange;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.unstablePrism = true;
        }
    }
}
