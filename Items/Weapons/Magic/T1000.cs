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
            Item.damage = 247;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 8;
            Item.width = 94;
            Item.height = 54;
            Item.useTime = Item.useAnimation = UseTime;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.knockBack = 4f;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<AetherfluxCannonHoldout>();
            Item.shootSpeed = 24f;

            Item.rare = ItemRarityID.Purple;
            Item.Calamity().customRarity = CalamityRarity.Violet;
            Item.value = CalamityGlobalItem.Rarity15BuyPrice;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<Purge>()).AddIngredient(ModContent.ItemType<PurgeGuzzler>()).AddIngredient(ModContent.ItemType<AuricBar>(), 5).AddIngredient(ModContent.ItemType<UeliaceBar>(), 12).AddIngredient(ModContent.ItemType<DivineGeode>(), 8).AddTile(ModContent.TileType<CosmicAnvil>()).Register();
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
