using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class RadiantResolutionAura : ModProjectile
    {
        public const float DistanceToCheck = 1600f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Radiant Resolution Aura");
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 66;
            projectile.height = 66;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minionSlots = 1f;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft *= 5;
            projectile.minion = true;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 2f, 2f, 2f);
            bool isCorrectProjectile = projectile.type == ModContent.ProjectileType<RadiantResolutionAura>();
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            player.AddBuff(ModContent.BuffType<RadiantResolutionBuff>(), 3600);
            if (isCorrectProjectile)
            {
                if (player.dead)
                {
                    modPlayer.radiantResolution = false;
                }
                if (modPlayer.radiantResolution)
                {
                    projectile.timeLeft = 2;
                }
            }

            if (projectile.localAI[0] == 0f)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = (player.allDamage + player.minionDamage - 1f);
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                projectile.localAI[0] += 1f;
            }
            if ((player.allDamage + player.minionDamage - 1f) != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int trueDamage = (int)(projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    (player.allDamage + player.minionDamage - 1f));
                projectile.damage = trueDamage;
            }
            projectile.minionSlots = projectile.ai[0];

            projectile.Center = player.Center - Vector2.UnitY * 16f;

            float allocatedSlots = projectile.ai[0];
            projectile.rotation += MathHelper.ToRadians(3f) + MathHelper.ToRadians(allocatedSlots * 0.85f);

            int radiantOrbDamage = (int)(139 * Math.Log(allocatedSlots + 1f, 1.414214)) + 655;
            int radiantOrbAppearRate = (int)(124 * Math.Pow(0.9, allocatedSlots));

            if (radiantOrbAppearRate < 7)
                radiantOrbAppearRate = 7;

            if (radiantOrbDamage > 10000)
                radiantOrbDamage = 10000;

            projectile.ai[1]++;
            NPC potentialTarget = projectile.Center.MinionHoming(DistanceToCheck, player);
            if (potentialTarget != null)
            {
                if (projectile.ai[1] % 35 == 34)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        float angle = MathHelper.Lerp(-MathHelper.ToRadians(30f), MathHelper.ToRadians(30f), i / 3f);
                        Projectile.NewProjectile(projectile.Center, projectile.DirectionTo(potentialTarget.Center).RotatedBy(angle) * 15f,
                            ModContent.ProjectileType<RadiantResolutionFire>(), radiantOrbDamage / 2, 1f, projectile.owner);
                    }
                }
                if (projectile.ai[1] % radiantOrbAppearRate == radiantOrbAppearRate - 1)
                {
                    Projectile.NewProjectile(projectile.Center + Utils.NextVector2Unit(Main.rand) * Main.rand.NextFloat(100f, 360f),
                        projectile.DirectionTo(potentialTarget.Center) * 2f, ModContent.ProjectileType<RadiantResolutionOrb>(), radiantOrbDamage, 4f, projectile.owner);
                    for (int i = 0; i < 4; i++)
                    {
                        float angle = MathHelper.Lerp(-MathHelper.ToRadians(40f), MathHelper.ToRadians(40f), i / 4f);
                        Projectile.NewProjectile(projectile.Center, projectile.DirectionTo(potentialTarget.Center).RotatedBy(angle) * 19f,
                            ModContent.ProjectileType<RadiantResolutionFire>(), radiantOrbDamage / 2, 1f, projectile.owner);
                    }
                }
            }
        }

        public override bool CanDamage() => false;
        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D currentTexture = ModContent.GetTexture(Texture);
            spriteBatch.Draw(currentTexture,
                projectile.Center - Main.screenPosition,
                null,
                lightColor,
                projectile.rotation + MathHelper.PiOver2,
                currentTexture.Size() / 2f,
                1f,
                SpriteEffects.None,
                0f);
        }
    }
}
