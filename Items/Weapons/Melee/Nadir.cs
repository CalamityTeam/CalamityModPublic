using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee.Spears;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Nadir : ModItem
    {
        public static int BaseDamage = 280;
        public static float ShootSpeed = 12f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nadir");
            Tooltip.SetDefault("Fires void essences which flay nearby enemies with tentacles\n" + "Ignores immunity frames\n" +
                "'The abyss has stared back at you long enough. It now speaks, and it does not speak softly.'");
        }

        public override void SetDefaults()
        {
            item.width = 144;
            item.height = 144;
            item.noUseGraphic = true;
            item.melee = true;
            item.damage = BaseDamage;
            item.knockBack = 8f;
            item.useAnimation = 18;
            item.useTime = 18;
            item.autoReuse = true;
            item.noMelee = true;

            item.useStyle = ItemUseStyleID.HoldingOut;
            item.UseSound = SoundID.Item1;

            item.value = CalamityGlobalItem.Rarity14BuyPrice;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
            item.Calamity().donorItem = true;

            item.shoot = ModContent.ProjectileType<NadirSpear>();
            item.shootSpeed = ShootSpeed;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] <= 0;

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<SpatialLance>());
            recipe.AddIngredient(ModContent.ItemType<TwistingNether>(), 5);
            recipe.AddIngredient(ModContent.ItemType<AscendantSpiritEssence>(), 5);
            recipe.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 10);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
