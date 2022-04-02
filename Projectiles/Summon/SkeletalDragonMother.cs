using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class SkeletalDragonMother : ModProjectile
    {
        public const float DistanceToCheck = 1100f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mother");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 92;
            projectile.height = 78;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 6;
            projectile.minionSlots = 6f;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft *= 5;
            projectile.minion = true;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (projectile.localAI[0] == 0f)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                for (int i = 0; i < 2; i++)
                {
                    Projectile.NewProjectile(projectile.Center + Utils.RandomVector2(Main.rand, -24f, 24f), 
                        Vector2.One.RotatedByRandom(MathHelper.TwoPi) * 4f, ModContent.ProjectileType<SkeletalDragonChild>(), projectile.damage, projectile.knockBack, player.whoAmI, projectile.whoAmI);
                }
                projectile.localAI[0] = 1f;
            }
            if (player.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int trueDamage = (int)(projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    player.MinionDamage());
                projectile.damage = trueDamage;
            }
            bool isProperProjectile = projectile.type == ModContent.ProjectileType<SkeletalDragonMother>();
            player.AddBuff(ModContent.BuffType<BloodDragonsBuff>(), 3600);
            if (isProperProjectile)
            {
                if (player.dead)
                {
                    modPlayer.dragonFamily = false;
                }
                if (modPlayer.dragonFamily)
                {
                    projectile.timeLeft = 2;
                }
            }

            NPC target = projectile.Center.MinionHoming(DistanceToCheck, player);

            if (target != null)
            {
                projectile.extraUpdates = 1;
                projectile.ai[0]++;

                // Arc towards enemy every 60 frames for 30 frames.
                float modulo = projectile.ai[0] % 150f;
                if ((modulo < 30f) ||
                    (modulo >= 90 && modulo < 120f))
                {
                    if (projectile.velocity.Length() == 0f)
                        projectile.velocity = projectile.SafeDirectionTo(target.Center).RotatedByRandom(0.5f) * -8f;
                    float angleToTarget = projectile.AngleTo(target.Center);
                    float velocityAngle = projectile.velocity.ToRotation();
                    float resultantAngle = velocityAngle.AngleLerp(angleToTarget, 0.08f);
                    if (projectile.Distance(target.Center) > 70f)
                    {
                        projectile.velocity = new Vector2(projectile.velocity.Length(), 0f).RotatedBy(resultantAngle);
                    }
                    else
                    {
                        projectile.velocity = (projectile.velocity * 44f + projectile.SafeDirectionTo(player.Center) * 24f) / 45f;
                        projectile.ai[0] += 30 - projectile.ai[0] % 30f;
                    }
                    projectile.ai[1] = 1f;
                }
                else
                {
                    projectile.ai[1] = 0f;
                }
                if ((modulo >= 30f && modulo <= 60f) || (modulo >= 90f && modulo <= 120f))
                {
                    if (projectile.owner == player.whoAmI && projectile.spriteDirection == (projectile.SafeDirectionTo(target.Center).X > 0).ToDirectionInt())
                    {
                        Projectile.NewProjectile(projectile.Center, projectile.SafeDirectionTo(target.Center) * 11.5f, ModContent.ProjectileType<BloodBreath>(), projectile.damage, 0f, projectile.owner);
                    }
                }
            }
            else if (projectile.Distance(player.Center) > 175f)
            {
                projectile.extraUpdates = 0;
                projectile.ai[1] = 0f;
                projectile.velocity = (projectile.velocity * 24f + projectile.SafeDirectionTo(player.Center) * 16f) / 25f;
                if (projectile.Distance(player.Center) > 3250f)
                {
                    projectile.Center = player.Center;
                    for (int i = 0; i < Main.projectile.Length; i++)
                    {
                        if (Main.projectile[i].active && Main.projectile[i].owner == projectile.owner &&
                            Main.projectile[i].type == ModContent.ProjectileType<SkeletalDragonChild>())
                        {
                            Main.projectile[i].Center = player.Center;
                            Main.projectile[i].netUpdate = true;
                        }
                    }
                    projectile.netUpdate = true;
                }
            }
            projectile.direction = projectile.spriteDirection = (projectile.velocity.X > 0).ToDirectionInt();

            projectile.frameCounter++;
            if (projectile.frameCounter > 6)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= Main.projFrames[projectile.type])
            {
                projectile.frame = 0;
            }
        }
    }
}
