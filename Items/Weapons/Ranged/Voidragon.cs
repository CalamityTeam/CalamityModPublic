using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    [LegacyName("Megafleet")]
    public class Voidragon : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        private int shotType = 1;

        public override void SetStaticDefaults()
        {
            // Long live Megafleet. You will be missed. Maybe one day we can revive you.
                   }

        public override void SetDefaults()
        {
            Item.damage = 240;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 96;
            Item.height = 38;
            Item.useTime = 5;
            Item.useAnimation = 5;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5f;
            Item.UseSound = SoundID.Item11;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 18f;
            Item.useAmmo = AmmoID.Bullet;

            Item.value = CalamityGlobalItem.Rarity16BuyPrice;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.Calamity().devItem = true;
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, 0);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float SpeedX = velocity.X + (float)Main.rand.Next(-5, 6) * 0.05f;
            float SpeedY = velocity.Y + (float)Main.rand.Next(-5, 6) * 0.05f;

            if (shotType > 2)
                shotType = 1;

            if (shotType == 1)
                Projectile.NewProjectile(source, position.X, position.Y, SpeedX, SpeedY, type, damage, knockback, player.whoAmI, 0.0f, 0.0f);
            else
                Projectile.NewProjectile(source, position.X, position.Y, SpeedX, SpeedY, ModContent.ProjectileType<Projectiles.Ranged.Voidragon>(), damage, knockback, player.whoAmI, 0f, 0f);

            shotType++;

            Projectile.NewProjectile(source, position.X, position.Y, SpeedX, SpeedY, ModContent.ProjectileType<VoidragonTentacle>(), damage, knockback, player.whoAmI, (Main.rand.Next(-160, 160) * 0.001f), (Main.rand.Next(-160, 160) * 0.001f));

            return false;
        }

        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            if (Main.rand.Next(0, 100) < 50 || shotType % 2 == 0)
                return false;
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Seadragon>().
                AddIngredient(ItemID.SoulofFright, 30).
                AddIngredient<ShadowspecBar>(5).
                AddTile<DraedonsForge>().
                Register();
        }
    }
}
