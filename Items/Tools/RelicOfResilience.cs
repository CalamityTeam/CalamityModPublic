using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Tools
{
    public class RelicOfResilience : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Tools";
        public static int baseTimeMax => 300;
        public static int maxPowerTime => 1800;
        public static int baseCooldown => 900;
        public static int baseDefenseFloor => 200;
        public static float baseDrFloor => 0.1f;
        public static int shardBaseDamage => 180;
        public static float orbitDamageMult => 0.2f;
        public static int baseMaxShardCount => 30;
        public static float maxPowerShardMult => 2; // Applied to number of shards at max power
        public static float additionalMaxPowerDefensesMult => 1; // This is added to the base 100% effectiveness
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 34;
            Item.value = CalamityGlobalItem.RarityPurpleBuyPrice;
            Item.rare = ItemRarityID.Purple;
        }
        public override void HoldItem(Player player)
        {
            player.Calamity().mouseWorldListener = true;
        }

        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = (ContentSamples.CreativeHelper.ItemGroup)CalamityResearchSorting.ToolsOther;
        }
    }
}
