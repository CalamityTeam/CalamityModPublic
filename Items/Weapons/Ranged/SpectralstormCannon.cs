using CalamityMod.Projectiles.Rogue;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class SpectralstormCannon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spectralstorm Cannon");
            Tooltip.SetDefault("70% chance to not consume flares\n" +
                "Fires a storm of lost souls and flares");
        }

        public override void SetDefaults()
        {
            item.damage = 48;
            item.ranged = true;
            item.width = 66;
            item.height = 26;
            item.useTime = 4;
            item.useAnimation = 12;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 1.5f;
            item.value = CalamityGlobalItem.Rarity10BuyPrice;
            item.rare = ItemRarityID.Red;
            item.UseSound = SoundID.Item11;
            item.autoReuse = true;
            item.shoot = ProjectileID.Flare;
            item.shootSpeed = 9.5f;
            item.useAmmo = AmmoID.Flare;
            item.Calamity().canFirePointBlankShots = true;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-10, 0);

        public override bool ConsumeAmmo(Player player)
        {
            if (Main.rand.Next(0, 100) < 70)
                return false;
            return true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float SpeedX = speedX + (float)Main.rand.Next(-40, 41) * 0.05f;
            float SpeedY = speedY + (float)Main.rand.Next(-40, 41) * 0.05f;
            int flare = Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI);
            if (flare.WithinBounds(Main.maxProjectiles))
            {
                Main.projectile[flare].timeLeft = 200;
                Main.projectile[flare].Calamity().forceRanged = true;
            }

            float SpeedX2 = speedX + (float)Main.rand.Next(-20, 21) * 0.05f;
            float SpeedY2 = speedY + (float)Main.rand.Next(-20, 21) * 0.05f;
            int soul = Projectile.NewProjectile(position.X, position.Y, SpeedX2, SpeedY2, ModContent.ProjectileType<LostSoulFriendly>(), damage, knockBack, player.whoAmI, 2f, 0f);
            if (soul.WithinBounds(Main.maxProjectiles))
            {
                Main.projectile[soul].timeLeft = 600;
                Main.projectile[soul].Calamity().forceRanged = true;
                Main.projectile[soul].frame = Main.rand.Next(4);
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<FirestormCannon>());
            recipe.AddIngredient(ItemID.FragmentVortex, 20);
            recipe.AddIngredient(ItemID.Ectoplasm, 10);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
