using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Seadragon : ModItem
    {
        private int shotType = 1;
        private bool rocket = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Seadragon");
            Tooltip.SetDefault("50% chance to not consume ammo\n" +
                "Fires streams of water every other shot\n" +
                "Fires a homing rocket every 18 shots, which explodes into fire shards on death");
        }

        public override void SetDefaults()
        {
            item.damage = 70;
            item.ranged = true;
            item.width = 90;
            item.height = 38;
            item.useTime = 5;
            item.useAnimation = 5;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 2.5f;
            item.value = CalamityGlobalItem.Rarity12BuyPrice;
            item.rare = ItemRarityID.Purple;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
            item.UseSound = SoundID.Item11;
            item.autoReuse = true;
            item.shoot = ProjectileID.PurificationPowder;
            item.shootSpeed = 16f;
            item.useAmmo = AmmoID.Bullet;
            item.Calamity().canFirePointBlankShots = true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, 0);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float SpeedX = speedX + (float)Main.rand.Next(-10, 11) * 0.05f;
            float SpeedY = speedY + (float)Main.rand.Next(-10, 11) * 0.05f;

            if (shotType > 17)
            {
                shotType = 1;
                rocket = true;
            }

            if (!rocket)
            {
                if (shotType % 2 == 1)
                    Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI, 0.0f, 0.0f);
                else
                    Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, ModContent.ProjectileType<ArcherfishShot>(), damage, knockBack, player.whoAmI, 0f, 0f);

                shotType++;
            }
            else
            {
                Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, ModContent.ProjectileType<SeaDragonRocket>(), (int)(damage * 5), knockBack, player.whoAmI, 0f, 0f);
                rocket = false;
            }

            return false;
        }

        public override bool ConsumeAmmo(Player player)
        {
            if (shotType % 2 == 0)
                return false;
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<Megalodon>());
            recipe.AddIngredient(ModContent.ItemType<Phantoplasm>(), 9);
            recipe.AddIngredient(ModContent.ItemType<ArmoredShell>(), 3);
            recipe.AddIngredient(ModContent.ItemType<SeaPrism>(), 10);
            recipe.AddIngredient(ModContent.ItemType<DepthCells>(), 15);
            recipe.AddIngredient(ModContent.ItemType<Lumenite>(), 15);
            recipe.AddIngredient(ModContent.ItemType<Tenebris>(), 5);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
