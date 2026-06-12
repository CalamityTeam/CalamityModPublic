using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Bloodflare
{
    [AutoloadEquip(EquipType.Head)]
    [LegacyName("BloodflareHornedHelm")]
    public class BloodflareHeadRanged : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.PostMoonLord";
        public static readonly SoundStyle ActivationSound = new("CalamityMod/Sounds/Custom/AbilitySounds/BloodflareRangerActivation");

        public static float RangedDamageBoost = 0.1f;
        public static float AmmoReduction = 0.75f;
        public static int RangedCritBoost = 10; // NOTE: Tooltip shares this number with damage % as they're equal
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(RangedDamageBoost.ToPercent(), (1f - AmmoReduction).ToPercent());

        // Set Bonus
        public static int SoulCooldown = CalamityUtils.SecondsToFrames(30);
        public static int SoulDamage = 300;
        public static int SoulAmount = 16;
        public static int BloodBombCooldown = CalamityUtils.SecondsToFrames(2.5f);
        public static double BloodBombDamageRatio = 0.8D;
        public static int BloodBombDamageSoftcap = 120;

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.RarityPureGreenBuyPrice;
            Item.defense = 30; // 94
            Item.rare = ModContent.RarityType<PureGreen>();
        }

        public override bool IsArmorSet(Item head, Item body, Item legs) => body.type == ModContent.ItemType<BloodflareBodyArmor>() && legs.type == ModContent.ItemType<BloodflareCuisses>();

        public override void ArmorSetShadows(Player player) => player.armorEffectDrawShadowSubtle = true;

        public override void UpdateArmorSet(Player player)
        {
            var modPlayer = player.Calamity();
            modPlayer.bloodflareSet = true;
            modPlayer.bloodflareRanged = true;
            player.setBonus = this.GetLocalization("SetBonus").Format(CalamityUtils.GetArmorSetBonusKey(), SoulCooldown.FramesToSeconds(), BloodBombCooldown.FramesToSeconds());
        }

        public override void UpdateEquip(Player player)
        {
            var modPlayer = player.Calamity();
            modPlayer.ammoCost *= AmmoReduction;
            player.GetDamage<RangedDamageClass>() += RangedDamageBoost;
            player.GetCritChance<RangedDamageClass>() += RangedCritBoost;
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
