using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class PearlGod : ModItem
    {
        private const int defaultSpread = 1;
        private int spread = defaultSpread;
        private bool finalShot = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pearl God");
            Tooltip.SetDefault("Your life is mine...\n" +
                "Fires shockblast rounds that emit massive explosions and steal enemy life as well as additional bullets\n" +
                "Every seventh shot fires a massive shockblast");
        }

        public override void SetDefaults()
        {
            Item.damage = 32;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 80;
            Item.height = 46;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3f;
            Item.value = CalamityGlobalItem.Rarity8BuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = SoundID.Item41;
            Item.autoReuse = true;
            Item.shootSpeed = 12f;
            Item.shoot = ModContent.ProjectileType<ShockblastRound>();
            Item.useAmmo = AmmoID.Bullet;
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (spread > 6)
            {
                spread = defaultSpread;
                finalShot = true;
            }

            Vector2 velocity = new Vector2(speedX, speedY);
            float rotation = MathHelper.ToRadians(spread);
            if (!finalShot)
            {
                int totalLoops = 1;
                switch (spread)
                {
                    case 1:
                    case 2:
                        break;
                    case 3:
                    case 4:
                        totalLoops = 2;
                        break;
                    case 5:
                    case 6:
                        totalLoops = 3;
                        break;
                }

                for (int i = 0; i < totalLoops; i++)
                {
                    int proj = Projectile.NewProjectile(position, velocity.RotatedBy(-rotation * (i + 1)), type, (int)(damage * 0.5), knockBack * 0.5f, player.whoAmI);
                    Main.projectile[proj].extraUpdates += spread;
                    int proj2 = Projectile.NewProjectile(position, velocity.RotatedBy(+rotation * (i + 1)), type, (int)(damage * 0.5), knockBack * 0.5f, player.whoAmI);
                    Main.projectile[proj2].extraUpdates += spread;
                }

                int proj3 = Projectile.NewProjectile(position, velocity, ModContent.ProjectileType<ShockblastRound>(), damage, knockBack, player.whoAmI, 0f, spread);
                Main.projectile[proj3].extraUpdates += spread;

                spread++;
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    int proj = Projectile.NewProjectile(position, velocity, ModContent.ProjectileType<ShockblastRound>(), damage * 2, knockBack * 2f, player.whoAmI, 0f, 10f);
                    Main.projectile[proj].extraUpdates += 9;
                }

                finalShot = false;
            }

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<CursedCapper>()).AddIngredient(ItemID.SpectreBar, 5).AddIngredient(ItemID.ShroomiteBar, 5).AddTile(TileID.MythrilAnvil).Register();
        }
    }
}
