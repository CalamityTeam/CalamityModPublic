using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class MagnaCannon : ModItem, ILocalizedModType
    {
        public static readonly SoundStyle ChargeFull = new("CalamityMod/Sounds/Item/MagnaCannonChargeFull") { Volume = 0.2f };
        public static readonly SoundStyle ChargeLoop = new("CalamityMod/Sounds/Item/MagnaCannonChargeLoop") { Volume = 0.2f };
        public static readonly SoundStyle ChargeStart = new("CalamityMod/Sounds/Item/MagnaCannonChargeStart") { Volume = 0.2f };
        public static readonly SoundStyle Fire = new("CalamityMod/Sounds/Item/MagnaCannonShot") { PitchVariance = 0.3f, Volume = 0.3f };

        public static int MaxLoadedShots = 20;
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetDefaults()
        {
            Item.damage = 15;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 56;
            Item.height = 34;
            Item.useTime = Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.channel = true;
            Item.knockBack = 3.5f;
            Item.value = CalamityGlobalItem.Rarity2BuyPrice;
            Item.rare = ItemRarityID.Green;
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.shootSpeed = 10f;
            Item.shoot = ModContent.ProjectileType<MagnaShot>();
        }

        public override Vector2? HoldoutOffset() => new Vector2(-15, 0);
        public override void HoldItem(Player player)
        {
            Item.noUseGraphic = true;
        }
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[ModContent.ProjectileType<MagnaCannonHoldout>()] <= 0;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 shootVelocity = velocity;
            Vector2 shootDirection = shootVelocity.SafeNormalize(Vector2.UnitX * player.direction);
            Projectile.NewProjectile(source, position, shootDirection, ModContent.ProjectileType<MagnaCannonHoldout>(), damage, knockback, player.whoAmI);
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Granite, 25).
                AddRecipeGroup("AnyCopperBar", 12).
                AddIngredient(ItemID.Diamond, 3).
                AddIngredient(ItemID.Sapphire, 8).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
