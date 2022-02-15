using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Scorpion : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Scorpio");
            Tooltip.SetDefault("BOOM\n" +
                "Right click to fire a nuke\n" +
				"Rockets will destroy tiles with tile-destroying ammo");
        }

        public override void SetDefaults()
        {
            item.damage = 80;
            item.ranged = true;
            item.width = 58;
            item.height = 26;
            item.useTime = 13;
			item.reuseDelay = 30;
			item.useAnimation = 39;
			item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 6.5f;
            item.value = CalamityGlobalItem.Rarity10BuyPrice;
            item.rare = ItemRarityID.Red;
            item.UseSound = null;
            item.autoReuse = true;
            item.shootSpeed = 20f;
            item.shoot = ModContent.ProjectileType<MiniRocket>();
            item.useAmmo = AmmoID.Rocket;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-20, 10);
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            return base.CanUseItem(player);
        }

		public override float UseTimeMultiplier(Player player)
		{
			if (player.altFunctionUse == 2)
				return 13f/39f;
			return 1f;
		}

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse == 2)
            {
                Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<BigNuke>(), (int)(damage * 1.25), knockBack * 2f, player.whoAmI);
                return false;
            }
            else
            {
                Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<MiniRocket>(), damage, knockBack, player.whoAmI);
                return false;
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.SnowmanCannon);
            recipe.AddIngredient(ItemID.GrenadeLauncher);
            recipe.AddIngredient(ItemID.RocketLauncher);
            recipe.AddIngredient(ItemID.FragmentVortex, 20);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
