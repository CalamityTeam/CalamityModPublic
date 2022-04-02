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
                "Wooden arrows are converted into typhoon arrows and sharks");
        }

        public override void SetDefaults()
        {
            item.damage = 121;
            item.ranged = true;
            item.width = 46;
            item.height = 78;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 2.5f;
            item.value = CalamityGlobalItem.Rarity13BuyPrice;
            item.Calamity().customRarity = CalamityRarity.PureGreen;
            item.UseSound = SoundID.Item5;
            item.autoReuse = true;
            item.shoot = ProjectileID.WoodenArrowFriendly;
            item.shootSpeed = 12f;
            item.useAmmo = AmmoID.Arrow;
            item.Calamity().canFirePointBlankShots = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 source = player.RotatedRelativePoint(player.MountedCenter);
            float piOver10 = MathHelper.Pi * 0.1f;
            int totalProjectiles = 5;
            Vector2 velocity = new Vector2(speedX, speedY);
            velocity.Normalize();
            velocity *= 40f;
            bool canHit = Collision.CanHit(source, 0, 0, source + velocity, 0, 0);
            for (int p = 0; p < totalProjectiles; p++)
            {
                float offsetAmt = p - (totalProjectiles - 1f) / 2f;
                Vector2 offset = velocity.RotatedBy(piOver10 * offsetAmt);
                if (!canHit)
                    offset -= velocity;

                if (type == ProjectileID.WoodenArrowFriendly)
                {
                    int newType = type;
                    switch (p)
                    {
                        case 0:
                        case 1:
                        case 3:
                        case 4:
                            newType = ModContent.ProjectileType<MiniSharkron>();
                            break;
                        case 2:
                            newType = ModContent.ProjectileType<TyphoonArrow>();
                            break;
                    }
                    int proj = Projectile.NewProjectile(source.X + offset.X, source.Y + offset.Y, speedX, speedY, newType, (int)(damage * 1.1), knockBack, player.whoAmI);
                    if (proj.WithinBounds(Main.maxProjectiles))
                    {
                        Main.projectile[proj].arrow = true;
                        Main.projectile[proj].extraUpdates += 1;
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
            recipe.AddIngredient(ItemID.Tsunami);
            recipe.AddIngredient(ModContent.ItemType<FlarewingBow>());
            recipe.AddIngredient(ModContent.ItemType<ReaperTooth>(), 6);
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
