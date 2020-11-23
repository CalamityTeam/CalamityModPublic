using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class ScourgeoftheCosmos : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Scourge of the Cosmos");
            Tooltip.SetDefault("Throws a bouncing cosmic scourge that emits tiny homing cosmic scourges on death and tile hits");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.damage = 1300;
            item.melee = true;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useTime = 20;
            item.knockBack = 5f;
            item.UseSound = SoundID.Item109;
            item.autoReuse = true;
            item.height = 20;
            item.value = Item.buyPrice(platinum: 2, gold: 50);
            item.rare = ItemRarityID.Red;
            item.shoot = ModContent.ProjectileType<ScourgeoftheCosmosProj>();
            item.shootSpeed = 15f;
            item.Calamity().customRarity = CalamityRarity.Violet;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.ScourgeoftheCorruptor);
            recipe.AddIngredient(ModContent.ItemType<Bonebreaker>());
            recipe.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 10);
            recipe.AddIngredient(ModContent.ItemType<DarksunFragment>(), 10);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
