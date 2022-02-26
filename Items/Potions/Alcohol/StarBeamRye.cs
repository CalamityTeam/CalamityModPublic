using CalamityMod.Buffs.Alcohol;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions.Alcohol
{
    public class StarBeamRye : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Star Beam Rye");
            Tooltip.SetDefault(@"Made from some stuff I found near the Astral Meteor crash site, don't worry it's safe, trust me
Boosts max mana by 50, magic damage by 8%,
and reduces mana usage by 10%
Reduces defense by 6% and life regen by 1");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 18;
            item.useTurn = true;
            item.maxStack = 30;
            item.rare = ItemRarityID.Lime;
            item.useAnimation = 17;
            item.useTime = 17;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.UseSound = SoundID.Item3;
            item.consumable = true;
            item.buffType = ModContent.BuffType<StarBeamRyeBuff>();
            item.buffTime = CalamityUtils.SecondsToFrames(480f);
            item.value = Item.buyPrice(0, 4, 0, 0);
        }
    }
}
