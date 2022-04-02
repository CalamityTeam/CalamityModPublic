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
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Celestus");
            Tooltip.SetDefault("Throws a scythe that splits into multiple scythes on enemy hits\n" +
            "Stealth strikes reverse direction and home in on enemies after returning to the player");
        }

        public override void SafeSetDefaults()
        {
            item.damage = 280;
            item.knockBack = 6f;
            item.useAnimation = item.useTime = 22;
            item.Calamity().rogue = true;
            item.autoReuse = true;
            item.shootSpeed = 25f;
            item.shoot = ModContent.ProjectileType<CelestusBoomerang>();

            item.width = item.height = 20;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.UseSound = SoundID.Item1;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.value = CalamityGlobalItem.Rarity15BuyPrice;
            item.rare = ItemRarityID.Red;
            item.Calamity().customRarity = CalamityRarity.Violet;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable()) //setting the stealth strike
            {
                int stealth = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, (int)(damage * 0.4), knockBack, player.whoAmI);
                if (stealth.WithinBounds(Main.maxProjectiles))
                    Main.projectile[stealth].Calamity().stealthStrike = true;
            }
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<AccretionDisk>());
            recipe.AddIngredient(ModContent.ItemType<AlphaVirus>());
            recipe.AddIngredient(ModContent.ItemType<MoltenAmputator>());
            recipe.AddIngredient(ModContent.ItemType<FrostcrushValari>());
            recipe.AddIngredient(ModContent.ItemType<EnchantedAxe>());
            recipe.AddIngredient(ModContent.ItemType<MiracleMatter>());
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
