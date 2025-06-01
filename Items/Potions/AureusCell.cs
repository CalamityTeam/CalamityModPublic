using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions
{
    [LegacyName("AstralJelly")]
    public class AureusCell : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Potions";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 20;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 18;
            Item.useTurn = true;
            Item.maxStack = 9999;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.rare = ItemRarityID.Lime;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.healMana = 200;
            Item.UseSound = SoundID.Item3;
            Item.consumable = true;
            Item.value = Item.buyPrice(0, 4, 50, 0);
            Item.buffType = BuffID.MagicPower;
            Item.buffTime = CalamityUtils.SecondsToFrames(360f);
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
            player.AddBuff(BuffID.MagicPower, Item.buffTime);
            player.AddBuff(BuffID.ManaRegeneration, Item.buffTime);
        }
    }
}
