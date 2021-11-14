using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Typeless
{
    public class GoldenGun : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Golden Gun");
            Tooltip.SetDefault("Shoots a straight line of ichor");
        }

        public override void SetDefaults()
        {
            item.damage = 5;
            item.width = 78;
            item.height = 36;
            item.useTime = 15;
            item.useAnimation = 15;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 2f;
            item.value = Item.buyPrice(0, 4, 0, 0);
            item.rare = ItemRarityID.Orange;
            item.UseSound = SoundID.Item11;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<GoldenGunProj>();
            item.shootSpeed = 12f;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, 0f, 0f);
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Ichor, 15);
            recipe.AddIngredient(ItemID.HellstoneBar, 10);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
