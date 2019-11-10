using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class BlazingStar : RogueWeapon
    {
        public static int BaseDamage = 92;
        public static float Speed = 13f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blazing Star");
            Tooltip.SetDefault("Stacks up to 3\n" +
                               "Stealth strikes releases all stars at once with infinite piercing");
        }

        public override void SafeSetDefaults()
        {
            item.damage = BaseDamage;
            item.crit = 4;
            item.Calamity().rogue = true;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.width = 1;
            item.height = 1;
            item.useTime = 15;
            item.useAnimation = 15;
            item.useStyle = 1;
            item.knockBack = 4f;
            item.value = Item.buyPrice(0, 4, 0, 0);
            item.rare = 4;
            item.UseSound = SoundID.Item1;
            item.maxStack = 3;

            item.shootSpeed = Speed;
            item.shoot = ModContent.ProjectileType<BlazingStarProj>();
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                if (item.stack != 1)
                {
                    for (int i = 0; i < item.stack; i++)
                    {
                        Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedBy(MathHelper.Lerp(-MathHelper.ToRadians(8f), MathHelper.ToRadians(8f), i / (float)(item.stack - 1)));
                        int projectileIndex2 = Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, i == 1 ? type : ModContent.ProjectileType<BlazingStarProj>(), damage, knockBack, player.whoAmI, 0f, 0f);
                        Main.projectile[projectileIndex2].Calamity().stealthStrike = true;
                        int projectileIndex = Projectile.NewProjectile(position, perturbedSpeed, type, damage, knockBack, player.whoAmI, 0f);
                        Main.projectile[projectileIndex].penetrate = -1;
                        Main.projectile[projectileIndex].Calamity().stealthStrike = true;

                    }
                    return false;
                }
                return true;
            }
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[item.shoot] < item.stack;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<Glaive>(), 1);
            recipe.AddIngredient(ItemID.HellstoneBar, 3);
            recipe.AddIngredient(ModContent.ItemType<EssenceofChaos>(), 4);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
