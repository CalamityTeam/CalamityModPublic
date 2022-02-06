using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.AdultEidolonWyrm
{
    public class EidolonWyrmBodyAltHuge : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Adult Eidolon Wyrm");
        }

        public override void SetDefaults()
        {
            npc.damage = 100;
            npc.width = 60;
            npc.height = 88;
            npc.defense = 0;
			npc.LifeMaxNERB(2012500, 2415000);
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

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(npc.localAI[0]);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			npc.localAI[0] = reader.ReadSingle();
		}

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => false;

        public override void AI()
        {
            npc.damage = 0;

			// Difficulty modes
			bool malice = CalamityWorld.malice;
			bool death = CalamityWorld.death;
			bool revenge = CalamityWorld.revenge;
			bool expertMode = Main.expertMode;

			if (npc.ai[2] > 0f)
                npc.realLife = (int)npc.ai[2];

			// Check if other segments are still alive, if not, die
			bool shouldDespawn = true;
			for (int i = 0; i < Main.maxNPCs; i++)
			{
				if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<EidolonWyrmHeadHuge>())
				{
					shouldDespawn = false;
					break;
				}
			}
			if (!shouldDespawn)
			{
				if (npc.ai[1] <= 0f)
					shouldDespawn = true;
				else if (Main.npc[(int)npc.ai[1]].life <= 0)
					shouldDespawn = true;
			}
			if (shouldDespawn)
			{
				npc.life = 0;
				npc.HitEffect(0, 10.0);
				npc.checkDead();
				npc.active = false;
			}

			CalamityGlobalNPC calamityGlobalNPC_Head = Main.npc[(int)npc.ai[2]].Calamity();

			float chargePhaseGateValue = malice ? 120f : death ? 180f : revenge ? 210f : expertMode ? 240f : 300f;
			float lightningChargePhaseGateValue = malice ? 90f : death ? 120f : revenge ? 135f : expertMode ? 150f : 180f;

			bool invisiblePartOfChargePhase = calamityGlobalNPC_Head.newAI[2] >= chargePhaseGateValue && calamityGlobalNPC_Head.newAI[2] <= chargePhaseGateValue + 1f && (calamityGlobalNPC_Head.newAI[0] == (float)EidolonWyrmHeadHuge.Phase.ChargeOne || calamityGlobalNPC_Head.newAI[0] == (float)EidolonWyrmHeadHuge.Phase.ChargeTwo || calamityGlobalNPC_Head.newAI[0] == (float)EidolonWyrmHeadHuge.Phase.FastCharge);
			bool invisiblePartOfLightningChargePhase = calamityGlobalNPC_Head.newAI[2] >= lightningChargePhaseGateValue && calamityGlobalNPC_Head.newAI[2] <= lightningChargePhaseGateValue + 1f && calamityGlobalNPC_Head.newAI[0] == (float)EidolonWyrmHeadHuge.Phase.LightningCharge;
			bool invisiblePhase = calamityGlobalNPC_Head.newAI[0] == 1f || calamityGlobalNPC_Head.newAI[0] == 5f || calamityGlobalNPC_Head.newAI[0] == 7f;
			if (!invisiblePartOfChargePhase && !invisiblePartOfLightningChargePhase && !invisiblePhase)
			{
				if (Main.npc[(int)npc.ai[1]].Opacity > 0.5f)
				{
					npc.Opacity += 0.2f;
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

			bool shootShadowFireballs = (calamityGlobalNPC_Head.newAI[0] == (float)EidolonWyrmHeadHuge.Phase.ShadowFireballSpin && calamityGlobalNPC_Head.newAI[2] > 0f) ||
				(calamityGlobalNPC_Head.newAI[0] == (float)EidolonWyrmHeadHuge.Phase.FinalPhase && calamityGlobalNPC_Head.newAI[1] > 0f);
			if (shootShadowFireballs && Main.netMode != NetmodeID.MultiplayerClient)
			{
				if (Vector2.Distance(npc.Center, Main.player[Main.npc[(int)npc.ai[2]].target].Center) > 160f)
				{
					npc.localAI[0] += 1f;
					float shootShadowFireballGateValue = malice ? 60f : death ? 70f : revenge ? 75f : expertMode ? 80f : 90f;
					float divisor = 2f;
					if (npc.ai[3] % divisor == 0f && npc.localAI[0] >= shootShadowFireballGateValue)
					{
						npc.localAI[0] = 0f;
						float distanceVelocityBoost = MathHelper.Clamp((Vector2.Distance(Main.npc[(int)npc.ai[2]].Center, Main.player[Main.npc[(int)npc.ai[2]].target].Center) - 1600f) * 0.025f, 0f, 16f);
						float fireballVelocity = (Main.player[Main.npc[(int)npc.ai[2]].target].Calamity().ZoneAbyssLayer4 ? 6f : 8f) + distanceVelocityBoost;
						Vector2 destination = Main.player[Main.npc[(int)npc.ai[2]].target].Center - npc.Center;
						Vector2 velocity = Vector2.Normalize(destination) * fireballVelocity;
						int type = ProjectileID.CultistBossFireBallClone;
						int damage = npc.GetProjectileDamage(type);
						int proj = Projectile.NewProjectile(npc.Center, velocity, type, damage, 0f, Main.myPlayer);
						Main.projectile[proj].tileCollide = false;
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
            vector -= new Vector2(ModContent.GetTexture("CalamityMod/NPCs/AdultEidolonWyrm/EidolonWyrmBodyAltGlowHuge").Width, ModContent.GetTexture("CalamityMod/NPCs/AdultEidolonWyrm/EidolonWyrmBodyAltGlowHuge").Height) * 0.5f;
            vector += vector11 * 1f + new Vector2(0f, 4f + npc.gfxOffY);
            Color color = new Color(127, 127, 127, 0).MultiplyRGBA(Color.LightYellow) * npc.Opacity;
            Main.spriteBatch.Draw(ModContent.GetTexture("CalamityMod/NPCs/AdultEidolonWyrm/EidolonWyrmBodyAltGlowHuge"), vector,
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

                Gore.NewGore(npc.Center, npc.velocity, mod.GetGoreSlot("Gores/WyrmAdult3"), 1f);
            }
        }
    }
}
