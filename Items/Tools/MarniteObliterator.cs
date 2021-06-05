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
            item.melee = true;
            item.width = 36;
            item.height = 18;
            item.useTime = 7;
            item.useAnimation = 25;
            item.channel = true;
            item.noUseGraphic = true;
            item.noMelee = true;
            item.pick = 57;
            item.axe = 10;
            item.tileBoost++;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.knockBack = 3;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = ItemRarityID.Green;
            item.UseSound = SoundID.Item23;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<MarniteObliteratorProj>();
            item.shootSpeed = 40f;
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
