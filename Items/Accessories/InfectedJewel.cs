using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Potions;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    [LegacyName("CelestialJewel")]
    public class InfectedJewel : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";

        public static int RegenBoost = 2;
        public static int ReducedDoTAmount = 8;
        public static int PostDebuffRegenTimeBoost = CalamityUtils.SecondsToFrames(10);
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(RegenBoost.ToRegenPerSecond(), ReducedDoTAmount.ToRegenPerSecond(), PostDebuffRegenTimeBoost.FramesToSeconds());
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.value = CalamityGlobalItem.RarityLimeBuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.infectedJewel = true;
            player.lifeRegen += RegenBoost;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<CrownJewel>().
                AddIngredient<AureusCell>(10).
                AddIngredient<StarblightSoot>(25).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
