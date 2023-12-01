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
    public class VoidVortex : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public const int OrbFireRate = 15;

        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 110;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 60;
            Item.width = 130;
            Item.height = 130;
            Item.useTime = 80;
            Item.useAnimation = 80;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 0f;
            Item.value = CalamityGlobalItem.Rarity15BuyPrice;
            Item.rare = ModContent.RarityType<Violet>();
            Item.UseSound = SoundID.Item20;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<VoidVortexProj>();
            Item.shootSpeed = 12f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
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
                Projectile.NewProjectile(source, orbPos, vel, type, damage, knockback, player.whoAmI, timingStagger, spinCoinflip);
                dir = dir.RotatedBy(MathHelper.TwoPi / numOrbs);
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<VoltaicClimax>().
                AddIngredient<AuricBar>(5).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
