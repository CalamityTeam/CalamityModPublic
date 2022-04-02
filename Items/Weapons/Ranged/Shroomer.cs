using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Shroomer : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shroomer");
            Tooltip.SetDefault("Fires bullets and an extremely powerful homing mushroom");
        }

        public override void SetDefaults()
        {
            item.damage = 150;
            item.ranged = true;
            item.width = 90;
            item.height = 28;
            item.useTime = 27;
            item.useAnimation = 27;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 9.75f;
            item.value = CalamityGlobalItem.Rarity10BuyPrice;
            item.rare = ItemRarityID.Red;
            item.UseSound = SoundID.Item40;
            item.autoReuse = true;
            item.shoot = ProjectileID.Bullet;
            item.shootSpeed = 10f;
            item.useAmmo = AmmoID.Bullet;
            item.Calamity().canFirePointBlankShots = true;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void GetWeaponCrit(Player player, ref int crit) => crit += 35;

        public override Vector2? HoldoutOffset() => new Vector2(-25, 0);

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position, new Vector2(speedX, speedY), ModContent.ProjectileType<Shroom>(), (int)(damage * 0.5), knockBack, player.whoAmI);
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.SniperRifle);
            recipe.AddIngredient(ItemID.ShroomiteBar, 11);
            recipe.AddIngredient(ItemID.FragmentVortex, 15);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
