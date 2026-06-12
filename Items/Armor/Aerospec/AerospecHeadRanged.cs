using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Aerospec
{
    [AutoloadEquip(EquipType.Head)]
    [LegacyName("AerospecHood")]
    public class AerospecHeadRanged : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.PreHardmode";

        public static float RangedDamageBoost = 0.1f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(RangedDamageBoost.ToPercent());

        // Set Bonus
        public static float SetBonusMoveSpeedBoost = 0.05f;
        public static int SetBonusRangedCritBoost = 5; // NOTE: Tooltip shares this number with move speed % as they're equal

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.defense = 5; //18
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
            player.GetCritChance<RangedDamageClass>() += SetBonusRangedCritBoost;
        }

        public override void UpdateEquip(Player player) => player.GetDamage<RangedDamageClass>() += RangedDamageBoost;

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AerialiteBar>(5).
                AddIngredient(ItemID.Feather).
                AddTile(TileID.Anvils).
                SortAfterFirstRecipesOf(ModContent.ItemType<AerospecHeadMagic>()).
                Register();
        }
    }
}
