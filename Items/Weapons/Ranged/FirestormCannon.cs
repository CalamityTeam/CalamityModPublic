using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class FirestormCannon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Firestorm Cannon");
            Tooltip.SetDefault("70% chance to not consume flares\n" +
                "Right click to fire a spread of flares");
        }

        public override void SetDefaults()
        {
            item.damage = 9;
            item.ranged = true;
            item.width = 56;
            item.height = 28;
            item.useTime = 9;
            item.useAnimation = 9;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 1.5f;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = ItemRarityID.Green;
            item.UseSound = SoundID.Item11;
            item.autoReuse = true;
            item.shoot = ProjectileID.Flare;
            item.shootSpeed = 5.5f;
            item.useAmmo = AmmoID.Flare;
            item.Calamity().canFirePointBlankShots = true;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-10, 0);

        public override bool ConsumeAmmo(Player player) => Main.rand.Next(100) >= 70;

        public override bool AltFunctionUse(Player player) => true;

        public override float UseTimeMultiplier    (Player player)
        {
            if (player.altFunctionUse == 2)
                return 0.3333f;
            return 1f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse == 2)
            {
                int num6 = Main.rand.Next(4, 6);
                for (int index = 0; index < num6; ++index)
                {
                    float SpeedX = speedX + (float)Main.rand.Next(-50, 51) * 0.05f;
                    float SpeedY = speedY + (float)Main.rand.Next(-50, 51) * 0.05f;
                    int flare = Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, (int)((double)damage * 1.4), knockBack, player.whoAmI);
                    if (flare.WithinBounds(Main.maxProjectiles))
                    {
                        Main.projectile[flare].penetrate = 1;
                        Main.projectile[flare].timeLeft = 600;
                        Main.projectile[flare].Calamity().forceRanged = true;
                    }
                }
                return false;
            }
            else
            {
                int num6 = Main.rand.Next(1, 3);
                for (int index = 0; index < num6; ++index)
                {
                    float SpeedX = speedX + (float)Main.rand.Next(-40, 41) * 0.05f;
                    float SpeedY = speedY + (float)Main.rand.Next(-40, 41) * 0.05f;
                    int flare = Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI);
                    if (flare.WithinBounds(Main.maxProjectiles))
                    {
                        Main.projectile[flare].Calamity().forceRanged = true;
                        Main.projectile[flare].timeLeft = 200;
                        Main.projectile[flare].penetrate = 3;
                    }
                }
                return false;
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.FlareGun);
            recipe.AddIngredient(ItemID.Boomstick);
            recipe.AddRecipeGroup("AnyGoldBar", 10);
            recipe.AddIngredient(ModContent.ItemType<VictoryShard>(), 10);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
