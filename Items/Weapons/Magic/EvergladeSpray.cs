using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class EvergladeSpray : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Everglade Spray");
            Tooltip.SetDefault("Fires a stream of burning green ichor");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 28;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 8;
            Item.width = 34;
            Item.height = 30;
            Item.useTime = 6;
            Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4.5f;
            Item.value = CalamityGlobalItem.Rarity7BuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.UseSound = SoundID.Item13;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<EvergladeSprayProjectile>();
            Item.shootSpeed = 10f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.GoldenShower).
                AddIngredient<PerennialBar>(3).
                AddTile(TileID.Bookcases).
                Register();
            CreateRecipe().
                AddIngredient(ItemID.CursedFlames).
                AddIngredient<PerennialBar>(3).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
