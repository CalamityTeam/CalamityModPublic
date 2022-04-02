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
            item.damage = 2250;
            item.knockBack = 9.5f;
            item.ranged = true;
            item.useTime = 55;
            item.useAnimation = 55;
            item.shoot = ProjectileID.BulletHighVelocity;
            item.shootSpeed = 12f;
            item.useAmmo = AmmoID.Bullet;
            item.autoReuse = true;

            item.width = 94;
            item.height = 32;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/LargeWeaponFire");

            item.value = CalamityGlobalItem.Rarity15BuyPrice;
            item.Calamity().customRarity = CalamityRarity.Violet;
            item.Calamity().donorItem = true;
            item.Calamity().canFirePointBlankShots = true;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void GetWeaponCrit(Player player, ref int crit) => crit += 35;

        public override Vector2? HoldoutOffset() => new Vector2(-10, 0);

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<GoldenEagle>());
            recipe.AddIngredient(ModContent.ItemType<AMR>());
            recipe.AddIngredient(ModContent.ItemType<AuricBar>(), 5);
            recipe.AddTile(ModContent.TileType<CosmicAnvil>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position, new Vector2(speedX, speedY), ModContent.ProjectileType<PiercingBullet>(), damage, knockBack, player.whoAmI, 0f, 0f);
            return false;
        }
    }
}
