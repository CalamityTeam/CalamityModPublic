using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Accessories
{
    public class NuclearRod : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nuclear Rod");
            Tooltip.SetDefault("Minions release an irradiated aura on enemy hits\n" +
                               "+1 max minion");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 32;
            item.value = CalamityGlobalItem.Rarity5BuyPrice;
            item.rare = ItemRarityID.Pink;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Calamity().nuclearRod = true;
        }
    }
}
