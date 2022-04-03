using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.NPCs.PlaguebringerGoliath
{
    public class PlagueMine : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plague Mine");
            Main.npcFrameCount[NPC.type] = 4;
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.GetNPCDamage();
            NPC.width = 42;
            NPC.height = 42;
            NPC.defense = 20;
            NPC.lifeMax = BossRushEvent.BossRushActive ? 10000 : 1000;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.canGhostHeal = false;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToElectricity = true;
        }

        public override void AI()
        {
            bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;

            Lighting.AddLight(NPC.Center, 0.03f, 0.2f, 0f);

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
            float timeBeforeExplosion = (malice ? 1000f : death ? 740f : revenge ? 520f : 400f) + NPC.ai[3] * 4f;
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
            float velocity = (malice ? 14f : death ? 12f : revenge ? 10f : 8f) + NPC.ai[3] * 0.04f;
            Vector2 vector167 = new Vector2(NPC.Center.X + (float)(NPC.direction * 20), NPC.Center.Y + 6f);
            float num1373 = player.position.X + (float)player.width * 0.5f - vector167.X;
            float num1374 = player.Center.Y - vector167.Y;
            float num1375 = (float)Math.Sqrt((double)(num1373 * num1373 + num1374 * num1374));
            float num1376 = velocity / num1375;
            num1373 *= num1376;
            num1374 *= num1376;
            float inertia = (malice ? 35f : death ? 40f : revenge ? 45f : 50f) - NPC.ai[3] * 0.25f;
            NPC.velocity.X = (NPC.velocity.X * inertia + num1373) / (inertia + 1f);
            NPC.velocity.Y = (NPC.velocity.Y * inertia + num1374) / (inertia + 1f);
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter % 6 == 5)
                NPC.frame.Y += frameHeight;

            if (NPC.frame.Y / frameHeight >= Main.npcFrameCount[NPC.type])
                NPC.frame.Y = 0;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture2D15 = Main.npcTexture[NPC.type];
            Vector2 vector11 = new Vector2(Main.npcTexture[NPC.type].Width / 2, Main.npcTexture[NPC.type].Height / 2);
            Vector2 vector43 = NPC.Center - Main.screenPosition;
            vector43 -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
            vector43 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY);

            spriteBatch.Draw(texture2D15, vector43, NPC.frame, NPC.GetAlpha(lightColor), NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);

            texture2D15 = ModContent.Request<Texture2D>("CalamityMod/NPCs/PlaguebringerGoliath/PlagueMineGlow");
            Color color37 = Color.Lerp(Color.White, Color.Red, 0.5f);

            spriteBatch.Draw(texture2D15, vector43, NPC.frame, color37, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);

            return false;
        }

        public override bool PreNPCLoot() => false;

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<Plague>(), 240, true);
        }

        public override bool CheckDead()
        {
            SoundEngine.PlaySound(SoundID.Item14, NPC.position);
            NPC.position.X = NPC.position.X + (float)(NPC.width / 2);
            NPC.position.Y = NPC.position.Y + (float)(NPC.height / 2);
            NPC.width = NPC.height = 216;
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
    }
}
