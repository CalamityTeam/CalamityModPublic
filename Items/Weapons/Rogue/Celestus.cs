using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class Celestus : RogueWeapon
    {
		private int counter = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Celestus");
            Tooltip.SetDefault("Throws a scythe that splits into multiple scythes on enemy hits\n" +
			"Stealth strikes throw a chain of three scythes");
        }

        public override void SafeSetDefaults()
        {
            item.width = 20;
            item.damage = 850;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.autoReuse = true;
            item.useAnimation = 21;
            item.useStyle = 1;
            item.useTime = 7;
            item.knockBack = 6f;
            item.UseSound = SoundID.Item1;
            item.height = 20;
            item.value = Item.buyPrice(2, 50, 0, 0);
            item.rare = 10;
            item.shoot = ModContent.ProjectileType<CelestusBoomerang>();
            item.shootSpeed = 25f;
            item.Calamity().rogue = true;
            item.Calamity().customRarity = CalamityRarity.Violet;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
			counter++;
			if (counter >= 3)
				counter = 0;

            if (player.Calamity().StealthStrikeAvailable()) //setting the stealth strike
            {
                int stealth = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage / 2, knockBack, player.whoAmI, 0f, 1f);
                Main.projectile[stealth].Calamity().stealthStrike = true;
                return false;
            }
			else if (counter == 1 || counter == 2)
			{
				return false;
			}
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<AccretionDisk>());
            recipe.AddIngredient(ModContent.ItemType<ShatteredSun>());
            recipe.AddIngredient(ModContent.ItemType<ExecutionersBlade>());
            recipe.AddIngredient(ModContent.ItemType<FrostcrushValari>());
            recipe.AddIngredient(ModContent.ItemType<PhantasmalRuin>());
			recipe.AddIngredient(ModContent.ItemType<AuricBar>(), 4);
			recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
