using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class FlakToxicannon : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";

        #region Other Stats
        public static float OwnerKnockbackStrength = 1.1f;

        public static float ProjectileGravityStrength = 0.17f;
        public static float ProjectileShootSpeed = 25f;

        public static float InitialShotDamageMultiplier = 1f;
        public static float InitialShotHitShrapnelDamageMultiplier = .2f;

        public static int ShrapnelAmount = 4;
        public static float ShrapnelAngleOffset = 0.11f;

        public static int ClusterShrapnelAmount = 7;
        public static float ClusterShrapnelAngleOffset = 0.53f;
        #endregion
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<Irradiated>()];
        }

        public override void SetDefaults()
        {
            Item.width = 88;
            Item.height = 28;
            Item.damage = 73; // Here you're modifying the shrapnel's damage.
            Item.DamageType = DamageClass.Ranged;
            Item.useAnimation = Item.useTime = 44;
            Item.knockBack = 0.25f;
            Item.shoot = ModContent.ProjectileType<FlakToxicannonHoldout>();
            Item.shootSpeed = 15f;

            Item.useAmmo = AmmoID.Rocket;
            Item.UseSound = new SoundStyle("CalamityMod/Sounds/Item/DudFire") with { Volume = .4f, Pitch = -.7f, PitchVariance = 0.1f };
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.channel = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;

            Item.value = CalamityGlobalItem.RarityPinkBuyPrice;
            Item.rare = ItemRarityID.Pink;
        }

        // Obviously we don't want multiple holdouts existing at the same time.
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] == 0;

        // Spawning the holdout won't consume ammo.
        public override bool CanConsumeAmmo(Item ammo, Player player) => player.ownedProjectileCounts[Item.shoot] != 0;

        // Makes the rotation of the mouse around the player sync in multiplayer.
        public override void HoldItem(Player player) => player.Calamity().mouseWorldListener = true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile holdout = Projectile.NewProjectileDirect(source, player.MountedCenter, Vector2.Zero, ModContent.ProjectileType<FlakToxicannonHoldout>(), 0, 0f, player.whoAmI);

            // 14NOV2024: Ozzatron: clamped mouse position unnecessary, only used for direction
            // We set the rotation to the direction to the mouse so the first frame doesn't appear bugged out.
            holdout.velocity = (player.Calamity().mouseWorld - player.MountedCenter).SafeNormalize(Vector2.Zero);

            return false;
        }
    }
}
