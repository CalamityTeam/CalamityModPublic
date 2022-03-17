using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class VoidVortex : ModItem
    {
        public const int OrbFireRate = 16;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Void Vortex");
            Tooltip.SetDefault("Conjures a swirling vortex of supercharged magnet spheres around the cursor");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 112;
            item.magic = true;
            item.mana = 60;
            item.width = 130;
            item.height = 130;
            item.useTime = 80;
            item.useAnimation = 80;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 0f;
            item.value = CalamityGlobalItem.Rarity15BuyPrice;
            item.Calamity().customRarity = CalamityRarity.Violet;
            item.rare = ItemRarityID.Purple;
            item.UseSound = SoundID.Item20;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<VoidVortexProj>();
            item.shootSpeed = 12f;
        }

        public override Vector2? HoldoutOrigin()
        {
            return new Vector2(30, 30);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int numOrbs = 12;
            Vector2 clickPos = Main.MouseWorld;
            float orbDistance = 48f;
            float orbSpeed = 5f;

            float spinCoinflip = Main.rand.NextBool() ? -1f : 1f;
            Vector2 dir = Main.rand.NextVector2Unit();
            for (int i = 0; i < numOrbs; i++)
            {
                Vector2 orbPos = clickPos + dir * orbDistance;
                Vector2 vel = dir.RotatedBy(spinCoinflip * MathHelper.PiOver2) * orbSpeed;

                // Choose random firing stagger values for each orb to create a desynchronized barrage of lasers
                float timingStagger = Main.rand.Next(OrbFireRate);
                Projectile.NewProjectile(orbPos, vel, type, damage, knockBack, player.whoAmI, timingStagger, spinCoinflip);
                dir = dir.RotatedBy(MathHelper.TwoPi / numOrbs);
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<Climax>());
            recipe.AddIngredient(ModContent.ItemType<AuricBar>(), 5);
            recipe.AddTile(ModContent.TileType<CosmicAnvil>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
