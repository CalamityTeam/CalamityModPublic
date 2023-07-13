using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
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

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.Rarity8BuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.defense = 20;
        }

        public override void UpdateEquip(Player player)
        {
            player.statLifeMax2 += 20;
            player.GetDamage<GenericDamageClass>() += 0.08f;
            player.GetCritChance<GenericDamageClass>() += 4;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ScoriaBar>(15).
                AddIngredient<CoreofHavoc>(3).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
