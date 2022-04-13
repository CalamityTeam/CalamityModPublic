using CalamityMod.Buffs.Alcohol;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

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
            Item.buffType = ModContent.BuffType<StarBeamRyeBuff>();
            Item.buffTime = CalamityUtils.SecondsToFrames(480f);
            Item.value = Item.buyPrice(0, 4, 0, 0);
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 5;
        }
    }
}
