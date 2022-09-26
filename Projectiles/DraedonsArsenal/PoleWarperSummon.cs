using CalamityMod.Buffs.Summon;
using Microsoft.Xna.Framework;
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
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public bool North
        {
            get => Projectile.ai[1] == 1f;
            set => Projectile.ai[1] = value.ToInt();
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pole Warper");
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 22;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.minionSlots = 0.5f;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.timeLeft *= 5;
            Projectile.minion = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 12;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (Projectile.localAI[0] == 0f)
            {
                Initialize(player);
                Projectile.localAI[0] = 1f;
            }
            GrantBuffs(player);
            NPC potentialTarget = Projectile.Center.MinionHoming(1400f, player);

            // Teleport near the target if very far away from them.
            if (!Projectile.WithinRange(player.Center, 4200f))
            {
                Projectile.Center = player.Center + Vector2.UnitY * North.ToDirectionInt() * 25f;
            }

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
            for (int i = 0; i < 45; i++)
            {
                float angle = MathHelper.TwoPi / 45f * i;
                Vector2 velocity = angle.ToRotationVector2() * 4f;
                Dust dust = Dust.NewDustPerfect(Projectile.Center + velocity * 2.75f, 261, velocity);
                dust.noGravity = true;
            }
        }

        public void GrantBuffs(Player player)
        {
            bool isCorrectProjectile = Projectile.type == ModContent.ProjectileType<PoleWarperSummon>();
            player.AddBuff(ModContent.BuffType<PoleWarperBuff>(), 3600);
            if (isCorrectProjectile)
            {
                if (player.dead)
                {
                    player.Calamity().poleWarper = false;
                }
                if (player.Calamity().poleWarper)
                {
                    Projectile.timeLeft = 2;
                }
            }
        }

        public void PlayerMovement(Player player)
        {
            Vector2 destination = player.Center + Vector2.UnitY.RotatedBy(Time / 16f + AngularOffset + (!North).ToInt() * MathHelper.Pi) * 180f;
            Projectile.velocity = (Projectile.velocity * 4f + Projectile.SafeDirectionTo(destination) * 10f) / 5f;
            Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * 10f;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public void NPCMovement(NPC npc)
        {
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                if (Main.projectile[i].type == Projectile.type &&
                    Main.projectile[i].active &&
                    Projectile.owner == Projectile.owner)
                {
                    PoleWarperSummon otherPole = (PoleWarperSummon)Main.projectile[i].ModProjectile;
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
                Projectile.velocity = (Projectile.velocity * 4f + Projectile.SafeDirectionTo(destination) * 10f) / 5f;
                Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * 25f;
                Projectile.rotation = Projectile.AngleTo(npc.Center) + MathHelper.PiOver2;
            }
            else if (Time % 60f < 35f)
            {
                Projectile.velocity *= 0.96f;
                Projectile.rotation += 0.05f;
            }
            else if (Time % 60f == 35f)
            {
                Projectile.velocity = Projectile.SafeDirectionTo(npc.Center, -Vector2.UnitY) * 29f;
                Projectile.rotation = Projectile.AngleTo(npc.Center) + MathHelper.PiOver2;
            }
        }

        public void RepelMovement()
        {
            // This does not incorporate attraction on purpose. Doing so causes the minions to very easily become distracted.
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                if (Main.projectile[i].type == Projectile.type &&
                    Main.projectile[i].active &&
                    Projectile.Distance(Main.projectile[i].Center) < 40f)
                {
                    PoleWarperSummon otherPole = (PoleWarperSummon)Main.projectile[i].ModProjectile;
                    if (otherPole.North != North)
                    {
                        float distanceFromOtherPole = Projectile.Distance(Main.projectile[i].Center) + 1f;
                        if (float.IsNaN(distanceFromOtherPole) || distanceFromOtherPole < 1f)
                        {
                            distanceFromOtherPole = 1f;
                        }
                        float repulsionSpeed = MaximumRepulsionSpeed * (float)Math.Pow(3f, -distanceFromOtherPole / 27f);
                        Projectile.velocity -= (Main.projectile[i].Center - Projectile.Center).SafeNormalize(Vector2.UnitY) * repulsionSpeed;
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, 1, lightColor, 2);
            return false;
        }
    }
}
