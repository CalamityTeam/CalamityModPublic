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
            item.damage = 70;
            item.ranged = true;
            item.width = 46;
            item.height = 24;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 7.5f;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.rare = ItemRarityID.Lime;
            item.UseSound = SoundID.Item10;
            item.autoReuse = true;
            item.shootSpeed = 20f;
            item.shoot = ModContent.ProjectileType<TriploonProj>();
            item.Calamity().canFirePointBlankShots = true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, 0);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 source = player.RotatedRelativePoint(player.MountedCenter, true);
            float piOverTen = MathHelper.Pi * 0.1f;
            int projCount = 3;
            Vector2 velocity = new Vector2(speedX, speedY);
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
                Projectile.NewProjectile(source.X + offset.X, source.Y + offset.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, 0f, 0f);
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<Dualpoon>());
            recipe.AddIngredient(ItemID.Harpoon);
            recipe.AddIngredient(ModContent.ItemType<DepthCells>(), 15);
            recipe.AddIngredient(ModContent.ItemType<Lumenite>(), 5);
            recipe.AddIngredient(ModContent.ItemType<Tenebris>(), 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
