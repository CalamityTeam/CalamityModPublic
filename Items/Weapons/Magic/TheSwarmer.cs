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
            Item.damage = 40;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 7;
            Item.width = 60;
            Item.height = 52;
            Item.useTime = 8;
            Item.useAnimation = 8;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.value = CalamityGlobalItem.Rarity10BuyPrice;
            Item.rare = ItemRarityID.Red;
            Item.UseSound = SoundID.Item11;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.Wasp;
            Item.shootSpeed = 12f;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-15, -5);
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.BeeGun).AddIngredient(ItemID.WaspGun).AddIngredient(ItemID.FragmentVortex, 20).AddTile(TileID.LunarCraftingStation).Register();
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
                int bees = Projectile.NewProjectile(position.X, position.Y, SpeedX2, SpeedY2, player.beeType(), player.beeDamage(Item.damage), player.beeKB(0f), player.whoAmI);
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
