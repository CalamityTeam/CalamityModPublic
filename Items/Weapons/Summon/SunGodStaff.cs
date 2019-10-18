using Terraria;
using CalamityMod.Projectiles;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Weapons.Summon
{
    public class SunGodStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sun God Staff");
            Tooltip.SetDefault("Summons a solar god spirit to protect you");
        }

        public override void SetDefaults()
        {
            item.damage = 60;
            item.mana = 10;
            item.width = 50;
            item.height = 50;
            item.useTime = 36;
            item.useAnimation = 36;
            item.useStyle = 1;
            item.noMelee = true;
            item.knockBack = 1.25f;
            item.value = Item.buyPrice(0, 48, 0, 0);
            item.rare = 6;
            item.UseSound = SoundID.Item44;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<SolarGod>();
            item.summon = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "SunSpiritStaff");
            recipe.AddIngredient(null, "EssenceofCinder", 5);
            recipe.AddIngredient(ItemID.SoulofMight, 3);
            recipe.AddIngredient(ItemID.SoulofSight, 3);
            recipe.AddIngredient(ItemID.SoulofFright, 3);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            position = Main.MouseWorld;
            speedX = 0;
            speedY = 0;
            for (int x = 0; x < 1000; x++)
            {
                Projectile projectile = Main.projectile[x];
                if (projectile.active && projectile.owner == player.whoAmI && projectile.type == ModContent.ProjectileType<SolarGod>())
                {
                    projectile.Kill();
                }
            }
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI);
            return false;
        }
    }
}
