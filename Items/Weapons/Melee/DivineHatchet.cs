using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using Terraria;
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
            item.damage = 241;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.autoReuse = true;
            item.useAnimation = 17;
            item.useStyle = 1;
            item.useTime = 17;
            item.knockBack = 8.5f;
            item.UseSound = SoundID.Item1;
            item.melee = true;
            item.height = 62;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = 8;
            item.shoot = ModContent.ProjectileType<DivineHatchetBoomerang>();
            item.shootSpeed = 14f;
            item.Calamity().postMoonLordRarity = 13;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.PossessedHatchet);
            recipe.AddIngredient(ModContent.ItemType<DivineGeode>(), 5);
            recipe.AddIngredient(ModContent.ItemType<UeliaceBar>(), 9);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
