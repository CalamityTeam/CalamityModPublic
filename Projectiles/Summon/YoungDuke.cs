using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class YoungDuke : ModProjectile
    {
        public const float DistanceBeforeCharge = 420f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Young Old Duke");
            Main.projFrames[Projectile.type] = 16;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft *= 5;
            Projectile.minion = true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (!modPlayer.miniOldDuke)
            {
                Projectile.active = false;
                return;
            }

            bool correctMinion = Projectile.type == ModContent.ProjectileType<YoungDuke>();
            if (correctMinion)
            {
                if (player.dead)
                {
                    modPlayer.youngDuke = false;
                }
                if (modPlayer.youngDuke)
                {
                    Projectile.timeLeft = 2;
                }
            }

            // Adjust damage as needed
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
                Projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = Projectile.damage;
                Projectile.localAI[0] = 1f;
            }
            if (player.MinionDamage() != Projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int trueDamage = (int)(Projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    Projectile.Calamity().spawnedPlayerMinionDamageValue *
                    player.MinionDamage());
                Projectile.damage = trueDamage;
            }

            bool playerHalfLife = player.statLife <= player.statLifeMax2 * 0.5f;

            // Frames
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
            {
                Projectile.frame = 0;
            }
            if (playerHalfLife && Projectile.frame < Main.projFrames[Projectile.type] / 2)
            {
                Projectile.frame += Main.projFrames[Projectile.type] / 2;
            }
            else if (!playerHalfLife && Projectile.frame >= Main.projFrames[Projectile.type] / 2)
            {
                Projectile.frame -= Main.projFrames[Projectile.type] / 2;
            }

            NPC potentialTarget = Projectile.Center.ClosestNPCAt(1600f);
            if (potentialTarget != null)
            {
                if (Projectile.Distance(potentialTarget.Center) < DistanceBeforeCharge)
                {
                    Projectile.ai[0] += 1f;
                    int timePerCharge = playerHalfLife ? 25 : 35;
                    float chargeSpeed = playerHalfLife ? 25f : 20f;
                    if (Projectile.ai[0] >= timePerCharge)
                    {
                        Projectile.velocity = Projectile.SafeDirectionTo(potentialTarget.Center) * chargeSpeed;
                        Projectile.ai[0] = 0f;
                    }
                    else
                        Projectile.velocity *= 0.9825f;
                }
                else
                {
                    float intertia = 0.94f;
                    float homeSpeed = playerHalfLife ? 38f : 32f;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.SafeDirectionTo(potentialTarget.Center) * homeSpeed, 1f - intertia);
                }
            }
            else if (!Projectile.WithinRange(player.Center, 140f))
                Projectile.velocity = (Projectile.velocity * 30f + Projectile.SafeDirectionTo(player.Center) * 16f) / 31f;

            Projectile.direction = Projectile.spriteDirection = (Projectile.velocity.X > 0).ToDirectionInt();
        }
    }
}
