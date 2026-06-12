using CalamityMod.Buffs.DamageOverTime;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    [LegacyName("CrimsonFlask")]
    public class ViciousTonic : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";

        public static int DefenseBoostInCrimson = 3;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DefenseBoostInCrimson);
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
            player.buffImmune[ModContent.BuffType<BurningBlood>()] = true;
            if (player.ZoneCrimson)
                player.statDefense += DefenseBoostInCrimson;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.ViciousPowder, 15).
                AddIngredient(ItemID.Vertebrae, 10).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
