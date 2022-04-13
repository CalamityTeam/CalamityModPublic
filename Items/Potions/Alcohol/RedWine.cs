using CalamityMod.Buffs.Alcohol;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

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
            Item.width = 28;
            Item.height = 18;
            Item.useTurn = true;
            Item.maxStack = 30;
            Item.rare = ItemRarityID.LightRed;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.UseSound = SoundID.Item3;
            Item.healLife = 200;
            Item.consumable = true;
            Item.potion = true;
            Item.value = Item.buyPrice(0, 0, 65, 0);
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 5;
        }

        public override bool CanUseItem(Player player)
        {
            Item.healLife = player.Calamity().baguette ? 250 : 200;
            return base.CanUseItem(player);
        }

        public override void OnConsumeItem(Player player)
        {
            player.AddBuff(ModContent.BuffType<RedWineBuff>(), 900);
        }
    }
}
