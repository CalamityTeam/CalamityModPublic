using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Tools
{
    public class CrystylCrusher : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crystyl Crusher");
            Tooltip.SetDefault("Gotta dig faster, gotta go deeper\n" +
				"5000% pickaxe power");
			Item.staff[item.type] = true;
		}

        public override void SetDefaults()
        {
            item.damage = 2000;
            item.melee = true;
			item.noMelee = true;
			item.channel = true;
			item.crit += 25;
            item.width = 70;
            item.height = 70;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = 5;
            item.knockBack = 9f;
			item.shootSpeed = 14f;
            item.value = Item.buyPrice(5, 0, 0, 0);
            item.rare = 10;
            item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/CrystylCharge");
			item.shoot = ModContent.ProjectileType<CrystylCrusherRay>();
            item.Calamity().customRarity = CalamityRarity.Developer;
        }

		public override Vector2? HoldoutOrigin()
		{
			return new Vector2(10, 10);
		}

		public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddRecipeGroup("LunarPickaxe");
            recipe.AddIngredient(ModContent.ItemType<BlossomPickaxe>());
            recipe.AddIngredient(ModContent.ItemType<ShadowspecBar>(), 5);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
