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
            item.damage = 73;
            item.magic = true;
            item.mana = 6;
            item.width = 62;
            item.height = 34;
            item.useTime = item.useAnimation = UseTime;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.channel = true;
            item.knockBack = 3f;
            item.rare = ItemRarityID.Red;
            item.value = CalamityGlobalItem.Rarity10BuyPrice;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<NanoPurgeHoldout>();
            item.shootSpeed = 16f;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] <= 0;

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.LaserMachinegun);
            recipe.AddIngredient(ItemID.FragmentVortex, 20);
            recipe.AddIngredient(ItemID.Nanites, 100);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
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
