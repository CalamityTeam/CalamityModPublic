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
            Item.value = Item.buyPrice(0, 60, 0, 0);
            Item.rare = ItemRarityID.Lime;
            Item.UseSound = SoundID.Item13;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<EvergladeSprayProjectile>();
            Item.shootSpeed = 10f;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.GoldenShower).AddIngredient(ModContent.ItemType<DraedonBar>(), 3).AddTile(TileID.Bookcases).Register();
            CreateRecipe(1).AddIngredient(ItemID.CursedFlames).AddIngredient(ModContent.ItemType<DraedonBar>(), 3).AddTile(TileID.Bookcases).Register();
        }
    }
}
