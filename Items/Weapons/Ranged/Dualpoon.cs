using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Dualpoon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dualpoon");
            Tooltip.SetDefault("Launches two harpoons");
        }

        public override void SetDefaults()
        {
            item.damage = 70;
            item.ranged = true;
            item.width = 56;
            item.height = 28;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 6.5f;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = ItemRarityID.Pink;
            item.UseSound = SoundID.Item10;
            item.autoReuse = true;
            item.shootSpeed = 20f;
            item.shoot = ModContent.ProjectileType<DualpoonProj>();
            item.Calamity().canFirePointBlankShots = true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 source = player.RotatedRelativePoint(player.MountedCenter, true);
            float piOverTen = MathHelper.Pi * 0.1f;
            int projCount = 2;
            Vector2 velocity = new Vector2(speedX, speedY);
            velocity.Normalize();
            velocity *= 40f;
            bool canHit = Collision.CanHit(source, 0, 0, source + velocity, 0, 0);
            for (int projIndex = 0; projIndex < projCount; projIndex++)
            {
                float num120 = projIndex - (projCount - 1f) / 2f;
                Vector2 offset = velocity.RotatedBy((double)(piOverTen * num120), default);
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
            recipe.AddIngredient(ItemID.Harpoon, 2);
            recipe.AddIngredient(ModContent.ItemType<SeaPrism>(), 15);
            recipe.AddIngredient(ItemID.SoulofMight, 10);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
