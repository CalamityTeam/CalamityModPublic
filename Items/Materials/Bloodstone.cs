using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class Bloodstone : ModItem, ILocalizedModType
    {
        public string LocalizationCategory => "Items.Materials";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
			ItemID.Sets.SortingPriorityMaterials[Type] = 112;
        }

        public override void SetDefaults()
        {
            Item.width = 13;
            Item.height = 10;
            Item.maxStack = 9999;
            Item.value = Item.sellPrice(silver: 60);
            Item.rare = ModContent.RarityType<Turquoise>();
        }
    }
}
