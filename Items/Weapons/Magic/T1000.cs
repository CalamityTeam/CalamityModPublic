using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class T1000 : ModItem
    {
        public const int UseTime = 36;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aetherflux Cannon");
            Tooltip.SetDefault("Fires a barrage of phased god rays that pass through terrain");
        }

        public override void SetDefaults()
        {
            item.damage = 247;
            item.magic = true;
            item.mana = 8;
            item.width = 94;
            item.height = 54;
            item.useTime = item.useAnimation = UseTime;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.channel = true;
            item.knockBack = 4f;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<AetherfluxCannonHoldout>();
            item.shootSpeed = 24f;

            item.rare = ItemRarityID.Purple;
            item.Calamity().customRarity = CalamityRarity.Violet;
            item.value = CalamityGlobalItem.Rarity15BuyPrice;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] <= 0;

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<Purge>());
            recipe.AddIngredient(ModContent.ItemType<PurgeGuzzler>());
            recipe.AddIngredient(ModContent.ItemType<AuricBar>(), 5);
            recipe.AddIngredient(ModContent.ItemType<UeliaceBar>(), 12);
            recipe.AddIngredient(ModContent.ItemType<DivineGeode>(), 8);
            recipe.AddTile(ModContent.TileType<CosmicAnvil>());
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
