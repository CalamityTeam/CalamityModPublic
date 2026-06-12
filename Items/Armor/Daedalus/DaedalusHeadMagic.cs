using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Daedalus
{
    [AutoloadEquip(EquipType.Head)]
    [LegacyName("DaedalusHat")]
    public class DaedalusHeadMagic : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.Hardmode";

        public static int MaxManaBoost = 80;
        public static float ManaCostReduction = 0.1f;
        public static float MagicDamageBoost = 0.13f;
        public static int MagicCritBoost = 7;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MaxManaBoost, ManaCostReduction.ToPercent(), MagicDamageBoost.ToPercent(), MagicCritBoost);

        // Set Bonus
        public static int AbsorptionChanceDenominator = 10;
        public static float DamageAbsorptionPercent = 0.5f;

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.RarityPinkBuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.defense = 8; // 42
        }

        public override bool IsArmorSet(Item head, Item body, Item legs) => body.type == ModContent.ItemType<DaedalusBreastplate>() && legs.type == ModContent.ItemType<DaedalusLeggings>();

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadowSubtle = true;
            player.armorEffectDrawOutlines = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = this.GetLocalization("SetBonus").Format(AbsorptionChanceDenominator.GetChanceFromDenominator(), DamageAbsorptionPercent.ToPercent());
            var modPlayer = player.Calamity();
            modPlayer.daedalusAbsorb = true;
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
                AddIngredient<CryonicBar>(7).
                AddTile(TileID.MythrilAnvil).
                SortBeforeFirstRecipesOf(ModContent.ItemType<DaedalusBreastplate>()).
                Register();
        }
    }
}
