using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Alluvion : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetDefaults()
        {
            Item.damage = 165;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 62;
            Item.height = 90;
            Item.useTime = 15;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4f;
            Item.value = CalamityGlobalItem.RarityDarkBlueBuyPrice;
            Item.UseSound = SoundID.Item5;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.WoodenArrowFriendly;
            Item.shootSpeed = 12f;
            Item.useAmmo = AmmoID.Arrow;
            Item.rare = ModContent.RarityType<DarkBlue>();
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo spawnSource, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 source = player.RotatedRelativePoint(player.MountedCenter);
            float tenthPi = MathHelper.Pi * 0.1f;
            int totalProjectiles = 6;

            velocity.Normalize();
            velocity *= 35f;
            bool canHit = Collision.CanHit(source, 0, 0, source + velocity, 0, 0);
            for (int i = 0; i < totalProjectiles; i++)
            {
                float arrowOffset = i - (totalProjectiles - 1f) / 2f;
                Vector2 offset = velocity.RotatedBy(tenthPi * arrowOffset);
                if (!canHit)
                    offset -= velocity;

                if (CalamityUtils.CheckWoodenAmmo(type, player))
                {
                    int newType = type;
                    switch (i)
                    {
                        case 0:
                        case 5:
                            newType = ModContent.ProjectileType<TyphoonArrow>();
                            break;
                        case 1:
                        case 4:
                            newType = ModContent.ProjectileType<MiniSharkron>();
                            break;
                        case 2:
                        case 3:
                            newType = ModContent.ProjectileType<TorrentialArrow>();
                            break;
                    }
                    int proj = Projectile.NewProjectile(spawnSource, source.X + offset.X, source.Y + offset.Y, velocity.X, velocity.Y, newType, damage, knockback, player.whoAmI);
                    if (proj.WithinBounds(Main.maxProjectiles))
                    {
                        Main.projectile[proj].arrow = true;
                        Main.projectile[proj].extraUpdates += 1;
                    }
                }
                else
                {
                    int proj = Projectile.NewProjectile(spawnSource, source.X + offset.X, source.Y + offset.Y, velocity.X, velocity.Y, type, damage, knockback, player.whoAmI);
                    Main.projectile[proj].noDropItem = true;
                }
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Monsoon>().
                AddIngredient<Lumenyl>(20).
                AddIngredient<CosmiliteBar>(8).
                AddIngredient<EndothermicEnergy>(20).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
