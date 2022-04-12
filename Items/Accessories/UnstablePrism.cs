using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Accessories
{
    public class UnstablePrism : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Unstable Granite Core");
            Tooltip.SetDefault("Three sparks are released on critical hits");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(7, 5));
        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 44;
            Item.value = CalamityGlobalItem.Rarity3BuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.accessory = true;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.unstablePrism = true;
        }
    }
}
