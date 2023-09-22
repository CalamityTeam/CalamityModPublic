using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class IceSentry : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 18;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 94;
            Projectile.height = 94;
            Projectile.timeLeft = Projectile.SentryLifeTime;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.sentry = true;
            Projectile.coldDamage = true;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.velocity = Vector2.Zero;

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }

            if (Projectile.ai[1] < 300f)
            {
                Projectile.localAI[1] = 1f;
                if (Projectile.frame >= 9)
                    Projectile.frame = 0;
            }
            else
            {
                if (Projectile.frame >= 18)
                    Projectile.frame = 9;

                Projectile.localAI[1]++;
                if (Projectile.localAI[1] > 2)
                {
                    Projectile.localAI[1] = 0;

                    if (Projectile.owner == Main.myPlayer)
                    {
                        Vector2 speed = new Vector2(Main.rand.Next(-1000, 1001), Main.rand.Next(-1000, 1001));
                        speed.Normalize();
                        speed *= 15f;
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + speed, speed, ModContent.ProjectileType<IceSentryShard>(), Projectile.damage / 2, Projectile.knockBack / 2, Projectile.owner);
                    }
                }
            }

            NPC minionAttackTargetNpc = Projectile.OwnerMinionAttackTargetNPC;
            if (minionAttackTargetNpc != null && Projectile.ai[0] != minionAttackTargetNpc.whoAmI && minionAttackTargetNpc.CanBeChasedBy(Projectile) && Collision.CanHit(Projectile.Center, 0, 0, minionAttackTargetNpc.position, minionAttackTargetNpc.width, minionAttackTargetNpc.height))
            {
                Projectile.ai[0] = minionAttackTargetNpc.whoAmI;
                Projectile.ai[1] = 0f;
                Projectile.localAI[0] = 0f;
                Projectile.netUpdate = true;
            }

            if (Projectile.ai[0] >= 0 && Projectile.ai[0] < Main.maxNPCs)
            {
                NPC npc = Main.npc[(int)Projectile.ai[0]];

                bool rememberTarget = npc.CanBeChasedBy(Projectile);
                if (rememberTarget)
                {
                    Projectile.localAI[0]++;

                    if (Projectile.ai[1] < 300f)
                        Projectile.ai[1]++;

                    float delay = 60f - Projectile.ai[1] / 60f * 10f;
                    if (Projectile.localAI[0] > delay)
                    {
                        Projectile.localAI[0] = 0f;

                        rememberTarget = Collision.CanHit(Projectile.Center, 0, 0, npc.position, npc.width, npc.height);
                        if (rememberTarget && Projectile.owner == Main.myPlayer)
                        {
                            Vector2 speed = npc.Center - Projectile.Center;
                            speed.Normalize();
                            speed *= 8f;
                            if (Projectile.ai[1] >= 300f)
                                speed = speed.RotatedBy(MathHelper.ToRadians(Main.rand.Next(-5, 6))) * 1.5f + npc.velocity / 2f;
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, speed + npc.velocity / 2f, ModContent.ProjectileType<IceSentryFrostBolt>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                        }
                    }
                }

                if (!rememberTarget)
                {
                    Projectile.ai[0] = -1f;
                    Projectile.ai[1] = 0f;
                    Projectile.netUpdate = true;
                }
            }
            else
            {
                Projectile.localAI[0] = 0f;

                float maxDistance = 1000f;
                int possibleTarget = -1;

                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.CanBeChasedBy(Projectile))
                    {
                        float npcDistance = Projectile.Distance(npc.Center);
                        if (npcDistance < maxDistance)
                        {
                            maxDistance = npcDistance;
                            possibleTarget = i;
                        }
                    }
                }

                if (possibleTarget > 0)
                {
                    //Main.NewText("new target acquired");
                    Projectile.ai[0] = possibleTarget;
                    Projectile.ai[1] = 0f;
                    Projectile.netUpdate = true;
                }
            }
        }

        public override bool? CanDamage() => false;
    }
}
