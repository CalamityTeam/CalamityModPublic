using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Monsoon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Monsoon");
            Tooltip.SetDefault("Fires a spread of 5 arrows\n" +
                "Wooden arrows have a chance to be converted to typhoon arrows or sharks");
        }

        public override void SetDefaults()
        {
            item.damage = 85;
            item.ranged = true;
            item.width = 46;
            item.height = 78;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 2.5f;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.Calamity().customRarity = (CalamityRarity)13;
            item.UseSound = SoundID.Item5;
            item.autoReuse = true;
            item.shoot = ProjectileID.WoodenArrowFriendly;
            item.shootSpeed = 15f;
            item.useAmmo = AmmoID.Arrow;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 source = player.RotatedRelativePoint(player.MountedCenter, true);
            float piOver10 = MathHelper.Pi * 0.1f;
            int totalProjectiles = 5;
            Vector2 velocity = new Vector2(speedX, speedY);
            velocity.Normalize();
            velocity *= 40f;
            bool canHit = Collision.CanHit(source, 0, 0, source + velocity, 0, 0);
            for (int p = 0; p < totalProjectiles; p++)
            {
                float offsetAmt = (float)p - ((float)totalProjectiles - 1f) / 2f;
                Vector2 offset = velocity.RotatedBy((double)(piOver10 * offsetAmt), default);
                if (!canHit)
                {
                    offset -= velocity;
                }
                if (type == ProjectileID.WoodenArrowFriendly)
                {
                    if (Main.rand.NextBool(5))
                    {
                        type = ModContent.ProjectileType<MiniSharkron>();
                    }
                    if (Main.rand.NextBool(15))
                    {
                        type = ModContent.ProjectileType<TyphoonArrow>();
                    }
                    int arrow = Projectile.NewProjectile(source.X + offset.X, source.Y + offset.Y, speedX, speedY, type, (int)(damage * 1.1f), knockBack, player.whoAmI);
					if (arrow.WithinBounds(Main.maxProjectiles))
					{
						Main.projectile[arrow].Calamity().forceRanged = true;
						Main.projectile[arrow].noDropItem = true;
						Main.projectile[arrow].arrow = true;
						Main.projectile[arrow].extraUpdates += 1;
					}
                }
                else
                {
                    int arrow = Projectile.NewProjectile(source.X + offset.X, source.Y + offset.Y, speedX, speedY, type, damage, knockBack, player.whoAmI);
                    Main.projectile[arrow].noDropItem = true;
                }
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<ReaperTooth>(), 6);
            recipe.AddIngredient(ItemID.Tsunami);
            recipe.AddIngredient(ModContent.ItemType<FlarewingBow>());
            recipe.AddIngredient(ItemID.SharkFin, 2);
            recipe.AddIngredient(ModContent.ItemType<DepthCells>(), 10);
            recipe.AddIngredient(ModContent.ItemType<Lumenite>(), 10);
            recipe.AddIngredient(ModContent.ItemType<Tenebris>(), 5);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
