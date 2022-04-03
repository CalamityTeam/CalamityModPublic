using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class Purge : ModItem
    {
        public const int UseTime = 20;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nano Purge");
            Tooltip.SetDefault("Fires a barrage of nano lasers");
        }

        public override void SetDefaults()
        {
            Item.damage = 73;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 6;
            Item.width = 62;
            Item.height = 34;
            Item.useTime = Item.useAnimation = UseTime;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.knockBack = 3f;
            Item.rare = ItemRarityID.Red;
            Item.value = CalamityGlobalItem.Rarity10BuyPrice;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<NanoPurgeHoldout>();
            Item.shootSpeed = 16f;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.LaserMachinegun).AddIngredient(ItemID.FragmentVortex, 20).AddIngredient(ItemID.Nanites, 100).AddTile(TileID.LunarCraftingStation).Register();
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 shootVelocity = new Vector2(speedX, speedY);
            Vector2 shootDirection = shootVelocity.SafeNormalize(Vector2.UnitX * player.direction);
            Projectile.NewProjectile(position, shootDirection, type, damage, knockBack, player.whoAmI);
            return false;
        }
    }
}
