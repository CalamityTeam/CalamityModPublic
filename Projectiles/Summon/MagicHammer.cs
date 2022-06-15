using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class MagicHammer : ModProjectile
    {
        private int counter = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hammer");
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.alpha = 255;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.penetrate = 6;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 6;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            Projectile.localAI[1] += 1f;
            if (Projectile.localAI[1] > 10f && Main.rand.NextBool(3))
            {
                Projectile.alpha -= 5;
                if (Projectile.alpha < 50)
                {
                    Projectile.alpha = 50;
                }
            }

            Projectile.rotation += 0.075f;

            counter++;
            if (counter == 30)
            {
                Projectile.netUpdate = true;
            }
            else if (counter < 30)
            {
                return;
            }

            int targetIndex = -1;
            Vector2 targetVec = Projectile.Center;
            float maxDistance = MagicHat.Range;
            if (Projectile.localAI[0] > 0f)
            {
                Projectile.localAI[0] -= 1f;
            }
            Player player = Main.player[Projectile.owner];
            if (Projectile.ai[0] == 0f && Projectile.localAI[0] == 0f)
            {
                if (player.HasMinionAttackTargetNPC)
                {
                    NPC npc = Main.npc[player.MinionAttackTargetNPC];
                    if (npc.CanBeChasedBy(Projectile, false) && (Projectile.ai[0] == 0f || Projectile.ai[0] == player.MinionAttackTargetNPC + 1f))
                    {
                        float targetDist = Vector2.Distance(npc.Center, targetVec);
                        if (targetDist < maxDistance)
                        {
                            targetVec = npc.Center;
                            targetIndex = player.MinionAttackTargetNPC;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < Main.npc.Length; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (npc.CanBeChasedBy(Projectile, false) && (Projectile.ai[0] == 0f || Projectile.ai[0] == i + 1f))
                        {
                            float targetDist = Vector2.Distance(npc.Center, targetVec);
                            if (targetDist < maxDistance)
                            {
                                targetVec = npc.Center;
                                targetIndex = i;
                            }
                        }
                    }
                }
                if (targetIndex >= 0)
                {
                    Projectile.ai[0] = targetIndex + 1f;
                    Projectile.netUpdate = true;
                }
            }
            if (Projectile.localAI[0] == 0f && Projectile.ai[0] == 0f)
            {
                Projectile.localAI[0] = 30f;
            }
            bool canHome = false;
            if (Projectile.ai[0] != 0f)
            {
                int target = (int)(Projectile.ai[0] - 1f);
                NPC npc = Main.npc[target];
                if (npc.CanBeChasedBy(Projectile, false) && npc.immune[Projectile.owner] == 0)
                {
                    float targetDist = Vector2.Distance(npc.Center, targetVec);
                    if (targetDist < maxDistance * 1.25f)
                    {
                        canHome = true;
                        targetVec = npc.Center;
                    }
                }
                else
                {
                    Projectile.ai[0] = 0f;
                    canHome = false;
                    Projectile.netUpdate = true;
                }
            }
            if (canHome)
            {
                Vector2 velocity = targetVec - Projectile.Center;
                float trajectory = Projectile.velocity.ToRotation();
                float target = velocity.ToRotation();
                float rotateAmt = target - trajectory;
                rotateAmt = MathHelper.WrapAngle(rotateAmt);
                Projectile.velocity = Projectile.velocity.RotatedBy(rotateAmt * 0.25, default);
            }
            float speed = Projectile.velocity.Length();
            Projectile.velocity.Normalize();
            Projectile.velocity *= speed + 0.0025f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(255, 56, 0, Projectile.alpha);

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Vector2 dspeed = new Vector2(Main.rand.NextFloat(-7f, 7f), Main.rand.NextFloat(-7f, 7f));
                int dust = Dust.NewDust(Projectile.Center, 1, 1, 67, dspeed.X, dspeed.Y, 50, default, 1.2f);
                Main.dust[dust].noGravity = true;
            }
        }
    }
}
