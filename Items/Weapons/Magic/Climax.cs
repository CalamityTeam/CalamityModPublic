using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class Climax : ModItem
    {
        public const int OrbFireRate = 10;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Voltaic Climax");
            Tooltip.SetDefault("Conjures an octagon of supercharged magnet spheres around the cursor");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 100;
            item.magic = true;
            item.mana = 30;
            item.width = 78;
            item.height = 78;
            item.useTime = 33;
            item.useAnimation = 33;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 5f;
            item.value = CalamityGlobalItem.Rarity14BuyPrice;
            item.rare = ItemRarityID.Purple;
            item.UseSound = SoundID.Item20;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<ClimaxProj>();
            item.shootSpeed = 12f;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
        }

        public override Vector2? HoldoutOrigin()
        {
            return new Vector2(15, 15);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int numOrbs = 8;
            Vector2 clickPos = Main.MouseWorld;
            float orbSpeed = 14f;
            Vector2 vel = Main.rand.NextVector2CircularEdge(orbSpeed, orbSpeed);
            for (int i = 0; i < numOrbs; i++)
            {
                // Choose random firing stagger values for each orb to create a desynchronized barrage of lasers
                float timingStagger = Main.rand.Next(OrbFireRate);
                Projectile.NewProjectile(clickPos, vel, type, damage, knockBack, player.whoAmI, ai0: timingStagger);

                vel = vel.RotatedBy(MathHelper.TwoPi / numOrbs);
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<MagneticMeltdown>());
            recipe.AddIngredient(ModContent.ItemType<DarksunFragment>(), 8);
            recipe.AddTile(ModContent.TileType<CosmicAnvil>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
