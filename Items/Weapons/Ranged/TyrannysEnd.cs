using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class TyrannysEnd : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tyranny's End");
            Tooltip.SetDefault("Pierce the heart of even the most heavily-armored foe\n" +
                "Fires a .70 caliber sniper round that bypasses enemy defense and DR\n" +
                "Rounds mark enemies for death and summon a swarm of additional bullets on crits");
        }

        public override void SetDefaults()
        {
            Item.damage = 2250;
            Item.knockBack = 9.5f;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 55;
            Item.useAnimation = 55;
            Item.shoot = ProjectileID.BulletHighVelocity;
            Item.shootSpeed = 12f;
            Item.useAmmo = AmmoID.Bullet;
            Item.autoReuse = true;

            Item.width = 94;
            Item.height = 32;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.UseSound = Mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/LargeWeaponFire");

            Item.value = CalamityGlobalItem.Rarity15BuyPrice;
            Item.Calamity().customRarity = CalamityRarity.Violet;
            Item.Calamity().donorItem = true;
            Item.Calamity().canFirePointBlankShots = true;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void GetWeaponCrit(Player player, ref int crit) => crit += 35;

        public override Vector2? HoldoutOffset() => new Vector2(-10, 0);

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<GoldenEagle>()).AddIngredient(ModContent.ItemType<AMR>()).AddIngredient(ModContent.ItemType<AuricBar>(), 5).AddTile(ModContent.TileType<CosmicAnvil>()).Register();
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position, new Vector2(speedX, speedY), ModContent.ProjectileType<PiercingBullet>(), damage, knockBack, player.whoAmI, 0f, 0f);
            return false;
        }
    }
}
