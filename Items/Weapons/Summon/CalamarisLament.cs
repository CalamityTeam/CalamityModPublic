using System.Collections.Generic;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class CalamarisLament : ModItem, ILocalizedModType
    {
        public static readonly SoundStyle GFB = new("CalamityMod/Sounds/Item/Inkling", 5);
        public new string LocalizationCategory => "Items.Weapons.Summon";

        #region Minion Stats

        public static float EnemyDistanceDetection = 8000f; // It's this high so it can target DoG reliably.

        public static float ShootingExtraTargettingSpeed = 10f;
        public static float ShootingMinionDistance = 320f;
        public static int ShootingFireRate = 30; // In frames.
        public static float ShootingProjectileSpeed = 20f;

        public static float LatchingDistanceRequired = 400f;
        public static float LatchingExtraTargettingSpeed = 30f;
        public static float LatchingDamageMultiplier = 1.25f;
        public static int LatchingIFrames = 30; // If the value is changed, respawn all minions.

        #endregion

        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 110;
            Item.shoot = ModContent.ProjectileType<CalamarisLamentMinion>();
            Item.DamageType = DamageClass.Summon;

            Item.useTime = Item.useAnimation = 10;
            Item.mana = 10;
            Item.width = 88;
            Item.height = 108;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item85;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.rare = ModContent.RarityType<PureGreen>();
            Item.value = CalamityGlobalItem.Rarity13BuyPrice;

            // This does nothing, it's just here so it's able to act like a staff.
            Item.shootSpeed = 1f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, Main.MouseWorld, velocity, type, damage, knockback, player.whoAmI);
            return false;
        }

        public override void ModifyTooltips(List<TooltipLine> list) => list.FindAndReplace("[GFB]", this.GetLocalizedValue(Main.zenithWorld ? "TooltipGFB" : "TooltipNormal"));
    }
}
