using CalamityMod.Buffs.Summon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class PoleWarperSummon : ModProjectile
    {
        public float AngularOffset = 0f;
        public const float MaximumRepulsionSpeed = 13f;
        public float Time
        {
            get => projectile.ai[0];
            set => projectile.ai[0] = value;
        }
        public bool North
        {
            get => projectile.ai[1] == 1f;
            set => projectile.ai[1] = value.ToInt();
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pole Warper");
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 22;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.minionSlots = 0.5f;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.timeLeft *= 5;
            projectile.minion = true;
            projectile.tileCollide = false;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 12;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            if (projectile.localAI[0] == 0f)
            {
                Initialize(player);
                projectile.localAI[0] = 1f;
            }
            GrantBuffs(player);
            NPC potentialTarget = projectile.Center.MinionHoming(1400f, player);
            if (potentialTarget is null)
            {
                PlayerMovement(player);
                RepelMovement();
            }
            else
            {
                NPCMovement(potentialTarget);
                if (Time % 60f < 35f)
                {
                    RepelMovement();
                }
            }
            Time++;
        }

        public void Initialize(Player player)
        {
            projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
            projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
            for (int i = 0; i < 45; i++)
            {
                float angle = MathHelper.TwoPi / 45f * i;
                Vector2 velocity = angle.ToRotationVector2() * 4f;
                Dust dust = Dust.NewDustPerfect(projectile.Center + velocity * 2.75f, 261, velocity);
                dust.noGravity = true;
            }
        }

        // While this projectile cannot attack, the projectiles it shoots derive from the damage.
        public void AdjustDamage(Player player)
        {
            if (player.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int trueDamage = (int)(projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    player.MinionDamage());
                projectile.damage = trueDamage;
            }
        }

        public void GrantBuffs(Player player)
        {
            bool isCorrectProjectile = projectile.type == ModContent.ProjectileType<PoleWarperSummon>();
            player.AddBuff(ModContent.BuffType<PoleWarperBuff>(), 3600);
            if (isCorrectProjectile)
            {
                if (player.dead)
                {
                    player.Calamity().poleWarper = false;
                }
                if (player.Calamity().poleWarper)
                {
                    projectile.timeLeft = 2;
                }
            }
        }

        public void PlayerMovement(Player player)
        {
            Vector2 destination = player.Center + Vector2.UnitY.RotatedBy(Time / 16f + AngularOffset + (!North).ToInt() * MathHelper.Pi) * 180f;
            projectile.velocity = (projectile.velocity * 4f + projectile.SafeDirectionTo(destination) * 10f) / 5f;
            projectile.velocity = projectile.velocity.SafeNormalize(Vector2.UnitY) * 10f;
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public void NPCMovement(NPC npc)
        {
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                if (Main.projectile[i].type == projectile.type &&
                    Main.projectile[i].active &&
                    projectile.owner == projectile.owner)
                {
                    PoleWarperSummon otherPole = (PoleWarperSummon)Main.projectile[i].modProjectile;
                    if (otherPole.Time != Time && otherPole.Time != Time + 1)
                    {
                        otherPole.Time = Time;
                    }
                }
            }
            if (Time % 60f < 20f)
            {
                float offsetAngle = AngularOffset * 0.5f + (!North).ToInt() * MathHelper.Pi;
                Vector2 destination = npc.Center + Vector2.UnitY.RotatedBy(offsetAngle) * 180f;
                projectile.velocity = (projectile.velocity * 4f + projectile.SafeDirectionTo(destination) * 10f) / 5f;
                projectile.velocity = projectile.velocity.SafeNormalize(Vector2.UnitY) * 25f;
                projectile.rotation = projectile.AngleTo(npc.Center) + MathHelper.PiOver2;
            }
            else if (Time % 60f < 35f)
            {
                projectile.velocity *= 0.96f;
                projectile.rotation += 0.05f;
            }
            else if (Time % 60f == 35f)
            {
                projectile.velocity = projectile.SafeDirectionTo(npc.Center, -Vector2.UnitY) * 29f;
                projectile.rotation = projectile.AngleTo(npc.Center) + MathHelper.PiOver2;
            }
        }

        public void RepelMovement()
        {
            // This does not incorporate attraction on purpose. Doing so causes the minions to very easily become distracted.
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                if (Main.projectile[i].type == projectile.type &&
                    Main.projectile[i].active &&
                    projectile.Distance(Main.projectile[i].Center) < 40f)
                {
                    PoleWarperSummon otherPole = (PoleWarperSummon)Main.projectile[i].modProjectile;
                    if (otherPole.North != North)
                    {
                        float distanceFromOtherPole = projectile.Distance(Main.projectile[i].Center) + 1f;
                        if (float.IsNaN(distanceFromOtherPole) || distanceFromOtherPole < 1f)
                        {
                            distanceFromOtherPole = 1f;
                        }
                        float repulsionSpeed = MaximumRepulsionSpeed * (float)Math.Pow(3f, -distanceFromOtherPole / 27f);
                        projectile.velocity -= (Main.projectile[i].Center - projectile.Center).SafeNormalize(Vector2.UnitY) * repulsionSpeed;
                    }
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, 1, lightColor, 2);
            return false;
        }
    }
}
