using CalamityMod.Projectiles.Typeless;
using CalamityMod.Items.Placeables;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Weapons.Typeless
{
    public class StarStruckWater : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Star Struck Water");
            Tooltip.SetDefault("Spreads the astral infection to some blocks");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.shootSpeed = 14f;
            Item.rare = ItemRarityID.Orange;
            Item.damage = 20;
            Item.shoot = ModContent.ProjectileType<StarStruckWaterBottle>();
            Item.width = 18;
            Item.height = 20;
            Item.maxStack = 999;
            Item.consumable = true;
            Item.knockBack = 3f;
            Item.UseSound = SoundID.Item1;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.value = 200;
        }

        public override void AddRecipes()
        {
            CreateRecipe(10).AddIngredient(ItemID.BottledWater, 10).AddIngredient(ModContent.ItemType<AstralSand>()).AddIngredient(ModContent.ItemType<AstralMonolith>()).Register();
        }
    }
}
