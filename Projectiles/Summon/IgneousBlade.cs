using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class IgneousBlade : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Igneous Blade");
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 52;
            projectile.height = 86;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minionSlots = 1f;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 7;
            projectile.tileCollide = false;
            projectile.timeLeft *= 5;
            projectile.minion = true;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.localAI[1]);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.localAI[1] = reader.ReadSingle();
        }
        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (projectile.localAI[0] == 0f)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = (player.allDamage + player.minionDamage - 1f);
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                projectile.localAI[0] = 1f;
			}
            if ((player.allDamage + player.minionDamage - 1f) != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int trueDamage = (int)(projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    (player.allDamage + player.minionDamage - 1f));
                projectile.damage = trueDamage;
            }
            bool isProperProjectile = projectile.type == ModContent.ProjectileType<IgneousBlade>();
            player.AddBuff(ModContent.BuffType<IgneousExaltationBuff>(), 3600);
            if (isProperProjectile)
            {
                if (player.dead)
                {
                    modPlayer.igneousExaltation = false;
                }
                if (modPlayer.igneousExaltation)
                {
                    projectile.timeLeft = 2;
                }
            }

            // Orbiting. 1 = Shooting
            if (projectile.localAI[1] == 0f)
            {
                const float outwardPosition = 180f;
                projectile.Center = player.Center + projectile.ai[0].ToRotationVector2() * outwardPosition;
                projectile.rotation = projectile.ai[0] + MathHelper.PiOver2 + MathHelper.PiOver4;
                projectile.ai[0] -= MathHelper.ToRadians(4f);
            }
            else
            {
                projectile.ai[0]--;
                if (projectile.ai[0] == 1)
                    projectile.Kill();
            }
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (projectile.localAI[1] == 1f)
            {
                projectile.ai[0] = 50;
                for (int i = 0; i < 3; i++)
                {
                    Vector2 spawnPosition = target.Center - new Vector2(0f, 550f).RotatedByRandom(MathHelper.ToRadians(8f));
                    Projectile.NewProjectile(spawnPosition, Vector2.Normalize(target.Center - spawnPosition) * 24f, ModContent.ProjectileType<IgneousBladeStrike>(),
                        (int)(projectile.damage * 0.666), projectile.knockBack, projectile.owner);
                }
                for (int i = 0; i < Main.rand.Next(28, 41); i++)
                {
                    Dust.NewDustPerfect(
                        projectile.Center + Utils.NextVector2Unit(Main.rand) * Main.rand.NextFloat(10f),
                        6,
                        Utils.NextVector2Unit(Main.rand) * Main.rand.NextFloat(1f, 4f));
                }
                projectile.netUpdate = true;
            }
        }
        public override void Kill(int timeLeft)
        {
            for (int j = 0; j < 40; j++)
            {
                Dust dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, 6);
                dust.velocity = Vector2.UnitY * Main.rand.NextFloat(3f, 5.5f) * Main.rand.NextBool(2).ToDirectionInt();
                dust.noGravity = true;
            }
        }
    }
}
