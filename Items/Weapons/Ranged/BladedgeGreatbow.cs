using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class BladedgeGreatbow : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bladedge Railbow");
            Tooltip.SetDefault("Fires 4 arrows at once\n" +
                "Fires 2 additional leaves");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 28;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 74;
            Item.height = 24;
            Item.useTime = 26;
            Item.useAnimation = 26;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3.5f;
            Item.value = Item.buyPrice(0, 60, 0, 0);
            Item.rare = ItemRarityID.Lime;
            Item.UseSound = SoundID.Item5;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 14f;
            Item.useAmmo = AmmoID.Arrow;
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, 0);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 4; i++)
            {
                float SpeedX = velocity.X + (float)Main.rand.Next(-60, 61) * 0.05f;
                float SpeedY = velocity.Y + (float)Main.rand.Next(-60, 61) * 0.05f;
                int index = Projectile.NewProjectile(source, position.X, position.Y, SpeedX, SpeedY, type, damage, knockback, player.whoAmI);
                Main.projectile[index].noDropItem = true;
            }
            Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
            float num117 = 0.314159274f;
            int num118 = 2;
            Vector2 vector7 = velocity;
            vector7.Normalize();
            vector7 *= 10f;
            bool flag11 = Collision.CanHit(vector2, 0, 0, vector2 + vector7, 0, 0);
            for (int num119 = 0; num119 < num118; num119++)
            {
                float num120 = (float)num119 - ((float)num118 - 1f) / 2f;
                Vector2 value9 = vector7.RotatedBy((double)(num117 * num120), default);
                if (!flag11)
                {
                    value9 -= vector7;
                }
                int projectile = Projectile.NewProjectile(source, vector2.X + value9.X, vector2.Y + value9.Y, velocity.X, velocity.Y, ProjectileID.Leaf, damage / 2, 0f, player.whoAmI);
                if (projectile.WithinBounds(Main.maxProjectiles))
                    Main.projectile[projectile].Calamity().forceRanged = true;
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<PerennialBar>(12).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
