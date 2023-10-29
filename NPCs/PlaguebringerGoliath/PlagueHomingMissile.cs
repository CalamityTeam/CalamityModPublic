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
                float dustXOffset = 0f;
                float dustYOffset = 0f;
                if (Main.rand.NextBool())
                {
                    dustXOffset = NPC.velocity.X * 0.5f;
                    dustYOffset = NPC.velocity.Y * 0.5f;
                }
                int smokyFire = Dust.NewDust(new Vector2(NPC.position.X + 3f + dustXOffset, NPC.position.Y + 3f + dustYOffset) - NPC.velocity * 0.5f, NPC.width - 8, NPC.height - 8, 6, 0f, 0f, 100, default, 0.5f);
                Main.dust[smokyFire].scale *= 2f + (float)Main.rand.Next(10) * 0.1f;
                Main.dust[smokyFire].velocity *= 0.2f;
                Main.dust[smokyFire].noGravity = true;
                smokyFire = Dust.NewDust(new Vector2(NPC.position.X + 3f + dustXOffset, NPC.position.Y + 3f + dustYOffset) - NPC.velocity * 0.5f, NPC.width - 8, NPC.height - 8, 31, 0f, 0f, 100, default, 0.25f);
                Main.dust[smokyFire].fadeIn = 1f + (float)Main.rand.Next(5) * 0.1f;
                Main.dust[smokyFire].velocity *= 0.05f;
            }
            else if (Main.rand.NextBool(4))
            {
                int smokyFire2 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 31, 0f, 0f, 100, default, 0.5f);
                Main.dust[smokyFire2].scale = 0.1f + (float)Main.rand.Next(5) * 0.1f;
                Main.dust[smokyFire2].fadeIn = 1.5f + (float)Main.rand.Next(5) * 0.1f;
                Main.dust[smokyFire2].noGravity = true;
                Main.dust[smokyFire2].position = NPC.Center + new Vector2(0f, (float)(-(float)NPC.height / 2)).RotatedBy((double)NPC.rotation, default) * 1.1f;
                smokyFire2 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 6, 0f, 0f, 100, default, 1f);
                Main.dust[smokyFire2].scale = 1f + (float)Main.rand.Next(5) * 0.1f;
                Main.dust[smokyFire2].noGravity = true;
                Main.dust[smokyFire2].position = NPC.Center + new Vector2(0f, (float)(-(float)NPC.height / 2 - 6)).RotatedBy((double)NPC.rotation, default) * 1.1f;
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
                float npcSpeed = NPC.velocity.Length();
                NPC.velocity = Vector2.Normalize(NPC.velocity) * (npcSpeed + 1f);
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
                    float facingDirection = NPC.velocity.ToRotation();
                    float playerDirection = v3.ToRotation();
                    float angle = playerDirection - facingDirection;
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
            Vector2 halfSizeTexture = new Vector2((float)(TextureAssets.Npc[NPC.type].Value.Width / 2), (float)(TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type] / 2));
            int afterimageAmt = 5;

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int i = 1; i < afterimageAmt; i += 2)
                {
                    Color afterimageColor = drawColor;
                    afterimageColor = Color.Lerp(afterimageColor, Color.White, 0.5f);
                    afterimageColor = NPC.GetAlpha(afterimageColor);
                    afterimageColor *= (float)(afterimageAmt - i) / 15f;
                    Vector2 afterimagePos = NPC.oldPos[i] + new Vector2((float)NPC.width, (float)NPC.height) / 2f - screenPos;
                    afterimagePos -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[NPC.type])) * NPC.scale / 2f;
                    afterimagePos += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture2D15, afterimagePos, NPC.frame, afterimageColor, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);
                }
            }

            Vector2 drawLocation = NPC.Center - screenPos;
            drawLocation -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[NPC.type])) * NPC.scale / 2f;
            drawLocation += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.Draw(texture2D15, drawLocation, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

            texture2D15 = ModContent.Request<Texture2D>("CalamityMod/NPCs/PlaguebringerGoliath/PlagueHomingMissileGlow").Value;
            Color redLerpColor = Color.Lerp(Color.White, Color.Red, 0.5f);

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int j = 1; j < afterimageAmt; j++)
                {
                    Color secondAfterimageColor = redLerpColor;
                    secondAfterimageColor = Color.Lerp(secondAfterimageColor, Color.White, 0.5f);
                    secondAfterimageColor *= (float)(afterimageAmt - j) / 15f;
                    Vector2 secondAfterimagePos = NPC.oldPos[j] + new Vector2((float)NPC.width, (float)NPC.height) / 2f - screenPos;
                    secondAfterimagePos -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[NPC.type])) * NPC.scale / 2f;
                    secondAfterimagePos += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture2D15, secondAfterimagePos, NPC.frame, secondAfterimageColor, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);
                }
            }

            spriteBatch.Draw(texture2D15, drawLocation, NPC.frame, redLerpColor, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

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
            for (int i = 0; i < 15; i++)
            {
                int greenPlague = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 89, 0f, 0f, 100, default, 2f);
                Main.dust[greenPlague].velocity *= 3f;
                if (Main.rand.NextBool())
                {
                    Main.dust[greenPlague].scale = 0.5f;
                    Main.dust[greenPlague].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
                Main.dust[greenPlague].noGravity = true;
            }
            for (int j = 0; j < 30; j++)
            {
                int greenPlague2 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 89, 0f, 0f, 100, default, 3f);
                Main.dust[greenPlague2].noGravity = true;
                Main.dust[greenPlague2].velocity *= 5f;
                greenPlague2 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 89, 0f, 0f, 100, default, 2f);
                Main.dust[greenPlague2].velocity *= 2f;
                Main.dust[greenPlague2].noGravity = true;
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
                target.AddBuff(ModContent.BuffType<Plague>(), 120, true);
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
