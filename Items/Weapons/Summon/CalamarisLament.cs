using System.Collections.Generic;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.Summon;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Rarities;
using CalamityMod.Systems.Collections;
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
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<HadopelagicPressure>()];
        }

        public override void SetDefaults()
        {
            Item.width = 88;
            Item.height = 108;
            Item.damage = 110;
            Item.DamageType = DamageClass.Summon;
            Item.mana = 10;
            Item.useAnimation = Item.useTime = 24;
            Item.knockBack = 0.25f;
            Item.buffType = ModContent.BuffType<CalamarisLamentBuff>();
            Item.shoot = ModContent.ProjectileType<CalamarisLamentMinion>();
            Item.shootSpeed = 1f; // This does nothing, it's just here so it's able to act like a staff.
            
            Item.UseSound = SoundID.Item85;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.noMelee = true;

            Item.value = CalamityGlobalItem.RarityPureGreenBuyPrice;
            Item.rare = ModContent.RarityType<PureGreen>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            var minion = Projectile.NewProjectileDirect(source, player.ClampedMouseWorld(), velocity, type, damage, knockback, player.whoAmI);
            minion.originalDamage = Item.damage;
            return false;
        }

        public override void ModifyTooltips(List<TooltipLine> list) => list.FindAndReplace("[GFB]", this.GetLocalizedValue(Main.zenithWorld ? "TooltipGFB" : "TooltipNormal"));
    }
}
