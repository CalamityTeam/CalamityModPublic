using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Bloodflare
{
    [AutoloadEquip(EquipType.Head)]
    [LegacyName("BloodflareHornedMask")]
    public class BloodflareHeadMagic : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.PostMoonLord";

        public static int MaxManaBoost = 100;
        public static float ManaCostReduction = 0.17f;
        public static float MagicDamageBoost = 0.2f;
        public static int MagicCritBoost = 10;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MaxManaBoost, ManaCostReduction.ToPercent(), MagicDamageBoost.ToPercent(), MagicCritBoost);

        // Set Bonus
        public static int GhostBoltCooldown = CalamityUtils.SecondsToFrames(1.67f);
        public static double GhostBoltDamageRatio = 1.3D;
        public static int GhostBoltonDamageSoftcap = 2600;
        public static int BloodsplosionCooldown = CalamityUtils.SecondsToFrames(2);
        public static double BloodsplosionDamageRatio = 0.5D;
        public static int BloodsplosionDamageSoftcap = 250;

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.RarityPureGreenBuyPrice;
            Item.defense = 18; // 82
            Item.rare = ModContent.RarityType<PureGreen>();
        }

        public override bool IsArmorSet(Item head, Item body, Item legs) => body.type == ModContent.ItemType<BloodflareBodyArmor>() && legs.type == ModContent.ItemType<BloodflareCuisses>();

        public override void ArmorSetShadows(Player player) => player.armorEffectDrawShadowSubtle = true;

        public override void UpdateArmorSet(Player player)
        {
            var modPlayer = player.Calamity();
            modPlayer.bloodflareSet = true;
            modPlayer.bloodflareMage = true;
            player.setBonus = this.GetLocalization("SetBonus").Format(GhostBoltCooldown.FramesToSeconds(), BloodsplosionCooldown.FramesToSeconds());
        }

        public override void UpdateEquip(Player player)
        {
            player.statManaMax2 += MaxManaBoost;
            player.manaCost -= ManaCostReduction;
            player.GetDamage<MagicDamageClass>() += MagicDamageBoost;
            player.GetCritChance<MagicDamageClass>() += MagicCritBoost;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BloodstoneCore>(11).
                AddIngredient<RuinousSoul>(2).
                AddTile(TileID.MythrilAnvil).
                SortBeforeFirstRecipesOf(ModContent.ItemType<BloodflareBodyArmor>()).
                Register();
        }
    }
}
