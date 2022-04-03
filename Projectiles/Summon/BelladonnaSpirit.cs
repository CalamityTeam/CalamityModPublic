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
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Belladonna Spirit");
            Main.projFrames[Projectile.type] = 5;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 48;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.minionSlots = 1;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.timeLeft *= 5;
            Projectile.minion = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (Projectile.localAI[0] == 0f)
            {
                Initialize(player);
                Projectile.localAI[0] = 1f;
            }
            if (Projectile.frameCounter++ > 6f)
            {
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Projectile.type];
                Projectile.frameCounter = 0;
            }
            if (player.MinionDamage() != Projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int trueDamage = (int)((float)Projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    Projectile.Calamity().spawnedPlayerMinionDamageValue *
                    player.MinionDamage());
                Projectile.damage = trueDamage;
            }
            bool isCorrectProjectile = Projectile.type == ModContent.ProjectileType<BelladonnaSpirit>();
            player.AddBuff(ModContent.BuffType<BelladonnaSpiritBuff>(), 3600);
            if (isCorrectProjectile)
            {
                if (player.dead)
                {
                    modPlayer.belladonaSpirit = false;
                }
                if (modPlayer.belladonaSpirit)
                {
                    Projectile.timeLeft = 2;
                }
            }

            if (Projectile.velocity.X > 0.25f)
                Projectile.spriteDirection = 1;
            else if (Projectile.velocity.X < -0.25f)
                Projectile.spriteDirection = -1;

            NPC potentialTarget = Projectile.Center.MinionHoming(1200f, player);
            if (potentialTarget is null)
            {
                Vector2 targetPosition = player.Bottom;
                FollowPlayer(player, targetPosition);
            }
            else
            {
                TargetNPC(potentialTarget);
            }
            Projectile.MinionAntiClump();
        }
        public void Initialize(Player player)
        {
            Projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
            Projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = Projectile.damage;
            for (int i = 0; i < 45; i++)
            {
                float angle = MathHelper.TwoPi / 45f * i;
                Vector2 velocity = angle.ToRotationVector2() * 4f;
                Dust dust = Dust.NewDustPerfect(Projectile.Center + velocity * 2.75f, 39, velocity);
                dust.noGravity = true;
            }
        }
        public void FollowPlayer(Player player, Vector2 targetPosition)
        {
            Projectile.velocity.X = (player.Center.X + player.direction * 75f - Projectile.Center.X) / 60f;
            if (Projectile.Distance(player.Center) > 2500f ||
                targetPosition.Y - Projectile.Top.Y > 360f)
            {
                Projectile.Center = player.Center;
                Projectile.netUpdate = true;
            }
            else if (targetPosition.Y - Projectile.Top.Y < -550f)
            {
                Projectile.velocity.Y += Math.Sign(targetPosition.Y - targetPosition.Y) * 0.08f;
            }
            else
            {
                Projectile.velocity.Y = (targetPosition.Y - Projectile.Center.Y) / 60f;
            }
        }
        public void TargetNPC(NPC target)
        {
            Vector2 targetPosition = target.Center;
            if (Math.Abs(targetPosition.X - Projectile.Center.X) < 180f)
            {
                Projectile.velocity.X *= 0.95f;
                PetalFireTimer++;
                if (Main.myPlayer == Projectile.owner)
                    FirePetals(target);
            }
            else
            {
                Projectile.velocity.X += (targetPosition.X - Projectile.Center.X + target.spriteDirection * 75f > 0).ToDirectionInt() * 0.5f;
                Projectile.velocity.X = MathHelper.Clamp(Projectile.velocity.X, -12f, 12f);
            }
            Projectile.velocity.Y = (targetPosition.Y - Projectile.Center.Y + target.spriteDirection * 75f) / 90f;
        }
        public void FirePetals(NPC target)
        {
            int petalID = ModContent.ProjectileType<BelladonnaPetal>();
            if (PetalFireTimer % 20f == 19f)
            {
                for (int i = -1; i <= 1; i++)
                {
                    float angle = Main.rand.NextFloat(-0.1f, 0.1f) + i * 0.05f;
                    Vector2 petalSpawnPosition = Projectile.Center - Vector2.UnitY * 6f;
                    Vector2 petalShootVelocity = Projectile.SafeDirectionTo(target.Center).RotatedBy(angle) * 7.5f;
                    Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), petalSpawnPosition, petalShootVelocity, petalID, Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
            }

            if (PetalFireTimer % 180f == 179f)
            {
                for (int i = 0; i < 5; i++)
                {
                    float angle = MathHelper.Lerp(MathHelper.ToRadians(-Main.rand.NextFloat(30f, 36f)), MathHelper.ToRadians(Main.rand.NextFloat(30f, 36f)), i / 4f);
                    Vector2 petalSpawnPosition = Projectile.Center - Vector2.UnitY * 6f;
                    Vector2 petalShootVelocity = -Vector2.UnitY.RotatedBy(angle) * 9f;
                    Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), petalSpawnPosition, petalShootVelocity, petalID, Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
            }
        }
    }
}
