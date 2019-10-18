using Microsoft.Xna.Framework;
using Terraria;
using CalamityMod.Projectiles;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Seadragon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Seadragon");
            Tooltip.SetDefault("50% chance to not consume ammo\n" +
                "Has a chance to fire homing rockets that explode into fire shards on death");
        }

        public override void SetDefaults()
        {
            item.damage = 125;
            item.ranged = true;
            item.width = 78;
            item.height = 26;
            item.useTime = 5;
            item.useAnimation = 5;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 2.5f;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = 10;
            item.UseSound = SoundID.Item11;
            item.autoReuse = true;
            item.shoot = 10;
            item.shootSpeed = 16f;
            item.useAmmo = 97;
            item.Calamity().postMoonLordRarity = 13;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, 0);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float SpeedX = speedX + (float)Main.rand.Next(-10, 11) * 0.05f;
            float SpeedY = speedY + (float)Main.rand.Next(-10, 11) * 0.05f;
            Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI, 0f, 0f);
            if (Main.rand.NextBool(10))
            {
                Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, ModContent.ProjectileType<SeaDragonRocket>(), damage * 2, knockBack, player.whoAmI, 0f, 0f);
            }
            return false;
        }

        public override bool ConsumeAmmo(Player player)
        {
            if (Main.rand.Next(0, 100) < 50)
                return false;
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "Megalodon");
            recipe.AddIngredient(null, "Phantoplasm", 9);
            recipe.AddIngredient(null, "ArmoredShell", 3);
            recipe.AddIngredient(null, "DepthCells", 15);
            recipe.AddIngredient(null, "Lumenite", 15);
            recipe.AddIngredient(null, "Tenebris", 5);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
