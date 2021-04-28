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
            DisplayName.SetDefault("Saros Possession");
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
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            player.AddBuff(ModContent.BuffType<RadiantResolutionBuff>(), 3600);
            bool isCorrectProjectile = projectile.type == ModContent.ProjectileType<RadiantResolutionAura>();
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
                projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                projectile.localAI[0] += 1f;
            }
            if (player.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int trueDamage = (int)(projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    player.MinionDamage());
                projectile.damage = trueDamage;
            }
            projectile.minionSlots = projectile.ai[0];

            projectile.Center = player.Center - Vector2.UnitY * 16f;

            float allocatedSlots = projectile.ai[0];
            projectile.rotation += MathHelper.ToRadians(3f) + MathHelper.ToRadians(allocatedSlots * 0.85f);

			float damageMult = (float)Math.Log(allocatedSlots, 3) + 1f;

			//Softcap the mult after 9 slots
			float newMult = damageMult;
			if (newMult > 3f)
			{
				newMult = ((damageMult - 3f) * 0.1f) + 3f;
			}

            int radiantOrbDamage = (int)(projectile.damage * newMult);
            int radiantOrbAppearRate = (int)(130 * Math.Pow(0.9, allocatedSlots));

            if (radiantOrbAppearRate < 7)
                radiantOrbAppearRate = 7;

            if (radiantOrbDamage > 10000)
                radiantOrbDamage = 10000;

            projectile.ai[1]++;
            NPC potentialTarget = projectile.Center.MinionHoming(DistanceToCheck, player);
            if (potentialTarget != null && Main.myPlayer == projectile.owner)
            {
                if (projectile.ai[1] % 35 == 34)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        float angle = MathHelper.Lerp(-MathHelper.ToRadians(20f), MathHelper.ToRadians(20f), i / 2f);
                        Vector2 fireVelocity = projectile.SafeDirectionTo(potentialTarget.Center).RotatedBy(angle) * 15f;
                        Projectile.NewProjectile(projectile.Center, fireVelocity, ModContent.ProjectileType<RadiantResolutionFire>(), radiantOrbDamage / 2, projectile.knockBack, projectile.owner);
                    }
                }
                if (projectile.ai[1] % radiantOrbAppearRate == radiantOrbAppearRate - 1)
                {
                    Vector2 spawnPosition = projectile.Center + Main.rand.NextVector2Unit() * Main.rand.NextFloat(100f, 360f);
                    Vector2 bootlegRadianceOrbVelocity = projectile.SafeDirectionTo(potentialTarget.Center) * 2f;
                    Projectile.NewProjectile(spawnPosition, bootlegRadianceOrbVelocity, ModContent.ProjectileType<RadiantResolutionOrb>(), radiantOrbDamage, projectile.knockBack * 4f, projectile.owner);
                    for (int i = 0; i < 3; i++)
                    {
                        float angle = MathHelper.Lerp(-MathHelper.ToRadians(30f), MathHelper.ToRadians(30f), i / 3f);
                        Vector2 fireVelocity = projectile.SafeDirectionTo(potentialTarget.Center).RotatedBy(angle) * 19f;
                        Projectile.NewProjectile(projectile.Center, fireVelocity, ModContent.ProjectileType<RadiantResolutionFire>(), radiantOrbDamage / 2, projectile.knockBack, projectile.owner);
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
