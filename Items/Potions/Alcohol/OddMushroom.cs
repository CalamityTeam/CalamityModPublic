using CalamityMod.Buffs.Alcohol;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions.Alcohol
{
    public class OddMushroom : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Odd Mushroom");
            Tooltip.SetDefault("Causes you to see many fake, vibrant copies of all nearby entities\n" +
                "These visual effects may be nauseating or otherwise bad for some\n" +
                "Trippy");
        }

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 48;
            Item.useTurn = true;
            Item.maxStack = 30;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.rare = ItemRarityID.LightRed;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.UseSound = SoundID.Item2;
            Item.consumable = true;
            Item.buffType = ModContent.BuffType<Trippy>();
            Item.buffTime = CalamityUtils.SecondsToFrames(3600f);
            Item.value = Item.buyPrice(0, 50, 0, 0);
        }
    }
}
