using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.SummonItems;
using CalamityMod.Items.Weapons.Typeless;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables.Furniture.Trophies;

namespace CalamityMod.NPCs.ProfanedGuardians
{
    [AutoloadBossHead]
    public class ProfanedGuardianBoss : ModNPC
    {
        private int spearType = 0;
        private int dustTimer = 3;
        private int immuneTimer = 300;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Profaned Guardian");
            Main.npcFrameCount[npc.type] = 6;
			NPCID.Sets.TrailingMode[npc.type] = 1;
		}

        public override void SetDefaults()
        {
            npc.npcSlots = 20f;
            npc.aiStyle = -1;
            npc.damage = 140;
            npc.width = 100;
            npc.height = 80;
            npc.defense = 50;
			npc.DR_NERD(0.4f);
            npc.LifeMaxNERB(102500, 112500, 1650000);
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            npc.lifeMax += (int)(npc.lifeMax * HPBoost);
            npc.knockBackResist = 0f;
            npc.noGravity = true;
            npc.noTileCollide = true;
            aiType = -1;
            npc.boss = true;
            Mod calamityModMusic = ModLoader.GetMod("CalamityModMusic");
            if (calamityModMusic != null)
                music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/Guardians");
            else
                music = MusicID.Boss1;
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
            npc.buffImmune[BuffID.Ichor] = false;
            npc.buffImmune[BuffID.CursedInferno] = false;
			npc.buffImmune[BuffID.StardustMinionBleed] = false;
			npc.buffImmune[BuffID.Oiled] = false;
            npc.buffImmune[BuffID.BetsysCurse] = false;
            npc.buffImmune[ModContent.BuffType<AstralInfectionDebuff>()] = false;
            npc.buffImmune[ModContent.BuffType<AbyssalFlames>()] = false;
            npc.buffImmune[ModContent.BuffType<ArmorCrunch>()] = false;
            npc.buffImmune[ModContent.BuffType<DemonFlames>()] = false;
            npc.buffImmune[ModContent.BuffType<GodSlayerInferno>()] = false;
            npc.buffImmune[ModContent.BuffType<Nightwither>()] = false;
            npc.buffImmune[ModContent.BuffType<Shred>()] = false;
            npc.buffImmune[ModContent.BuffType<WarCleave>()] = false;
            npc.buffImmune[ModContent.BuffType<WhisperingDeath>()] = false;
            npc.buffImmune[ModContent.BuffType<SilvaStun>()] = false;
            npc.value = Item.buyPrice(0, 25, 0, 0);
            npc.HitSound = SoundID.NPCHit52;
            npc.DeathSound = SoundID.NPCDeath55;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(spearType);
            writer.Write(dustTimer);
            writer.Write(immuneTimer);
            writer.Write(npc.dontTakeDamage);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            spearType = reader.ReadInt32();
            dustTimer = reader.ReadInt32();
            immuneTimer = reader.ReadInt32();
            npc.dontTakeDamage = reader.ReadBoolean();
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 0.15f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }

