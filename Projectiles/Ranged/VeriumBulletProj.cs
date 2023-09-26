using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class VeriumBulletProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        private float speed = 0f;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 600;
            Projectile.extraUpdates = 1;
            AIType = ProjectileID.Bullet;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.DefaultPointBlankDuration;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesFromEdge(Projectile, 0, lightColor);
            return false;
        }

        public override bool PreAI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);
            Projectile.spriteDirection = Projectile.direction;

            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > 4f)
            {
                if (Main.rand.NextBool())
                {
                    int purple = Dust.NewDust(Projectile.position, 1, 1, 70, 0f, 0f, 0, default, 0.5f);
                    Main.dust[purple].alpha = Projectile.alpha;
                    Main.dust[purple].velocity *= 0f;
                    Main.dust[purple].noGravity = true;
                }
            }

            if (Projectile.ai[0] > 0f)
                Projectile.ai[0]--;
            if (speed == 0f)
                speed = Projectile.velocity.Length();
            if (Projectile.penetrate == 1 && Projectile.ai[0] <= 0f)
            {
                float inertia = 15f;
                Vector2 center = Projectile.Center;
                float maxDistance = 300f;
                bool homeIn = false;

                int targetIndex = (int)Projectile.ai[1];
                NPC target = Main.npc[targetIndex];
                if (target.CanBeChasedBy(Projectile, false))
                {
                    float extraDistance = (target.width / 2) + (target.height / 2);

                    bool canHit = true;
                    if (extraDistance < maxDistance)
                        canHit = Collision.CanHit(Projectile.Center, 1, 1, target.Center, 1, 1);

                    if (Vector2.Distance(target.Center, Projectile.Center) < (maxDistance + extraDistance) && canHit)
                    {
                        center = target.Center;
                        homeIn = true;
                    }
                }

                if (!homeIn)
                {
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (npc.CanBeChasedBy(Projectile, false))
                        {
                            float extraDistance = (npc.width / 2) + (npc.height / 2);

                            bool canHit = true;
                            if (extraDistance < maxDistance)
                                canHit = Collision.CanHit(Projectile.Center, 1, 1, npc.Center, 1, 1);

                            if (Vector2.Distance(npc.Center, Projectile.Center) < (maxDistance + extraDistance) && canHit)
                            {
                                center = npc.Center;
                                homeIn = true;
                                break;
                            }
                        }
                    }
                }

                if (!Projectile.friendly)
                {
                    homeIn = false;
                }

                if (homeIn)
                {
                    Vector2 moveDirection = Projectile.SafeDirectionTo(center, Vector2.UnitY);
                    Projectile.velocity = (Projectile.velocity * inertia + moveDirection * speed) / (inertia + 1f);
                }
                return false;
            }
            return true;
        }

        public override bool? CanHitNPC(NPC target) => Projectile.ai[0] <= 0f && target.CanBeChasedBy(Projectile);

        public override bool CanHitPvp(Player target) => Projectile.ai[0] <= 0f;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.ai[0] = 10f;
            Projectile.damage /= 2;
            if (target.life > 0)
                Projectile.ai[1] = target.whoAmI;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            Projectile.ai[0] = 10f;
            Projectile.damage /= 2;
        }
    }
}
