using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.StormWeaver
{
    public class StormWeaverBody : ModNPC
    {
        private int invinceTime = 180;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Storm Weaver");
			NPCID.Sets.TrailingMode[npc.type] = 1;
		}

        public override void SetDefaults()
        {
			npc.GetNPCDamage();
			npc.npcSlots = 5f;
            npc.width = 40;
            npc.height = 40;

			// 10% of HP is phase one
			bool notDoGFight = CalamityWorld.DoGSecondStageCountdown <= 0 || !CalamityWorld.downedSentinel2;
			npc.lifeMax = notDoGFight ? 825500 : 139750;
			npc.LifeMaxNERB(npc.lifeMax, npc.lifeMax, 475000);

			// If fought alone, Storm Weaver plays its own theme
			if (notDoGFight)
                music = CalamityMod.Instance.GetMusicFromMusicMod("Weaver") ?? MusicID.Boss3;
            // If fought as a DoG interlude, keep the DoG music playing
            else
                music = CalamityMod.Instance.GetMusicFromMusicMod("ScourgeofTheUniverse") ?? MusicID.Boss3;

			// Phase one settings
			CalamityGlobalNPC global = npc.Calamity();
			npc.defense = 150;
			global.DR = 0.999999f;
			global.unbreakableDR = true;
			npc.chaseable = false;
			npc.HitSound = SoundID.NPCHit4;
			npc.DeathSound = SoundID.NPCDeath14;

			double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            npc.lifeMax += (int)(npc.lifeMax * HPBoost);
            npc.aiStyle = -1;
            aiType = -1;
            npc.knockBackResist = 0f;
            npc.alpha = 255;
            npc.behindTiles = true;
            npc.noGravity = true;
            npc.boss = true;
            npc.noTileCollide = true;
            npc.canGhostHeal = false;
            npc.netAlways = true;
            npc.dontCountMe = true;

            if (CalamityWorld.malice || BossRushEvent.BossRushActive)
                npc.scale = 1.25f;
            else if (CalamityWorld.death)
                npc.scale = 1.2f;
            else if (CalamityWorld.revenge)
                npc.scale = 1.15f;
            else if (Main.expertMode)
                npc.scale = 1.1f;

			npc.Calamity().VulnerableToElectricity = false;
		}

        public override void SendExtraAI(BinaryWriter writer)
        {
			writer.Write(npc.chaseable);
			writer.Write(invinceTime);
            writer.Write(npc.dontTakeDamage);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
			npc.chaseable = reader.ReadBoolean();
			invinceTime = reader.ReadInt32();
            npc.dontTakeDamage = reader.ReadBoolean();
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        public override void AI()
        {
            if (invinceTime > 0)
            {
                invinceTime--;
                npc.damage = 0;
                npc.dontTakeDamage = true;
            }
            else
            {
                npc.damage = npc.defDamage;
                npc.dontTakeDamage = false;
            }

			if (npc.ai[2] > 0f)
				npc.realLife = (int)npc.ai[2];

			if (npc.life > Main.npc[(int)npc.ai[1]].life)
				npc.life = Main.npc[(int)npc.ai[1]].life;

			// Shed armor
			bool shedArmor = npc.life / (float)npc.lifeMax < 0.9f;

			// Update armored settings to naked settings
			if (shedArmor)
			{
				// Spawn armor gore and set other crucial variables
				if (!npc.chaseable)
				{
					npc.Calamity().VulnerableToHeat = true;
					npc.Calamity().VulnerableToCold = true;
					npc.Calamity().VulnerableToSickness = true;

					Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/SWArmorBody1"), 1f);
					Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/SWArmorBody2"), 1f);
					Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/SWArmorBody3"), 1f);

					CalamityGlobalNPC global = npc.Calamity();
					npc.defense = 30;
					global.DR = 0.2f;
					global.unbreakableDR = false;
					npc.chaseable = true;
					npc.HitSound = SoundID.NPCHit13;
					npc.DeathSound = SoundID.NPCDeath13;
					npc.frame = new Rectangle(0, 0, 54, 52);
				}
			}

			Lighting.AddLight((int)((npc.position.X + (npc.width / 2)) / 16f), (int)((npc.position.Y + (npc.height / 2)) / 16f), 0.2f, 0.05f, 0.2f);

			// Check if other segments are still alive, if not, die
			bool shouldDespawn = true;
			for (int i = 0; i < Main.maxNPCs; i++)
			{
				if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<StormWeaverHead>())
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

			if (Main.npc[(int)npc.ai[1]].alpha < 128)
            {
                if (npc.alpha != 0)
                {
                    for (int num934 = 0; num934 < 2; num934++)
                    {
                        int num935 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 182, 0f, 0f, 100, default, 2f);
                        Main.dust[num935].noGravity = true;
                        Main.dust[num935].noLight = true;
                    }
                }

                npc.alpha -= 42;
                if (npc.alpha < 0)
                    npc.alpha = 0;
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

            float num193 = (float)System.Math.Sqrt(num191 * num191 + num192 * num192);
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

                npc.rotation = (float)System.Math.Atan2(num192, num191) + MathHelper.PiOver2;
                num193 = (float)System.Math.Sqrt(num191 * num191 + num192 * num192);
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

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (npc.spriteDirection == 1)
				spriteEffects = SpriteEffects.FlipHorizontally;

			bool shedArmor = npc.life / (float)npc.lifeMax < 0.9f;
			Texture2D texture2D15 = shedArmor ? ModContent.GetTexture("CalamityMod/NPCs/StormWeaver/StormWeaverBodyNaked") : Main.npcTexture[npc.type];
			Vector2 vector11 = new Vector2(texture2D15.Width / 2, texture2D15.Height / 2);
			Color color36 = Color.White;
			float amount9 = 0.5f;
			int num153 = 5;

			if (CalamityConfig.Instance.Afterimages)
			{
				for (int num155 = 1; num155 < num153; num155 += 2)
				{
					Color color38 = lightColor;

					if (Main.npc[(int)npc.ai[2]].Calamity().newAI[0] > 280f && (CalamityWorld.revenge || BossRushEvent.BossRushActive))
						color38 = Color.Lerp(color38, Color.Cyan, MathHelper.Clamp((Main.npc[(int)npc.ai[2]].Calamity().newAI[0] - 280f) / 120f, 0f, 1f));

					color38 = Color.Lerp(color38, color36, amount9);
					color38 = npc.GetAlpha(color38);
					color38 *= (num153 - num155) / 15f;
					Vector2 vector41 = npc.oldPos[num155] + new Vector2(npc.width, npc.height) / 2f - Main.screenPosition;
					vector41 -= new Vector2(texture2D15.Width, texture2D15.Height) * npc.scale / 2f;
					vector41 += vector11 * npc.scale + new Vector2(0f, npc.gfxOffY);
					spriteBatch.Draw(texture2D15, vector41, npc.frame, color38, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
				}
			}

			Vector2 vector43 = npc.Center - Main.screenPosition;
			vector43 -= new Vector2(texture2D15.Width, texture2D15.Height) * npc.scale / 2f;
			vector43 += vector11 * npc.scale + new Vector2(0f, npc.gfxOffY);
			Color color = npc.GetAlpha(lightColor);

			if (Main.npc[(int)npc.ai[2]].Calamity().newAI[0] > 280f && (CalamityWorld.revenge || BossRushEvent.BossRushActive))
				color = Color.Lerp(color, Color.Cyan, MathHelper.Clamp((Main.npc[(int)npc.ai[2]].Calamity().newAI[0] - 280f) / 120f, 0f, 1f));

			spriteBatch.Draw(texture2D15, vector43, npc.frame, color, npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			return false;
		}

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;
            return true;
        }

		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			int buffDuration = Main.npc[(int)npc.ai[2]].Calamity().newAI[0] >= 400f ? 180 : 90;
			player.AddBuff(BuffID.Electrified, buffDuration, true);
		}

		public override bool CheckActive()
        {
            return false;
        }

        public override bool PreNPCLoot()
        {
            return false;
        }

		public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			if (CalamityLists.projectileDestroyExceptionList.TrueForAll(x => projectile.type != x) && npc.life / (float)npc.lifeMax >= 0.9f)
			{
				if (projectile.penetrate == -1 && !projectile.minion)
					projectile.penetrate = 1;
				else if (projectile.penetrate >= 1)
					projectile.penetrate = 1;
			}
		}

		public override void HitEffect(int hitDirection, double damage)
        {
			for (int k = 0; k < 3; k++)
				Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.PurpleCosmilite, hitDirection, -1f, 0, default, 1f);

			if (npc.life <= 0)
            {
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/SWNudeBody1"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/SWNudeBody2"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/SWNudeBody3"), 1f);

                npc.position.X = npc.position.X + (npc.width / 2);
                npc.position.Y = npc.position.Y + (npc.height / 2);
                npc.width = 30;
                npc.height = 30;
                npc.position.X = npc.position.X - (npc.width / 2);
                npc.position.Y = npc.position.Y - (npc.height / 2);

                for (int num621 = 0; num621 < 20; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }

                for (int num623 = 0; num623 < 40; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
            }
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
            npc.damage = (int)(npc.damage * npc.GetExpertDamageMultiplier());
        }

    }
}
