using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Reaver
{
    [AutoloadEquip(EquipType.Head)]
    [LegacyName("ReaverVisage")]
    public class ReaverHeadMobility : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.Hardmode";

        public static float MoveSpeedBoost = 0.15f;
        public static float JumpSpeedBoost = 0.5f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MoveSpeedBoost.ToPercent(), JumpSpeedBoost.ToJumpSpeedPercent());

        public static float SetBonusFlightBoost = 0.1f;
        public static float SetBonusHookBoost = 0.5f;
        public static int SetBonusDashDelayReductionInterval = 3;

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 28;
            Item.value = CalamityGlobalItem.RarityLimeBuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.defense = 13; // 55
        }

        public override bool IsArmorSet(Item head, Item body, Item legs) => body.type == ModContent.ItemType<ReaverScaleMail>() && legs.type == ModContent.ItemType<ReaverCuisses>();

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadowSubtle = true;
            player.armorEffectDrawOutlines = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            var modPlayer = player.Calamity();
            modPlayer.reaverSpeed = true;
            modPlayer.wearingRogueArmor = true;
            player.setBonus = this.GetLocalization("SetBonus").Format(SetBonusFlightBoost.ToPercent(), SetBonusHookBoost.ToPercent(), (1 / (float)SetBonusDashDelayReductionInterval).ToPercent());
            player.noFallDmg = true;
            player.autoJump = true;
            if (player.miscCounter % SetBonusDashDelayReductionInterval == 1 && player.dashDelay > 0)
                player.dashDelay--;
        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += MoveSpeedBoost;
            player.jumpSpeedBoost += JumpSpeedBoost;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<PerennialBar>(7).
                AddTile(TileID.MythrilAnvil).
                SortBeforeFirstRecipesOf(ModContent.ItemType<ReaverHeadExplore>()).
                Register();
        }
    }
}
