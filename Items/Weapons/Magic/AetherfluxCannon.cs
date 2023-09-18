using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    [LegacyName("T1000")]
    public class AetherfluxCannon : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public const int UseTime = 36;

        public override void SetDefaults()
        {
            Item.damage = 285;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 7;
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

            Item.rare = ModContent.RarityType<Violet>();
            Item.value = CalamityGlobalItem.Rarity15BuyPrice;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 shootVelocity = velocity;
            Vector2 shootDirection = shootVelocity.SafeNormalize(Vector2.UnitX * player.direction);
            Projectile.NewProjectile(source, position, shootDirection, type, damage, knockback, player.whoAmI);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<NanoPurge>().
                AddIngredient<PurgeGuzzler>().
                AddIngredient<AuricBar>(5).
                AddIngredient<UelibloomBar>(12).
                AddIngredient<DivineGeode>(8).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
