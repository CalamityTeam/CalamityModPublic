using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Bloodflare
{
    [AutoloadEquip(EquipType.Head)]
    [LegacyName("BloodflareMask")]
    public class BloodflareHeadMelee : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.PostMoonLord";

        public static float MeleeDamageBoost = 0.1f;
        public static int MeleeCritBoost = 5;
        public static float MeleeSpeedBoost = 0.18f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MeleeDamageBoost.ToPercent(), MeleeCritBoost, MeleeSpeedBoost.ToPercent());

        // Set Bonus
        public static int SetBonusAggroBoost = 900;
        public static int HitsToActivateFrenzy = 15;
        public static float FrenzyMeleeDamageBoost = 0.25f;
        public static int FrenzyMeleeCritBoost = 25; // NOTE: Tooltip shares this number with damage % as they're equal
        public static float FrenzyContactDamageReduction = 0.5f;
        public static int FrenzyDuration = CalamityUtils.SecondsToFrames(5);
        public static int FrenzyCooldown = CalamityUtils.SecondsToFrames(30);

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.RarityPureGreenBuyPrice;
            Item.defense = 44; // 108
            Item.rare = ModContent.RarityType<PureGreen>();
        }

        public override bool IsArmorSet(Item head, Item body, Item legs) => body.type == ModContent.ItemType<BloodflareBodyArmor>() && legs.type == ModContent.ItemType<BloodflareCuisses>();

        public override void ArmorSetShadows(Player player) => player.armorEffectDrawShadowSubtle = true;

        public override void UpdateArmorSet(Player player)
        {
            var modPlayer = player.Calamity();
            modPlayer.bloodflareSet = true;
            modPlayer.bloodflareMelee = true;
            player.aggro += SetBonusAggroBoost;
            player.setBonus = this.GetLocalization("SetBonus").Format(HitsToActivateFrenzy, FrenzyDuration.FramesToSeconds(), FrenzyMeleeDamageBoost.ToPercent(), FrenzyContactDamageReduction.ToPercent(), FrenzyCooldown.FramesToSeconds());
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<MeleeDamageClass>() += MeleeDamageBoost;
            player.GetCritChance<MeleeDamageClass>() += MeleeCritBoost;
            player.GetAttackSpeed<MeleeDamageClass>() += MeleeSpeedBoost;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BloodstoneCore>(11).
                AddIngredient<RuinousSoul>(2).
                AddTile(TileID.MythrilAnvil).
                SortBeforeFirstRecipesOf(ModContent.ItemType<BloodflareHeadMagic>()).
                Register();
        }
    }
}
