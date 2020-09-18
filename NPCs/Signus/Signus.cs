using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.Signus
{
    [AutoloadBossHead]
    public class Signus : ModNPC
    {
        private int phaseSwitch = 0;
        private int chargeSwitch = 0;
        private int dustTimer = 3;
        private int spawnX = 750;
        private int spawnY = 120;
        private int lifeToAlpha = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Signus, Envoy of the Devourer");
            Main.npcFrameCount[npc.type] = 6;
			NPCID.Sets.TrailingMode[npc.type] = 1;
		}

        public override void SetDefaults()
        {
            npc.npcSlots = 32f;
			npc.GetNPCDamage();
			npc.width = 130;
            npc.height = 130;
            npc.defense = 70;
            Mod calamityModMusic = ModLoader.GetMod("CalamityModMusic");
            if (calamityModMusic != null)
                music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/ScourgeofTheUniverse");
            else
                music = MusicID.Boss4;
			bool notDoGFight = CalamityWorld.DoGSecondStageCountdown <= 0 || !CalamityWorld.downedSentinel3;
			npc.LifeMaxNERB(notDoGFight ? 280000 : 70000, notDoGFight ? 445500 : 109500, 2400000);
            if (notDoGFight)
            {
                npc.value = Item.buyPrice(0, 35, 0, 0);
                if (calamityModMusic != null)
                    music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/Signus");
                else
                    music = MusicID.Boss4;
            }
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            npc.lifeMax += (int)(npc.lifeMax * HPBoost);
            npc.knockBackResist = 0f;
            npc.aiStyle = -1;
            aiType = -1;
            npc.boss = true;
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
            npc.buffImmune[ModContent.BuffType<ExoFreeze>()] = false;
            npc.buffImmune[ModContent.BuffType<AbyssalFlames>()] = false;
            npc.buffImmune[ModContent.BuffType<ArmorCrunch>()] = false;
            npc.buffImmune[ModContent.BuffType<DemonFlames>()] = false;
            npc.buffImmune[ModContent.BuffType<GodSlayerInferno>()] = false;
            npc.buffImmune[ModContent.BuffType<Nightwither>()] = false;
            npc.buffImmune[ModContent.BuffType<Shred>()] = false;
            npc.buffImmune[ModContent.BuffType<WarCleave>()] = false;
            npc.buffImmune[ModContent.BuffType<WhisperingDeath>()] = false;
            npc.buffImmune[ModContent.BuffType<SilvaStun>()] = false;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.netAlways = true;
            npc.HitSound = SoundID.NPCHit49;
            npc.DeathSound = SoundID.NPCDeath51;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(phaseSwitch);
            writer.Write(chargeSwitch);
            writer.Write(dustTimer);
            writer.Write(spawnX);
            writer.Write(spawnY);
            writer.Write(lifeToAlpha);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            phaseSwitch = reader.ReadInt32();
            chargeSwitch = reader.ReadInt32();
            dustTimer = reader.ReadInt32();
            spawnX = reader.ReadInt32();
            spawnY = reader.ReadInt32();
            lifeToAlpha = reader.ReadInt32();
        }

        public override void AI()
        {
			CalamityGlobalNPC.signus = npc.whoAmI;

			bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
			bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;

			Vector2 vectorCenter = npc.Center;

			double lifeRatio = npc.life / (double)npc.lifeMax;
            lifeToAlpha = (int)(100.0 * (1.0 - lifeRatio));

            bool phase2 = (lifeRatio < 0.75f && expertMode) || death;
            bool phase3 = lifeRatio < 0.5f || death;

			// Get a target
			if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead || !Main.player[npc.target].active)
				npc.TargetClosest(true);

			Player player = Main.player[npc.target];

			if (!player.active || player.dead || Vector2.Distance(player.Center, vectorCenter) > 6400f)
			{
				npc.TargetClosest(false);
				player = Main.player[npc.target];
				if (!player.active || player.dead || Vector2.Distance(player.Center, vectorCenter) > 6400f)
				{
					if (npc.timeLeft < 10)
					{
						CalamityWorld.DoGSecondStageCountdown = 0;
						if (Main.netMode == NetmodeID.Server)
						{
							var netMessage = mod.GetPacket();
							netMessage.Write((byte)CalamityModMessageType.DoGCountdownSync);
							netMessage.Write(CalamityWorld.DoGSecondStageCountdown);
							netMessage.Send();
						}
					}

					npc.rotation = npc.velocity.X * 0.04f;

					if (npc.velocity.Y > 3f)
						npc.velocity.Y = 3f;
					npc.velocity.Y -= 0.15f;
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
						phaseSwitch = 0;
						chargeSwitch = 0;
						spawnY = 120;
						npc.netUpdate = true;
					}
					return;
				}
			}
			else if (npc.timeLeft < 1800)
				npc.timeLeft = 1800;

			float num1000 = expertMode ? 11f : 13f;
			float num1006 = 0.111111117f * num1000;

            if (lifeToAlpha < 50 && npc.ai[0] != 1f)
            {
                for (int num1011 = 0; num1011 < 2; num1011++)
                {
                    if (Main.rand.Next(3) < 1)
                    {
                        int num1012 = Dust.NewDust(vectorCenter - new Vector2(70f), 70 * 2, 70 * 2, (int)CalamityDusts.PurpleCosmolite, npc.velocity.X * 0.5f, npc.velocity.Y * 0.5f, 90, default, 1.5f);
                        Main.dust[num1012].noGravity = true;
                        Main.dust[num1012].velocity *= 0.2f;
                        Main.dust[num1012].fadeIn = 1f;
                    }
                }
            }

            if (npc.ai[0] <= 2f)
            {
                npc.rotation = npc.velocity.X * 0.04f;
				float playerLocation = vectorCenter.X - player.Center.X;
				npc.direction = playerLocation < 0f ? 1 : -1;
				npc.spriteDirection = npc.direction;

				npc.knockBackResist = 0.05f;
                if (expertMode)
                    npc.knockBackResist *= Main.expertKnockBack;
                if (phase3 || revenge)
                    npc.knockBackResist = 0f;

                float speed = expertMode ? 14f : 12f;
                if (expertMode)
                    speed += death ? 4f : 4f * (float)(1D - lifeRatio);

                if (npc.Calamity().enraged > 0 || (CalamityConfig.Instance.BossRushXerocCurse && BossRushEvent.BossRushActive))
                    speed += 3f;

                float num795 = player.Center.X - vectorCenter.X;
                float num796 = player.Center.Y - vectorCenter.Y;
                float num797 = (float)Math.Sqrt(num795 * num795 + num796 * num796);
                num797 = speed / num797;
                num795 *= num797;
                num796 *= num797;
                npc.velocity.X = (npc.velocity.X * 50f + num795) / 51f;
                npc.velocity.Y = (npc.velocity.Y * 50f + num796) / 51f;
            }
            else
            {
                npc.knockBackResist = 0f;
            }
            if (npc.ai[0] == 0f)
            {
				if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.localAI[1] += 1f;

					if (expertMode)
						npc.localAI[1] += death ? 2f : 2f * (float)(1D - lifeRatio);

                    if (npc.localAI[1] >= 120f)
                    {
                        npc.localAI[1] = 0f;
                        npc.TargetClosest(true);
                        int num1249 = 0;
                        int num1250;
                        int num1251;
                        while (true)
                        {
                            num1249++;
                            num1250 = (int)player.Center.X / 16;
                            num1251 = (int)player.Center.Y / 16;

                            int min = 14;
                            int max = 18;

                            if (Main.rand.NextBool(2))
                                num1250 += Main.rand.Next(min, max);
                            else
                                num1250 -= Main.rand.Next(min, max);

                            if (Main.rand.NextBool(2))
                                num1251 += Main.rand.Next(min, max);
                            else
                                num1251 -= Main.rand.Next(min, max);

                            if (!WorldGen.SolidTile(num1250, num1251))
                                break;

                            if (num1249 > 100)
                                return;
                        }
                        npc.ai[0] = 1f;
                        npc.ai[1] = num1250;
                        npc.ai[2] = num1251;
                        npc.netUpdate = true;
                        return;
                    }
                }
            }
            else if (npc.ai[0] == 1f)
            {
                Vector2 position = new Vector2(npc.ai[1] * 16f - (npc.width / 2), npc.ai[2] * 16f - (npc.height / 2));
                for (int m = 0; m < 5; m++)
                {
                    int dust = Dust.NewDust(position, npc.width, npc.height, (int)CalamityDusts.PurpleCosmolite, 0f, 0f, 90, default, 2f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].fadeIn = 1f;
                }

                npc.alpha += 2;
				if (expertMode)
					npc.alpha += death ? 2 : (int)(3D * (1D - lifeRatio));

                if (npc.alpha >= 255)
                {
                    Main.PlaySound(SoundID.Item8, vectorCenter);
                    npc.alpha = 255;
                    npc.position = position;
                    for (int n = 0; n < 15; n++)
                    {
                        int num39 = Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.PurpleCosmolite, 0f, 0f, 90, default, 3f);
                        Main.dust[num39].noGravity = true;
                    }
                    npc.ai[0] = 2f;
                    npc.netUpdate = true;
                }
            }
            else if (npc.ai[0] == 2f)
            {
                npc.alpha -= 50;
                if (npc.alpha <= lifeToAlpha)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient && revenge)
                    {
						Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 122);
						int num660 = NPC.NewNPC((int)(player.position.X + 750f), (int)player.position.Y, ModContent.NPCType<SignusBomb>(), 0, 0f, 0f, 0f, 0f, 255);
                        if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, num660, 0f, 0f, 0f, 0, 0, 0);
                        }
                        int num661 = NPC.NewNPC((int)(player.position.X - 750f), (int)player.position.Y, ModContent.NPCType<SignusBomb>(), 0, 0f, 0f, 0f, 0f, 255);
                        if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, num661, 0f, 0f, 0f, 0, 0, 0);
                        }
                        for (int num621 = 0; num621 < 5; num621++)
                        {
                            int num622 = Dust.NewDust(new Vector2(player.position.X + 750f, player.position.Y), npc.width, npc.height, (int)CalamityDusts.PurpleCosmolite, 0f, 0f, 100, default, 2f);
                            Main.dust[num622].velocity *= 3f;
                            Main.dust[num622].noGravity = true;
                            if (Main.rand.NextBool(2))
                            {
                                Main.dust[num622].scale = 0.5f;
                                Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                            }
                            int num623 = Dust.NewDust(new Vector2(player.position.X - 750f, player.position.Y), npc.width, npc.height, (int)CalamityDusts.PurpleCosmolite, 0f, 0f, 100, default, 2f);
                            Main.dust[num623].velocity *= 3f;
                            Main.dust[num623].noGravity = true;
                            if (Main.rand.NextBool(2))
                            {
                                Main.dust[num623].scale = 0.5f;
                                Main.dust[num623].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                            }
                        }
                        for (int num623 = 0; num623 < 20; num623++)
                        {
                            int num624 = Dust.NewDust(new Vector2(player.position.X + 750f, player.position.Y), npc.width, npc.height, (int)CalamityDusts.PurpleCosmolite, 0f, 0f, 100, default, 3f);
                            Main.dust[num624].noGravity = true;
                            Main.dust[num624].velocity *= 5f;
                            num624 = Dust.NewDust(new Vector2(player.position.X + 750f, player.position.Y), npc.width, npc.height, (int)CalamityDusts.PurpleCosmolite, 0f, 0f, 100, default, 2f);
                            Main.dust[num624].velocity *= 2f;
                            int num625 = Dust.NewDust(new Vector2(player.position.X - 750f, player.position.Y), npc.width, npc.height, (int)CalamityDusts.PurpleCosmolite, 0f, 0f, 100, default, 3f);
                            Main.dust[num625].noGravity = true;
                            Main.dust[num625].velocity *= 5f;
                            num625 = Dust.NewDust(new Vector2(player.position.X - 750f, player.position.Y), npc.width, npc.height, (int)CalamityDusts.PurpleCosmolite, 0f, 0f, 100, default, 2f);
                            Main.dust[num625].velocity *= 2f;
                        }
                    }
                    npc.ai[3] += 1f;
                    npc.alpha = lifeToAlpha;
                    if (npc.ai[3] >= 3f)
                    {
						npc.ai[0] = 3f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                        npc.ai[3] = 0f;
                    }
                    else
                        npc.ai[0] = 0f;

                    npc.netUpdate = true;
                }
            }
            else if (npc.ai[0] == 3f)
            {
                npc.rotation = npc.velocity.X * 0.04f;
				float playerLocation = vectorCenter.X - player.Center.X;
				npc.direction = playerLocation < 0f ? 1 : -1;
				npc.spriteDirection = npc.direction;
				Vector2 vector121 = new Vector2(npc.position.X + (npc.width / 2), npc.position.Y + (npc.height / 2));

                npc.ai[1] += 1f;
				float divisor = expertMode ? (death ? 55f : revenge ? 60f : 65f) - (float)Math.Ceiling(30D * (1D - lifeRatio)) : 70f;
                
                if (npc.ai[1] % divisor == divisor - 1f)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        float num1070 = 15f;
                        if (npc.Calamity().enraged > 0 || (CalamityConfig.Instance.BossRushXerocCurse && BossRushEvent.BossRushActive))
                        {
                            num1070 += 3f;
                        }
                        float num1071 = player.position.X + player.width * 0.5f - vector121.X;
                        float num1072 = player.position.Y + player.height * 0.5f - vector121.Y;
                        float num1073 = (float)Math.Sqrt(num1071 * num1071 + num1072 * num1072);
                        num1073 = num1070 / num1073;
                        num1071 *= num1073;
                        num1072 *= num1073;
                        int num1074 = expertMode ? 48 : 60;
                        int num1075 = ModContent.ProjectileType<SignusScythe>();
                        Projectile.NewProjectile(vector121.X, vector121.Y, num1071, num1072, num1075, num1074, 0f, Main.myPlayer, 0f, npc.target + 1);
                    }
                }
                if (npc.position.Y > player.position.Y - 200f) //200
                {
                    if (npc.velocity.Y > 0f)
                    {
                        npc.velocity.Y *= 0.975f;
                    }
                    npc.velocity.Y -= BossRushEvent.BossRushActive ? 0.15f : 0.1f;
                    if (npc.velocity.Y > 3f)
                    {
                        npc.velocity.Y = 3f;
                    }
                }
                else if (npc.position.Y < player.position.Y - 400f) //500
                {
                    if (npc.velocity.Y < 0f)
                    {
                        npc.velocity.Y *= 0.975f;
                    }
                    npc.velocity.Y += BossRushEvent.BossRushActive ? 0.15f : 0.1f;
                    if (npc.velocity.Y < -3f)
                    {
                        npc.velocity.Y = -3f;
                    }
                }
                if (npc.position.X + (npc.width / 2) > player.position.X + (player.width / 2) + 500f) //100
                {
                    if (npc.velocity.X > 0f)
                    {
                        npc.velocity.X *= 0.98f;
                    }
                    npc.velocity.X -= BossRushEvent.BossRushActive ? 0.15f : 0.1f;
                    if (npc.velocity.X > 8f)
                    {
                        npc.velocity.X = 8f;
                    }
                }
                if (npc.position.X + (npc.width / 2) < player.position.X + (player.width / 2) - 500f) //100
                {
                    if (npc.velocity.X < 0f)
                    {
                        npc.velocity.X *= 0.98f;
                    }
                    npc.velocity.X += BossRushEvent.BossRushActive ? 0.15f : 0.1f;
                    if (npc.velocity.X < -8f)
                    {
                        npc.velocity.X = -8f;
                    }
                }
                if (npc.ai[1] >= divisor * 5f)
                {
                    npc.ai[0] = 4f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] = 0f;
                    npc.netUpdate = true;
                }
            }
            else if (npc.ai[0] == 4f)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (NPC.CountNPCS(ModContent.NPCType<CosmicLantern>()) < 5)
                    {
                        for (int x = 0; x < 5; x++)
                        {
                            int num660 = NPC.NewNPC((int)(player.position.X + spawnX), (int)(player.position.Y + spawnY), ModContent.NPCType<CosmicLantern>(), 0, 0f, 0f, 0f, 0f, 255);
                            if (Main.netMode == NetmodeID.Server)
                            {
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, num660, 0f, 0f, 0f, 0, 0, 0);
                            }
                            int num661 = NPC.NewNPC((int)(player.position.X - spawnX), (int)(player.position.Y + spawnY), ModContent.NPCType<CosmicLantern>(), 0, 0f, 0f, 0f, 0f, 255);
                            if (Main.netMode == NetmodeID.Server)
                            {
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, num661, 0f, 0f, 0f, 0, 0, 0);
                            }
                            spawnY -= 60;
                        }
                        spawnY = 120;
                    }
                }
                npc.TargetClosest(false);
                npc.rotation = npc.velocity.ToRotation();
                if (Math.Sign(npc.velocity.X) != 0)
                {
                    npc.spriteDirection = -Math.Sign(npc.velocity.X);
                }
                if (npc.rotation < -1.57079637f)
                {
                    npc.rotation += 3.14159274f;
                }
                if (npc.rotation > 1.57079637f)
                {
                    npc.rotation -= 3.14159274f;
                }
                npc.spriteDirection = Math.Sign(npc.velocity.X);
                if (chargeSwitch == 0) //line up the charge
                {
                    float scaleFactor6 = 14f;
					if (expertMode)
						scaleFactor6 += death ? 6f : 6f * (float)(1D - lifeRatio);

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
                    flag64 = flag64 && vector126.ToRotation() > 3.14159274f / num1014 && vector126.ToRotation() < 3.14159274f - 3.14159274f / num1014;
                    if (num1013 > 1400f || !flag64)
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
                        chargeSwitch = 1;
                        npc.ai[2] = vector126.X;
                        npc.ai[3] = vector126.Y;
                        npc.netUpdate = true;
                    }
                }
                else if (chargeSwitch == 1) //pause before charge
                {
					npc.velocity *= 0.8f;
                    npc.ai[1] += 1f;
                    if (npc.ai[1] >= 5f)
                    {
                        chargeSwitch = 2;
                        npc.ai[1] = 0f;
                        npc.netUpdate = true;
                        Vector2 velocity = new Vector2(npc.ai[2], npc.ai[3]);
                        velocity.Normalize();
                        velocity *= 10f;
                        npc.velocity = velocity;
                    }
                }
                else if (chargeSwitch == 2) //charging
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        dustTimer--;
                        if (phase2 && dustTimer <= 0)
                        {
                            Main.PlaySound(SoundID.Item73, npc.position);
                            int damage = expertMode ? 60 : 70;
                            int projectile = Projectile.NewProjectile((int)vectorCenter.X, (int)vectorCenter.Y, 0f, 0f, ModContent.ProjectileType<EssenceDust>(), damage, 0f, Main.myPlayer, 0f, 0f);
                            dustTimer = 3;
                        }
                    }
                    npc.ai[1] += 1f;
					bool flag65 = vectorCenter.Y + 50f > player.Center.Y;
					if ((npc.ai[1] >= 90f && flag65) || npc.velocity.Length() < 8f)
                    {
						chargeSwitch = 3;
                        npc.ai[1] = 30f;
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
                else if (chargeSwitch == 3) //slow down after charging and reset
                {
                    npc.ai[1] -= 1f;
                    if (npc.ai[1] <= 0f)
                    {
						phaseSwitch += 1;
						if (phaseSwitch >= 3)
						{
							npc.ai[0] = 0f;
							npc.ai[1] = 0f;
							npc.ai[2] = 0f;
							npc.ai[3] = 0f;
							chargeSwitch = 0;
							phaseSwitch = 0;
							npc.netUpdate = true;
						}
						else
						{
							chargeSwitch = 0;
							npc.ai[1] = 0f;
							npc.netUpdate = true;
						}
                    }
                    npc.velocity *= 0.97f;
                }
            }
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;
            return true;
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 1.0;
            if (npc.ai[0] == 4f)
            {
                if (npc.frameCounter > 72.0) //12
                {
                    npc.frameCounter = 0.0;
                }
            }
            else
            {
                int frameY = 196;
                if (npc.frameCounter > 72.0)
                {
                    npc.frameCounter = 0.0;
                }
                npc.frame.Y = frameY * (int)(npc.frameCounter / 12.0); //1 to 6
                if (npc.frame.Y >= frameHeight * 6)
                {
                    npc.frame.Y = 0;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D NPCTexture = Main.npcTexture[npc.type];
			Texture2D glowMaskTexture = ModContent.GetTexture("CalamityMod/NPCs/Signus/SignusGlow");

			SpriteEffects spriteEffects = SpriteEffects.None;
			if (npc.spriteDirection == 1)
				spriteEffects = SpriteEffects.FlipHorizontally;

			int num153 = 5;
			Rectangle frame = npc.frame;
			int frameCount = Main.npcFrameCount[npc.type];

			if (npc.ai[0] == 4f)
			{
				NPCTexture = ModContent.GetTexture("CalamityMod/NPCs/Signus/SignusAlt2");
				glowMaskTexture = ModContent.GetTexture("CalamityMod/NPCs/Signus/SignusAlt2Glow");
				num153 = 10;
				int frameY = 94 * (int)(npc.frameCounter / 12.0);
				if (frameY >= 94 * 6)
					frameY = 0;
				frame = new Rectangle(0, frameY, NPCTexture.Width, NPCTexture.Height / frameCount);
			}
			else if (npc.ai[0] == 3f)
			{
				NPCTexture = ModContent.GetTexture("CalamityMod/NPCs/Signus/SignusAlt");
				glowMaskTexture = ModContent.GetTexture("CalamityMod/NPCs/Signus/SignusAltGlow");
				num153 = 7;
			}
			else
			{
				NPCTexture = Main.npcTexture[npc.type];
				glowMaskTexture = ModContent.GetTexture("CalamityMod/NPCs/Signus/SignusGlow");
			}

			Vector2 vector11 = new Vector2(NPCTexture.Width / 2, NPCTexture.Height / frameCount / 2);
			Color color36 = Color.White;
			float amount9 = 0.5f;
			float scale = npc.scale;
			float rotation = npc.rotation;
			float offsetY = npc.gfxOffY;

			if (CalamityConfig.Instance.Afterimages)
			{
				for (int num155 = 1; num155 < num153; num155 += 2)
				{
					Color color38 = drawColor;
					color38 = Color.Lerp(color38, color36, amount9);
					color38 = npc.GetAlpha(color38);
					color38 *= (num153 - num155) / 15f;
					Vector2 vector41 = npc.oldPos[num155] + new Vector2(npc.width, npc.height) / 2f - Main.screenPosition;
					vector41 -= new Vector2(NPCTexture.Width, NPCTexture.Height / frameCount) * scale / 2f;
					vector41 += vector11 * scale + new Vector2(0f, 4f + offsetY);
					spriteBatch.Draw(NPCTexture, vector41, new Rectangle?(frame), color38, rotation, vector11, scale, spriteEffects, 0f);
				}
			}

			Vector2 vector43 = npc.Center - Main.screenPosition;
			vector43 -= new Vector2(NPCTexture.Width, NPCTexture.Height / frameCount) * scale / 2f;
			vector43 += vector11 * scale + new Vector2(0f, 4f + offsetY);
			spriteBatch.Draw(NPCTexture, vector43, new Rectangle?(frame), npc.GetAlpha(drawColor), rotation, vector11, scale, spriteEffects, 0f);

			Color color40 = Color.Lerp(Color.White, Color.Fuchsia, 0.5f);

			if (CalamityConfig.Instance.Afterimages)
			{
				for (int num163 = 1; num163 < num153; num163++)
				{
					Color color41 = color40;
					color41 = Color.Lerp(color41, color36, amount9);
					color41 = npc.GetAlpha(color41);
					color41 *= (num153 - num163) / 15f;
					Vector2 vector44 = npc.oldPos[num163] + new Vector2(npc.width, npc.height) / 2f - Main.screenPosition;
					vector44 -= new Vector2(glowMaskTexture.Width, glowMaskTexture.Height / frameCount) * scale / 2f;
					vector44 += vector11 * scale + new Vector2(0f, 4f + offsetY);
					spriteBatch.Draw(glowMaskTexture, vector44, new Rectangle?(frame), color41, rotation, vector11, scale, spriteEffects, 0f);
				}
			}

			spriteBatch.Draw(glowMaskTexture, vector43, new Rectangle?(frame), color40, rotation, vector11, scale, spriteEffects, 0f);

			return false;
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.SuperHealingPotion;
        }

        public override void NPCLoot()
        {
            // Only drop items if fought alone
            if (CalamityWorld.DoGSecondStageCountdown <= 0)
            {
                // Materials
                DropHelper.DropItem(npc, ModContent.ItemType<TwistingNether>(), true, 2, 3);

                // Weapons
                DropHelper.DropItemChance(npc, ModContent.ItemType<CosmicKunai>(), Main.expertMode ? 3 : 4);
				DropHelper.DropItemRIV(npc, ModContent.ItemType<Cosmilamp>(), ModContent.ItemType<LanternoftheSoul>(), Main.expertMode ? 0.3333f : 0.25f, DropHelper.RareVariantDropRateFloat);

				//Equipment
                DropHelper.DropItemCondition(npc, ModContent.ItemType<SpectralVeil>(), CalamityWorld.revenge, 4, 1, 1);

                // Vanity
                DropHelper.DropItemChance(npc, ModContent.ItemType<SignusTrophy>(), 10);
                DropHelper.DropItemChance(npc, ModContent.ItemType<SignusMask>(), 7);
				if (Main.rand.NextBool(20))
				{
					DropHelper.DropItem(npc, ModContent.ItemType<AncientGodSlayerHelm>());
					DropHelper.DropItem(npc, ModContent.ItemType<AncientGodSlayerChestplate>());
					DropHelper.DropItem(npc, ModContent.ItemType<AncientGodSlayerLeggings>());
				}

                // Other
                bool lastSentinelKilled = CalamityWorld.downedSentinel1 && CalamityWorld.downedSentinel2 && !CalamityWorld.downedSentinel3;
                DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeSentinels>(), true, lastSentinelKilled);
                DropHelper.DropResidentEvilAmmo(npc, CalamityWorld.downedSentinel3, 5, 2, 1);
            }

            // If DoG's fight is active, set the timer precisely for DoG phase 2 to spawn
            if (CalamityWorld.DoGSecondStageCountdown > 600)
            {
                CalamityWorld.DoGSecondStageCountdown = 600;
                if (Main.netMode == NetmodeID.Server)
                {
                    var netMessage = mod.GetPacket();
                    netMessage.Write((byte)CalamityModMessageType.DoGCountdownSync);
                    netMessage.Write(CalamityWorld.DoGSecondStageCountdown);
                    netMessage.Send();
                }

				// Mark DoG fight sentinels as dead
				CalamityWorld.downedSecondSentinels = true;
				CalamityNetcode.SyncWorld();
			}

			// Mark Signus as dead
			if (CalamityWorld.DoGSecondStageCountdown <= 0)
			{
				CalamityWorld.downedSentinel3 = true;
				CalamityNetcode.SyncWorld();
			}
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
            npc.damage = (int)(npc.damage * npc.GetExpertDamageMultiplier());
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.PurpleCosmolite, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                npc.position.X = npc.position.X + (npc.width / 2);
                npc.position.Y = npc.position.Y + (npc.height / 2);
                npc.width = 200;
                npc.height = 150;
                npc.position.X = npc.position.X - (npc.width / 2);
                npc.position.Y = npc.position.Y - (npc.height / 2);
                for (int num621 = 0; num621 < 40; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.PurpleCosmolite, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 60; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.PurpleCosmolite, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.PurpleCosmolite, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
                float randomSpread = Main.rand.Next(-200, 200) / 100;
                Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/Signus"), 1f);
                Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/Signus2"), 1f);
                Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/Signus3"), 1f);
                Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/Signus4"), 1f);
                Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/Signus5"), 1f);
            }
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<WhisperingDeath>(), 420, true);
            if (CalamityWorld.revenge)
            {
                player.AddBuff(ModContent.BuffType<Horror>(), 300, true);
            }
        }
    }
}
