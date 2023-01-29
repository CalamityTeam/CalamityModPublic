using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions
{
    public class BlasphemousDonut : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blasphemous Donut");
            Tooltip.SetDefault("{$CommonItemTooltip.MediumStats}\n'Surprisingly edible'");
            SacrificeTotal = 5;
        }
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 26;
            Item.value = Item.buyPrice(0, 2, 0, 0);
            Item.rare = ItemRarityID.Purple;
            Item.maxStack = 30;
            Item.consumable = true;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.UseSound = SoundID.Item2;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.useTurn = true;
            Item.buffType = BuffID.WellFed2;
            Item.buffTime = CalamityUtils.SecondsToFrames(3600f);
        }
    }
}
