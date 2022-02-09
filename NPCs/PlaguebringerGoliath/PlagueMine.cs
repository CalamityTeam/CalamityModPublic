using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.PlaguebringerGoliath
{
    public class PlagueMine : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plague Mine");
            Main.npcFrameCount[npc.type] = 4;
        }

        public override void SetDefaults()
        {
			npc.Calamity().canBreakPlayerDefense = true;
			npc.GetNPCDamage();
            npc.width = 42;
            npc.height = 42;
            npc.defense = 20;
            npc.lifeMax = BossRushEvent.BossRushActive ? 10000 : 1000;
            npc.aiStyle = -1;
            aiType = -1;
            npc.knockBackResist = 0f;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.canGhostHeal = false;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath14;
			npc.Calamity().VulnerableToSickness = false;
			npc.Calamity().VulnerableToElectricity = true;
		}

        public override void AI()
        {
			bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
			bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
			bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;

			Lighting.AddLight(npc.Center, 0.03f, 0.2f, 0f);

			Player player = Main.player[npc.target];
            if (!player.active || player.dead)
            {
                npc.TargetClosest(false);
                player = Main.player[npc.target];
                if (!player.active || player.dead)
                {
                    if (npc.timeLeft > 10)
                        npc.timeLeft = 10;

                    return;
                }
            }
            else if (npc.timeLeft > 600)
                npc.timeLeft = 600;

            Vector2 vector = Main.player[npc.target].Center - npc.Center;
			float distanceRequiredForExplosion = 90f;
			float timeBeforeExplosion = (malice ? 1000f : death ? 740f : revenge ? 520f : 400f) + npc.ai[3] * 4f;
            if (vector.Length() < distanceRequiredForExplosion || npc.ai[0] >= timeBeforeExplosion)
            {
                CheckDead();
                npc.life = 0;
                return;
            }

            npc.ai[0] += 1f;
            if (npc.ai[0] >= timeBeforeExplosion * 0.8f)
            {
                npc.velocity *= 0.98f;
                return;
            }

            npc.TargetClosest(true);
            float velocity = (malice ? 14f : death ? 12f : revenge ? 10f : 8f) + npc.ai[3] * 0.04f;
            Vector2 vector167 = new Vector2(npc.Center.X + (float)(npc.direction * 20), npc.Center.Y + 6f);
            float num1373 = player.position.X + (float)player.width * 0.5f - vector167.X;
            float num1374 = player.Center.Y - vector167.Y;
            float num1375 = (float)Math.Sqrt((double)(num1373 * num1373 + num1374 * num1374));
            float num1376 = velocity / num1375;
            num1373 *= num1376;
            num1374 *= num1376;
			float inertia = (malice ? 35f : death ? 40f : revenge ? 45f : 50f) - npc.ai[3] * 0.25f;
            npc.velocity.X = (npc.velocity.X * inertia + num1373) / (inertia + 1f);
            npc.velocity.Y = (npc.velocity.Y * inertia + num1374) / (inertia + 1f);
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;
            if (npc.frameCounter % 6 == 5)
                npc.frame.Y += frameHeight;

            if (npc.frame.Y / frameHeight >= Main.npcFrameCount[npc.type])
                npc.frame.Y = 0;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (npc.spriteDirection == 1)
				spriteEffects = SpriteEffects.FlipHorizontally;

			Texture2D texture2D15 = Main.npcTexture[npc.type];
			Vector2 vector11 = new Vector2(Main.npcTexture[npc.type].Width / 2, Main.npcTexture[npc.type].Height / 2);
			Vector2 vector43 = npc.Center - Main.screenPosition;
			vector43 -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[npc.type]) * npc.scale / 2f;
			vector43 += vector11 * npc.scale + new Vector2(0f, npc.gfxOffY);

			spriteBatch.Draw(texture2D15, vector43, npc.frame, npc.GetAlpha(lightColor), npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			texture2D15 = ModContent.GetTexture("CalamityMod/NPCs/PlaguebringerGoliath/PlagueMineGlow");
			Color color37 = Color.Lerp(Color.White, Color.Red, 0.5f);

			spriteBatch.Draw(texture2D15, vector43, npc.frame, color37, npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			return false;
		}

		public override bool PreNPCLoot() => false;

		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			player.AddBuff(ModContent.BuffType<Plague>(), 240, true);
		}

		public override bool CheckDead()
        {
            Main.PlaySound(SoundID.Item14, npc.position);
            npc.position.X = npc.position.X + (float)(npc.width / 2);
            npc.position.Y = npc.position.Y + (float)(npc.height / 2);
            npc.width = npc.height = 216;
            npc.position.X = npc.position.X - (float)(npc.width / 2);
            npc.position.Y = npc.position.Y - (float)(npc.height / 2);
            for (int num621 = 0; num621 < 15; num621++)
            {
                int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 89, 0f, 0f, 100, default, 2f);
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
                int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 89, 0f, 0f, 100, default, 3f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 5f;
                num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 89, 0f, 0f, 100, default, 2f);
                Main.dust[num624].velocity *= 2f;
                Main.dust[num624].noGravity = true;
            }
            return true;
        }
    }
}
