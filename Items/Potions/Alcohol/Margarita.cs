using CalamityMod.Buffs.Alcohol;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions.Alcohol
{
    public class Margarita : ModItem
    {
        public static int BuffType = ModContent.BuffType<MargaritaBuff>();
        public static int BuffDuration = 10800;

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 5;
            DisplayName.SetDefault("Margarita");
            Tooltip.SetDefault(@"One of the best drinks ever created, enjoy it while it lasts
Reduces the duration of most debuffs
Reduces defense by 6% and life regen by 1
3 minute duration");
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
            Item.potion = true;
            Item.healLife = 200;
            Item.healMana = 200;
            Item.value = Item.buyPrice(0, 5, 30, 0);
        }

        public override void OnConsumeItem(Player player)
        {
            player.AddBuff(BuffType, BuffDuration);
        }
    }
}
