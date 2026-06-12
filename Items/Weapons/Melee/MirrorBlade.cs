using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Projectiles.BaseProjectiles;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Rarities;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class MirrorBlade : BaseSwordHoldoutItem, ILocalizedModType, IHoldShiftTooltipItem
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public bool HasFlavorTooltip => true;
        public override int ProjectileType => ModContent.ProjectileType<MirrorBladeProjectile>();

        internal const float SlashProjectileDamageMultiplier = 0.5f;
        internal const int SlashProjectileLimit = 4;
        internal const int SlashCreationRate = 18;

        public int reflectTimer = 0;

        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 5));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<Nightwither>()];
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            Item.width = 114;
            Item.height = 128;
            Item.damage = 600;
            Item.DamageType = TrueMeleeDamageClass.Instance;
            Item.useAnimation = 40;
            Item.useTime = 40;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 7f;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.shootSpeed = 9f;
            Item.shoot = ModContent.ProjectileType<MirrorBlast>();
            base.SetDefaults();
        }

        public override bool CanUseItem(Player player)
        {
            return base.CanUseItem(player);
        }

        public override void UpdateInventory(Player player)
        {
            if (reflectTimer > 0)
            {
                if (reflectTimer <= 50)
                {

                    float coneLength = 96f;
                    float maximumAngle = 1f;
                    float coneRotation = player.DirectionTo(Main.MouseWorld).ToRotation();
                    var shardCount = 0;
                    foreach (var proj in Main.projectile)
                    {
                        if (proj.active && proj.type == ModContent.ProjectileType<MirrorBlast>() && proj.owner == player.whoAmI && (proj.ModProjectile as MirrorBlast).isShard)
                        {
                            shardCount++;
                        }
                    }
                    foreach (var proj in Main.ActiveProjectiles)
                    {
                        if (proj.type == ModContent.ProjectileType<DoGLaserWalls>() && proj.ModProjectile<DoGLaserWalls>().time >= 29)
                        {
                            if (shardCount > 0)
                            {
                                SoundEngine.PlaySound(SeekingScorcher.LightShatterSound, player.Center);
                                SoundEngine.PlaySound(SoundID.DD2_WitherBeastDeath, player.Center);
                            }
                            if (shardCount >= 10)
                            {
                                player.SetImmuneTimeForAllTypes(player.longInvince ? 60 : 30);
                            }
                            proj.Calamity().multiplicativeDR += shardCount / 10f;
                            proj.Calamity().multiplicativeDRTimer = 60;
                            foreach (var proj2 in Main.projectile)
                            {
                                if (proj2.active && proj2.type == ModContent.ProjectileType<MirrorBlast>() && proj2.owner == player.whoAmI && (proj2.ModProjectile as MirrorBlast).isShard)
                                {
                                    //Don't amplify Evolution shards
                                    if (proj2.DamageType != DamageClass.Generic)
                                        proj2.damage = (int)(proj2.damage * (shardCount >= 10 ? 3f : 2f));
                                    (proj2.ModProjectile as MirrorBlast).shardShield = 0;
                                    (proj2.ModProjectile as MirrorBlast).shardNum = 11;
                                    proj2.netUpdate = true;
                                }
                            }
                            reflectTimer = 0;
                            return;
                        }
                        else
                            if (proj.hostile && proj.damage > 0 && proj.Hitbox.IntersectsConeSlowMoreAccurate(player.Center, coneLength, coneRotation, maximumAngle))
                            {
                                if (shardCount > 0)
                                {
                                    SoundEngine.PlaySound(SeekingScorcher.LightShatterSound, player.Center);
                                    SoundEngine.PlaySound(SoundID.DD2_WitherBeastDeath, player.Center);
                                }
                                if (shardCount >= 10)
                                {
                                    player.SetImmuneTimeForAllTypes(player.longInvince ? 60 : 30);
                                }
                                proj.Calamity().multiplicativeDR += shardCount / 10f;
                                proj.Calamity().multiplicativeDRTimer = 60;
                                foreach (var proj2 in Main.projectile)
                                {
                                    if (proj2.active && proj2.type == ModContent.ProjectileType<MirrorBlast>() && proj2.owner == player.whoAmI && (proj2.ModProjectile as MirrorBlast).isShard)
                                    {
                                        //Don't amplify Evolution shards
                                        if (proj2.DamageType != DamageClass.Generic)
                                            proj2.damage = (int)(proj2.damage * (shardCount >= 10 ? 3f : 2f));
                                        (proj2.ModProjectile as MirrorBlast).shardShield = 0;
                                        (proj2.ModProjectile as MirrorBlast).shardNum = 11;
                                        proj2.netUpdate = true;
                                    }
                                }
                                reflectTimer = 0;
                                return;
                            }
                    }
                }
                reflectTimer--;
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                bool alreadyReflecting = reflectTimer > 0;
                bool hasShard = false;
                reflectTimer = 60;
                foreach (var proj in Main.projectile)
                {
                    if (proj.active && proj.type == ModContent.ProjectileType<MirrorBlast>() && proj.owner == player.whoAmI && (proj.ModProjectile as MirrorBlast).isShard)
                    {
                        (proj.ModProjectile as MirrorBlast).shardShield = 60;
                        proj.netUpdate = true;
                        hasShard = true;
                    }
                }

                if (!alreadyReflecting && hasShard)
                    SoundEngine.PlaySound(SoundID.DD2_EtherianPortalSpawnEnemy, player.Center);
                return false;
            }
            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }


        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
    }
}
