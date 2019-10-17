using Microsoft.Xna.Framework;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class FlarewingBow : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flarewing Bow");
            Tooltip.SetDefault("Shoots a spread of arrows\n" +
                "Wooden arrows are converted to bouncing obsidian bats");
        }

        public override void SetDefaults()
        {
            item.damage = 25;
            item.ranged = true;
            item.width = 20;
            item.height = 62;
            item.useTime = 28;
            item.useAnimation = 28;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 1.5f;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
            item.UseSound = SoundID.Item5;
            item.autoReuse = true;
            item.shoot = 1;
            item.shootSpeed = 16f;
            item.useAmmo = 40;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
            float num117 = 0.314159274f;
            int num118 = 5;
            Vector2 vector7 = new Vector2(speedX, speedY);
            vector7.Normalize();
            vector7 *= 50f;
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
                    int num123 = Projectile.NewProjectile(vector2.X + value9.X, vector2.Y + value9.Y, speedX, speedY, ModContent.ProjectileType<FlareBat>(), (int)((double)damage * 1.5), knockBack, player.whoAmI, 0.0f, 0.0f);
                    Main.projectile[num123].noDropItem = true;
                }
                else
                {
                    int num123 = Projectile.NewProjectile(vector2.X + value9.X, vector2.Y + value9.Y, speedX, speedY, type, (int)((double)damage * 0.66), knockBack, player.whoAmI, 0.0f, 0.0f);
                    Main.projectile[num123].noDropItem = true;
                }
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.HellwingBow);
            recipe.AddIngredient(null, "EssenceofCinder", 5);
            recipe.AddIngredient(ItemID.LivingFireBlock, 50);
            recipe.AddIngredient(ItemID.Obsidian, 10);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
