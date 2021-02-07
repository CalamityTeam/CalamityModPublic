using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Alluvion : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Alluvion");
            Tooltip.SetDefault("Moderate chance to convert wooden arrows into sharks\n" +
                       "Low chance to convert wooden arrows into typhoon arrows\n" +
                       "Fires a torrent of six arrows at once");
        }

        public override void SetDefaults()
        {
            item.damage = 70;
            item.ranged = true;
            item.width = 60;
            item.height = 90;
            item.useTime = 15;
            item.useAnimation = 30;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 4f;
            item.value = Item.buyPrice(1, 80, 0, 0);
            item.rare = ItemRarityID.Red;
            item.UseSound = SoundID.Item5;
            item.autoReuse = true;
            item.shoot = ProjectileID.WoodenArrowFriendly;
            item.shootSpeed = 17f;
            item.useAmmo = AmmoID.Arrow;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 source = player.RotatedRelativePoint(player.MountedCenter, true);
            float num117 = MathHelper.Pi * 0.1f;
            int totalProjectiles = 6;
            Vector2 velocity = new Vector2(speedX, speedY);
            velocity.Normalize();
            velocity *= 35f;
            bool canHit = Collision.CanHit(source, 0, 0, source + velocity, 0, 0);
            for (int i = 0; i < totalProjectiles; i++)
            {
                float num120 = (float)i - ((float)totalProjectiles - 1f) / 2f;
                Vector2 offset = velocity.RotatedBy((double)(num117 * num120), default);
                if (!canHit)
                {
                    offset -= velocity;
                }
                if (type == ProjectileID.WoodenArrowFriendly)
                {
                    if (Main.rand.NextBool(12))
                    {
                        type = ModContent.ProjectileType<TorrentialArrow>();
                    }
                    if (Main.rand.NextBool(25))
                    {
                        type = ModContent.ProjectileType<MiniSharkron>();
                    }
                    if (Main.rand.NextBool(100))
                    {
                        type = ModContent.ProjectileType<TyphoonArrow>();
                    }
                    int proj = Projectile.NewProjectile(source.X + offset.X, source.Y + offset.Y, speedX, speedY, type, damage, knockBack, player.whoAmI);
					if (proj.WithinBounds(Main.maxProjectiles))
					{
						Main.projectile[proj].Calamity().forceRanged = true;
						Main.projectile[proj].noDropItem = true;
						Main.projectile[proj].arrow = true;
					}
                }
                else
                {
                    int proj = Projectile.NewProjectile(source.X + offset.X, source.Y + offset.Y, speedX, speedY, type, damage, knockBack, player.whoAmI);
                    Main.projectile[proj].noDropItem = true;
                }
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<Monsoon>());
            recipe.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 5);
            recipe.AddIngredient(ModContent.ItemType<AscendantSpiritEssence>());
            recipe.AddIngredient(ModContent.ItemType<DepthCells>(), 10);
            recipe.AddIngredient(ModContent.ItemType<Lumenite>(), 20);
            recipe.AddIngredient(ModContent.ItemType<Tenebris>(), 5);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
