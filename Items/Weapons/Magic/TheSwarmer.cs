using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class TheSwarmer : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetDefaults()
        {
            Item.damage = 36;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 14;
            Item.width = 60;
            Item.height = 52;
            Item.useTime = 12;
            Item.useAnimation = 12;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.value = CalamityGlobalItem.Rarity10BuyPrice;
            Item.rare = ItemRarityID.Red;
            Item.UseSound = SoundID.Item11;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.Wasp;
            Item.shootSpeed = 12f;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-15, -5);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i <= 3; i++)
            {
                float SpeedX = velocity.X + (float)Main.rand.Next(-35, 36) * 0.05f;
                float SpeedY = velocity.Y + (float)Main.rand.Next(-35, 36) * 0.05f;
                int wasps = Projectile.NewProjectile(source, position.X, position.Y, SpeedX, SpeedY, type, damage, 0f, player.whoAmI);
                if (wasps.WithinBounds(Main.maxProjectiles))
                {
                    Main.projectile[wasps].penetrate = 1;
                    Main.projectile[wasps].DamageType = DamageClass.Magic;
                }
            }
            for (int i = 0; i <= 3; i++)
            {
                float SpeedX2 = velocity.X + (float)Main.rand.Next(-35, 36) * 0.05f;
                float SpeedY2 = velocity.Y + (float)Main.rand.Next(-35, 36) * 0.05f;
                int bees = Projectile.NewProjectile(source, position.X, position.Y, SpeedX2, SpeedY2, player.beeType(), player.beeDamage(Item.damage), player.beeKB(0f), player.whoAmI);
                if (bees.WithinBounds(Main.maxProjectiles))
                {
                    Main.projectile[bees].penetrate = 1;
                    Main.projectile[bees].DamageType = DamageClass.Magic;
                }
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.BeeGun).
                AddIngredient(ItemID.WaspGun).
                AddIngredient(ItemID.FragmentStardust, 6).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
