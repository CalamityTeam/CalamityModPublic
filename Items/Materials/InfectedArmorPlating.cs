using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Materials
{
    public class InfectedArmorPlating : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Infected Armor Plating"); //haha recycled and recolored Corroded Metal Plating from T2 Acid Rain
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.maxStack = 999;
            item.value = Item.buyPrice(0, 5, 0, 0);
            item.rare = ItemRarityID.Yellow;
        }
    }
}
