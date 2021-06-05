using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Accessories
{
    public class GiantShell : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Giant Shell");
            Tooltip.SetDefault("15% reduced movement speed\n" +
                "Taking a hit will make you move very fast for a short time");
        }

        public override void SetDefaults()
        {
            item.defense = 6;
            item.width = 20;
            item.height = 24;
            item.value = CalamityGlobalItem.Rarity1BuyPrice;
            item.rare = ItemRarityID.Blue;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.moveSpeed -= 0.15f;
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.gShell = true;
        }
    }
}
