using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Daedalus
{
    [AutoloadEquip(EquipType.Head)]
    [LegacyName("DaedalusVisor")]
    public class DaedalusHeadRogue : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.Hardmode";

        public static float RogueDamageBoost = 0.13f;
        public static int RogueCritBoost = 5;
        public static float RogueVelocityBoost = 0.15f;
        public static float MoveSpeedBoost = 0.05f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(RogueDamageBoost.ToPercent(), RogueCritBoost, RogueVelocityBoost.ToPercent(), MoveSpeedBoost.ToPercent());

        // Set Bonus
        public static float SetBonusRogueStealth = 1.05f;
        public static int ShardCountLimit = 15;
        public static double ShardDamageRatio = 0.25D;
        public static int ShardDamageSoftcap = 30;

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.RarityPinkBuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.defense = 10; // 44
        }

        public override bool IsArmorSet(Item head, Item body, Item legs) => body.type == ModContent.ItemType<DaedalusBreastplate>() && legs.type == ModContent.ItemType<DaedalusLeggings>();

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadowSubtle = true;
            player.armorEffectDrawOutlines = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = this.GetLocalization("SetBonus").Format(SetBonusRogueStealth.ToStealth());
            var modPlayer = player.Calamity();
            modPlayer.daedalusSplit = true;
            modPlayer.rogueStealthMax += SetBonusRogueStealth;
            modPlayer.wearingRogueArmor = true;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<ThrowingDamageClass>() += RogueDamageBoost;
            player.GetCritChance<ThrowingDamageClass>() += RogueCritBoost;
            player.Calamity().rogueVelocity += RogueVelocityBoost;
            player.moveSpeed += MoveSpeedBoost;
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
