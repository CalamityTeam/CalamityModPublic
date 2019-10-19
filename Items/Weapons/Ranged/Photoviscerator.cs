using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Tiles.Furniture.CraftingStations;
using CalamityMod.Items.Materials;
using Terraria.ID;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Photoviscerator : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Photoviscerator");
            Tooltip.SetDefault("90% chance to not consume gel\n" +
                "Fires a stream of exo flames that literally melts everything");
        }

        public override void SetDefaults()
        {
            item.damage = 250;
            item.ranged = true;
            item.width = 84;
            item.height = 30;
            item.useTime = 2;
            item.useAnimation = 10;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 2f;
            item.UseSound = SoundID.Item34;
            item.value = Item.buyPrice(2, 50, 0, 0);
            item.rare = 10;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<ExoFire>();
            item.shootSpeed = 6f;
            item.useAmmo = 23;
            item.Calamity().postMoonLordRarity = 15;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-15, 0);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            for (int i = 0; i < 2; i++)
            {
                float SpeedX = speedX + (float)Main.rand.Next(-5, 6) * 0.05f;
                float SpeedY = speedY + (float)Main.rand.Next(-5, 6) * 0.05f;
                Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI, 0f, 0f);
            }
            return false;
        }

        public override bool ConsumeAmmo(Player player)
        {
            if (Main.rand.Next(0, 100) < 90)
                return false;
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<ElementalEruption>());
            recipe.AddIngredient(ModContent.ItemType<CleansingBlaze>());
            recipe.AddIngredient(ModContent.ItemType<HalleysInferno>());
            recipe.AddIngredient(ModContent.ItemType<NightmareFuel>(), 5);
            recipe.AddIngredient(ModContent.ItemType<EndothermicEnergy>(), 5);
            recipe.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 5);
            recipe.AddIngredient(ModContent.ItemType<DarksunFragment>(), 5);
            recipe.AddIngredient(ModContent.ItemType<HellcasterFragment>(), 3);
            recipe.AddIngredient(ModContent.ItemType<Phantoplasm>(), 5);
            recipe.AddIngredient(ModContent.ItemType<AuricOre>(), 25);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
