using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Eviscerator : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eviscerator");
            Tooltip.SetDefault("Fires a slow-moving blood clot");
        }

        public override void SetDefaults()
        {
            item.damage = 65;
            item.ranged = true;
            item.width = 58;
            item.height = 22;
            item.useTime = 60;
            item.useAnimation = 60;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 7.5f;
            item.value = Item.buyPrice(0, 4, 0, 0);
            item.rare = 3;
            item.UseSound = SoundID.Item40;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<BloodClotFriendly>();
            item.shootSpeed = 22f;
            item.useAmmo = AmmoID.Bullet;
        }

		// Terraria seems to really dislike high crit values in SetDefaults
		public override void GetWeaponCrit(Player player, ref int crit) => crit += 25;

        public override Vector2? HoldoutOffset() => new Vector2(-7, 0);

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<BloodClotFriendly>(), damage, knockBack, player.whoAmI, 0f, 0f);
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<BloodSample>(), 8);
            recipe.AddIngredient(ItemID.Vertebrae, 4);
            recipe.AddIngredient(ItemID.CrimtaneBar, 4);
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
