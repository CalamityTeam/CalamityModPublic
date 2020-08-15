using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
	public class AlphaSeeker : ModProjectile
    {
        public static float moveSpeed = 2f;
        public static float rotateSpeed = 0.04f;
        public static int lifetime = 120;
        public static int returnTime = 60;
        public bool initialized = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Seeker");
        }

        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.timeLeft = lifetime;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            if (!initialized)
            {
                projectile.rotation = Main.rand.NextFloat(0f, MathHelper.TwoPi);
                projectile.localAI[1] = Main.rand.NextFloat(-rotateSpeed, rotateSpeed);
                initialized = true;
            }
            if (projectile.ai[0] == 1f)
            {
                // Follow enemy
                float minDist = 999f;
                int index = 0;
                for (int i = 0; i < Main.npc.Length; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.CanBeChasedBy(projectile, false))
                    {
                        float dist = (projectile.Center - npc.Center).Length();
                        if (dist < minDist)
                        {
                            minDist = dist;
                            index = i;
                        }
                    }
                }

                Vector2 velocityNew;
                if (minDist < 999f)
                {
                    velocityNew = Main.npc[index].Center - projectile.Center;
                    velocityNew.Normalize();
                    projectile.velocity += velocityNew;
                    if (projectile.velocity.Length() > 10f)
                    {
                        projectile.velocity.Normalize();
                        projectile.velocity *= 10f;
                    }
                }
            }
            else if (projectile.ai[0] == 2f)
            {
                // projectile.localAI[0] controls the distance from the parent projectile

                projectile.tileCollide = false;

                Projectile parent = Main.projectile[0];
                bool active = false;
                for (int i = 0; i < Main.projectile.Length; i++)
                {
                    Projectile p = Main.projectile[i];
                    if (p.identity == projectile.ai[1] && p.active)
                    {
                        parent = p;
                        active = true;
                    }
                }

                if (active)
                {
                    Vector2 pos = new Vector2(0, projectile.localAI[0]);
                    pos = pos.RotatedBy(projectile.rotation);

                    projectile.Center = parent.Center + pos;
                    projectile.rotation += projectile.localAI[1];

                    if (projectile.timeLeft > returnTime)
                    {
                        projectile.localAI[0] += moveSpeed;
                    }
                    else
                    {
                        projectile.localAI[0] -= moveSpeed;
                    }
                }
                else
                {
                    projectile.Kill();
                }
            }

            int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 89, 0f, 0f, 100, default, 2f);
            Main.dust[dust].noGravity = true;
            Main.dust[dust].velocity.Y = -0.15f;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Plague>(), 180);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Plague>(), 180);
        }
    }
}
