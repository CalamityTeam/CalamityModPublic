using CalamityMod.Buffs.Alcohol;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions.Alcohol
{
    public class OldFashioned : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Potions";

        public static readonly float AccessoryAndSetBonusDamageMultiplier = 1.5f;
        public static readonly float DamageStatReduction = 0.25f;

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 40;
            Item.useTurn = true;
            Item.maxStack = 9999;
            Item.rare = ItemRarityID.Lime;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.UseSound = SoundID.Item3;
            Item.consumable = true;
            Item.buffType = ModContent.BuffType<OldFashionedBuff>();
            Item.buffTime = CalamityUtils.SecondsToFrames(360f);
            Item.value = Item.buyPrice(0, 5, 30, 0);
        }
    }
}
