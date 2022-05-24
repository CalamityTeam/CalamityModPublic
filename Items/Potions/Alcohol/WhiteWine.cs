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
            SacrificeTotal = 5;
            DisplayName.SetDefault("White Wine");
            Tooltip.SetDefault(@"I drank a full barrel of this stuff once in one night, I couldn't remember who I was the next day
Boosts magic damage by 10%
Reduces defense by 6% and life regen by 1");
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 18;
            Item.useTurn = true;
            Item.maxStack = 30;
            Item.rare = ItemRarityID.LightPurple;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.UseSound = SoundID.Item3;
            Item.consumable = true;
            Item.healMana = 400;
            Item.buffType = ModContent.BuffType<WhiteWineBuff>();
            Item.buffTime = CalamityUtils.SecondsToFrames(300f);
            Item.value = Item.buyPrice(0, 4, 0, 0);
        }

        public override bool? UseItem(Player player)
        {
            if (PlayerInput.Triggers.JustPressed.QuickBuff)
            {
                player.statMana += Item.healMana;
                if (player.statMana > player.statManaMax2)
                {
                    player.statMana = player.statManaMax2;
                }
                player.AddBuff(BuffID.ManaSickness, Player.manaSickTime, true);
                if (Main.myPlayer == player.whoAmI)
                {
                    player.ManaEffect(Item.healMana);
                }
            }
            player.AddBuff(Item.buffType, Item.buffTime);
            return true;
        }
    }
}