        public override void AI()
        {
            CalamityGlobalNPC.doughnutBoss = npc.whoAmI;

            // Percent life remaining
            float lifeRatio = npc.life / (float)npc.lifeMax;

			Vector2 vectorCenter = npc.Center;
			if (Main.netMode != NetmodeID.MultiplayerClient && npc.localAI[1] == 0f)
            {
                npc.localAI[1] = 1f;
                NPC.NewNPC((int)vectorCenter.X, (int)vectorCenter.Y, ModContent.NPCType<ProfanedGuardianBoss2>());
                NPC.NewNPC((int)vectorCenter.X, (int)vectorCenter.Y, ModContent.NPCType<ProfanedGuardianBoss3>());
            }
            
            npc.TargetClosest(false);
			Player player = Main.player[npc.target];
			if (!Main.dayTime || !player.active || player.dead)
            {
                npc.TargetClosest(false);
                player = Main.player[npc.target];
                if (!Main.dayTime || !player.active || player.dead)
                {
					if (npc.velocity.Y > 3f)
						npc.velocity.Y = 3f;
					npc.velocity.Y -= 0.1f;
					if (npc.velocity.Y < -12f)
						npc.velocity.Y = -12f;

					if (npc.timeLeft > 60)
                        npc.timeLeft = 60;

					if (npc.ai[0] != 0f)
					{
						npc.ai[0] = 0f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						npc.ai[3] = 0f;
						npc.netUpdate = true;
					}
					return;
                }
            }
            else if (npc.timeLeft < 1800)
                npc.timeLeft = 1800;

			bool isHoly = player.ZoneHoly;
			bool isHell = player.ZoneUnderworldHeight;
			bool expertMode = Main.expertMode || CalamityWorld.bossRushActive;
			bool revenge = CalamityWorld.revenge || CalamityWorld.bossRushActive;
			bool death = CalamityWorld.death || CalamityWorld.bossRushActive;

            // Become immune over time if target isn't in hell or hallow
            if (!isHoly && !isHell && !CalamityWorld.bossRushActive)
            {
                if (immuneTimer > 0)
                    immuneTimer--;
            }
            else
                immuneTimer = 300;

            // Take damage or not
            npc.dontTakeDamage = immuneTimer <= 0;

			bool flag100 = false;
            for (int num569 = 0; num569 < Main.maxNPCs; num569++)
            {
                if ((Main.npc[num569].active && Main.npc[num569].type == ModContent.NPCType<ProfanedGuardianBoss2>()) || (Main.npc[num569].active && Main.npc[num569].type == ModContent.NPCType<ProfanedGuardianBoss3>()))
                {
                    flag100 = true;
                }
            }
            if (flag100)
            {
                npc.dontTakeDamage = true;
            }
            else
            {
                npc.dontTakeDamage = false;
            }
            if (Math.Sign(npc.velocity.X) != 0)
            {
                npc.spriteDirection = -Math.Sign(npc.velocity.X);
            }
            npc.spriteDirection = Math.Sign(npc.velocity.X);
            float num1000 = (lifeRatio < 0.75f || death) ? 14f : 16f;
            if (revenge)
            {
                num1000 *= 1.15f;
            }
            float num1006 = 0.111111117f * num1000;
            int num1009 = (npc.ai[0] == 2f) ? 2 : 1;
            int num1010 = (npc.ai[0] == 2f) ? 80 : 60;
            for (int num1011 = 0; num1011 < 2; num1011++)
            {
                if (Main.rand.Next(3) < num1009)
                {
                    int num1012 = Dust.NewDust(vectorCenter - new Vector2(num1010), num1010 * 2, num1010 * 2, (int)CalamityDusts.ProfanedFire, npc.velocity.X * 0.5f, npc.velocity.Y * 0.5f, 90, default, 1.5f);
                    Main.dust[num1012].noGravity = true;
                    Main.dust[num1012].velocity *= 0.2f;
                    Main.dust[num1012].fadeIn = 1f;
                }
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
				float shootBoost = 2f * (1f - lifeRatio);
                npc.localAI[0] += 1f + shootBoost;
                if (npc.localAI[0] >= (CalamityWorld.bossRushActive ? 210f : 240f))
                {
                    npc.localAI[0] = 0f;

                    Main.PlaySound(SoundID.Item20, npc.position);

                    int totalProjectiles = 10;
					float velocity = 5f;
					int damage = expertMode ? 50 : 60;
					int projectileType = ModContent.ProjectileType<ProfanedSpear>();

                    switch (spearType)
                    {
                        case 0:
							break;
                        case 1:
                            totalProjectiles = 12;
                            velocity = 5.5f;
                            break;
                        case 2:
                            totalProjectiles = 8;
                            velocity = 6f;
                            break;
                        default:
                            break;
                    }

					float radians = MathHelper.TwoPi / totalProjectiles;
					for (int i = 0; i < totalProjectiles; i++)
					{
						Vector2 vector255 = new Vector2(0f, -velocity).RotatedBy(radians * i);
						Projectile.NewProjectile(npc.Center, vector255, projectileType, damage, 0f, Main.myPlayer, 0f, 0f);
					}

					spearType++;
                    if (spearType > 2)
                        spearType = 0;
                }
            }
            if (npc.ai[0] == 0f)
            {
                float scaleFactor6 = death ? 16f : 14f;
                Vector2 center5 = player.Center;
                Vector2 vector126 = center5 - vectorCenter;
                Vector2 vector127 = vector126 - Vector2.UnitY * 300f;
                float num1013 = vector126.Length();
                vector126 = Vector2.Normalize(vector126) * scaleFactor6;
                vector127 = Vector2.Normalize(vector127) * scaleFactor6;
                bool flag64 = Collision.CanHit(vectorCenter, 1, 1, player.Center, 1, 1);
                if (npc.ai[3] >= 120f)
                {
                    flag64 = true;
                }
                float num1014 = 8f;
                flag64 = flag64 && vector126.ToRotation() > MathHelper.Pi / num1014 && vector126.ToRotation() < MathHelper.Pi - MathHelper.Pi / num1014;
                if (num1013 > 1200f || !flag64)
                {
                    npc.velocity.X = (npc.velocity.X * (num1000 - 1f) + vector127.X) / num1000;
                    npc.velocity.Y = (npc.velocity.Y * (num1000 - 1f) + vector127.Y) / num1000;
                    if (!flag64)
                    {
                        npc.ai[3] += 1f;
                        if (npc.ai[3] == 120f)
                        {
                            npc.netUpdate = true;
                        }
                    }
                    else
                    {
                        npc.ai[3] = 0f;
                    }
                }
                else
                {
                    npc.ai[0] = 1f;
                    npc.ai[2] = vector126.X;
                    npc.ai[3] = vector126.Y;
                    npc.netUpdate = true;
                }
            }
            else if (npc.ai[0] == 1f)
            {
                npc.velocity *= 0.8f;
                npc.ai[1] += 1f;
                if (npc.ai[1] >= 5f)
                {
                    npc.ai[0] = 2f;
                    npc.ai[1] = 0f;
                    npc.netUpdate = true;
                    Vector2 velocity = new Vector2(npc.ai[2], npc.ai[3]);
                    velocity.Normalize();
                    velocity *= 10f;
                    npc.velocity = velocity;
                }
            }
            else if (npc.ai[0] == 2f)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    dustTimer--;
                    if ((lifeRatio < 0.5f || death) && dustTimer <= 0)
                    {
                        Main.PlaySound(SoundID.Item20, npc.position);
                        int damage = expertMode ? 55 : 70;
                        Vector2 vector173 = Vector2.Normalize(player.Center - vectorCenter) * (npc.width + 20) / 2f + vectorCenter;
						int projectile = Projectile.NewProjectile((int)vector173.X, (int)vector173.Y, npc.direction * 2, 4f, ModContent.ProjectileType<FlareDust>(), damage, 0f, Main.myPlayer, 2f, 0f);
                        Main.projectile[projectile].timeLeft = 120;
                        Main.projectile[projectile].velocity.X = 0f;
                        Main.projectile[projectile].velocity.Y = 0f;
                        dustTimer = 3;
                    }
                }
                npc.ai[1] += 1f;
                bool flag65 = vectorCenter.Y + 50f > player.Center.Y;
                if ((npc.ai[1] >= 90f && flag65) || npc.velocity.Length() < 8f)
                {
                    npc.ai[0] = 3f;
                    npc.ai[1] = 45f;
                    npc.ai[2] = 0f;
                    npc.ai[3] = 0f;
                    npc.velocity /= 2f;
                    npc.netUpdate = true;
                }
                else
                {
                    Vector2 center7 = player.Center;
                    Vector2 vec2 = center7 - vectorCenter;
                    vec2.Normalize();
                    if (vec2.HasNaNs())
                    {
                        vec2 = new Vector2(npc.direction, 0f);
                    }
                    npc.velocity = (npc.velocity * (num1000 - 1f) + vec2 * (npc.velocity.Length() + num1006)) / num1000;
                }
            }
            else if (npc.ai[0] == 3f)
            {
                npc.ai[1] -= 1f;
                if (npc.ai[1] <= 0f)
                {
                    npc.ai[0] = 0f;
                    npc.ai[1] = 0f;
                    npc.netUpdate = true;
                }
                npc.velocity *= 0.98f;
            }
        }

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (npc.spriteDirection == 1)
				spriteEffects = SpriteEffects.FlipHorizontally;

			Texture2D texture2D15 = Main.npcTexture[npc.type];
			Vector2 vector11 = new Vector2(Main.npcTexture[npc.type].Width / 2, Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type] / 2);
			Color color36 = Color.White;
			float amount9 = 0.5f;
			int num153 = 5;
			if (npc.ai[0] == 2f)
				num153 = 10;

			if (CalamityConfig.Instance.Afterimages)
			{
				for (int num155 = 1; num155 < num153; num155 += 2)
				{
					Color color38 = lightColor;
					color38 = Color.Lerp(color38, color36, amount9);
					color38 = npc.GetAlpha(color38);
					color38 *= (num153 - num155) / 15f;
					Vector2 vector41 = npc.oldPos[num155] + new Vector2(npc.width, npc.height) / 2f - Main.screenPosition;
					vector41 -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[npc.type]) * npc.scale / 2f;
					vector41 += vector11 * npc.scale + new Vector2(0f, 4f + npc.gfxOffY);
					spriteBatch.Draw(texture2D15, vector41, npc.frame, color38, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
				}
			}

			Vector2 vector43 = npc.Center - Main.screenPosition;
			vector43 -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[npc.type]) * npc.scale / 2f;
			vector43 += vector11 * npc.scale + new Vector2(0f, 4f + npc.gfxOffY);
			spriteBatch.Draw(texture2D15, vector43, npc.frame, npc.GetAlpha(lightColor), npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			texture2D15 = ModContent.GetTexture("CalamityMod/NPCs/ProfanedGuardians/ProfanedGuardianBossGlow");
			Color color37 = Color.Lerp(Color.White, Color.Yellow, 0.5f);

			if (CalamityConfig.Instance.Afterimages)
			{
				for (int num163 = 1; num163 < num153; num163++)
				{
					Color color41 = color37;
					color41 = Color.Lerp(color41, color36, amount9);
					color41 = npc.GetAlpha(color41);
					color41 *= (num153 - num163) / 15f;
					Vector2 vector44 = npc.oldPos[num163] + new Vector2(npc.width, npc.height) / 2f - Main.screenPosition;
					vector44 -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[npc.type]) * npc.scale / 2f;
					vector44 += vector11 * npc.scale + new Vector2(0f, 4f + npc.gfxOffY);
					spriteBatch.Draw(texture2D15, vector44, npc.frame, color41, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
				}
			}

			spriteBatch.Draw(texture2D15, vector43, npc.frame, color37, npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			return false;
		}

		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;
            return true;
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            name = "A Profaned Guardian";
            potionType = ItemID.GreaterHealingPotion;
        }

        public override void NPCLoot()
        {
            // Profaned Guardians have no actual drops and no treasure bag
            DropHelper.DropItemChance(npc, ModContent.ItemType<ProfanedGuardianMask>(), 7);
            DropHelper.DropItemChance(npc, ModContent.ItemType<ProfanedGuardianTrophy>(), 10);
            DropHelper.DropItemChance(npc, ModContent.ItemType<RelicOfDeliverance>(), 4);
            DropHelper.DropItem(npc, ModContent.ItemType<ProfanedCore>());
            DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeProfanedGuardians>(), true, !CalamityWorld.downedGuardians);
            DropHelper.DropResidentEvilAmmo(npc, CalamityWorld.downedGuardians, 5, 2, 1);

			CalamityGlobalTownNPC.SetNewShopVariable(new int[] { NPCID.Wizard }, CalamityWorld.downedGuardians);

			// Mark the Profaned Guardians as dead
			CalamityWorld.downedGuardians = true;
            CalamityMod.UpdateServerBoolean();
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<HolyFlames>(), 300, true);
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
            npc.damage = (int)(npc.damage * 0.8f);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.ProfanedFire, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ProfanedGuardianBossGores/ProfanedGuardianBossA"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ProfanedGuardianBossGores/ProfanedGuardianBossA2"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ProfanedGuardianBossGores/ProfanedGuardianBossA3"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ProfanedGuardianBossGores/ProfanedGuardianBossA4"), 1f);
                for (int k = 0; k < 50; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.ProfanedFire, hitDirection, -1f, 0, default, 1f);
                }
            }
        }
    }
}
