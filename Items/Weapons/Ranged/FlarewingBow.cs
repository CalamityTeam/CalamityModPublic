using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class FlarewingBow : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flarewing Bow");
            Tooltip.SetDefault("Shoots a spread of 5 arrows\n" +
                "Wooden arrows are converted into bouncing obsidian bats");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 17;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 40;
            Item.height = 72;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 1.5f;
            Item.value = CalamityGlobalItem.Rarity4BuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item5;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.WoodenArrowFriendly;
            Item.shootSpeed = 16f;
            Item.useAmmo = AmmoID.Arrow;
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
            float num117 = 0.314159274f;
            int num118 = 5;
            Vector2 vector7 = velocity;
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
                    int num123 = Projectile.NewProjectile(source, vector2.X + value9.X, vector2.Y + value9.Y, velocity.X, velocity.Y, ModContent.ProjectileType<FlareBat>(), damage, knockback, player.whoAmI);
                    Main.projectile[num123].noDropItem = true;
                }
                else
                {
                    int num123 = Projectile.NewProjectile(source, vector2.X + value9.X, vector2.Y + value9.Y, velocity.X, velocity.Y, type, damage, knockback, player.whoAmI);
                    Main.projectile[num123].noDropItem = true;
                }
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.HellwingBow).
                AddIngredient<EssenceofSunlight>(5).
                AddIngredient(ItemID.LivingFireBlock, 50).
                AddIngredient(ItemID.Obsidian, 10).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
