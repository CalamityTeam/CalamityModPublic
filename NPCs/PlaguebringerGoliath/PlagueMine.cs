using System;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.PlaguebringerGoliath
{
    public class PlagueMine : ModNPC
    {
        public static Asset<Texture2D> GlowTexture;
        public override void SetStaticDefaults()
        {
            this.HideFromBestiary();
            Main.npcFrameCount[Type] = 4;
            if (!Main.dedServ)
            {
                GlowTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/PlaguebringerGoliath/PlagueMineGlow", AssetRequestMode.AsyncLoad);
            }
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.damage = 70; // 140
            NPC.width = 42;
            NPC.height = 42;
            NPC.defense = 20;
            NPC.lifeMax = BossRushEvent.BossRushActive ? 10000 : 1000;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0.2f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToElectricity = true;
        }

        public override void AI()
        {
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;

            Lighting.AddLight(NPC.Center, 0.03f, 0.2f, 0f);

            NPC.rotation = NPC.velocity.X * 0.04f;

            Player player = Main.player[NPC.target];
            if (!player.active || player.dead)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (!player.active || player.dead)
                {
                    if (NPC.timeLeft > 10)
                        NPC.timeLeft = 10;

                    return;
                }
            }
            else if (NPC.timeLeft > 600)
                NPC.timeLeft = 600;

            Vector2 vector = Main.player[NPC.target].Center - NPC.Center;
            float distanceRequiredForExplosion = 90f;
            float timeBeforeExplosion = (death ? 740f : revenge ? 520f : 400f) + NPC.ai[3] * 4f;
            if (vector.Length() < distanceRequiredForExplosion || NPC.ai[0] >= timeBeforeExplosion)
            {
                CheckDead();
                NPC.life = 0;
                return;
            }

            NPC.ai[0] += 1f;
            if (NPC.ai[0] >= timeBeforeExplosion * 0.8f)
            {
                NPC.velocity *= 0.98f;
                return;
            }

            NPC.TargetClosest(true);
            float velocity = (death ? 12f : revenge ? 10f : 8f) + NPC.ai[3] * 0.04f;
            Vector2 npcDirection = new Vector2(NPC.Center.X + (float)(NPC.direction * 20), NPC.Center.Y + 6f);
            float targetX = player.position.X + (float)player.width * 0.5f - npcDirection.X;
            float targetY = player.Center.Y - npcDirection.Y;
            float targetDistance = (float)Math.Sqrt((double)(targetX * targetX + targetY * targetY));
            float npcSpeed = velocity / targetDistance;
            targetX *= npcSpeed;
            targetY *= npcSpeed;
            float inertia = (death ? 40f : revenge ? 45f : 50f) - NPC.ai[3] * 0.25f;
            NPC.velocity.X = (NPC.velocity.X * inertia + targetX) / (inertia + 1f);
            NPC.velocity.Y = (NPC.velocity.Y * inertia + targetY) / (inertia + 1f);
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter % 6 == 5)
                NPC.frame.Y += frameHeight;

            if (NPC.frame.Y / frameHeight >= Main.npcFrameCount[Type])
                NPC.frame.Y = 0;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture = TextureAssets.Npc[Type].Value;
            Vector2 halfSizeTexture = NPC.frame.Size() / 2f;
            Vector2 drawLocation = NPC.Center - screenPos;
            drawLocation -= new Vector2(texture.Width, texture.Height / Main.npcFrameCount[Type]) * NPC.scale / 2f;
            drawLocation += halfSizeTexture * NPC.scale + Vector2.UnitY * NPC.gfxOffY;

            Color backAfterimageColor = PlaguebringerGoliath.BackglowColor * NPC.Opacity;
            for (int i = 0; i < 10; i++)
            {
                Vector2 drawOffset = (MathHelper.TwoPi * i / 10f).ToRotationVector2() * 4f;
                spriteBatch.Draw(texture, drawLocation + drawOffset, NPC.frame, backAfterimageColor, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);
            }

            spriteBatch.Draw(texture, drawLocation, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

            texture = GlowTexture.Value;
            Color redLerpColor = Color.Lerp(Color.White, Color.Red, 0.5f);

            spriteBatch.Draw(texture, drawLocation, NPC.frame, redLerpColor, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

            return false;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
            {
                if (Main.zenithWorld) // it is the plague, you get very sick.
                {
                    target.AddBuff(ModContent.BuffType<SulphuricPoisoning>(), 300);
                    target.AddBuff(BuffID.Poisoned, 300);
                    target.AddBuff(BuffID.Venom, 300);
                }
                target.AddBuff(ModContent.BuffType<Plague>(), 180);
            }
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
                int greenPlague = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GemEmerald, 0f, 0f, 100, default, 2f);
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
                int greenPlague2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GemEmerald, 0f, 0f, 100, default, 3f);
                Main.dust[greenPlague2].noGravity = true;
                Main.dust[greenPlague2].velocity *= 5f;
                greenPlague2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GemEmerald, 0f, 0f, 100, default, 2f);
                Main.dust[greenPlague2].velocity *= 2f;
                Main.dust[greenPlague2].noGravity = true;
            }
            return true;
        }
    }
}
