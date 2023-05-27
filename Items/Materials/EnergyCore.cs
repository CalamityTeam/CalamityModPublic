using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Materials
{
    public class EnergyCore : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 22;
            Item.maxStack = 9999;
            Item.value = Item.sellPrice(copper: 80);
            Item.rare = ItemRarityID.Blue;
        }
    }
}
