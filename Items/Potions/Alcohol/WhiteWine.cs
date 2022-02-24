using CalamityMod.Buffs.Alcohol;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions.Alcohol
{
    public class WhiteWine : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("White Wine");
            Tooltip.SetDefault(@"I drank a full barrel of this stuff once in one night, I couldn't remember who I was the next day
Boosts magic damage by 10%
Reduces defense by 6% and life regen by 1");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 18;
            item.useTurn = true;
            item.maxStack = 30;
            item.rare = ItemRarityID.LightPurple;
            item.useAnimation = 17;
            item.useTime = 17;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.UseSound = SoundID.Item3;
            item.consumable = true;
            item.healMana = 400;
            item.buffType = ModContent.BuffType<WhiteWineBuff>();
            item.buffTime = CalamityUtils.SecondsToFrames(300f);
            item.value = Item.buyPrice(0, 4, 0, 0);
        }

		public override bool UseItem(Player player)
		{
			if (PlayerInput.Triggers.JustPressed.QuickBuff)
			{
				player.statMana += item.healMana;
				if (player.statMana > player.statManaMax2)
				{
					player.statMana = player.statManaMax2;
				}
				player.AddBuff(BuffID.ManaSickness, Player.manaSickTime, true);
				if (Main.myPlayer == player.whoAmI)
				{
					player.ManaEffect(item.healMana);
				}
			}
            player.AddBuff(item.buffType, item.buffTime);
			return true;
		}
    }
}
