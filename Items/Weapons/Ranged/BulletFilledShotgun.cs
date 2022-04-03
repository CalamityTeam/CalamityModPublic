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
            Item.damage = 1;
            Item.knockBack = 0f;
            Item.useTime = Item.useAnimation = 45;
            Item.DamageType = DamageClass.Ranged;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.useAmmo = AmmoID.Bullet;
            Item.shootSpeed = 9f;
            Item.shoot = ModContent.ProjectileType<BouncingShotgunPellet>();

            Item.width = 64;
            Item.height = 24;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item38;
            Item.value = CalamityGlobalItem.Rarity3BuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.Calamity().donorItem = true;
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-7, 0);

        public override bool CanUseItem(Player player) => CalamityGlobalItem.HasEnoughAmmo(player, Item, 5);

        // Disable vanilla ammo consumption
        public override bool ConsumeAmmo(Player player) => false;

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            type = Item.shoot;
            int bulletAmt = Main.rand.Next(25,35);
            for (int i = 0; i < bulletAmt; i++)
            {
                float newSpeedX = speedX + Main.rand.NextFloat(-15f, 15f);
                float newSpeedY = speedY + Main.rand.NextFloat(-15f, 15f);
                Projectile.NewProjectile(position.X, position.Y, newSpeedX, newSpeedY, type, damage, knockBack, player.whoAmI);
            }

            // Consume 5 ammo per shot
            CalamityGlobalItem.ConsumeAdditionalAmmo(player, Item, 5);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.MusketBall, 100).AddIngredient(ItemID.IronBar, 7).AddIngredient(ModContent.ItemType<AerialiteBar>(), 3).AddTile(TileID.Anvils).Register();
        }
    }
}
