using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class AquaticEmblem : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";

        public static int MaxDefenseBoost = 15;
        public static float MaxMoveSpeedReduction = 0.1f;
        public static int TimeToReachMaxBoost = CalamityUtils.SecondsToFrames(10);
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(TimeToReachMaxBoost.FramesToSeconds(), MaxDefenseBoost, MaxMoveSpeedReduction.ToPercent());

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 26;
            Item.value = CalamityGlobalItem.RarityPinkBuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.accessory = true;
            Item.expert = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.aquaticEmblem = true;
            player.npcTypeNoAggro[NPCID.Shark] = true;
            player.npcTypeNoAggro[NPCID.SeaSnail] = true;
            player.npcTypeNoAggro[NPCID.PinkJellyfish] = true;
            player.npcTypeNoAggro[NPCID.Crab] = true;
            player.npcTypeNoAggro[NPCID.Squid] = true;
            if (player.Calamity().countsAsAnyWet)
                player.gills = true;
        }
    }
}
