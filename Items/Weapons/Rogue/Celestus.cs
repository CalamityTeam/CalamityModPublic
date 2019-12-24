using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class Celestus : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Celestus");
            Tooltip.SetDefault("Throws a scythe that splits into multiple scythes on enemy hits");
        }

        public override void SafeSetDefaults()
        {
            item.width = 20;
            item.damage = 850;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.autoReuse = true;
            item.useAnimation = 20;
            item.useStyle = 1;
            item.useTime = 20;
            item.knockBack = 6f;
            item.UseSound = SoundID.Item1;
            item.height = 20;
            item.value = Item.buyPrice(2, 50, 0, 0);
            item.rare = 10;
            item.shoot = ModContent.ProjectileType<CelestusBoomerang>();
            item.shootSpeed = 25f;
            item.Calamity().rogue = true;
            item.Calamity().postMoonLordRarity = 15;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<AccretionDisk>());
            recipe.AddIngredient(ModContent.ItemType<ShatteredSun>());
            recipe.AddIngredient(ModContent.ItemType<ExecutionersBlade>());
            recipe.AddIngredient(ModContent.ItemType<Pwnagehammer>());
            recipe.AddIngredient(ModContent.ItemType<PhantasmalRuin>());
			recipe.AddIngredient(ModContent.ItemType<AuricBar>(), 4);
			recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
