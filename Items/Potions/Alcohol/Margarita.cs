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
            DisplayName.SetDefault("Margarita");
            Tooltip.SetDefault(@"One of the best drinks ever created, enjoy it while it lasts
Reduces the duration of most debuffs
Reduces defense by 6% and life regen by 1
3 minute duration");
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
            item.potion = true;
            item.healLife = 200;
            item.healMana = 200;
            item.value = Item.buyPrice(0, 5, 30, 0);
        }

        public override void OnConsumeItem(Player player)
        {
            player.AddBuff(BuffType, BuffDuration);
        }
    }
}
