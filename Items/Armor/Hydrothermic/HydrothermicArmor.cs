using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Hydrothermic
{
    [AutoloadEquip(EquipType.Body)]
    [LegacyName("AtaxiaArmor")]
    public class HydrothermicArmor : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.Hardmode";
        internal static string VanitySmokeEntitySourceContext => "SetBonus_Calamity_Hydrothermic_Vanity";
        internal static string InfernoPotionEntitySourceContext => "SetBonus_Calamity_Hydrothermic_InfernoPotionBoost";

        public static float DamageBoost = 0.13f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageBoost.ToPercent());

        // Common Set Bonus
        public static int BlazeDamage => CalamityUtils.ScaleWithDifficulty(115);
        public static float InfernoHealthThreshold = 0.5f;
        public static int InfernoHitRate = 30;
        public static int InfernoDamage = 50;
        public static float InfernoRange = 300f;

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.RarityYellowBuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.defense = 20;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<GenericDamageClass>() += DamageBoost;
            player.lavaImmune = true;
            player.buffImmune[BuffID.OnFire] = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ScoriaBar>(15).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
