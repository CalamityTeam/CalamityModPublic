using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Melee;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class CrystalBlade : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crystal Blade");
            Tooltip.SetDefault("Fires slow-moving clouds of crystal dust");
        }

        public override void SetDefaults()
        {
            Item.width = 66;
            Item.damage = 45;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 25;
            Item.useTurn = true;
            Item.knockBack = 4.5f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.height = 66;
            Item.value = CalamityGlobalItem.Rarity4BuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.shoot = ModContent.ProjectileType<CrystalDust>();
            Item.shootSpeed = 3f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.CrystalShard, 10).
                AddIngredient<SeaPrism>(15).
                AddRecipeGroup("AnyCobaltBar", 8).
                AddIngredient(ItemID.PixieDust, 10).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
