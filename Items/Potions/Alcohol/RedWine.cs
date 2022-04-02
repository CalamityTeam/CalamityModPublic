using CalamityMod.Buffs.Alcohol;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions.Alcohol
{
    public class RedWine : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Red Wine");
            Tooltip.SetDefault(@"Too dry for my taste
Reduces life regen by 1");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 18;
            item.useTurn = true;
            item.maxStack = 30;
            item.rare = ItemRarityID.LightRed;
            item.useAnimation = 17;
            item.useTime = 17;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.UseSound = SoundID.Item3;
            item.healLife = 200;
            item.consumable = true;
            item.potion = true;
            item.value = Item.buyPrice(0, 0, 65, 0);
        }

        public override bool CanUseItem(Player player)
        {
            item.healLife = player.Calamity().baguette ? 250 : 200;
            return base.CanUseItem(player);
        }

        public override void OnConsumeItem(Player player)
        {
            player.AddBuff(ModContent.BuffType<RedWineBuff>(), 900);
        }
    }
}