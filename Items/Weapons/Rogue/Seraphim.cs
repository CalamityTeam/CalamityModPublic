using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class Seraphim : RogueWeapon
    {
        public const int SplitDaggerCount = 6;
        public const int StealthStrikeLightCount = 7;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Seraphim");
            Tooltip.SetDefault("Throws an extraordinarily fast dagger which slows down exponentially and dissipates into light\n" +
                $"Once dissipation has ended, {SplitDaggerCount} fast, splitting blades that aim at and slice nearby targets are released, along\n" +
                "with a large laserbeam\n" +
                $"Stealth strikes release a volley of {StealthStrikeLightCount} lights which explode into smaller laser beams along with the dagger");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 82;
            Item.height = 82;
            Item.damage = 300;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 13;
            Item.useTime = 13;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.rare = ModContent.RarityType<Violet>();
            Item.shoot = ModContent.ProjectileType<SeraphimProjectile>();
            Item.shootSpeed = SeraphimProjectile.InitialSpeed;
            Item.DamageType = RogueDamageClass.Instance;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

            int knife = Projectile.NewProjectile(source, position + velocity, velocity, type, damage, knockback, player.whoAmI);
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
                    Projectile.NewProjectile(source, position, lightShootVelocity, ModContent.ProjectileType<SeraphimAngelicLight2>(), stealthDamage, knife, player.whoAmI, 1f);
                }
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ModContent.ItemType<ShatteredSun>()).
                AddIngredient(ModContent.ItemType<AuricBar>(), 5).
                AddTile(ModContent.TileType<CosmicAnvil>()).
                Register();
        }
    }
}
