using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Accessories
{
    public class VoltaicJelly : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Voltaic Jelly");
            Tooltip.SetDefault("+1 max minions\n" +
                               "Minion attacks inflict Electrified");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 22;
            Item.value = Item.buyPrice(0, 2, 0, 0);
            Item.accessory = true;
            Item.rare = ItemRarityID.Green;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Calamity().voltaicJelly = true;
        }
    }
}
