using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class Seraphim : RogueWeapon
    {
        public const int SplitDaggerCount = 6;
        public const int StealthStrikeLightCount = 9;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Seraphim");
            Tooltip.SetDefault("Throws an extraordinarily fast dagger which slows down exponentially and dissipates into light\n" +
                $"Once dissipation has ended, {SplitDaggerCount} fast, splitting blades that aim at and slice nearby targets are released, along\n" +
                "with a large laserbeam\n" +
                $"Stealth strikes release a volley of {StealthStrikeLightCount} lights which explode into smaller laser beams along with the dagger");
        }

        public override void SafeSetDefaults()
        {
            item.width = 82;
            item.height = 82;
            item.damage = 332;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 12;
            item.useTime = 12;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 6f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            item.rare = ItemRarityID.Purple;
            item.Calamity().customRarity = CalamityRarity.Violet;
            item.shoot = ModContent.ProjectileType<SeraphimProjectile>();
            item.shootSpeed = SeraphimProjectile.InitialSpeed;
            item.Calamity().rogue = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<ShatteredSun>());
            recipe.AddIngredient(ModContent.ItemType<AuricBar>(), 5);
            recipe.AddTile(ModContent.TileType<CosmicAnvil>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 velocity = new Vector2(speedX, speedY);
            int knife = Projectile.NewProjectile(position + velocity, velocity, type, damage, knockBack, player.whoAmI);
            if (Main.projectile.IndexInRange(knife))
                Main.projectile[knife].Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();

            // Have stealth strikes release bursts of light that explode.
            if (player.Calamity().StealthStrikeAvailable())
            {
                int stealthDamage = (int)(damage * 1.4);
                for (int i = 0; i < StealthStrikeLightCount; i++)
                {
                    float offsetAngle = MathHelper.Lerp(-0.97f, 0.97f, i / (float)(StealthStrikeLightCount - 1f));
                    Vector2 lightShootVelocity = velocity.SafeNormalize(Vector2.UnitY).RotatedBy(offsetAngle) * 23f;
                    Projectile.NewProjectile(position, lightShootVelocity, ModContent.ProjectileType<SeraphimAngelicLight2>(), stealthDamage, knife, player.whoAmI, 1f);
                }
            }
            return false;
        }
    }
}
