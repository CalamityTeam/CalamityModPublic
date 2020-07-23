using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items
{
    public class PowerCell : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Draedon Power Cell");
            Tooltip.SetDefault("Used to charge Draedon's weaponry at a charger");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 14;
            item.rare = ItemRarityID.Red;
            item.Calamity().customRarity = CalamityRarity.DraedonRust;
            item.maxStack = 999;
        }
    }
}
