using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Events;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using CalamityMod;
namespace CalamityMod.NPCs.Polterghast
{
    [AutoloadBossHead]
    public class Polterghast : ModNPC
    {
        private int despawnTimer = 600;
        private bool spawnGhost = false;
        public static float phase1DR = 0.15f;
        public static float phase2DR = 0.2f;
        public static float phase3DR = 0.5f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Polterghast");
            Main.npcFrameCount[npc.type] = 12;
        }

        public override void SetDefaults()
        {
            npc.npcSlots = 50f;
            npc.damage = 150;
            npc.width = 90;
            npc.height = 120;
            npc.defense = 90;
            CalamityGlobalNPC global = npc.Calamity();
            global.DR = 0.15f;
            global.customDR = true;
            global.multDRReductions.Add(BuffID.Ichor, 0.88f);
            global.multDRReductions.Add(BuffID.CursedInferno, 0.9f);
            npc.LifeMaxNERB(412500, 495000, 3250000);
            double HPBoost = CalamityMod.CalamityConfig.BossHealthPercentageBoost * 0.01;
            npc.lifeMax += (int)(npc.lifeMax * HPBoost);
            npc.knockBackResist = 0f;
            npc.aiStyle = -1;
            aiType = -1;
            npc.value = Item.buyPrice(0, 60, 0, 0);
            npc.boss = true;
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
            npc.buffImmune[BuffID.Ichor] = false;
            npc.buffImmune[BuffID.CursedInferno] = false;
            npc.buffImmune[BuffID.Daybreak] = false;
			npc.buffImmune[BuffID.StardustMinionBleed] = false;
			npc.buffImmune[BuffID.Oiled] = false;
            npc.buffImmune[ModContent.BuffType<AbyssalFlames>()] = false;
            npc.buffImmune[ModContent.BuffType<DemonFlames>()] = false;
            npc.buffImmune[ModContent.BuffType<GodSlayerInferno>()] = false;
            npc.buffImmune[ModContent.BuffType<Nightwither>()] = false;
            npc.buffImmune[ModContent.BuffType<Shred>()] = false;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.netAlways = true;
            Mod calamityModMusic = ModLoader.GetMod("CalamityModMusic");
            if (calamityModMusic != null)
                music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/RUIN");
            else
                music = MusicID.Plantera;
            npc.HitSound = SoundID.NPCHit7;
            npc.DeathSound = SoundID.NPCDeath39;
            bossBag = ModContent.ItemType<PolterghastBag>();
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(spawnGhost);
            writer.Write(despawnTimer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            spawnGhost = reader.ReadBoolean();
            despawnTimer = reader.ReadInt32();
        }

        public override void AI()
        {
            // Emit light
            Lighting.AddLight((int)((npc.position.X + (float)(npc.width / 2)) / 16f), (int)((npc.position.Y + (float)(npc.height / 2)) / 16f), 0.5f, 1.25f, 1.25f);

            // whoAmI variable
            CalamityGlobalNPC.ghostBoss = npc.whoAmI;

            // Detect clone
            bool cloneAlive = false;
            if (CalamityGlobalNPC.ghostBossClone != -1)
                cloneAlive = Main.npc[CalamityGlobalNPC.ghostBossClone].active;

            // Variables
            Vector2 vector = npc.Center;
            bool speedBoost = false;
            bool despawnBoost = false;
			bool death = CalamityWorld.death || CalamityWorld.bossRushActive;
			bool revenge = CalamityWorld.revenge || CalamityWorld.bossRushActive;
            bool expertMode = Main.expertMode || CalamityWorld.bossRushActive;
            bool phase2 = (double)npc.life < (double)npc.lifeMax * (death ? 0.9 : 0.75); //hooks fire beams
            bool phase3 = (double)npc.life < (double)npc.lifeMax * (revenge ? (death ? 0.8 : 0.5) : 0.33); //hooks stop shooting and polter begins charging with ghosts spinning around player
            bool phase4 = (double)npc.life < (double)npc.lifeMax * (revenge ? (death ? 0.5 : 0.33) : 0.2); //starts spitting ghost dudes
            bool phase5 = (double)npc.life < (double)npc.lifeMax * (revenge ? (death ? 0.25 : 0.1) : 0.05); //starts moving incredibly fast

            // Target
            npc.TargetClosest(true);
			Player player = Main.player[npc.target];

			if (!player.active || player.dead)
			{
				npc.TargetClosest(false);
				player = Main.player[npc.target];
				if (!player.active || player.dead)
				{
					speedBoost = true;
					despawnBoost = true;
				}
			}

            // Stop rain
            CalamityMod.StopRain();

            // Set time left
            if (npc.timeLeft < 1800)
                npc.timeLeft = 1800;

            // Spawn hooks
            if (npc.localAI[0] == 0f && Main.netMode != NetmodeID.MultiplayerClient)
            {
                npc.localAI[0] = 1f;
                int num729 = NPC.NewNPC((int)vector.X, (int)vector.Y, ModContent.NPCType<PolterghastHook>(), npc.whoAmI, 0f, 0f, 0f, 0f, 255);
                num729 = NPC.NewNPC((int)vector.X, (int)vector.Y, ModContent.NPCType<PolterghastHook>(), npc.whoAmI, 0f, 0f, 0f, 0f, 255);
                num729 = NPC.NewNPC((int)vector.X, (int)vector.Y, ModContent.NPCType<PolterghastHook>(), npc.whoAmI, 0f, 0f, 0f, 0f, 255);
                num729 = NPC.NewNPC((int)vector.X, (int)vector.Y, ModContent.NPCType<PolterghastHook>(), npc.whoAmI, 0f, 0f, 0f, 0f, 255);
            }

            int[] array2 = new int[4];
            float num730 = 0f;
            float num731 = 0f;
            int num732 = 0;
            int num;
            for (int num733 = 0; num733 < 200; num733 = num + 1)
            {
                if (Main.npc[num733].active && Main.npc[num733].type == ModContent.NPCType<PolterghastHook>())
                {
                    num730 += Main.npc[num733].Center.X;
                    num731 += Main.npc[num733].Center.Y;
                    array2[num732] = num733;
                    num732++;
                    if (num732 > 3)
                        break;
                }
                num = num733;
            }
            num730 /= (float)num732;
            num731 /= (float)num732;

			// Velocity and acceleration
			bool charging = npc.ai[2] >= 300f;
			bool reset = npc.ai[2] >= 600f;
			float num734 = 2.5f;
            float num735 = 0.025f;
            if (!player.ZoneDungeon && !CalamityWorld.bossRushActive && (double)player.position.Y < Main.worldSurface * 16.0)
            {
                despawnTimer--;
                if (despawnTimer <= 0)
                    despawnBoost = true;

                speedBoost = true;
                num734 += 2f;
                num735 += 0.025f;
            }
            else
                despawnTimer++;

            // Despawn
            if (Vector2.Distance(player.Center, vector) > (despawnBoost ? 1500f : 6000f))
			{
				npc.active = false;
				npc.netUpdate = true;
				return;
			}

			if (phase2)
            {
                num734 += 1.5f;
                num735 += 0.01f;
			}

			if (!phase3)
			{
				if (charging)
				{
					num734 += phase2 ? 9.5f : 8.5f;
					num735 += phase2 ? 0.05f : 0.045f;
				}

				npc.ai[2] += 1f;
				if (reset)
				{
					npc.ai[2] = 0f;
					npc.netUpdate = true;
				}
			}
			else
			{
				if (charging)
				{
					num734 += phase5 ? 14.5f : 10.5f;
					num735 += phase5 ? 0.085f : 0.055f;
				}
				else
				{
					if (phase5)
					{
						num734 += 1.5f;
						num735 += 0.015f;
					}
					else if (phase4)
					{
						num734 += 1f;
						num735 += 0.01f;
					}
					else
					{
						num734 += 0.5f;
						num735 += 0.005f;
					}
				}

				npc.ai[2] += 1f;
				if (reset)
				{
					npc.ai[2] = 0f;
					npc.netUpdate = true;
				}
			}

            // Set DR based on phase
            float dr = phase3 ? phase3DR : phase2 ? phase2DR : phase1DR;
            npc.Calamity().DR = dr;

            if (expertMode)
            {
                num734 += revenge ? 1.5f : 1f;
                num734 *= revenge ? 1.2f : 1.1f;
                num735 += revenge ? 0.015f : 0.01f;
                num735 *= revenge ? 1.2f : 1.1f;
            }

            Vector2 vector91 = new Vector2(num730, num731);
            float num736 = player.Center.X - vector91.X;
            float num737 = player.Center.Y - vector91.Y;

            if (despawnBoost)
            {
                num737 *= -1f;
                num736 *= -1f;
                num734 += 10f;
            }

            float num738 = (float)Math.Sqrt((double)(num736 * num736 + num737 * num737));
            int num739 = 500;
            if (speedBoost)
                num739 += 250;
            if (expertMode)
                num739 += 150;

            if (num738 >= (float)num739)
            {
                num738 = (float)num739 / num738;
                num736 *= num738;
                num737 *= num738;
            }

            num730 += num736;
            num731 += num737;
            vector91 = new Vector2(vector.X, vector.Y);
            num736 = num730 - vector91.X;
            num737 = num731 - vector91.Y;
            num738 = (float)Math.Sqrt((double)(num736 * num736 + num737 * num737));

            if (num738 < num734)
            {
                num736 = npc.velocity.X;
                num737 = npc.velocity.Y;
            }
            else
            {
                num738 = num734 / num738;
                num736 *= num738;
                num737 *= num738;
            }

            Vector2 vector92 = new Vector2(vector.X, vector.Y);
            float num740 = player.Center.X - vector92.X;
            float num741 = player.Center.Y - vector92.Y;
            npc.rotation = (float)Math.Atan2((double)num741, (double)num740) + 1.57f;

            if (npc.velocity.X < num736)
            {
                npc.velocity.X += num735;
                if (npc.velocity.X < 0f && num736 > 0f)
                    npc.velocity.X += num735 * 2f;
            }
            else if (npc.velocity.X > num736)
            {
                npc.velocity.X -= num735;
                if (npc.velocity.X > 0f && num736 < 0f)
                    npc.velocity.X -= num735 * 2f;
            }
            if (npc.velocity.Y < num737)
            {
                npc.velocity.Y += num735;
                if (npc.velocity.Y < 0f && num737 > 0f)
                    npc.velocity.Y += num735 * 2f;
            }
            else if (npc.velocity.Y > num737)
            {
                npc.velocity.Y -= num735;
                if (npc.velocity.Y > 0f && num737 < 0f)
                    npc.velocity.Y -= num735 * 2f;
            }

            if (!phase2 && !phase3)
            {
                npc.damage = npc.defDamage;
                npc.defense = npc.defDefense;

                if (speedBoost)
                {
                    npc.defense *= 2;
                    npc.damage *= 2;
                }

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.localAI[1] += expertMode ? 1.5f : 1f;
                    if (speedBoost || CalamityWorld.bossRushActive)
                        npc.localAI[1] += 3f;

                    if (npc.localAI[1] >= 40f)
                    {
                        npc.localAI[1] = 0f;

                        bool flag47 = Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height);
                        if (npc.localAI[3] > 0f)
                        {
                            flag47 = true;
                            npc.localAI[3] = 0f;
                        }

                        if (flag47)
                        {
							float num742 = CalamityWorld.bossRushActive ? 7f : 5f;
							if (speedBoost || npc.Calamity().enraged > 0 || (CalamityMod.CalamityConfig.BossRushXerocCurse && CalamityWorld.bossRushActive))
                                num742 *= 2f;

                            Vector2 vector93 = new Vector2(vector.X, vector.Y);
                            float num743 = player.position.X + (float)player.width * 0.5f - vector93.X;
                            float num744 = player.position.Y + (float)player.height * 0.5f - vector93.Y;
                            float num745 = (float)Math.Sqrt((double)(num743 * num743 + num744 * num744));

                            num745 = num742 / num745;
                            num743 *= num745;
                            num744 *= num745;

                            int num746 = expertMode ? 48 : 60;
                            int num747 = ModContent.ProjectileType<PhantomShot>();

                            if (Main.rand.NextBool(3))
                            {
                                num746 = expertMode ? 60 : 70;
                                npc.localAI[1] = -30f;
                                num747 = ModContent.ProjectileType<PhantomBlast>();
                            }

                            if (speedBoost || npc.Calamity().enraged > 0 || (CalamityMod.CalamityConfig.BossRushXerocCurse && CalamityWorld.bossRushActive))
                                num746 *= 2;

                            vector93.X += num743 * 3f;
                            vector93.Y += num744 * 3f;

                            int num748 = Projectile.NewProjectile(vector93.X, vector93.Y, num743, num744, num747, num746, 0f, Main.myPlayer, 0f, 0f);
                            Main.projectile[num748].timeLeft = num747 == ModContent.ProjectileType<PhantomBlast>() ? 300 : 1200;
                        }
                        else
                        {
                            float num742 = CalamityWorld.bossRushActive ? 14f : 11f;
                            if (speedBoost || npc.Calamity().enraged > 0 || (CalamityMod.CalamityConfig.BossRushXerocCurse && CalamityWorld.bossRushActive))
                                num742 *= 2f;

                            Vector2 vector93 = new Vector2(vector.X, vector.Y);
                            float num743 = player.position.X + (float)player.width * 0.5f - vector93.X;
                            float num744 = player.position.Y + (float)player.height * 0.5f - vector93.Y;
                            float num745 = (float)Math.Sqrt((double)(num743 * num743 + num744 * num744));

                            num745 = num742 / num745;
                            num743 *= num745;
                            num744 *= num745;

                            int num746 = expertMode ? 60 : 70;
                            int num747 = ModContent.ProjectileType<PhantomBlast>();
                            if (speedBoost || npc.Calamity().enraged > 0 || (CalamityMod.CalamityConfig.BossRushXerocCurse && CalamityWorld.bossRushActive))
                                num746 *= 2;

                            vector93.X += num743 * 3f;
                            vector93.Y += num744 * 3f;

                            int num748 = Projectile.NewProjectile(vector93.X, vector93.Y, num743, num744, num747, num746, 0f, Main.myPlayer, 0f, 0f);
                            Main.projectile[num748].timeLeft = 180;
                        }
                    }
                }
            }
            else if (!phase3)
            {
                if (npc.ai[0] == 0f)
                {
                    npc.ai[0] += 1f;

                    Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 122);

                    Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Polt"), 1f);
                    Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Polt2"), 1f);
                    Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Polt3"), 1f);
                    Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Polt4"), 1f);
                    Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Polt5"), 1f);

                    for (int num621 = 0; num621 < 10; num621++)
                    {
                        int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 60, 0f, 0f, 100, default, 2f);
                        Main.dust[num622].velocity *= 3f;
                        Main.dust[num622].noGravity = true;
                        if (Main.rand.NextBool(2))
                        {
                            Main.dust[num622].scale = 0.5f;
                            Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                        }
                    }
                    for (int num623 = 0; num623 < 30; num623++)
                    {
                        int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 180, 0f, 0f, 100, default, 3f);
                        Main.dust[num624].noGravity = true;
                        Main.dust[num624].velocity *= 5f;
                        num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 180, 0f, 0f, 100, default, 2f);
                        Main.dust[num624].velocity *= 2f;
                    }
                }

                npc.GivenName = "Necroghast";

                npc.damage = (int)((float)npc.defDamage * 1.2f);
                npc.defense = (int)((float)npc.defDefense * 0.8f);

                if (speedBoost || npc.Calamity().enraged > 0 || (CalamityMod.CalamityConfig.BossRushXerocCurse && CalamityWorld.bossRushActive))
                {
                    npc.defense *= 2;
                    npc.damage *= 2;
                }

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.localAI[1] += expertMode ? 1.5f : 1f;
                    if (speedBoost || CalamityWorld.bossRushActive)
                        npc.localAI[1] += 3f;

                    if (npc.localAI[1] >= 36f)
                    {
                        npc.localAI[1] = 0f;

                        bool flag47 = Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height);
                        if (npc.localAI[3] > 0f)
                        {
                            flag47 = true;
                            npc.localAI[3] = 0f;
                        }

                        if (flag47)
                        {
                            float num742 = CalamityWorld.bossRushActive ? 8f : 6f;
                            if (speedBoost || npc.Calamity().enraged > 0 || (CalamityMod.CalamityConfig.BossRushXerocCurse && CalamityWorld.bossRushActive))
                                num742 *= 2f;

                            Vector2 vector93 = new Vector2(vector.X, vector.Y);
                            float num743 = player.position.X + (float)player.width * 0.5f - vector93.X;
                            float num744 = player.position.Y + (float)player.height * 0.5f - vector93.Y;
                            float num745 = (float)Math.Sqrt((double)(num743 * num743 + num744 * num744));

                            num745 = num742 / num745;
                            num743 *= num745;
                            num744 *= num745;

                            int num746 = expertMode ? 53 : 65;
                            int num747 = ModContent.ProjectileType<PhantomShot2>();

                            if (Main.rand.NextBool(3))
                            {
                                num746 = expertMode ? 65 : 75;
                                npc.localAI[1] = -30f;
                                num747 = ModContent.ProjectileType<PhantomBlast2>();
                            }

                            if (speedBoost || npc.Calamity().enraged > 0 || (CalamityMod.CalamityConfig.BossRushXerocCurse && CalamityWorld.bossRushActive))
                                num746 *= 2;

                            vector93.X += num743 * 3f;
                            vector93.Y += num744 * 3f;

                            int num748 = Projectile.NewProjectile(vector93.X, vector93.Y, num743, num744, num747, num746, 0f, Main.myPlayer, 0f, 0f);
                            Main.projectile[num748].timeLeft = num747 == ModContent.ProjectileType<PhantomBlast2>() ? 300 : 1200;
                        }
                        else
                        {
                            float num742 = CalamityWorld.bossRushActive ? 14f : 11f;
                            if (speedBoost || npc.Calamity().enraged > 0 || (CalamityMod.CalamityConfig.BossRushXerocCurse && CalamityWorld.bossRushActive))
                                num742 *= 2f;

                            Vector2 vector93 = new Vector2(vector.X, vector.Y);
                            float num743 = player.position.X + (float)player.width * 0.5f - vector93.X;
                            float num744 = player.position.Y + (float)player.height * 0.5f - vector93.Y;
                            float num745 = (float)Math.Sqrt((double)(num743 * num743 + num744 * num744));

                            num745 = num742 / num745;
                            num743 *= num745;
                            num744 *= num745;

                            int num746 = expertMode ? 65 : 75;
                            int num747 = ModContent.ProjectileType<PhantomBlast2>();

                            if (speedBoost || npc.Calamity().enraged > 0 || (CalamityMod.CalamityConfig.BossRushXerocCurse && CalamityWorld.bossRushActive))
                                num746 *= 2;

                            vector93.X += num743 * 3f;
                            vector93.Y += num744 * 3f;

                            int num748 = Projectile.NewProjectile(vector93.X, vector93.Y, num743, num744, num747, num746, 0f, Main.myPlayer, 0f, 0f);
                            Main.projectile[num748].timeLeft = 240;
                        }
                    }
                }
            }
            else
            {
                npc.HitSound = SoundID.NPCHit36;

                if (!spawnGhost)
                {
                    spawnGhost = true;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        NPC.NewNPC((int)vector.X, (int)vector.Y, ModContent.NPCType<PolterPhantom>(), 0, 0f, 0f, 0f, 0f, 255);

                        for (int I = 0; I < 3; I++)
                        {
                            int Phantom = NPC.NewNPC((int)(player.Center.X + (Math.Sin(I * 120) * 500)),
                                (int)(player.Center.Y + (Math.Cos(I * 120) * 500)), ModContent.NPCType<PhantomFuckYou>(), 0, 0, 0, 0, -1);
                            NPC Eye = Main.npc[Phantom];
                            Eye.ai[0] = I * 120;
                            Eye.ai[3] = I * 120;
                        }
                    }

                    Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 122);

                    Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Polt"), 1f);
                    Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Polt2"), 1f);
                    Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Polt3"), 1f);
                    Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Polt4"), 1f);
                    Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Polt5"), 1f);

                    for (int num621 = 0; num621 < 10; num621++)
                    {
                        int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 60, 0f, 0f, 100, default, 2f);
                        Main.dust[num622].velocity *= 3f;
                        Main.dust[num622].noGravity = true;
                        if (Main.rand.NextBool(2))
                        {
                            Main.dust[num622].scale = 0.5f;
                            Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                        }
                    }
                    for (int num623 = 0; num623 < 30; num623++)
                    {
                        int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 180, 0f, 0f, 100, default, 3f);
                        Main.dust[num624].noGravity = true;
                        Main.dust[num624].velocity *= 5f;
                        num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 180, 0f, 0f, 100, default, 2f);
                        Main.dust[num624].velocity *= 2f;
                    }
                }

                npc.GivenName = "Necroplasm";

                npc.damage = (int)((float)npc.defDamage * 1.4f);
                npc.defense = (int)((float)npc.defDefense * 0.5f);

                if (speedBoost || npc.Calamity().enraged > 0 || (CalamityMod.CalamityConfig.BossRushXerocCurse && CalamityWorld.bossRushActive))
                {
                    npc.defense *= 2;
                    npc.damage *= 2;
                }

				npc.ai[1] += 1f;
				if (npc.ai[1] >= 420f && Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height))
				{
					npc.ai[1] = 0f;
					Vector2 vector93 = new Vector2(vector.X, vector.Y);
					float num742 = CalamityWorld.bossRushActive ? 7f : 5f;
					float num743 = player.position.X + (float)player.width * 0.5f - vector93.X;
					float num744 = player.position.Y + (float)player.height * 0.5f - vector93.Y;
					float num745 = (float)Math.Sqrt((double)(num743 * num743 + num744 * num744));

					num745 = num742 / num745;
					num743 *= num745;
					num744 *= num745;
					vector93.X += num743 * 3f;
					vector93.Y += num744 * 3f;

					int damage = expertMode ? 53 : 65;
					int numProj = 4;
					int spread = 45;
					float rotation = MathHelper.ToRadians(spread);
					float baseSpeed = (float)Math.Sqrt(num743 * num743 + num744 * num744);
					double startAngle = Math.Atan2(num743, num744) - rotation / 2;
					double deltaAngle = rotation / (float)numProj;
					double offsetAngle;
					int type = ModContent.ProjectileType<PhantomShot>();
					for (int i = 0; i < numProj; i++)
					{
						offsetAngle = startAngle + deltaAngle * i;
						if (i == 1 || i == 2)
							type = ModContent.ProjectileType<PhantomShot2>();
						Projectile.NewProjectile(vector93.X, vector93.Y, baseSpeed * (float)Math.Sin(offsetAngle), baseSpeed * (float)Math.Cos(offsetAngle), type, damage, 0f, Main.myPlayer, 0f, 0f);
					}
				}

				if (phase4)
                {
                    npc.localAI[1] += 1f;
                    if (npc.localAI[1] >= 150f)
                    {
                        npc.localAI[1] = 0f;

                        float num757 = 8f;
                        Vector2 vector94 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                        float num758 = player.position.X + (float)player.width * 0.5f - vector94.X;
                        float num759 = Math.Abs(num758 * 0.2f);
                        float num760 = player.position.Y + (float)player.height * 0.5f - vector94.Y;
                        if (num760 > 0f)
                            num759 = 0f;

                        num760 -= num759;
                        float num761 = (float)Math.Sqrt((double)(num758 * num758 + num760 * num760));
                        num761 = num757 / num761;
                        num758 *= num761;
                        num760 *= num761;

                        if (NPC.CountNPCS(ModContent.NPCType<PhantomSpiritL>()) < 2 && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int num762 = NPC.NewNPC((int)vector.X, (int)vector.Y, ModContent.NPCType<PhantomSpiritL>(), 0, 0f, 0f, 0f, 0f, 255);
                            Main.npc[num762].velocity.X = num758;
                            Main.npc[num762].velocity.Y = num760;
                            Main.npc[num762].netUpdate = true;
                        }
                    }
                }
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.SuperHealingPotion;
        }

        public override void NPCLoot()
        {
            DropHelper.DropBags(npc);

            DropHelper.DropItemChance(npc, ModContent.ItemType<PolterghastTrophy>(), 10);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgePolterghast>(), true, !CalamityWorld.downedPolterghast);
            DropHelper.DropResidentEvilAmmo(npc, CalamityWorld.downedPolterghast, 6, 3, 2);

            // All other drops are contained in the bag, so they only drop directly on Normal
            if (!Main.expertMode)
            {
                // Materials
                DropHelper.DropItem(npc, ModContent.ItemType<RuinousSoul>(), 7, 15);
                DropHelper.DropItem(npc, ModContent.ItemType<Phantoplasm>(), 10, 20);

                // Vanity
                DropHelper.DropItemChance(npc, ModContent.ItemType<PolterghastMask>(), 7);

                // Weapons
                DropHelper.DropItemChance(npc, ModContent.ItemType<TerrorBlade>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<BansheeHook>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<DaemonsFlame>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<FatesReveal>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<GhastlyVisage>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<EtherealSubjugator>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<GhoulishGouger>(), 4);
            }

            // If Polterghast has not been killed, notify players about the Abyss minibosses now dropping items
            if (!CalamityWorld.downedPolterghast)
            {
                if (!Main.player[Main.myPlayer].dead && Main.player[Main.myPlayer].active)
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/ReaperSearchRoar"), (int)Main.player[Main.myPlayer].position.X, (int)Main.player[Main.myPlayer].position.Y);

                string key = "Mods.CalamityMod.GhostBossText";
                Color messageColor = Color.RoyalBlue;
                string sulfSeaBoostMessage = "Mods.CalamityMod.GhostBossText4";
                Color sulfSeaBoostColor = AcidRainEvent.TextColor;

                if (Main.netMode == NetmodeID.SinglePlayer)
				{
                    Main.NewText(Language.GetTextValue(key), messageColor);
                    Main.NewText(Language.GetTextValue(sulfSeaBoostMessage), sulfSeaBoostColor);
				}
                else if (Main.netMode == NetmodeID.Server)
				{
                    NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                    NetMessage.BroadcastChatMessage(NetworkText.FromKey(sulfSeaBoostMessage), sulfSeaBoostColor);
				}
            }

            // Mark Polterghast as dead
            CalamityWorld.downedPolterghast = true;
            CalamityMod.UpdateServerBoolean();
        }

        public override void FindFrame(int frameHeight)
        {
			bool revenge = CalamityWorld.revenge || CalamityWorld.bossRushActive;
			bool death = CalamityWorld.death || CalamityWorld.bossRushActive;
			bool phase2 = (double)npc.life >= (double)npc.lifeMax * (revenge ? (death ? 0.8 : 0.5) : 0.33);
            npc.frameCounter += 1.0;
            if (npc.frameCounter > 6.0)
            {
                npc.frameCounter = 0.0;
                npc.frame.Y = npc.frame.Y + frameHeight;
            }
            if ((double)npc.life >= (double)npc.lifeMax * (death ? 0.9 : 0.75))
            {
                if (npc.frame.Y > frameHeight * 3)
                {
                    npc.frame.Y = 0;
                }
            }
            else if (phase2)
            {
                if (npc.frame.Y < frameHeight * 4)
                {
                    npc.frame.Y = frameHeight * 4;
                }
                if (npc.frame.Y > frameHeight * 7)
                {
                    npc.frame.Y = frameHeight * 4;
                }
            }
            else
            {
                if (npc.frame.Y < frameHeight * 8)
                {
                    npc.frame.Y = frameHeight * 8;
                }
                if (npc.frame.Y > frameHeight * 11)
                {
                    npc.frame.Y = frameHeight * 8;
                }
            }
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            if (CalamityWorld.revenge)
                player.AddBuff(ModContent.BuffType<Horror>(), 300, true);
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;
            return true;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
            npc.damage = (int)(npc.damage * 0.8f);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            Dust.NewDust(npc.position, npc.width, npc.height, 180, hitDirection, -1f, 0, default, 1f);
            if (npc.life <= 0)
            {
                npc.position.X = npc.position.X + (float)(npc.width / 2);
                npc.position.Y = npc.position.Y + (float)(npc.height / 2);
                npc.width = 90;
                npc.height = 90;
                npc.position.X = npc.position.X - (float)(npc.width / 2);
                npc.position.Y = npc.position.Y - (float)(npc.height / 2);
                for (int num621 = 0; num621 < 10; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 60, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 60; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 180, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 180, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
            }
        }
    }
}
