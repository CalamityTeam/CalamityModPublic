using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Pumpler : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pumpler");
            Tooltip.SetDefault("33% chance to not consume ammo\n" +
                "Right click to fire an explosive pumpkin");
        }

        public override void SetDefaults()
        {
            item.damage = 8;
            item.ranged = true;
            item.width = 62;
            item.height = 36;
            item.useTime = 10;
            item.useAnimation = 10;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 1.25f;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = 2;
            item.UseSound = SoundID.Item11;
            item.autoReuse = true;
            item.shoot = ProjectileID.PurificationPowder;
            item.shootSpeed = 11f;
            item.useAmmo = AmmoID.Bullet;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                item.shootSpeed = 4f;
            }
            else
            {
                item.shootSpeed = 11f;
            }
            return base.CanUseItem(player);
        }

		public override float UseTimeMultiplier	(Player player)
		{
			if (player.altFunctionUse == 2)
				return (5f/18f);
			return 1f;
		}

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse == 2)
            {
                float SpeedX = speedX + (float)Main.rand.Next(-10, 11) * 0.05f;
                float SpeedY = speedY + (float)Main.rand.Next(-10, 11) * 0.05f;
                Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, ProjectileID.JackOLantern, (int)((double)damage * 1.65), knockBack * 4f, player.whoAmI, 0f, 0f);
            }
            else
            {
                float SpeedX = speedX + (float)Main.rand.Next(-10, 11) * 0.05f;
                float SpeedY = speedY + (float)Main.rand.Next(-10, 11) * 0.05f;
                Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI, 0f, 0f);
            }
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
            recipe.AddIngredient(ItemID.Pumpkin, 30);
            recipe.AddIngredient(ItemID.PumpkinSeed, 5);
            recipe.AddIngredient(ItemID.IllegalGunParts);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
