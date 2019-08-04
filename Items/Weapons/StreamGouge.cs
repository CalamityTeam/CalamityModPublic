using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons
{
    public class StreamGouge : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stream Gouge");
            Tooltip.SetDefault("Fires an essence flame beam\n" +
				"Ignores immunity frames");
        }

        public override void SetDefaults()
        {
            item.width = 86;
            item.damage = 350;
            item.melee = true;
            item.noMelee = true;
            item.useTurn = true;
            item.noUseGraphic = true;
            item.useAnimation = 19;
            item.useStyle = 5;
            item.useTime = 19;
            item.knockBack = 9.75f;
            item.UseSound = SoundID.Item20;
            item.autoReuse = true;
            item.height = 86;
            item.value = Item.buyPrice(1, 80, 0, 0);
            item.rare = 10;
            item.shoot = mod.ProjectileType("StreamGouge");
            item.shootSpeed = 15f;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 14;
		}

        public override bool CanUseItem(Player player)
        {
            for (int i = 0; i < 1000; ++i)
            {
                if (Main.projectile[i].active && Main.projectile[i].owner == Main.myPlayer && Main.projectile[i].type == item.shoot)
                {
                    return false;
                }
            }
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "CosmiliteBar", 14);
			recipe.AddIngredient(null, "NightmareFuel", 7);
			recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
			recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "CosmiliteBar", 14);
			recipe.AddIngredient(null, "EndothermicEnergy", 7);
			recipe.AddTile(TileID.LunarCraftingStation);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
    }
}
