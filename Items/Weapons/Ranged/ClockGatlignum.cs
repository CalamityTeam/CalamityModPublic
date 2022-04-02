using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class ClockGatlignum : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Clock Gatlignum");
            Tooltip.SetDefault("33% chance to not consume ammo\n" +
                "Converts musket balls into strings of 3 high velocity bullets");
        }

        public override void SetDefaults()
        {
            item.damage = 55;
            item.ranged = true;
            item.width = 66;
            item.height = 34;
            item.useTime = 3;
            item.reuseDelay = 12;
            item.useAnimation = 9;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 3.75f;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = ItemRarityID.Yellow;
            item.UseSound = SoundID.Item31;
            item.autoReuse = true;
            item.shoot = ProjectileID.PurificationPowder;
            item.shootSpeed = 20f;
            item.useAmmo = AmmoID.Bullet;
            item.Calamity().canFirePointBlankShots = true;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-5, 0);

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float SpeedX = speedX + Main.rand.Next(-15, 16) * 0.05f;
            float SpeedY = speedY + Main.rand.Next(-15, 16) * 0.05f;

            if (type == ProjectileID.Bullet)
                Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, ProjectileID.BulletHighVelocity, damage, knockBack, player.whoAmI);
            else
                Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI);

            return false;
        }

        public override bool ConsumeAmmo(Player player)
        {
            if (Main.rand.Next(0, 100) < 33)
                return false;
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Gatligator);
            recipe.AddIngredient(ItemID.VenusMagnum);
            recipe.AddIngredient(ItemID.ClockworkAssaultRifle);
            recipe.AddIngredient(ModContent.ItemType<BarofLife>(), 3);
            recipe.AddIngredient(ItemID.Ectoplasm, 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
