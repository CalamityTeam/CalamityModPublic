using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Summon
{
    public class HowlsHeartFireball : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.netImportant = true;
            Projectile.timeLeft = 180;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            //Cycle through animation
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
            {
                Projectile.frame = 0;
            }

            Vector2 center = Projectile.Center;
            float maxDistance = 500f;
            bool homeIn = false;
            int target = (int)Projectile.ai[0];

            if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
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
                    }
                }
            }
            else if (Main.npc[target].CanBeChasedBy(Projectile, false))
            {
                NPC npc = Main.npc[target];

                float extraDistance = (npc.width / 2) + (npc.height / 2);

                bool canHit = true;
                if (extraDistance < maxDistance)
                    canHit = Collision.CanHit(Projectile.Center, 1, 1, npc.Center, 1, 1);

                if (Vector2.Distance(npc.Center, Projectile.Center) < (maxDistance + extraDistance) && canHit)
                {
                    center = npc.Center;
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

            if (homeIn)
            {
                Vector2 moveDirection = Projectile.SafeDirectionTo(center, Vector2.UnitY);
                Projectile.velocity = (Projectile.velocity * 20f + moveDirection * 21f) / (21f);
            }

            int blueT = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 59, 0f, 0f, 100, default, 0.6f);
            Main.dust[blueT].noGravity = true;
            Main.dust[blueT].velocity *= 0.5f;
            Main.dust[blueT].velocity += Projectile.velocity * 0.1f;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item45, Projectile.position);
            int blue = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 59, 0f, 0f, 100, default, 1f);
            Main.dust[blue].velocity *= 0.5f;
            if (Main.rand.NextBool())
            {
                Main.dust[blue].scale = 0.5f;
                Main.dust[blue].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
            }
            int torch = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 59, 0f, 0f, 100, default, 1.4f);
            Main.dust[torch].noGravity = true;
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 59, 0f, 0f, 100, default, 0.8f);
        }
    }
}
