using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class BulletFilledShotgun : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bullet-Filled Shotgun");
            Tooltip.SetDefault("Fires a massive spread of bouncing bullets\n" +
                               "Consumes five bullets per-use\n" +
                               "Aim? What's that?");
        }
        public override void SetDefaults()
        {
            item.damage = 1;
            item.knockBack = 0f;
            item.useTime = item.useAnimation = 45;
            item.ranged = true;
            item.noMelee = true;
            item.autoReuse = true;
            item.useAmmo = AmmoID.Bullet;
            item.shootSpeed = 9f;
            item.shoot = ModContent.ProjectileType<BouncingShotgunPellet>();

            item.width = 64;
            item.height = 24;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.UseSound = SoundID.Item38;
            item.value = CalamityGlobalItem.Rarity3BuyPrice;
            item.rare = ItemRarityID.Orange;
            item.Calamity().donorItem = true;
            item.Calamity().canFirePointBlankShots = true;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-7, 0);

        public override bool CanUseItem(Player player) => CalamityGlobalItem.HasEnoughAmmo(player, item, 5);

        // Disable vanilla ammo consumption
        public override bool ConsumeAmmo(Player player) => false;

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            type = item.shoot;
            int bulletAmt = Main.rand.Next(25,35);
            for (int i = 0; i < bulletAmt; i++)
            {
                float newSpeedX = speedX + Main.rand.NextFloat(-15f, 15f);
                float newSpeedY = speedY + Main.rand.NextFloat(-15f, 15f);
                Projectile.NewProjectile(position.X, position.Y, newSpeedX, newSpeedY, type, damage, knockBack, player.whoAmI);
            }

            // Consume 5 ammo per shot
            CalamityGlobalItem.ConsumeAdditionalAmmo(player, item, 5);
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.MusketBall, 100);
            recipe.AddIngredient(ItemID.IronBar, 7);
            recipe.anyIronBar = true;
            recipe.AddIngredient(ModContent.ItemType<AerialiteBar>(), 3);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
