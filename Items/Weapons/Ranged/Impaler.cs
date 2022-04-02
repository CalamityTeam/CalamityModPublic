using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Impaler : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Impaler");
            Tooltip.SetDefault("Fires explosive and flaming stakes\n" +
                "Instantly kills vampires and vampire bats");
        }

        public override void SetDefaults()
        {
            item.damage = 120;
            item.ranged = true;
            item.width = 40;
            item.height = 26;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 7f;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = ItemRarityID.Yellow;
            item.UseSound = SoundID.Item5;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<FlamingStake>();
            item.shootSpeed = 10f;
            item.useAmmo = AmmoID.Stake;
            item.Calamity().canFirePointBlankShots = true;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void GetWeaponCrit(Player player, ref int crit) => crit += 14;

        public override Vector2? HoldoutOffset() => new Vector2(0, -10);

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float SpeedX = speedX + (float)Main.rand.Next(-2, 3) * 0.05f;
            float SpeedY = speedY + (float)Main.rand.Next(-2, 3) * 0.05f;
            if (Main.rand.NextBool(3))
            {
                Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, ModContent.ProjectileType<ExplodingStake>(), damage, knockBack, player.whoAmI);
            }
            else
            {
                Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, ModContent.ProjectileType<FlamingStake>(), damage, knockBack, player.whoAmI);
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.StakeLauncher);
            recipe.AddIngredient(ItemID.ExplosivePowder, 100);
            recipe.AddIngredient(ModContent.ItemType<CruptixBar>(), 5);
            recipe.AddIngredient(ItemID.LivingFireBlock, 75);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
