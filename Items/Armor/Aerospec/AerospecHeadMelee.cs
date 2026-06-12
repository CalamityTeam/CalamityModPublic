using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Aerospec
{
    [AutoloadEquip(EquipType.Head)]
    [LegacyName("AerospecHelm")]
    public class AerospecHeadMelee : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.PreHardmode";

        public static float MeleeDamageBoost = 0.1f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MeleeDamageBoost.ToPercent());

        // Set Bonus
        public static float SetBonusMoveSpeedBoost = 0.05f;
        public static int SetBonusMeleeCritBoost = 5; // NOTE: Tooltip shares this number with move speed % as they're equal
        public static int SetBonusAggroBoost = 300;

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.defense = 7; //20
        }

        public override bool IsArmorSet(Item head, Item body, Item legs) => body.type == ModContent.ItemType<AerospecBreastplate>() && legs.type == ModContent.ItemType<AerospecLeggings>();

        public override void ArmorSetShadows(Player player) => player.armorEffectDrawShadow = true;

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = this.GetLocalization("SetBonus").Format(SetBonusMoveSpeedBoost.ToPercent(), AerospecBreastplate.SetBonusHurtDamageThreshold);
            var modPlayer = player.Calamity();
            modPlayer.aeroSet = true;
            player.noFallDmg = true;
            player.moveSpeed += SetBonusMoveSpeedBoost;
            player.GetCritChance<MeleeDamageClass>() += SetBonusMeleeCritBoost;
            player.aggro += SetBonusAggroBoost;
        }

        public override void UpdateEquip(Player player) => player.GetDamage<MeleeDamageClass>() += MeleeDamageBoost;

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AerialiteBar>(5).
                AddIngredient(ItemID.Feather).
                AddTile(TileID.Anvils).
                SortBeforeFirstRecipesOf(ModContent.ItemType<AerospecHeadMagic>()).
                Register();
        }
    }
}
