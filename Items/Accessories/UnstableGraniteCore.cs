using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.DataStructures;

namespace CalamityMod.Items.Accessories
{
    [LegacyName("UnstablePrism")]
    public class UnstableGraniteCore : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Unstable Granite Core");
            Tooltip.SetDefault("Periodically gain an unstable energy field that repeatedly zaps nearby enemies with arcing energy\n" +
                "The arcing energy ignores a substantial amount of enemy defense");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(7, 5));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 44;
            Item.value = CalamityGlobalItem.Rarity3BuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.unstableGraniteCore = true;
        }
    }
}
