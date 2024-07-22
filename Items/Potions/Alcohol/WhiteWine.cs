using CalamityMod.Buffs.Alcohol;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions.Alcohol
{
    public class WhiteWine : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Potions";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 18;
            Item.useTurn = true;
            Item.maxStack = 9999;
            Item.rare = ItemRarityID.LightPurple;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.UseSound = SoundID.Item3;
            Item.consumable = true;
            Item.healMana = 300;
            Item.buffType = ModContent.BuffType<WhiteWineBuff>();
            Item.buffTime = CalamityUtils.SecondsToFrames(300f);
            Item.value = Item.buyPrice(0, 4, 0, 0);
        }

        public override void OnConsumeItem(Player player)
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
        }
    }
}
