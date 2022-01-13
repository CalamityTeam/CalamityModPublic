using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

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
            item.width = 32;
            item.height = 30;
            item.value = Item.buyPrice(0, 0, 50, 0);
            item.rare = ItemRarityID.Pink;
            item.maxStack = 30;
            item.consumable = true;
            item.useAnimation = 17;
            item.useTime = 17;
            item.UseSound = SoundID.Item2;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.useTurn = true;
            item.buffType = BuffID.WellFed;
            item.buffTime = CalamityUtils.SecondsToFrames(1800f);
        }
    }
}
