using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.NPCs.PlaguebringerGoliath
{
    public class PlagueHomingMissile : ModNPC
    {
        public override void SetStaticDefaults()
        {
            this.HideFromBestiary();
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.GetNPCDamage();
            NPC.width = 22;
            NPC.height = 22;
            NPC.defense = 20;
            NPC.lifeMax = BossRushEvent.BossRushActive ? 5000 : 500;
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.noGravity = true;
            NPC.canGhostHeal = false;
            NPC.noTileCollide = true;
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToElectricity = true;
        }

        public override void AI()
        {
            bool bossRush = BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || bossRush;
            bool revenge = CalamityWorld.revenge || bossRush;
            bool expertMode = Main.expertMode || bossRush;

            Lighting.AddLight(NPC.Center, 0.015f, 0.1f, 0f);

            if (Math.Abs(NPC.velocity.X) >= 3f || Math.Abs(NPC.velocity.Y) >= 3f)
            {
                float num247 = 0f;
                float num248 = 0f;
                if (Main.rand.NextBool())
                {
                    num247 = NPC.velocity.X * 0.5f;
                    num248 = NPC.velocity.Y * 0.5f;
                }
                int num249 = Dust.NewDust(new Vector2(NPC.position.X + 3f + num247, NPC.position.Y + 3f + num248) - NPC.velocity * 0.5f, NPC.width - 8, NPC.height - 8, 6, 0f, 0f, 100, default, 0.5f);
                Main.dust[num249].scale *= 2f + (float)Main.rand.Next(10) * 0.1f;
                Main.dust[num249].velocity *= 0.2f;
                Main.dust[num249].noGravity = true;
                num249 = Dust.NewDust(new Vector2(NPC.position.X + 3f + num247, NPC.position.Y + 3f + num248) - NPC.velocity * 0.5f, NPC.width - 8, NPC.height - 8, 31, 0f, 0f, 100, default, 0.25f);
                Main.dust[num249].fadeIn = 1f + (float)Main.rand.Next(5) * 0.1f;
                Main.dust[num249].velocity *= 0.05f;
            }
            else if (Main.rand.NextBool(4))
            {
                int num252 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 31, 0f, 0f, 100, default, 0.5f);
                Main.dust[num252].scale = 0.1f + (float)Main.rand.Next(5) * 0.1f;
                Main.dust[num252].fadeIn = 1.5f + (float)Main.rand.Next(5) * 0.1f;
                Main.dust[num252].noGravity = true;
                Main.dust[num252].position = NPC.Center + new Vector2(0f, (float)(-(float)NPC.height / 2)).RotatedBy((double)NPC.rotation, default) * 1.1f;
                num252 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 6, 0f, 0f, 100, default, 1f);
                Main.dust[num252].scale = 1f + (float)Main.rand.Next(5) * 0.1f;
                Main.dust[num252].noGravity = true;
                Main.dust[num252].position = NPC.Center + new Vector2(0f, (float)(-(float)NPC.height / 2 - 6)).RotatedBy((double)NPC.rotation, default) * 1.1f;
            }

            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;

            float timeBeforeHoming = 120f - NPC.ai[3] * 0.5f;
            if (NPC.ai[2] < timeBeforeHoming)
            {
                NPC.ai[2] += 1f;
                return;
            }

            if (NPC.ai[0] == 0f && NPC.ai[1] == 0f)
            {
                NPC.ai[0] = 1f;
                NPC.ai[1] = (float)Player.FindClosest(NPC.position, NPC.width, NPC.height);
                NPC.netUpdate = true;
                float num754 = NPC.velocity.Length();
                NPC.velocity = Vector2.Normalize(NPC.velocity) * (num754 + 1f);
            }
            else if (NPC.ai[0] == 1f)
            {
                NPC.localAI[1] += 1f;
                float timeBeforeExploding = 480f + NPC.ai[3] * 2f;
                float homingDuration = (bossRush ? 430f : death ? 340f : revenge ? 290f : expertMode ? 240f : 150f) + NPC.ai[3] * 2f;
                if (NPC.localAI[1] == timeBeforeExploding)
                {
                    CheckDead();
                    NPC.life = 0;
                    return;
                }

                if (NPC.localAI[1] < homingDuration)
                {
                    NPC.noTileCollide = true;
                    Vector2 v3 = Main.player[(int)NPC.ai[1]].Center - NPC.Center;
                    float num760 = NPC.velocity.ToRotation();
                    float num761 = v3.ToRotation();
                    float angle = num761 - num760;
                    angle = MathHelper.WrapAngle(angle);
                    NPC.velocity = NPC.velocity.RotatedBy(angle * 0.2);
                }
                else
                {
                    NPC.noTileCollide = false;

                    if (NPC.velocity.Length() < (bossRush ? 25f : 20f))
                        NPC.velocity *= 1.01f;

                    if (Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                    {
                        CheckDead();
                        NPC.life = 0;
                        return;
                    }
                }
            }

            float distanceBeforeExploding = 42f;
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (player.active && !player.dead && Vector2.Distance(player.Center, NPC.Center) <= distanceBeforeExploding)
                {
                    CheckDead();
                    NPC.life = 0;
                    return;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture2D15 = TextureAssets.Npc[NPC.type].Value;
            Vector2 vector11 = new Vector2((float)(TextureAssets.Npc[NPC.type].Value.Width / 2), (float)(TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type] / 2));
            Color color36 = Color.White;
            float amount9 = 0.5f;
            int num153 = 5;

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int num155 = 1; num155 < num153; num155 += 2)
                {
                    Color color38 = drawColor;
                    color38 = Color.Lerp(color38, color36, amount9);
                    color38 = NPC.GetAlpha(color38);
                    color38 *= (float)(num153 - num155) / 15f;
                    Vector2 vector41 = NPC.oldPos[num155] + new Vector2((float)NPC.width, (float)NPC.height) / 2f - screenPos;
                    vector41 -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[NPC.type])) * NPC.scale / 2f;
                    vector41 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture2D15, vector41, NPC.frame, color38, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);
                }
            }

            Vector2 vector43 = NPC.Center - screenPos;
            vector43 -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[NPC.type])) * NPC.scale / 2f;
            vector43 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.Draw(texture2D15, vector43, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);

            texture2D15 = ModContent.Request<Texture2D>("CalamityMod/NPCs/PlaguebringerGoliath/PlagueHomingMissileGlow").Value;
            Color color37 = Color.Lerp(Color.White, Color.Red, 0.5f);

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int num163 = 1; num163 < num153; num163++)
                {
                    Color color41 = color37;
                    color41 = Color.Lerp(color41, color36, amount9);
                    color41 *= (float)(num153 - num163) / 15f;
                    Vector2 vector44 = NPC.oldPos[num163] + new Vector2((float)NPC.width, (float)NPC.height) / 2f - screenPos;
                    vector44 -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[NPC.type])) * NPC.scale / 2f;
                    vector44 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture2D15, vector44, NPC.frame, color41, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);
                }
            }

            spriteBatch.Draw(texture2D15, vector43, NPC.frame, color37, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);

            return false;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.15f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override bool CheckDead()
        {
            SoundEngine.PlaySound(SoundID.Item14, NPC.Center);
            NPC.position.X = NPC.position.X + (float)(NPC.width / 2);
            NPC.position.Y = NPC.position.Y + (float)(NPC.height / 2);
            NPC.width = NPC.height = Main.zenithWorld ? 300 : 216;
            NPC.position.X = NPC.position.X - (float)(NPC.width / 2);
            NPC.position.Y = NPC.position.Y - (float)(NPC.height / 2);
            for (int num621 = 0; num621 < 15; num621++)
            {
                int num622 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 89, 0f, 0f, 100, default, 2f);
                Main.dust[num622].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
                Main.dust[num622].noGravity = true;
            }
            for (int num623 = 0; num623 < 30; num623++)
            {
                int num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 89, 0f, 0f, 100, default, 3f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 5f;
                num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 89, 0f, 0f, 100, default, 2f);
                Main.dust[num624].velocity *= 2f;
                Main.dust[num624].noGravity = true;
            }
            return true;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
            {
                if (Main.zenithWorld) // it is the plague, you get very sick.
                {
                    target.AddBuff(ModContent.BuffType<SulphuricPoisoning>(), 240, true);
                    target.AddBuff(BuffID.Poisoned, 240, true);
                    target.AddBuff(BuffID.Venom, 240, true);
                }
                target.AddBuff(ModContent.BuffType<Plague>(), 240, true);
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Plague, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 10; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Plague, hit.HitDirection, -1f, 0, default, 1f);
                }
            }
        }
    }
}
