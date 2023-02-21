using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.Buffs.StatBuffs;

namespace CalamityMod.Items.Accessories
{
    public class TrinketofChi : ModItem
    {
        internal const int ChiBuffTimerMax = 900;

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Trinket of Chi");
            Tooltip.SetDefault("Provides 10% damage reduction after not being hit for 15 seconds, this is removed when you are hit\n" +
                "Provides 2 life regen for you and everyone on your team");
        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 32;
            Item.value = CalamityGlobalItem.Rarity3BuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.trinketOfChi = true;
            if (player.whoAmI != Main.myPlayer && player.miscCounter % 10 == 0)
            {
                if (Main.LocalPlayer.team == player.team && player.team != 0)
                {
                    Main.LocalPlayer.AddBuff(ModContent.BuffType<ChiRegenBuff>(), 20, true);
                }
            }
        }
    }
}
