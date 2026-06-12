using System;
using CalamityMod.Items.Materials;
using CalamityMod.Particles;
using CalamityMod.Projectiles;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    [LegacyName("CursedCapper")]
    public class ThermoclineBlaster : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public bool swapType = false;
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [BuffID.OnFire3, BuffID.Frostburn2];
        }
        public override void SetDefaults()
        {
            Item.width = 60;
            Item.height = 40;
            Item.scale = 0.75f;
            Item.damage = 45;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 16;
            Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2.25f;
            Item.value = CalamityGlobalItem.RarityLightRedBuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item41;
            Item.autoReuse = true;
            Item.shootSpeed = 12f;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.useAmmo = AmmoID.Bullet;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float itemRotation = player.compositeFrontArm.rotation + MathHelper.PiOver2 * player.gravDir;

            Vector2 itemPosition = player.MountedCenter + itemRotation.ToRotationVector2() * 7f;

            // Ice bullet
            if (!swapType)
            {
                for (int k = 0; k < 8; k++)
                {
                    CritSpark spark = new CritSpark(itemPosition + velocity.RotatedBy(-0.6 * player.direction) + velocity * 0.5f, velocity.RotatedByRandom(0.25) * Main.rand.NextFloat(0.2f, 1.5f), Main.rand.NextBool() ? Color.DeepSkyBlue : Color.LightSkyBlue, Color.White, Main.rand.NextFloat(0.3f, 0.7f), Main.rand.Next(10, 15 + 1), Main.rand.NextFloat(-2f, 2f), 1.5f);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
                Projectile iceShot = Projectile.NewProjectileDirect(source, itemPosition + velocity.RotatedBy(-0.6 * player.direction) - velocity * 0.5f, velocity, type, damage, knockback, player.whoAmI);
                CalamityGlobalProjectile cgp = iceShot.Calamity();
                cgp.iceBullet = true;
            }
            // Fire bullet
            if (swapType)
            {
                for (int k = 0; k < 8; k++)
                {
                    CritSpark spark = new CritSpark(itemPosition + velocity.RotatedBy(-0.6 * player.direction) + velocity * 0.5f, velocity.RotatedByRandom(0.25) * Main.rand.NextFloat(0.2f, 1.5f), Main.rand.NextBool() ? Color.Orange : Color.OrangeRed, Color.Yellow, Main.rand.NextFloat(0.3f, 0.7f), Main.rand.Next(10, 15 + 1), Main.rand.NextFloat(-2f, 2f), 1.5f);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
                Projectile fireShot = Projectile.NewProjectileDirect(source, itemPosition + velocity.RotatedBy(-0.6 * player.direction) - velocity * 0.5f, velocity, type, damage, knockback, player.whoAmI);
                CalamityGlobalProjectile cgp = fireShot.Calamity();
                cgp.fireBullet = true;
            }
            for (int k = 0; k < 4; k++)
            {
                Vector2 spawnPosition = itemPosition + velocity.RotatedBy(-0.6 * player.direction) + velocity * 0.5f;
                Vector2 smokeVel = velocity.RotatedByRandom(0.25) * Main.rand.NextFloat(0.2f, 1f);
                Particle smoke = new HeavySmokeParticle(spawnPosition, smokeVel, Color.White, Main.rand.Next(40, 60 + 1), Main.rand.NextFloat(0.2f, 0.45f), 0.5f, Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextBool(), required: true);
                GeneralParticleHandler.SpawnParticle(smoke);

                Dust dust = Dust.NewDustPerfect(spawnPosition, DustID.SteampunkSteam, smokeVel.RotatedByRandom(0.15f), 80, default, Main.rand.NextFloat(0.25f, 1f));
                dust.noGravity = false;
                dust.color = Color.White;
            }

            swapType = !swapType;
            return false;
        }
        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            player.ChangeDir(Math.Sign((player.Calamity().mouseWorld - player.Center).X));
            float itemRotation = player.compositeFrontArm.rotation + MathHelper.PiOver2 * player.gravDir;

            Vector2 itemPosition = player.MountedCenter + itemRotation.ToRotationVector2() * 7f;
            Vector2 itemSize = new Vector2(60, 40);
            Vector2 itemOrigin = new Vector2(-24, 3);

            CalamityUtils.CleanHoldStyle(player, itemRotation, itemPosition, itemSize, itemOrigin);

            base.UseStyle(player, heldItemFrame);
        }

        public override void UseItemFrame(Player player)
        {
            player.ChangeDir(Math.Sign((player.Calamity().mouseWorld - player.Center).X));

            float animProgress = 0.5f - player.itemTime / (float)player.itemTimeMax;
            float rotation = (player.Center - player.Calamity().mouseWorld).ToRotation() * player.gravDir + MathHelper.PiOver2;
            if (animProgress < 0.4f)
                rotation += -0.05f * (float)Math.Pow((0.6f - animProgress) / 0.6f, 2) * player.direction;

            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotation);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.PhoenixBlaster).
                AddIngredient<EssenceofHavoc>(5).
                AddIngredient<EssenceofEleum>(5).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
