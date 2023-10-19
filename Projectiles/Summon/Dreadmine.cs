using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Summon
{
    public class Dreadmine : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.SentryShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 58;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 3600;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            float xflag = 1f;
            float yflag = 1f;
            if (Projectile.identity % 6 == 0)
            {
                yflag *= -1f;
            }
            if (Projectile.identity % 6 == 1)
            {
                xflag *= -1f;
            }
            if (Projectile.identity % 6 == 2)
            {
                yflag *= -1f;
                xflag *= -1f;
            }
            if (Projectile.identity % 6 == 3)
            {
                yflag = 0f;
            }
            if (Projectile.identity % 6 == 4)
            {
                xflag = 0f;
            }
            Projectile.localAI[1] += 1f;
            if (Projectile.localAI[1] > 60f)
            {
                Projectile.localAI[1] = -180f;
            }
            if (Projectile.localAI[1] >= -60f)
            {
                Projectile.velocity.X = Projectile.velocity.X + 0.002f * yflag;
                Projectile.velocity.Y = Projectile.velocity.Y + 0.002f * xflag;
            }
            else
            {
                Projectile.velocity.X = Projectile.velocity.X - 0.002f * yflag;
                Projectile.velocity.Y = Projectile.velocity.Y - 0.002f * xflag;
            }
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] > 5400f)
            {
                Projectile.ai[1] = 1f;
                if (Projectile.ai[0] < 5500f)
                {
                    return;
                }
                if (Projectile.owner == Main.myPlayer)
                {
                    Projectile.Kill();
                }
            }
            else
            {
                float playerDist = (Projectile.Center - Main.player[Projectile.owner].Center).Length() / 100f;
                if (playerDist > 4f)
                {
                    playerDist *= 1.1f;
                }
                if (playerDist > 5f)
                {
                    playerDist *= 1.2f;
                }
                if (playerDist > 6f)
                {
                    playerDist *= 1.3f;
                }
                if (playerDist > 7f)
                {
                    playerDist *= 1.4f;
                }
                if (playerDist > 8f)
                {
                    playerDist *= 1.5f;
                }
                if (playerDist > 9f)
                {
                    playerDist *= 1.6f;
                }
                if (playerDist > 10f)
                {
                    playerDist *= 1.7f;
                }
                Projectile.ai[0] += playerDist;
                if (Projectile.alpha > 0)
                {
                    Projectile.alpha -= 25;
                    if (Projectile.alpha < 0)
                    {
                        Projectile.alpha = 0;
                    }
                }
            }
            bool canAttack = false;
            Vector2 center12 = new Vector2(0f, 0f);
            float attackDistance = 600f;
            if (Main.player[Projectile.owner].HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[Main.player[Projectile.owner].MinionAttackTargetNPC];
                if (npc.CanBeChasedBy(Projectile, false))
                {
                    float npcX = npc.position.X + (float)(npc.width / 2);
                    float npcY = npc.position.Y + (float)(npc.height / 2);
                    float npcDist = Math.Abs(Projectile.position.X + (float)(Projectile.width / 2) - npcX) + Math.Abs(Projectile.position.Y + (float)(Projectile.height / 2) - npcY);
                    if (npcDist < attackDistance)
                    {
                        attackDistance = npcDist;
                        center12 = npc.Center;
                        canAttack = true;
                    }
                }
            }
            if (!canAttack)
            {
                for (int i = 0; i < Main.npc.Length; i++)
                {
                    if (Main.npc[i].CanBeChasedBy(Projectile, false))
                    {
                        float npcX = Main.npc[i].position.X + (float)(Main.npc[i].width / 2);
                        float npcY = Main.npc[i].position.Y + (float)(Main.npc[i].height / 2);
                        float npcDist = Math.Abs(Projectile.position.X + (float)(Projectile.width / 2) - npcX) + Math.Abs(Projectile.position.Y + (float)(Projectile.height / 2) - npcY);
                        if (npcDist < attackDistance)
                        {
                            attackDistance = npcDist;
                            center12 = Main.npc[i].Center;
                            canAttack = true;
                        }
                    }
                }
            }
            if (canAttack)
            {
                Vector2 attackPosition = center12 - Projectile.Center;
                attackPosition.Normalize();
                attackPosition *= 0.75f;
                Projectile.velocity = (Projectile.velocity * 10f + attackPosition) / 10.8f; //11
                return;
            }
            if ((double)Projectile.velocity.Length() > 0.2)
            {
                Projectile.velocity *= 0.98f;
            }
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 112;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.Damage();
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            for (int j = 0; j < 20; j++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 31, 0f, 0f, 100, default, 2f);
                Main.dust[dust].velocity *= 3f;
                if (Main.rand.NextBool())
                {
                    Main.dust[dust].scale = 0.5f;
                    Main.dust[dust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int k = 0; k < 30; k++)
            {
                int dust2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 6, 0f, 0f, 100, default, 3f);
                Main.dust[dust2].noGravity = true;
                Main.dust[dust2].velocity *= 5f;
                dust2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 6, 0f, 0f, 100, default, 2f);
                Main.dust[dust2].velocity *= 2f;
            }

            if (Main.netMode != NetmodeID.Server)
            {
                Vector2 goreSource = Projectile.Center;
                int goreAmt = 3;
                Vector2 source = new Vector2(goreSource.X - 24f, goreSource.Y - 24f);
                for (int goreIndex = 0; goreIndex < goreAmt; goreIndex++)
                {
                    float velocityMult = 0.33f;
                    if (goreIndex < (goreAmt / 3))
                    {
                        velocityMult = 0.66f;
                    }
                    if (goreIndex >= (2 * goreAmt / 3))
                    {
                        velocityMult = 1f;
                    }
                    Mod mod = ModContent.GetInstance<CalamityMod>();
                    int type = Main.rand.Next(61, 64);
                    int smoke = Gore.NewGore(Projectile.GetSource_Death(), source, default, type, 1f);
                    Gore gore = Main.gore[smoke];
                    gore.velocity *= velocityMult;
                    gore.velocity.X += 1f;
                    gore.velocity.Y += 1f;
                    type = Main.rand.Next(61, 64);
                    smoke = Gore.NewGore(Projectile.GetSource_Death(), source, default, type, 1f);
                    gore = Main.gore[smoke];
                    gore.velocity *= velocityMult;
                    gore.velocity.X -= 1f;
                    gore.velocity.Y += 1f;
                    type = Main.rand.Next(61, 64);
                    smoke = Gore.NewGore(Projectile.GetSource_Death(), source, default, type, 1f);
                    gore = Main.gore[smoke];
                    gore.velocity *= velocityMult;
                    gore.velocity.X += 1f;
                    gore.velocity.Y -= 1f;
                    type = Main.rand.Next(61, 64);
                    smoke = Gore.NewGore(Projectile.GetSource_Death(), source, default, type, 1f);
                    gore = Main.gore[smoke];
                    gore.velocity *= velocityMult;
                    gore.velocity.X -= 1f;
                    gore.velocity.Y -= 1f;
                }
            }
        }
    }
}
