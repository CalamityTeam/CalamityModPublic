using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Summon
{
    public class SakuraBullet : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sakura Bullet");
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.netImportant = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 150;
            Projectile.minion = true;
            Projectile.minionSlots = 0f;
            Projectile.extraUpdates = 1;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.02f;

            Vector2 center = Projectile.Center;
            float maxDistance = 400f;
            bool homeIn = false;

            if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                if ((npc.CanBeChasedBy(Projectile, false) || npc.type == NPCID.DukeFishron) && npc.active)
                {
                    float extraDistance = (npc.width / 2) + (npc.height / 2);

                    if (Vector2.Distance(npc.Center, Projectile.Center) < (maxDistance + extraDistance))
                    {
                        center = npc.Center;
                        homeIn = true;
                    }
                }
            }
            else if (Projectile.ai[0] != -1f)
            {
                NPC npc = Main.npc[(int)Projectile.ai[0]];
                if ((npc.CanBeChasedBy(Projectile, false) || npc.type == NPCID.DukeFishron) && npc.active)
                {
                    float extraDistance = (npc.width / 2) + (npc.height / 2);

                    if (Vector2.Distance(npc.Center, Projectile.Center) < (maxDistance + extraDistance))
                    {
                        center = npc.Center;
                        homeIn = true;
                    }
                }
            }
            if (!homeIn)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if ((npc.CanBeChasedBy(Projectile, false) || (npc.type == NPCID.DukeFishron && (!npc.dontTakeDamage || npc.ai[0] > 9f))) && npc.active)
                    {
                        float extraDistance = (npc.width / 2) + (npc.height / 2);

                        if (Vector2.Distance(npc.Center, Projectile.Center) < (maxDistance + extraDistance))
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
                Projectile.velocity = (Projectile.velocity * 20f + moveDirection * 11f) / (21f);
            }

            int num458 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 73, 0f, 0f, 100, default, 0.6f);
            Main.dust[num458].noGravity = true;
            Main.dust[num458].velocity *= 0.5f;
            Main.dust[num458].velocity += Projectile.velocity * 0.1f;
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item25, Projectile.position);
            int num622 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 73, 0f, 0f, 100, default, 1f);
            Main.dust[num622].velocity *= 0.5f;
            if (Main.rand.NextBool(2))
            {
                Main.dust[num622].scale = 0.5f;
                Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
            }
            int num624 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 73, 0f, 0f, 100, default, 1.4f);
            Main.dust[num624].noGravity = true;
            num624 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 73, 0f, 0f, 100, default, 0.8f);
        }
    }
}
