using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
	public class DazzlingStabber : ModProjectile
    {
        public float NPCTargetTimer
        {
            get => projectile.ai[0];
            set => projectile.ai[0] = value;
        }
        public float IdleOffsetAngle
        {
            get => projectile.ai[1];
            set => projectile.ai[1] = value;
        }
        public const float DistanceToCheck = 1500f;
        public const float TeleportSlice = 40f;
        public static readonly float TeleportSliceAngleMax = MathHelper.ToRadians(23f);
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dazzling Stabber");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 26;
            projectile.height = 58;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minionSlots = 1f;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft *= 5;
            projectile.minion = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 12;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, Color.White.ToVector3());
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (projectile.localAI[0] == 0f)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                projectile.localAI[0] = 1f;
            }
            if (player.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int trueDamage = (int)(projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    player.MinionDamage());
                projectile.damage = trueDamage;
            }
            bool isProperProjectile = projectile.type == ModContent.ProjectileType<DazzlingStabber>();
            player.AddBuff(ModContent.BuffType<DazzlingStabberBuff>(), 3600);
            if (isProperProjectile)
            {
                if (player.dead)
                {
                    modPlayer.providenceStabber = false;
                }
                if (modPlayer.providenceStabber)
                {
                    projectile.timeLeft = 2;
                }
            }

            NPC potentialTarget = projectile.Center.MinionHoming(DistanceToCheck, player);

            if (projectile.frameCounter++ > 6)
            {
                projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
                projectile.frameCounter = 0;
            }

            if (potentialTarget is null)
            {
                projectile.rotation = projectile.rotation.AngleTowards(IdleOffsetAngle, 0.05f);
                Vector2 destination = player.Center + new Vector2(0f, -120f).RotatedBy(IdleOffsetAngle);
                projectile.velocity = (destination - projectile.Center) / 15f;
            }
            else
            {
                NPCTargetTimer++;
                // Alternate between normal charge and slower charge/knife summon
                if (NPCTargetTimer % 160f < 100f)
                {
                    ChargeAttack(potentialTarget);
                }
                // Teleport onto the target and just the rotation for a slice.
                else if (NPCTargetTimer % 160f == 160f - TeleportSlice)
                {
                    TeleportOntoTarget(potentialTarget);
                }
                // Slice the target and don't move.
                else if (NPCTargetTimer % 160f > 160f - TeleportSlice)
                {
                    projectile.rotation -= TeleportSliceAngleMax * 2f / TeleportSlice;
                }
            }
            projectile.direction = projectile.spriteDirection = (projectile.velocity.X > 0).ToDirectionInt();
        }

        public void ChargeAttack(NPC target)
        {
            if (NPCTargetTimer % 160f < 30f)
            {
                projectile.velocity *= 0.99f;
                projectile.rotation = projectile.rotation.AngleLerp(projectile.AngleTo(target.Center) + MathHelper.PiOver2, 0.25f);
            }
            else if (NPCTargetTimer % 160f == 30f)
            {
                projectile.velocity = projectile.DirectionTo(target.Center) * 20f;
            }
            else if (NPCTargetTimer % 160f < 60f)
            {
                projectile.velocity *= 0.99f;
                projectile.rotation = projectile.rotation.AngleLerp(projectile.AngleTo(target.Center) + MathHelper.PiOver2, 0.25f);
            }
            else if (NPCTargetTimer % 160f == 60f)
            {
                projectile.velocity = projectile.DirectionTo(target.Center) * 16f;
                for (int i = 0; i < 3; i++)
                {
                    float angle = MathHelper.Lerp(-0.3f, 0.3f, i / 3f);
                    Projectile.NewProjectile(projectile.Center, projectile.velocity.RotatedBy(angle), ModContent.ProjectileType<DazzlingStabberKnife>(), (int)(projectile.damage * 0.25), 1f, projectile.owner);
                }
            }
            else projectile.velocity *= 0.99f;
        }

        public void TeleportOntoTarget(NPC target)
        {
            // Spawn a spiral of holy flame dust.
            float angleStart = Main.rand.NextFloat(MathHelper.TwoPi);
            for (int i = 0; i < 30; i++)
            {
                float angle = MathHelper.TwoPi / 30f * i + angleStart;
                Dust dust = Dust.NewDustPerfect(projectile.Center + angle.ToRotationVector2() * 10f, (int)CalamityDusts.ProfanedFire);
                dust.velocity = angle.ToRotationVector2() * 5f * MathHelper.Lerp(1f, 0f, i % 6f / 6f);
            }

            Vector2 teleportOffset = Utils.RandomVector2(Main.rand, -12f, 12f);

            projectile.Center = target.Center + teleportOffset;
            projectile.rotation = projectile.AngleTo(target.Center) + MathHelper.PiOver2 + TeleportSliceAngleMax;
            projectile.velocity = Vector2.Zero;
            projectile.netUpdate = true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 180);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 180);
        }
    }
}
