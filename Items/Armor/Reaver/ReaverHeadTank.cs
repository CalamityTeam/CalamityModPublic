using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Reaver
{
    [AutoloadEquip(EquipType.Head)]
    [LegacyName("ReaverHelm")]
    public class ReaverHeadTank : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.Hardmode";
        internal static string HealOrbEntitySourceContext => "SetBonus_Calamity_ReaverTank";

        public static int MaxLifeBoost = 50;
        public static int RegenBoost = 8;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MaxLifeBoost, RegenBoost.ToRegenPerSecond());

        // Set Bonus
        public static int SetBonusAggroBoost = 600;
        public static float SetBonusDebuffDamageReduction = 0.2f;
        public static float SetBonusMobilityReduction = 0.3f;
        public static int ReaverRageDuration = CalamityUtils.SecondsToFrames(5);
        public static int ReaverRageDefenseBoost = 5;
        public static float ReaverRageDamageBoost = 0.1f;

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 30;
            Item.value = CalamityGlobalItem.RarityLimeBuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.defense = 28; // 70 (75 with Reaver Rage)
        }

        public override bool IsArmorSet(Item head, Item body, Item legs) => body.type == ModContent.ItemType<ReaverScaleMail>() && legs.type == ModContent.ItemType<ReaverCuisses>();

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadowSubtle = true;
            player.armorEffectDrawOutlines = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            var modPlayer = player.Calamity();
            player.moveSpeed -= SetBonusMobilityReduction;
            player.aggro += SetBonusAggroBoost;
            modPlayer.reaverDefense = true;
            modPlayer.wearingRogueArmor = true;
            player.setBonus = this.GetLocalization("SetBonus").Format(SetBonusDebuffDamageReduction.ToPercent(), SetBonusMobilityReduction.ToPercent(), ReaverRageDuration.FramesToSeconds());
        }

        public override void UpdateEquip(Player player)
        {
            player.statLifeMax2 += MaxLifeBoost;
            player.lifeRegen += RegenBoost;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<PerennialBar>(7).
                AddTile(TileID.MythrilAnvil).
                SortBeforeFirstRecipesOf(ModContent.ItemType<ReaverHeadMobility>()).
                Register();
        }
    }
}
