using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Bloodflare
{
    [AutoloadEquip(EquipType.Head)]
    [LegacyName("BloodflareHelmet")]
    public class BloodflareHeadSummon : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.PostMoonLord";
        internal static string GhostMineEntitySourceContext => "SetBonus_Calamity_BloodflareSummon";

        public static int MinionSlotBoost = 1;
        public static float SummonDamageBoost = 0.3f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MinionSlotBoost, SummonDamageBoost.ToPercent());

        // Set Bonus
        public static int SetBonusMinionSlotBoost = 2;
        public static float SetBonusSummonDamageBoost = 0.3f;
        public static int MineDamage = 3750;
        public static int MineCooldown = 900;
        public static int DefenseBoostBelowHealthThreshold = 20;
        public static float DefenseBoostHealthThreshold = 0.5f;

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.RarityPureGreenBuyPrice;
            Item.defense = 12; // 76
            Item.rare = ModContent.RarityType<PureGreen>();
        }

        public override bool IsArmorSet(Item head, Item body, Item legs) => body.type == ModContent.ItemType<BloodflareBodyArmor>() && legs.type == ModContent.ItemType<BloodflareCuisses>();

        public override void ArmorSetShadows(Player player) => player.armorEffectDrawShadowSubtle = true;

        public override void UpdateArmorSet(Player player)
        {
            var modPlayer = player.Calamity();
            modPlayer.bloodflareSet = true;
            modPlayer.bloodflareSummon = true;
            modPlayer.WearingPostMLSummonerSet = true;
            player.setBonus = this.GetLocalization("SetBonus").Format(SetBonusMinionSlotBoost, SetBonusSummonDamageBoost.ToPercent(), DefenseBoostBelowHealthThreshold, DefenseBoostHealthThreshold.ToPercent());
            player.maxMinions += SetBonusMinionSlotBoost;
            player.GetDamage<SummonDamageClass>() += SetBonusSummonDamageBoost;
        }

        public override void UpdateEquip(Player player)
        {
            player.maxMinions += MinionSlotBoost;
            player.GetDamage<SummonDamageClass>() += SummonDamageBoost;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BloodstoneCore>(11).
                AddIngredient<RuinousSoul>(2).
                AddTile(TileID.MythrilAnvil).
                SortBeforeFirstRecipesOf(ModContent.ItemType<BloodflareHeadRogue>()).
                Register();
        }
    }
}
