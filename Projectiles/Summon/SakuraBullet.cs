using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Summon
{
    public class SakuraBullet : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.netImportant = true;
            Projectile.timeLeft = 150;
            Projectile.extraUpdates = 1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Summon;
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

            int pinkDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 73, 0f, 0f, 100, default, 0.6f);
            Main.dust[pinkDust].noGravity = true;
            Main.dust[pinkDust].velocity *= 0.5f;
            Main.dust[pinkDust].velocity += Projectile.velocity * 0.1f;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item25, Projectile.position);
            int dust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 73, 0f, 0f, 100, default, 1f);
            Main.dust[dust].velocity *= 0.5f;
            if (Main.rand.NextBool())
            {
                Main.dust[dust].scale = 0.5f;
                Main.dust[dust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
            }
            int dust2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 73, 0f, 0f, 100, default, 1.4f);
            Main.dust[dust2].noGravity = true;
            dust2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 73, 0f, 0f, 100, default, 0.8f);
        }
    }
}
