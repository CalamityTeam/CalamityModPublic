using CalamityMod.Buffs.Alcohol;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Potions.Alcohol
{
    public class BloodyMary : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 5;
            DisplayName.SetDefault("Bloody Mary");
            Tooltip.SetDefault(@"Extra spicy and bloody!
Boosts damage by 15%, movement speed by 10%, melee speed by 15% and crit chance by 7% during a Blood Moon
Reduces life regen by 2 and defense by 4%");
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 18;
            Item.useTurn = true;
            Item.maxStack = 30;
            Item.rare = ItemRarityID.Lime;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.UseSound = SoundID.Item3;
            Item.consumable = true;
            Item.buffType = ModContent.BuffType<BloodyMaryBuff>();
            Item.buffTime = CalamityUtils.SecondsToFrames(480f);
            Item.value = Item.buyPrice(0, 2, 60, 0);
        }
    }
}
