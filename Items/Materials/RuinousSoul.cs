using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Materials
{
    public class RuinousSoul : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ruinous Soul");
            Tooltip.SetDefault("A shard of the distant past");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 6));
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 42;
            Item.maxStack = 999;
            Item.value = Item.buyPrice(0, 7, 0, 0);
            Item.Calamity().customRarity = CalamityRarity.PureGreen;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
        }
    }
}
