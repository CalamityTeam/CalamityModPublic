using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Accessories
{
    public class VoltaicJelly : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Voltaic Jelly");
            Tooltip.SetDefault("+1 max minions\n" +
                               "Minion attacks inflict Electrified");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 22;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.accessory = true;
            item.rare = ItemRarityID.Green;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Calamity().voltaicJelly = true;
        }
    }
}
