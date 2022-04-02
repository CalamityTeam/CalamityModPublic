using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class BelladonnaSpirit : ModProjectile
    {
        public float PetalFireTimer
        {
            get => projectile.ai[0];
            set => projectile.ai[0] = value;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Belladonna Spirit");
            Main.projFrames[projectile.type] = 5;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 28;
            projectile.height = 48;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.minionSlots = 1;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.timeLeft *= 5;
            projectile.minion = true;
            projectile.tileCollide = false;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (projectile.localAI[0] == 0f)
            {
                Initialize(player);
                projectile.localAI[0] = 1f;
            }
            if (projectile.frameCounter++ > 6f)
            {
                projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
                projectile.frameCounter = 0;
            }
            if (player.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int trueDamage = (int)((float)projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    player.MinionDamage());
                projectile.damage = trueDamage;
            }
            bool isCorrectProjectile = projectile.type == ModContent.ProjectileType<BelladonnaSpirit>();
            player.AddBuff(ModContent.BuffType<BelladonnaSpiritBuff>(), 3600);
            if (isCorrectProjectile)
            {
                if (player.dead)
                {
                    modPlayer.belladonaSpirit = false;
                }
                if (modPlayer.belladonaSpirit)
                {
                    projectile.timeLeft = 2;
                }
            }

            if (projectile.velocity.X > 0.25f)
                projectile.spriteDirection = 1;
            else if (projectile.velocity.X < -0.25f)
                projectile.spriteDirection = -1;

            NPC potentialTarget = projectile.Center.MinionHoming(1200f, player);
            if (potentialTarget is null)
            {
                Vector2 targetPosition = player.Bottom;
                FollowPlayer(player, targetPosition);
            }
            else
            {
                TargetNPC(potentialTarget);
            }
            projectile.MinionAntiClump();
        }
        public void Initialize(Player player)
        {
            projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
            projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
            for (int i = 0; i < 45; i++)
            {
                float angle = MathHelper.TwoPi / 45f * i;
                Vector2 velocity = angle.ToRotationVector2() * 4f;
                Dust dust = Dust.NewDustPerfect(projectile.Center + velocity * 2.75f, 39, velocity);
                dust.noGravity = true;
            }
        }
        public void FollowPlayer(Player player, Vector2 targetPosition)
        {
            projectile.velocity.X = (player.Center.X + player.direction * 75f - projectile.Center.X) / 60f;
            if (projectile.Distance(player.Center) > 2500f ||
                targetPosition.Y - projectile.Top.Y > 360f)
            {
                projectile.Center = player.Center;
                projectile.netUpdate = true;
            }
            else if (targetPosition.Y - projectile.Top.Y < -550f)
            {
                projectile.velocity.Y += Math.Sign(targetPosition.Y - targetPosition.Y) * 0.08f;
            }
            else
            {
                projectile.velocity.Y = (targetPosition.Y - projectile.Center.Y) / 60f;
            }
        }
        public void TargetNPC(NPC target)
        {
            Vector2 targetPosition = target.Center;
            if (Math.Abs(targetPosition.X - projectile.Center.X) < 180f)
            {
                projectile.velocity.X *= 0.95f;
                PetalFireTimer++;
                if (Main.myPlayer == projectile.owner)
                    FirePetals(target);
            }
            else
            {
                projectile.velocity.X += (targetPosition.X - projectile.Center.X + target.spriteDirection * 75f > 0).ToDirectionInt() * 0.5f;
                projectile.velocity.X = MathHelper.Clamp(projectile.velocity.X, -12f, 12f);
            }
            projectile.velocity.Y = (targetPosition.Y - projectile.Center.Y + target.spriteDirection * 75f) / 90f;
        }
        public void FirePetals(NPC target)
        {
            if (PetalFireTimer % 20f == 19f)
            {
                for (int i = -1; i <= 1; i++)
                {
                    float angle = Main.rand.NextFloat(-0.1f, 0.1f) + i * 0.05f;
                    Projectile.NewProjectile(projectile.Center + new Vector2(0f, -6f),
                                             projectile.SafeDirectionTo(target.Center).RotatedBy(angle) * 7.5f,
                                             ModContent.ProjectileType<BelladonnaPetal>(),
                                             projectile.damage,
                                             projectile.knockBack,
                                             projectile.owner);
                }
            }

            if (PetalFireTimer % 180f == 179f)
            {
                for (int i = 0; i < 5; i++)
                {
                    float angle = MathHelper.Lerp(MathHelper.ToRadians(-Main.rand.NextFloat(30f, 36f)), MathHelper.ToRadians(Main.rand.NextFloat(30f, 36f)), i / 4f);
                    Projectile.NewProjectile(projectile.Center + Vector2.UnitY * -6f,
                                             Vector2.UnitY.RotatedBy(angle) * -9f,
                                             ModContent.ProjectileType<BelladonnaPetal>(),
                                             projectile.damage,
                                             projectile.knockBack,
                                             projectile.owner);
                }
            }
        }
    }
}
