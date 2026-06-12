using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Aerospec
{
    [AutoloadEquip(EquipType.Body)]
    public class AerospecBreastplate : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.PreHardmode";
        internal static string FeatherEntitySourceContext => "SetBonus_Calamity_Aerospec";

        public static float DamageBoost = 0.03f;
        public static int CritBoost = 3; // NOTE: Tooltip shares this number with damage % as they're equal
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageBoost.ToPercent());

        // Common Set Bonus
        public static int SetBonusHurtDamageThreshold = 25;
        public static int SetBonusFeatherDamage => CalamityUtils.ScaleWithDifficulty(15);
        public static float SetBonusFallSpeed = 15f;

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.defense = 7;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<GenericDamageClass>() += DamageBoost;
            player.GetCritChance<GenericDamageClass>() += CritBoost;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AerialiteBar>(15).
                AddIngredient(ItemID.Feather, 2).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
