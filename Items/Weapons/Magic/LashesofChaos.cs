using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    [LegacyName("CalamitasInferno")]
    public class LashesofChaos : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";

        public int FiringMode = 0; // 0 = hellfireballs, 1 = hellblasts
        public int ProjectilesFired = 0;
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<BrimstoneFlames>()];
        }
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 30;
            Item.damage = 130;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 28;
            Item.useAnimation = Item.useTime = 46;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 7.5f;
            Item.value = CalamityGlobalItem.RarityLimeBuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<BrimstoneHellfireballFriendly>();
            Item.shootSpeed = 14f;
        }

        // Shoots twice as fast (and uses half as much mana to compensate) during hellblasts
        public override float UseSpeedMultiplier(Player player) => (FiringMode == 1 && ProjectilesFired < 9) ? 2f : 1f;
        public override void ModifyManaCost(Player player, ref float reduce, ref float mult)
        {
            if (FiringMode == 1)
                mult = 0.5f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            ProjectilesFired++;
            type = FiringMode == 1 ? ModContent.ProjectileType<SeethingDischargeBrimstoneHellblast>() : Item.shoot;
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, Main.myPlayer);

            // DUST
            int circleDustAmt = FiringMode == 1 ? 8 : 5 + ProjectilesFired;
            for (int d = 0; d < circleDustAmt; d++)
            {
                Dust hellblastDust = Dust.NewDustPerfect(player.Center, (int)CalamityDusts.Brimstone, velocity.RotatedByRandom(MathHelper.ToRadians(30f)) * Main.rand.NextFloat(0.8f, 1.1f), 150, default, FiringMode == 1 ? 1.7f : 0.8f + (ProjectilesFired * 0.15f));
                hellblastDust.noGravity = true;
            }

            // Extra DUST and smoke while firing hellblasts
            if (FiringMode == 1)
            {
                for (int d2 = 0; d2 < 8; d2++)
                {
                    Vector2 dustVel = Main.rand.NextVector2CircularEdge(1f, 1f);
                    dustVel.SafeNormalize(Vector2.Zero);
                    dustVel *= Main.rand.NextFloat(4f, 7f);

                    int hellfireballDust = Dust.NewDust(player.position, player.width, player.height, (int)CalamityDusts.Brimstone, dustVel.X, dustVel.Y, 150, default, 1.6f);
                    Main.dust[hellfireballDust].noGravity = true;
                }
                for (int p = 0; p < 3; p++)
                {
                    HeavySmokeParticle hellblastSmoke = new(position + velocity, velocity.RotatedByRandom(MathHelper.ToRadians(15f)), Color.LightGray, 30, 0.6f, 0.5f, 0f, true);
                    GeneralParticleHandler.SpawnParticle(hellblastSmoke);
                }
            }

            if (FiringMode == 0 && ProjectilesFired >= 5)
            {
                FiringMode = 1;
                ProjectilesFired = 0;
            }
            if (FiringMode == 1 && ProjectilesFired >= 10)
            {
                FiringMode = 0;
                ProjectilesFired = 0;
            }
            return false;
        }

        // This weapon intentionally does not reset its variables when swapping to a different item
    }
}
