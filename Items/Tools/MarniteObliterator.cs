using CalamityMod.Projectiles.Melee;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Tools
{
    public class MarniteObliterator : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Marnite Obliterator");
        }

        public override void SetDefaults()
        {
            item.damage = 13;
            item.knockBack = 1f;
            item.useTime = 6;
            item.useAnimation = 25;
            item.pick = 50;
            item.axe = 30 / 5;

            item.melee = true;
            item.width = 36;
            item.height = 18;
            item.channel = true;
            item.noUseGraphic = true;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.value = CalamityGlobalItem.Rarity1BuyPrice;
            item.rare = ItemRarityID.Blue;
            item.UseSound = SoundID.Item23;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<MarniteObliteratorProj>();
            item.shootSpeed = 40f;
			item.Calamity().trueMelee = true;
		}

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddRecipeGroup("AnyGoldBar", 3);
            recipe.AddIngredient(ItemID.Granite, 5);
            recipe.AddIngredient(ItemID.Marble, 5);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
