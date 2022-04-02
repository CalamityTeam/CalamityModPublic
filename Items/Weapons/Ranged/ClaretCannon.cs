using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class ClaretCannon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Claret Cannon");
            Tooltip.SetDefault("Fires strings of 3 bullets\n" +
                "Converts musket balls into bloody tears that drain enemy health");
        }

        public override void SetDefaults()
        {
            item.damage = 140;
            item.ranged = true;
            item.width = 48;
            item.height = 30;
            item.useTime = 3;
            item.reuseDelay = 10;
            item.useAnimation = 9;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 5.5f;
            item.value = CalamityGlobalItem.Rarity12BuyPrice;
            item.rare = ItemRarityID.Purple;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
            item.UseSound = SoundID.Item40;
            item.autoReuse = true;
            item.shootSpeed = 24f;
            item.shoot = ModContent.ProjectileType<ClaretCannonProj>();
            item.useAmmo = AmmoID.Bullet;
            item.Calamity().canFirePointBlankShots = true;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-5, 0);

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<BloodstoneCore>(), 4);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (type == ProjectileID.Bullet)
                Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<ClaretCannonProj>(), damage, knockBack, player.whoAmI);
            else
                Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI);

            return false;
        }
    }
}
