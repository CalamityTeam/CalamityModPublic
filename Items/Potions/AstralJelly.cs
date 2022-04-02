using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions
{
    public class AstralJelly : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aureus Cell");
            Tooltip.SetDefault("Grants increased mana regeneration and magic power");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 18;
            item.useTurn = true;
            item.maxStack = 30;
            item.useAnimation = 17;
            item.useTime = 17;
            item.rare = ItemRarityID.Lime;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.healMana = 200;
            item.UseSound = SoundID.Item3;
            item.consumable = true;
            item.value = Item.buyPrice(0, 4, 50, 0);
            item.buffType = BuffID.MagicPower;
            item.buffTime = CalamityUtils.SecondsToFrames(360f);
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
            player.AddBuff(BuffID.MagicPower, item.buffTime);
            player.AddBuff(BuffID.ManaRegeneration, item.buffTime);
            return true;
        }
    }
}
