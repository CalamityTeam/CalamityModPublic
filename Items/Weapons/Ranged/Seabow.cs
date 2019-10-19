using Terraria;
using Terraria.ModLoader;
using CalamityMod.Items.Materials;
using Terraria.ID;
using CalamityMod.Projectiles.Ranged;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Seabow : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Seabow");
            Tooltip.SetDefault("Fires slow-moving water blasts");
        }

        public override void SetDefaults()
        {
            item.damage = 14;
            item.ranged = true;
            item.width = 22;
            item.height = 46;
            item.useTime = 25;
            item.useAnimation = 25;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 2.5f;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = 2;
            item.UseSound = SoundID.Item5;
            item.autoReuse = true;
            item.shoot = 10;
            item.shootSpeed = 16f;
            item.useAmmo = 40;
        }

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float SpeedX = speedX + (float)Main.rand.Next(-30, 31) * 0.05f;
            float SpeedY = speedY + (float)Main.rand.Next(-30, 31) * 0.05f;
            Projectile.NewProjectile(position.X, position.Y, SpeedX * 0.5f, SpeedY * 0.5f, ModContent.ProjectileType<SandWater>(), (int)((double)damage * 0.5), 0f, player.whoAmI, 0f, 0f);
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<VictideBar>(), 2);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
