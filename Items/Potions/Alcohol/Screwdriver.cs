using CalamityMod.Buffs.Alcohol;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions.Alcohol
{
    public class Screwdriver : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 5;
            DisplayName.SetDefault("Screwdriver");
            Tooltip.SetDefault(@"Do you have a screw loose?
Multiplies piercing projectile damage by 1.05
Reduces life regen by 1");
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 26;
            Item.useTurn = true;
            Item.maxStack = 30;
            Item.rare = ItemRarityID.LightPurple;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.UseSound = SoundID.Item3;
            Item.consumable = true;
            Item.buffType = ModContent.BuffType<ScrewdriverBuff>();
            Item.buffTime = CalamityUtils.SecondsToFrames(480f);
            Item.value = Item.buyPrice(0, 4, 0, 0);
        }
    }
}
