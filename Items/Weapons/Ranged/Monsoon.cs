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
            item.damage = 142;
            item.ranged = true;
            item.width = 46;
            item.height = 78;
            item.useTime = 12;
            item.useAnimation = 12;
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
            Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
            float num117 = 0.314159274f;
            int num118 = 5;
            Vector2 vector7 = new Vector2(speedX, speedY);
            vector7.Normalize();
            vector7 *= 40f;
            bool flag11 = Collision.CanHit(vector2, 0, 0, vector2 + vector7, 0, 0);
            for (int num119 = 0; num119 < num118; num119++)
            {
                float num120 = (float)num119 - ((float)num118 - 1f) / 2f;
                Vector2 value9 = vector7.RotatedBy((double)(num117 * num120), default);
                if (!flag11)
                {
                    value9 -= vector7;
                }
                if (type == ProjectileID.WoodenArrowFriendly)
                {
                    if (Main.rand.NextBool(5))
                    {
                        type = ProjectileID.MiniSharkron;
                    }
                    if (Main.rand.NextBool(15))
                    {
                        type = ModContent.ProjectileType<TyphoonArrow>();
                    }
                    int num121 = Projectile.NewProjectile(vector2.X + value9.X, vector2.Y + value9.Y, speedX, speedY, type, (int)(damage * 1.1f), knockBack, player.whoAmI, 0f, 0f);
                    Main.projectile[num121].Calamity().forceRanged = true;
                    Main.projectile[num121].noDropItem = true;
                    Main.projectile[num121].arrow = true;
                    Main.projectile[num121].extraUpdates += 1;
                }
                else
                {
                    int num121 = Projectile.NewProjectile(vector2.X + value9.X, vector2.Y + value9.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, 0f, 0f);
                    Main.projectile[num121].noDropItem = true;
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
