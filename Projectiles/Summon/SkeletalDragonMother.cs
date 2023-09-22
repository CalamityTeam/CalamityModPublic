using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class SkeletalDragonMother : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public const float DistanceToCheck = 1100f;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 92;
            Projectile.height = 78;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 6;
            Projectile.minionSlots = 6f;
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
            if (Projectile.localAI[0] == 0f)
            {
                for (int i = 0; i < 2; i++)
                {
                    int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + Utils.RandomVector2(Main.rand, -24f, 24f),
                        Main.rand.NextVector2CircularEdge(4f, 4f), ModContent.ProjectileType<SkeletalDragonChild>(), Projectile.damage, Projectile.knockBack, player.whoAmI, Projectile.whoAmI);
                    if (Main.projectile.IndexInRange(p))
                        Main.projectile[p].originalDamage = Projectile.originalDamage;
                }
                Projectile.localAI[0] = 1f;
            }
            bool isProperProjectile = Projectile.type == ModContent.ProjectileType<SkeletalDragonMother>();
            player.AddBuff(ModContent.BuffType<SkeletalDragonsBuff>(), 3600);
            if (isProperProjectile)
            {
                if (player.dead)
                {
                    modPlayer.dragonFamily = false;
                }
                if (modPlayer.dragonFamily)
                {
                    Projectile.timeLeft = 2;
                }
            }

            NPC target = Projectile.Center.MinionHoming(DistanceToCheck, player);

            if (target != null)
            {
                Projectile.extraUpdates = 1;
                Projectile.ai[0]++;

                // Arc towards enemy every 60 frames for 30 frames.
                float modulo = Projectile.ai[0] % 150f;
                if ((modulo < 30f) ||
                    (modulo >= 90 && modulo < 120f))
                {
                    if (Projectile.velocity.Length() == 0f)
                        Projectile.velocity = Projectile.SafeDirectionTo(target.Center).RotatedByRandom(0.5f) * -8f;
                    float angleToTarget = Projectile.AngleTo(target.Center);
                    float velocityAngle = Projectile.velocity.ToRotation();
                    float resultantAngle = velocityAngle.AngleLerp(angleToTarget, 0.08f);
                    if (Projectile.Distance(target.Center) > 70f)
                    {
                        Projectile.velocity = new Vector2(Projectile.velocity.Length(), 0f).RotatedBy(resultantAngle);
                    }
                    else
                    {
                        Projectile.velocity = (Projectile.velocity * 44f + Projectile.SafeDirectionTo(target.Center) * 24f) / 45f;
                        Projectile.ai[0] += 30 - Projectile.ai[0] % 30f;
                    }
                    Projectile.ai[1] = 1f;
                }
                else
                {
                    Projectile.ai[1] = 0f;
                }
                if ((modulo >= 30f && modulo <= 60f) || (modulo >= 90f && modulo <= 120f))
                {
                    if (Projectile.owner == player.whoAmI && Projectile.spriteDirection == (Projectile.SafeDirectionTo(target.Center).X > 0).ToDirectionInt())
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.SafeDirectionTo(target.Center) * 11.5f, ModContent.ProjectileType<BloodBreath>(), Projectile.damage, 0f, Projectile.owner);
                    }
                }
            }
            else if (Projectile.Distance(player.Center) > 175f)
            {
                Projectile.extraUpdates = 0;
                Projectile.ai[1] = 0f;
                Projectile.velocity = (Projectile.velocity * 24f + Projectile.SafeDirectionTo(player.Center) * 16f) / 25f;
                if (Projectile.Distance(player.Center) > 3250f)
                {
                    Projectile.Center = player.Center;
                    for (int i = 0; i < Main.projectile.Length; i++)
                    {
                        if (Main.projectile[i].active && Main.projectile[i].owner == Projectile.owner &&
                            Main.projectile[i].type == ModContent.ProjectileType<SkeletalDragonChild>())
                        {
                            Main.projectile[i].Center = player.Center;
                            Main.projectile[i].netUpdate = true;
                        }
                    }
                    Projectile.netUpdate = true;
                }
            }
            Projectile.direction = Projectile.spriteDirection = (Projectile.velocity.X > 0).ToDirectionInt();

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
    }
}
