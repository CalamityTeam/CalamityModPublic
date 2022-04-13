using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Potions
{
    public class DeliciousMeat : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Delicious Meat");
            Tooltip.SetDefault("{$CommonItemTooltip.MinorStats}\n'So very delicious'");
        }
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 30;
            Item.value = Item.buyPrice(0, 0, 50, 0);
            Item.rare = ItemRarityID.Pink;
            Item.maxStack = 30;
            Item.consumable = true;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.UseSound = SoundID.Item2;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.useTurn = true;
            Item.buffType = BuffID.WellFed;
            Item.buffTime = CalamityUtils.SecondsToFrames(1800f);
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 5;
        }
    }
}
