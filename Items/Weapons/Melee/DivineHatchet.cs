using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class DivineHatchet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Seeking Scorcher");
            Tooltip.SetDefault("May your enemies burn in hell for the sins they have committed\n" +
            "Throws a holy boomerang that seeks out up to three enemies before returning to the player");
        }

        public override void SetDefaults()
        {
            item.width = 60;
            item.damage = 177;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.autoReuse = true;
            item.useAnimation = 17;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 17;
            item.knockBack = 8.5f;
            item.UseSound = SoundID.Item1;
            item.melee = true;
            item.height = 62;
            item.shoot = ModContent.ProjectileType<DivineHatchetBoomerang>();
            item.shootSpeed = 14f;
            item.value = CalamityGlobalItem.Rarity12BuyPrice;
            item.rare = ItemRarityID.Purple;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.PossessedHatchet);
            recipe.AddIngredient(ModContent.ItemType<DivineGeode>(), 5);
            recipe.AddIngredient(ModContent.ItemType<UeliaceBar>(), 9);
            recipe.AddIngredient(ModContent.ItemType<UnholyEssence>(), 8);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
