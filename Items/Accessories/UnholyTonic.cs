using CalamityMod.Buffs.DamageOverTime;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    [LegacyName("CorruptFlask")]
    public class UnholyTonic : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";

        public static int DefenseBoostInCorruption = 3;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DefenseBoostInCorruption);
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.value = CalamityGlobalItem.RarityGreenBuyPrice;
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[ModContent.BuffType<BrainRot>()] = true;
            if (player.ZoneCorrupt)
                player.statDefense += DefenseBoostInCorruption;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.VilePowder, 15).
                AddIngredient(ItemID.RottenChunk, 10).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
