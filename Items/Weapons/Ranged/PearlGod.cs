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
            item.damage = 32;
            item.ranged = true;
            item.width = 80;
            item.height = 46;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 3f;
            item.value = CalamityGlobalItem.Rarity8BuyPrice;
            item.rare = ItemRarityID.Yellow;
            item.UseSound = SoundID.Item41;
            item.autoReuse = true;
            item.shootSpeed = 12f;
            item.shoot = ModContent.ProjectileType<ShockblastRound>();
            item.useAmmo = AmmoID.Bullet;
            item.Calamity().canFirePointBlankShots = true;
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
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<CursedCapper>());
            recipe.AddIngredient(ItemID.SpectreBar, 5);
            recipe.AddIngredient(ItemID.ShroomiteBar, 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
