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
        public static Color mainColor = Color.Goldenrod;
        public static Color accentColor = Color.LightGreen;

        public override void SetDefaults()
        {
            Item.width = 94;
            Item.height = 54;
            Item.damage = 315;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 7;
            Item.useAnimation = Item.useTime = UseTime;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.knockBack = 4f;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<AetherfluxCannonHoldout>();
            Item.shootSpeed = 24f;

            Item.rare = ModContent.RarityType<BurnishedAuric>();
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        public override void HoldItem(Player player) => player.Calamity().mouseRotationListener = true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 spawnPosition = player.RotatedRelativePoint(player.MountedCenter, true);
            // 14NOV2024: Ozzatron: clamped mouse position unnecessary, only used for direction
            Projectile.NewProjectile(source, spawnPosition, player.Calamity().mouseWorld - spawnPosition, ModContent.ProjectileType<AetherfluxCannonHoldout>(), damage, knockback, player.whoAmI);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<NanoPurge>().
                AddIngredient<AuricBar>(5).
                AddIngredient<UelibloomBar>(12).
                AddIngredient<DivineGeode>(8).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
