using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class StressPills : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<Laudanum>();
        }
        
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.defense = 4;
            Item.value = CalamityGlobalItem.Rarity3BuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.stressPills = true;
        }
    }
}
