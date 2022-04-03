using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class MarniteRifleSpear : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Marnite Bayonet");
            Tooltip.SetDefault("The gun damages enemies that touch it");
        }

        public override void SetDefaults()
        {
            Item.damage = 20;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 72;
            Item.height = 20;
            Item.useTime = 28;
            Item.useAnimation = 28;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2.25f;
            Item.value = CalamityGlobalItem.Rarity1BuyPrice;
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item41;
            Item.autoReuse = true;
            Item.shootSpeed = 22f;
            Item.useAmmo = AmmoID.Bullet;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddRecipeGroup("AnyGoldBar", 7).AddIngredient(ItemID.Granite, 5).AddIngredient(ItemID.Marble, 5).AddTile(TileID.Anvils).Register();
        }
    }
}
