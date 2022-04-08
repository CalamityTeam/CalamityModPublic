using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Triploon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Triploon");
            Tooltip.SetDefault("Launches three harpoons");
        }

        public override void SetDefaults()
        {
            Item.damage = 70;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 46;
            Item.height = 24;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 7.5f;
            Item.value = Item.buyPrice(0, 60, 0, 0);
            Item.rare = ItemRarityID.Lime;
            Item.UseSound = SoundID.Item10;
            Item.autoReuse = true;
            Item.shootSpeed = 20f;
            Item.shoot = ModContent.ProjectileType<TriploonProj>();
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, 0);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 center = player.RotatedRelativePoint(player.MountedCenter, true);
            float piOverTen = MathHelper.Pi * 0.1f;
            int projCount = 3;

            velocity.Normalize();
            velocity *= 30f;
            bool canHit = Collision.CanHit(source, 0, 0, source + velocity, 0, 0);
            for (int projIndex = 0; projIndex < projCount; projIndex++)
            {
                float num120 = projIndex - (projCount - 1f) / 2f;
                Vector2 offset = velocity.RotatedBy(piOverTen * num120);
                if (!canHit)
                {
                    offset -= velocity;
                }
                Projectile.NewProjectile(source, center.X + offset.X, center.Y + offset.Y, velocity.X, velocity.Y, type, damage, knockback, player.whoAmI, 0f, 0f);
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<Dualpoon>()).AddIngredient(ItemID.Harpoon).AddIngredient(ModContent.ItemType<DepthCells>(), 15).AddIngredient(ModContent.ItemType<Lumenite>(), 5).AddIngredient(ModContent.ItemType<Tenebris>(), 5).AddTile(TileID.MythrilAnvil).Register();
        }
    }
}
