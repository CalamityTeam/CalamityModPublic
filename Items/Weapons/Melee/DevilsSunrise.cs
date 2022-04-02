using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class DevilsSunrise : ModItem
    {
        public static int BaseDamage = 480;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Devil's Sunrise");
            Tooltip.SetDefault("Balls? Smalls.");
        }

        public override void SetDefaults()
        {
            item.width = 66;
            item.height = 66;
            item.melee = true;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.channel = true;
            item.damage = BaseDamage;
            item.knockBack = 4f;
            item.useAnimation = 25;
            item.useTime = 5;
            item.autoReuse = false;
            item.useStyle = ItemUseStyleID.HoldingOut;

            item.value = CalamityGlobalItem.Rarity12BuyPrice;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
            item.Calamity().donorItem = true;

            item.Calamity().trueMelee = true;
            item.shoot = ModContent.ProjectileType<DevilsSunriseProj>();
            item.shootSpeed = 24f;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void GetWeaponCrit(Player player, ref int crit) => crit += 10;

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<DevilsSunriseProj>(), damage, knockBack, player.whoAmI, 0f, 0f);
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<DevilsSunriseCyclone>(), damage, knockBack, player.whoAmI, 0f, 0f);
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe r = new ModRecipe(mod);
            r.AddIngredient(ItemID.Arkhalis);
            r.AddIngredient(ModContent.ItemType<DemonicBoneAsh>(), 10);
            r.AddIngredient(ModContent.ItemType<BloodstoneCore>(), 25);
            r.AddTile(TileID.LunarCraftingStation);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}
