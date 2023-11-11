using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class DankCreeperMinion : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.minionSlots = 1;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.timeLeft *= 5;
            Projectile.minion = true;
            Projectile.tileCollide = false;
            Projectile.MaxUpdates = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = Projectile.MaxUpdates * 30;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (Projectile.localAI[0] == 0f)
            {
                int constant = 36;
                for (int i = 0; i < constant; i++)
                {
                    Vector2 rotate = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.75f;
                    rotate = rotate.RotatedBy((double)((float)(i - (constant / 2 - 1)) * 6.28318548f / (float)constant), default) + Projectile.Center;
                    Vector2 faceDirection = rotate - Projectile.Center;
                    int dust = Dust.NewDust(rotate + faceDirection, 0, 0, 14, faceDirection.X * 1.5f, faceDirection.Y * 1.5f, 100, default, 1.4f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].noLight = true;
                    Main.dust[dust].velocity = faceDirection;
                }
                Projectile.localAI[0] += 1f;
            }
            bool isMinion = Projectile.type == ModContent.ProjectileType<DankCreeperMinion>();
            player.AddBuff(ModContent.BuffType<DankCreeperBuff>(), 3600);
            if (isMinion)
            {
                if (player.dead)
                {
                    modPlayer.dCreeper = false;
                }
                if (modPlayer.dCreeper)
                {
                    Projectile.timeLeft = 2;
                }
            }
            Projectile.MinionAntiClump();
            float projX = Projectile.position.X;
            float projY = Projectile.position.Y;
            float attackRange = 1300f;
            bool canAttack = false;
            int separationAnxietyDist = 1100;
            if (Projectile.ai[1] != 0f)
            {
                separationAnxietyDist = 1800;
            }
            if (Math.Abs(Projectile.Center.X - Main.player[Projectile.owner].Center.X) + Math.Abs(Projectile.Center.Y - Main.player[Projectile.owner].Center.Y) > (float)separationAnxietyDist)
            {
                Projectile.ai[0] = 1f;
            }
            if (Projectile.ai[0] == 0f)
            {
                if (player.HasMinionAttackTargetNPC)
                {
                    NPC npc = Main.npc[player.MinionAttackTargetNPC];
                    if (npc.CanBeChasedBy(Projectile, false))
                    {
                        float npcX = npc.position.X + (float)(npc.width / 2);
                        float npcY = npc.position.Y + (float)(npc.height / 2);
                        float npcDist = Math.Abs(Projectile.position.X + (float)(Projectile.width / 2) - npcX) + Math.Abs(Projectile.position.Y + (float)(Projectile.height / 2) - npcY);
                        if (npcDist < attackRange && Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height))
                        {
                            projX = npcX;
                            projY = npcY;
                            canAttack = true;
                        }
                    }
                }
                if (!canAttack)
                {
                    for (int j = 0; j < Main.maxNPCs; j++)
                    {
                        if (Main.npc[j].CanBeChasedBy(Projectile, false))
                        {
                            float otherNPCX = Main.npc[j].position.X + (float)(Main.npc[j].width / 2);
                            float otherNPCY = Main.npc[j].position.Y + (float)(Main.npc[j].height / 2);
                            float otherNPCDist = Math.Abs(Projectile.position.X + (float)(Projectile.width / 2) - otherNPCX) + Math.Abs(Projectile.position.Y + (float)(Projectile.height / 2) - otherNPCY);
                            if (otherNPCDist < attackRange && Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, Main.npc[j].position, Main.npc[j].width, Main.npc[j].height))
                            {
                                attackRange = otherNPCDist;
                                projX = otherNPCX;
                                projY = otherNPCY;
                                canAttack = true;
                            }
                        }
                    }
                }
            }
            if (!canAttack)
            {
                float returnSpeed = 8f;
                if (Projectile.ai[0] == 1f)
                {
                    returnSpeed = 12f;
                }
                Vector2 playerDirection = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
                float playerXDist = player.Center.X - playerDirection.X;
                float playerYDist = player.Center.Y - playerDirection.Y - 60f;
                float playerDist = (float)Math.Sqrt((double)(playerXDist * playerXDist + playerYDist * playerYDist));
                if (playerDist < 100f && Projectile.ai[0] == 1f && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
                {
                    Projectile.ai[0] = 0f;
                }
                if (playerDist > 2000f)
                {
                    Projectile.position.X = player.Center.X - (float)(Projectile.width / 2);
                    Projectile.position.Y = player.Center.Y - (float)(Projectile.width / 2);
                }
                if (playerDist > 70f)
                {
                    playerDist = returnSpeed / playerDist;
                    playerXDist *= playerDist;
                    playerYDist *= playerDist;
                    Projectile.velocity.X = (Projectile.velocity.X * 20f + playerXDist) / 21f;
                    Projectile.velocity.Y = (Projectile.velocity.Y * 20f + playerYDist) / 21f;
                }
                else
                {
                    if (Projectile.velocity.X == 0f && Projectile.velocity.Y == 0f)
                    {
                        Projectile.velocity.X = -0.15f;
                        Projectile.velocity.Y = -0.05f;
                    }
                    Projectile.velocity *= 1.01f;
                }
                Projectile.rotation = Projectile.velocity.X * 0.05f;
                if ((double)Math.Abs(Projectile.velocity.X) > 0.2)
                {
                    Projectile.spriteDirection = -Projectile.direction;
                    return;
                }
            }
            else
            {
                if (Projectile.ai[1] == -1f)
                {
                    Projectile.ai[1] = 11f;
                }
                if (Projectile.ai[1] > 0f)
                {
                    Projectile.ai[1] -= 1f;
                }
                if (Projectile.ai[1] == 0f)
                {
                    float hoverSpeed = 8f; //12
                    Vector2 playerDirectionAgain = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
                    float playerXDistAgain = projX - playerDirectionAgain.X;
                    float playerYDistAgain = projY - playerDirectionAgain.Y;
                    float playerDistAgain = (float)Math.Sqrt((double)(playerXDistAgain * playerXDistAgain + playerYDistAgain * playerYDistAgain));
                    if (playerDistAgain < 100f)
                    {
                        hoverSpeed = 10f; //14
                    }
                    playerDistAgain = hoverSpeed / playerDistAgain;
                    playerXDistAgain *= playerDistAgain;
                    playerYDistAgain *= playerDistAgain;
                    Projectile.velocity.X = (Projectile.velocity.X * 40f + playerXDistAgain) / 41f;
                    Projectile.velocity.Y = (Projectile.velocity.Y * 40f + playerYDistAgain) / 41f;
                }
                else
                {
                    if (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y) < 10f)
                    {
                        Projectile.velocity *= 1.05f;
                    }
                }
                Projectile.rotation = Projectile.velocity.X * 0.05f;
                if ((double)Math.Abs(Projectile.velocity.X) > 0.2)
                {
                    Projectile.spriteDirection = -Projectile.direction;
                    return;
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.ai[1] = -1f;
                Projectile.netUpdate = true;
            }

            target.AddBuff(ModContent.BuffType<BrainRot>(), 90);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
