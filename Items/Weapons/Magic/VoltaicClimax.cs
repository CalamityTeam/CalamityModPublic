using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    [LegacyName("Climax")]
    public class VoltaicClimax : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public const int OrbFireRate = 10;

        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 123;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 30;
            Item.width = 78;
            Item.height = 78;
            Item.useTime = 33;
            Item.useAnimation = 33;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5f;
            Item.value = CalamityGlobalItem.Rarity14BuyPrice;
            Item.UseSound = SoundID.Item20;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<ClimaxProj>();
            Item.shootSpeed = 12f;
            Item.rare = ModContent.RarityType<DarkBlue>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int numOrbs = 8;
            Vector2 clickPos = Main.MouseWorld;
            float orbSpeed = 14f;
            Vector2 vel = Main.rand.NextVector2CircularEdge(orbSpeed, orbSpeed);
            for (int i = 0; i < numOrbs; i++)
            {
                // Choose random firing stagger values for each orb to create a desynchronized barrage of lasers
                float timingStagger = Main.rand.Next(OrbFireRate);
                Projectile.NewProjectile(source, clickPos, vel, type, damage, knockback, player.whoAmI, ai0: timingStagger);

                vel = vel.RotatedBy(MathHelper.TwoPi / numOrbs);
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<MagneticMeltdown>().
                AddIngredient<DarksunFragment>(8).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
