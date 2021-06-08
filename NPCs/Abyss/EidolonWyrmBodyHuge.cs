using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Abyss
{
    public class EidolonWyrmBodyHuge : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eidolon Wyrm");
        }

        public override void SetDefaults()
        {
            npc.damage = 100;
            npc.width = 60;
            npc.height = 88;
            npc.defense = 0;
			npc.LifeMaxNERB(1000000, 1150000);
			double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
			npc.lifeMax += (int)(npc.lifeMax * HPBoost);
			npc.aiStyle = -1;
            aiType = -1;
            npc.knockBackResist = 0f;
            npc.Opacity = 0f;
            npc.behindTiles = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath6;
            npc.netAlways = true;
            npc.dontCountMe = true;
            npc.dontTakeDamage = true;
            npc.chaseable = false;
        }

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => false;

        public override void AI()
        {
            npc.damage = 0;

            if (npc.ai[2] > 0f)
                npc.realLife = (int)npc.ai[2];

            bool flag = false;
            if (npc.ai[1] <= 0f)
            {
                flag = true;
            }
            else if (Main.npc[(int)npc.ai[1]].life <= 0)
            {
                flag = true;
            }
            if (flag)
            {
                npc.life = 0;
                npc.HitEffect(0, 10.0);
                npc.checkDead();
            }

            if (!NPC.AnyNPCs(ModContent.NPCType<EidolonWyrmHeadHuge>()))
                npc.active = false;

			bool invisiblePhase = Main.npc[(int)npc.ai[2]].Calamity().newAI[0] == 1f || Main.npc[(int)npc.ai[2]].Calamity().newAI[0] == 5f || Main.npc[(int)npc.ai[2]].Calamity().newAI[0] == 7f;
			if (!invisiblePhase)
			{
				if (Main.npc[(int)npc.ai[1]].Opacity > 0.5f)
				{
					npc.Opacity += 0.15f;
					if (npc.Opacity > 1f)
						npc.Opacity = 1f;
				}
			}
			else
			{
				npc.Opacity -= 0.05f;
				if (npc.Opacity < 0f)
					npc.Opacity = 0f;
			}

			bool shootShadowFireballs = (Main.npc[(int)npc.ai[2]].Calamity().newAI[0] == 6f && Main.npc[(int)npc.ai[2]].Calamity().newAI[2] > 0f) ||
				(Main.npc[(int)npc.ai[2]].Calamity().newAI[0] == 10f && Main.npc[(int)npc.ai[2]].Calamity().newAI[1] > 0f);
			if (shootShadowFireballs && Main.netMode != NetmodeID.MultiplayerClient)
			{
				if (Vector2.Distance(npc.Center, Main.player[Main.npc[(int)npc.ai[2]].target].Center) > 320f)
				{
					npc.ai[3] += 1f;
					if (npc.ai[3] >= 90f)
					{
						float fireballVelocity = 4f;
						Vector2 destination = Main.player[Main.npc[(int)npc.ai[2]].target].Center - npc.Center;
						Vector2 velocity = Vector2.Normalize(destination) * fireballVelocity;
						int type = ProjectileID.CultistBossIceMist;
						int damage = npc.GetProjectileDamage(type);
						Projectile.NewProjectile(npc.Center, velocity, type, damage, 0f, Main.myPlayer);
					}
				}
			}

			Vector2 vector18 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
            float num191 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2);
            float num192 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2);
            num191 = (int)(num191 / 16f) * 16;
            num192 = (int)(num192 / 16f) * 16;
            vector18.X = (int)(vector18.X / 16f) * 16;
            vector18.Y = (int)(vector18.Y / 16f) * 16;
            num191 -= vector18.X;
            num192 -= vector18.Y;

            float num193 = (float)Math.Sqrt(num191 * num191 + num192 * num192);
            if (npc.ai[1] > 0f && npc.ai[1] < Main.npc.Length)
            {
                try
                {
                    vector18 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                    num191 = Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2) - vector18.X;
                    num192 = Main.npc[(int)npc.ai[1]].position.Y + (Main.npc[(int)npc.ai[1]].height / 2) - vector18.Y;
                } catch
                {
                }

                npc.rotation = (float)Math.Atan2(num192, num191) + MathHelper.PiOver2;
                num193 = (float)Math.Sqrt(num191 * num191 + num192 * num192);
                int num194 = npc.width;
                num193 = (num193 - num194) / num193;
                num191 *= num193;
                num192 *= num193;
                npc.velocity = Vector2.Zero;
                npc.position.X = npc.position.X + num191;
                npc.position.Y = npc.position.Y + num192;

                if (num191 < 0f)
                    npc.spriteDirection = -1;
                else if (num191 > 0f)
                    npc.spriteDirection = 1;
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            SpriteEffects spriteEffects = npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Vector2 center = new Vector2(npc.Center.X, npc.Center.Y);
            Vector2 vector11 = new Vector2(Main.npcTexture[npc.type].Width / 2, Main.npcTexture[npc.type].Height / 2);
            Vector2 vector = center - Main.screenPosition;
            vector -= new Vector2(ModContent.GetTexture("CalamityMod/NPCs/Abyss/EidolonWyrmBodyGlowHuge").Width, ModContent.GetTexture("CalamityMod/NPCs/Abyss/EidolonWyrmBodyGlowHuge").Height) * 0.5f;
            vector += vector11 * 1f + new Vector2(0f, 0f + 4f + npc.gfxOffY);
            Color color = new Color(127, 127, 127, 0).MultiplyRGBA(Color.LightYellow) * npc.Opacity;
            Main.spriteBatch.Draw(ModContent.GetTexture("CalamityMod/NPCs/Abyss/EidolonWyrmBodyGlowHuge"), vector,
                new Microsoft.Xna.Framework.Rectangle?(npc.frame), color, npc.rotation, vector11, 1f, spriteEffects, 0f);
        }

		public override bool CheckActive() => false;

		public override bool PreNPCLoot() => false;

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                for (int k = 0; k < 10; k++)
                    Dust.NewDust(npc.position, npc.width, npc.height, 4, hitDirection, -1f, 0, default, 1f);

                Gore.NewGore(npc.Center, npc.velocity, mod.GetGoreSlot("Gores/WyrmAdult2"), 1f);
            }
        }
    }
}
