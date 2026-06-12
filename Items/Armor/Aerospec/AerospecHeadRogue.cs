using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Aerospec
{
    [AutoloadEquip(EquipType.Head)]
    [LegacyName("AerospecHeadgear")]
    public class AerospecHeadRogue : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.PreHardmode";

        public static float RogueDamageBoost = 0.1f;
        public static float MoveSpeedBoost = 0.05f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(RogueDamageBoost.ToPercent(), MoveSpeedBoost.ToPercent());

        // Set Bonus
        public static float SetBonusRogueStealth = 0.8f;
        public static float SetBonusMoveSpeedBoost = 0.05f;
        public static int SetBonusRogueCritBoost = 5; // NOTE: Tooltip shares this number with move speed % as they're equal

        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
                ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.defense = 4; //17
        }

        public override bool IsArmorSet(Item head, Item body, Item legs) => body.type == ModContent.ItemType<AerospecBreastplate>() && legs.type == ModContent.ItemType<AerospecLeggings>();

        public override void ArmorSetShadows(Player player) => player.armorEffectDrawShadow = true;

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = this.GetLocalization("SetBonus").Format(SetBonusRogueStealth.ToStealth(), SetBonusMoveSpeedBoost.ToPercent(), AerospecBreastplate.SetBonusHurtDamageThreshold);
            var modPlayer = player.Calamity();
            modPlayer.aeroSet = true;
            modPlayer.rogueStealthMax += SetBonusRogueStealth;
            player.noFallDmg = true;
            player.moveSpeed += SetBonusMoveSpeedBoost;
            player.GetCritChance<ThrowingDamageClass>() += SetBonusRogueCritBoost;
            player.Calamity().wearingRogueArmor = true;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<ThrowingDamageClass>() += RogueDamageBoost;
            player.moveSpeed += MoveSpeedBoost;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AerialiteBar>(5).
                AddIngredient(ItemID.Feather).
                AddTile(TileID.Anvils).
                SortAfterFirstRecipesOf(ModContent.ItemType<AerospecBreastplate>()).
                Register();
        }
    }
}
