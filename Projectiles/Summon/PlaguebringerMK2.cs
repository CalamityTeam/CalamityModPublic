using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class PlaguebringerMK2 : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public const float DistanceToCheck = 1000.0001f;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 38;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 1f;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft *= 5;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            bool isProperProjectile = Projectile.type == ModContent.ProjectileType<PlaguebringerMK2>();
            player.AddBuff(ModContent.BuffType<MiniPlaguebringerBuff>(), 3600);
            if (isProperProjectile)
            {
                if (player.dead)
                {
                    modPlayer.plaguebringerMK2 = false;
                }
                if (modPlayer.plaguebringerMK2)
                {
                    Projectile.timeLeft = 2;
                }
            }

            NPC potentialTarget = Projectile.Center.MinionHoming(DistanceToCheck, player);

            if (potentialTarget != null)
            {
                int sign = (potentialTarget.Center.X - Projectile.Center.X < 0).ToDirectionInt();
                float x = (125f + 40f * (int)(Projectile.ai[1] % 10f)) * sign;
                int y = -160 - 50 * (int)(Projectile.ai[1] / 10);
                Vector2 destination = potentialTarget.Center + new Vector2(x, y);
                if (Projectile.Distance(destination) < 6f)
                {
                    Projectile.velocity *= 0.9f;
                }
                else
                    Projectile.velocity = Projectile.SafeDirectionTo(destination) * Projectile.Distance(destination) / 36f;

                int timeNeeded = (int)MathHelper.Lerp(60f, 18f, MathHelper.Clamp(Projectile.localAI[1] / 320f, 0f, 1f));
                if (Projectile.ai[0] >= timeNeeded && Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center,
                        Projectile.SafeDirectionTo(potentialTarget.Center) * 14f,
                        ModContent.ProjectileType<MK2RocketNormal>(),
                        Projectile.damage,
                        3f,
                        Projectile.owner);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center,
                        Projectile.SafeDirectionTo(potentialTarget.Center) * 11.5f,
                        ModContent.ProjectileType<MK2RocketHoming>(),
                        Projectile.damage,
                        3f,
                        Projectile.owner);
                    Projectile.ai[0] = 0;
                }
                else Projectile.ai[0]++;
                Projectile.localAI[1]++;
                Projectile.direction = Projectile.spriteDirection = -sign;
            }
            else
            {
                Projectile.localAI[1] = 0;
                float x = (45f + 35f * (int)(Projectile.ai[1] % 10f)) * -player.direction;
                int y = -60 - 50 * (int)(Projectile.ai[1] / 10);
                Vector2 distanceToDestination = player.Center - Projectile.Center + new Vector2(x, y);
                float distance = distanceToDestination.Length();
                if (distance > 10f)
                {
                    float speed = 20f;
                    if (distance < 50f)
                    {
                        speed /= 2f;
                    }
                    Vector2 velocity = distanceToDestination.SafeNormalize(Projectile.direction * Vector2.UnitX) * speed;
                    Projectile.velocity = (Projectile.velocity * 20f + velocity) / 21f;
                    if (distance > 2250f)
                    {
                        Projectile.Center = player.Center;
                        Projectile.netUpdate = true;
                    }
                }
                else
                {
                    Projectile.direction = player.direction;
                    Projectile.velocity *= 0.9f;
                }
                Projectile.direction = Projectile.spriteDirection = player.direction;
            }

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
        }

        public override bool? CanDamage() => false;
    }
}
