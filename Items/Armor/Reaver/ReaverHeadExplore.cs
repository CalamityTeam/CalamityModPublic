using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Reaver
{
    [AutoloadEquip(EquipType.Head)]
    [LegacyName("ReaverHeadgear")]
    public class ReaverHeadExplore : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.Hardmode";

        public static float MiningSpeedBoost = 0.2f;
        public static float PlacementSpeedBoost = 0.5f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MiningSpeedBoost.ToPercent(), PlacementSpeedBoost.ToPercent());

        // Set Bonus
        public static int SetBonusAggroReduction = 400;
        public static int SetBonusTileRangeBoost = 7;
        public static int SetBonusGrabRangeBoost = 246; // (2.625 + 15.375 = 18 tiles)

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 22;
            Item.value = CalamityGlobalItem.RarityLimeBuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.defense = 6; // 48
        }

        public override bool IsArmorSet(Item head, Item body, Item legs) => body.type == ModContent.ItemType<ReaverScaleMail>() && legs.type == ModContent.ItemType<ReaverCuisses>();

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadowSubtle = true;
            player.armorEffectDrawOutlines = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = this.GetLocalization("SetBonus").Format(SetBonusTileRangeBoost);
            var modPlayer = player.Calamity();
            modPlayer.reaverExplore = true;
            modPlayer.wearingRogueArmor = true;
            player.findTreasure = true;
            player.aggro -= SetBonusAggroReduction;
            if (player.Calamity().countsAsAnyWet)
                player.gills = true;

            DelegateMethods.v3_1 = new Vector3(1f, 1f, 1f);
            Utils.PlotTileLine(player.Center, player.Center + player.velocity * 6f, 20f, DelegateMethods.CastLightOpen);
            Utils.PlotTileLine(player.Left, player.Right, 20f, DelegateMethods.CastLightOpen);

            if (player.whoAmI == Main.myPlayer)
            {
                // These are static variables. Awesome
                Player.tileRangeX += SetBonusTileRangeBoost;
                Player.tileRangeY += SetBonusTileRangeBoost;
            }
        }

        public override void UpdateEquip(Player player)
        {
            player.ignoreWater = true;
            player.lavaImmune = true;
            player.pickSpeed -= MiningSpeedBoost;
            player.tileSpeed += PlacementSpeedBoost;
            player.wallSpeed += PlacementSpeedBoost;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<PerennialBar>(7).
                AddTile(TileID.MythrilAnvil).
                SortBeforeFirstRecipesOf(ModContent.ItemType<ReaverCuisses>()).
                Register();
        }
    }
}
