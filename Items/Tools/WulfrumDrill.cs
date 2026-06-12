using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Typeless;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Tools
{
    [LegacyName("WulfrumPickaxe")]
    public class WulfrumDrill : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Tools";
        public static int ArmorPenetration = 15;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ArmorPenetration);
        public override void SetDefaults()
        {
            Item.width = 46;
            Item.height = 38;
            Item.damage = 5;
            Item.DamageType = DamageClass.Melee;
            Item.ArmorPenetration = ArmorPenetration;
            Item.pick = 35;
            Item.tileBoost += 1;
            Item.useAnimation = 16;
            Item.useTime = 5;
            Item.knockBack = 0.5f;
            Item.shoot = ModContent.ProjectileType<WulfrumDrillProj>();

            Item.UseSound = SoundID.Item23;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.channel = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;

            Item.value = CalamityGlobalItem.RarityBlueBuyPrice;
            Item.rare = ItemRarityID.Blue;
        }

        public override void HoldItem(Player player) => player.Calamity().mouseWorldListener = true;

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<WulfrumMetalScrap>(5).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
