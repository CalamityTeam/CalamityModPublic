using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class RustyDrone : ModProjectile
    {
        public const float PlayerHomingInertia = 32f;
        public const float PlayerHomingSpeed = 11f;

        public const float NPCHomingInertia = 20f;
        public const float NPCHomingSpeed = 14f;

        public const float AttackRate = 140f;

        public const float DistanceToCheck = 585f;

        public const int ExplosionShrapnelBaseDamage = 16; // Uses player.MinionDamage

        public const int ResummonCooldownTime = 60 * 3;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rusty Drone");
            Main.projFrames[Projectile.type] = 12;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 36;
            Projectile.height = 30;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 1f;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft *= 5;
            Projectile.minion = true;
            Projectile.alpha = 255;
            Projectile.Calamity().overridesMinionDamagePrevention = true; // Will only do damage once, and the location of said damage must be deliberate.
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (Projectile.localAI[0] == 0f)
            {
                int oldDamage = Projectile.damage;
                Projectile.damage = 1; // For the initial tiny prick effect
                Projectile.Damage();
                Projectile.damage = oldDamage;
                Projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
                Projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = Projectile.damage;
                Projectile.Center -= Vector2.UnitY * 1400f;
                Projectile.localAI[0] += 1f;
            }
            if (Projectile.alpha > 0)
            {
                Projectile.alpha = (int)MathHelper.Lerp(Projectile.alpha, 0f, 0.02f);
                Projectile.velocity = Vector2.UnitY * 10f;
            }
            if (player.MinionDamage() != Projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int trueDamage = (int)(Projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    Projectile.Calamity().spawnedPlayerMinionDamageValue *
                    player.MinionDamage());
                Projectile.damage = trueDamage;
            }
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 8)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            bool isCorrectProjectile = Projectile.type == ModContent.ProjectileType<RustyDrone>();
            player.AddBuff(ModContent.BuffType<RustyDroneBuff>(), 3600);
            if (isCorrectProjectile)
            {
                if (player.dead)
                {
                    modPlayer.rustyDrone = false;
                }
                if (modPlayer.rustyDrone)
                {
                    Projectile.timeLeft = 2;
                }
            }
            NPC potentialTarget = Projectile.Center.MinionHoming(DistanceToCheck, player);
            if (potentialTarget == null)
            {
                if (Projectile.Distance(player.Center) > 120f)
                {
                    Projectile.velocity = (Projectile.velocity * PlayerHomingInertia + Projectile.SafeDirectionTo(player.Top) * PlayerHomingSpeed) / (PlayerHomingInertia + 1);
                }
                if (Projectile.frame >= 8)
                {
                    Projectile.frame = 0;
                }
            }
            else
            {
                // A bit above the target, and always on the opposite side of the target's sprite direction.
                Vector2 destination = potentialTarget.Top + Vector2.UnitX * -potentialTarget.spriteDirection * Main.rand.NextFloat(50f, 75f) - Vector2.UnitY * 42f;
                Projectile.velocity = (Projectile.velocity * NPCHomingInertia + Projectile.SafeDirectionTo(destination) * NPCHomingSpeed) / (NPCHomingInertia + 1);
                if (Projectile.frame < 8)
                {
                    Projectile.frame = 8;
                }
                if (Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 8;
                }
                if (Projectile.Distance(destination) < 55f)
                {
                    if (Projectile.ai[0]++ % AttackRate == AttackRate - 1)
                    {
                        if (Projectile.owner == Main.myPlayer)
                        {
                            Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), potentialTarget.Center, Vector2.Zero, ModContent.ProjectileType<RustyDroneTargetIndicator>(), 0, 0f, Projectile.owner, Projectile.whoAmI, potentialTarget.whoAmI);
                        }
                    }
                }
            }
            Projectile.spriteDirection = (Projectile.velocity.X > 0).ToDirectionInt();
            Projectile.rotation = Projectile.velocity.X * 0.05f;
            Projectile.ai[1]++;
            if (Projectile.ai[1] >= 360f)
            {
                Utils.PoofOfSmoke(Projectile.Center);
                for (int i = 0; i < 4; i++)
                {
                    Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), Projectile.Center, Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * 8f, ModContent.ProjectileType<RustShrapnel>(),
                        (int)(ExplosionShrapnelBaseDamage * player.MinionDamage()), 2f, Projectile.owner);
                }
                Projectile.Kill();
            }
        }

        public override bool? CanDamage() => Projectile.localAI[0] == 0f; // Only do damage from the initial 1 damage prick, and never again. This is a support summon.
    }
}
