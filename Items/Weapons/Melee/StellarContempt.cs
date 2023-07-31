using CalamityMod.Projectiles.Melee;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Materials;

namespace CalamityMod.Items.Weapons.Melee
{
    [LegacyName("StellarContemptMelee", "StellarContemptRogue")]
    public class StellarContempt : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public static int BaseDamage = 650;
        public static float Speed = 25f;

        public override void SetDefaults()
        {
            Item.width = 74;
            Item.height = 74;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.damage = BaseDamage;
            Item.knockBack = 28f;
            Item.useTime = 45;
            Item.useAnimation = 45;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;

            Item.value = CalamityGlobalItem.Rarity10BuyPrice;
            Item.rare = ItemRarityID.Red;

            Item.shoot = ModContent.ProjectileType<StellarContemptHammer>();
            Item.shootSpeed = Speed;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<FallenPaladinsHammer>().
                AddIngredient(ItemID.LunarBar, 5).
                AddIngredient<GalacticaSingularity>(5).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
