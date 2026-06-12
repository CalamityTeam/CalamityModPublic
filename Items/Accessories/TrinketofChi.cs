using CalamityMod.Buffs.StatBuffs;
using CalamityMod.CalPlayer;
using CalamityMod.CustomRecipes;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class TrinketofChi : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";

        public static float ChiBuffDamageReductionBoost = 0.2f;
        public static int ChiBuffHitlessTime = CalamityUtils.SecondsToFrames(10);
        public static int RegenBoost = 1;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ChiBuffDamageReductionBoost.ToPercent(), ChiBuffHitlessTime.FramesToSeconds(), RegenBoost.ToRegenPerSecond());

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 32;
            Item.value = Item.buyPrice(gold: 10); // Sold by Shady Salesman
            Item.rare = ItemRarityID.Orange;
            Item.accessory = true;
        }

        public override void UpdateInventory(Player player)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient && !RecipeUnlockHandler.HasFoundTrinketOfChi)
            {
                RecipeUnlockHandler.HasFoundTrinketOfChi = true;
                CalamityNetcode.SyncWorld();
            }
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
