using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Materials
{
    public class Stardust : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stardust");
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 18;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(silver: 3);
            Item.rare = ItemRarityID.LightRed;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
        }
    }
}
