using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    [LegacyName("MagnaStriker")]
    public class ArcNovaDiffuser : ModItem, ILocalizedModType
    {
        public static readonly SoundStyle ChargeLV1 = new("CalamityMod/Sounds/Item/ArcNovaDiffuserChargeLV1") { Volume = 0.6f };
        public static readonly SoundStyle ChargeLV2 = new("CalamityMod/Sounds/Item/ArcNovaDiffuserChargeLV2") { Volume = 0.6f };
        public static readonly SoundStyle ChargeStart = new("CalamityMod/Sounds/Item/ArcNovaDiffuserChargeStart") { Volume = 0.6f };
        public static readonly SoundStyle ChargeLoop = new("CalamityMod/Sounds/Item/ArcNovaDiffuserChargeLoop") { Volume = 0.6f };
        internal static readonly int ChargeLoopSoundFrames = 151;
        public static readonly SoundStyle SmallShot = new("CalamityMod/Sounds/Item/ArcNovaDiffuserSmallShot") { PitchVariance = 0.3f, Volume = 0.5f };
        public static readonly SoundStyle BigShot = new("CalamityMod/Sounds/Item/ArcNovaDiffuserBigShot") { PitchVariance = 0.3f, Volume = 0.8f };

        public static int AftershotCooldownFrames = 9;
        public static int Charge1Frames = 156;
        public static int Charge2Frames = 308;

        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetDefaults()
        {
            Item.damage = 128;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 72;
            Item.height = 38;
            Item.useTime = Item.useAnimation = AftershotCooldownFrames;
            Item.noMelee = true;
            Item.channel = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4f;
            Item.value = CalamityGlobalItem.Rarity8BuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = null;
            Item.autoReuse = false;
            Item.shoot = ModContent.ProjectileType<NovaShot>();
            Item.shootSpeed = 12f;
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }
        public override void HoldItem(Player player)
        {
            Item.noUseGraphic = true;
        }
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[ModContent.ProjectileType<ArcNovaDiffuserHoldout>()] <= 0;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 shootVelocity = velocity;
            Vector2 shootDirection = shootVelocity.SafeNormalize(Vector2.UnitX * player.direction);
            Projectile.NewProjectile(source, position, shootDirection, ModContent.ProjectileType<ArcNovaDiffuserHoldout>(), damage, knockback, player.whoAmI);
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<OpalStriker>().
                AddIngredient<MagnaCannon>().
                AddIngredient<LifeAlloy>(3).
                AddIngredient(ItemID.MartianConduitPlating, 15).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
