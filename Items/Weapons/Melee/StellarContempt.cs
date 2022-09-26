using CalamityMod.Projectiles.Melee;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    [LegacyName("StellarContemptMelee", "StellarContemptRogue")]
    public class StellarContempt : ModItem
    {
        public static int BaseDamage = 300;
        public static float Speed = 18f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stellar Contempt");
            Tooltip.SetDefault("Lunar flares rain down on enemy hits");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 74;
            Item.height = 74;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.damage = BaseDamage;
            Item.knockBack = 9f;
            Item.useTime = 13;
            Item.useAnimation = 13;
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
                AddIngredient(ItemID.FragmentSolar, 10).
                AddIngredient(ItemID.FragmentNebula, 10).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
