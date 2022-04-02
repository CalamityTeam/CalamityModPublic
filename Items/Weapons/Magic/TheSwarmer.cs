using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class TheSwarmer : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Swarmer");
            Tooltip.SetDefault("Fires a swarm of bees and wasps");
        }

        public override void SetDefaults()
        {
            item.damage = 40;
            item.magic = true;
            item.mana = 7;
            item.width = 60;
            item.height = 52;
            item.useTime = 8;
            item.useAnimation = 8;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.value = CalamityGlobalItem.Rarity10BuyPrice;
            item.rare = ItemRarityID.Red;
            item.UseSound = SoundID.Item11;
            item.autoReuse = true;
            item.shoot = ProjectileID.Wasp;
            item.shootSpeed = 12f;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-15, -5);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.BeeGun);
            recipe.AddIngredient(ItemID.WaspGun);
            recipe.AddIngredient(ItemID.FragmentVortex, 20);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            for (int i = 0; i <= 3; i++)
            {
                float SpeedX = speedX + (float)Main.rand.Next(-35, 36) * 0.05f;
                float SpeedY = speedY + (float)Main.rand.Next(-35, 36) * 0.05f;
                int wasps = Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, 0f, player.whoAmI);
                if (wasps.WithinBounds(Main.maxProjectiles))
                {
                    Main.projectile[wasps].penetrate = 1;
                    Main.projectile[wasps].Calamity().forceMagic = true;
                }
            }
            for (int i = 0; i <= 3; i++)
            {
                float SpeedX2 = speedX + (float)Main.rand.Next(-35, 36) * 0.05f;
                float SpeedY2 = speedY + (float)Main.rand.Next(-35, 36) * 0.05f;
                int bees = Projectile.NewProjectile(position.X, position.Y, SpeedX2, SpeedY2, player.beeType(), player.beeDamage(item.damage), player.beeKB(0f), player.whoAmI);
                if (bees.WithinBounds(Main.maxProjectiles))
                {
                    Main.projectile[bees].penetrate = 1;
                    Main.projectile[bees].Calamity().forceMagic = true;
                }
            }
            return false;
        }
    }
}
