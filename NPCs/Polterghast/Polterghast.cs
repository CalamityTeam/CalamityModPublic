using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
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
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
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
			NPCID.Sets.TrailingMode[npc.type] = 1;
		}

        public override void SetDefaults()
        {
            npc.npcSlots = 50f;
            npc.damage = 150;
            npc.width = 90;
            npc.height = 120;
            npc.defense = 90;
			npc.DR_NERD(0.15f, null, null, null, true);
			CalamityGlobalNPC global = npc.Calamity();
            global.multDRReductions.Add(BuffID.CursedInferno, 0.9f);
            npc.LifeMaxNERB(412500, 495000, 3250000);
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
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
			npc.buffImmune[BuffID.BetsysCurse] = false;
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
            Lighting.AddLight((int)((npc.position.X + (npc.width / 2)) / 16f), (int)((npc.position.Y + (npc.height / 2)) / 16f), 0.1f, 0.5f, 0.5f);

            // whoAmI variable
            CalamityGlobalNPC.ghostBoss = npc.whoAmI;

            // Detect clone
            bool cloneAlive = false;
            if (CalamityGlobalNPC.ghostBossClone != -1)
                cloneAlive = Main.npc[CalamityGlobalNPC.ghostBossClone].active;

			// Percent life remaining
			float lifeRatio = npc.life / (float)npc.lifeMax;

			// Variables
			Vector2 vector = npc.Center;
            bool speedBoost = false;
            bool despawnBoost = false;
			bool death = CalamityWorld.death || CalamityWorld.bossRushActive;
			bool revenge = CalamityWorld.revenge || CalamityWorld.bossRushActive;
            bool expertMode = Main.expertMode || CalamityWorld.bossRushActive;
            bool phase2 = lifeRatio < (death ? 0.9f : 0.75f);
            bool phase3 = lifeRatio < (revenge ? (death ? 0.8f : 0.5f) : 0.33f);
            bool phase4 = lifeRatio < (revenge ? (death ? 0.5f : 0.33f) : 0.2f);
            bool phase5 = lifeRatio < (revenge ? (death ? 0.25f : 0.1f) : 0.05f);

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
            num730 /= num732;
            num731 /= num732;

			// Velocity and acceleration
			bool charging = npc.ai[2] >= 300f;
			bool reset = npc.ai[2] >= 600f;
			float speedUpDistance = 480f - 360f * (1f - lifeRatio);
			bool speedUp = Vector2.Distance(player.Center, npc.Center) > speedUpDistance; // 30 or 40 tile distance
			float velocity = 10f; // max should be 21
            float acceleration = 0.05f; // max should be 0.13
            if (!player.ZoneDungeon && !CalamityWorld.bossRushActive && player.position.Y < Main.worldSurface * 16.0)
            {
                despawnTimer--;
                if (despawnTimer <= 0)
                    despawnBoost = true;

                speedBoost = true;
				velocity += 5f;
				acceleration += 0.05f;
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
				velocity += 2.5f;
				acceleration += 0.02f;
			}

			if (!phase3)
			{
				if (charging)
				{
					velocity += phase2 ? 4.5f : 3.5f;
					acceleration += phase2 ? 0.03f : 0.025f;
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
					velocity += phase5 ? 8.5f : 4.5f;
					acceleration += phase5 ? 0.06f : 0.03f;
				}
				else
				{
					if (phase5)
					{
						velocity += 1.5f;
						acceleration += 0.015f;
					}
					else if (phase4)
					{
						velocity += 1f;
						acceleration += 0.01f;
					}
					else
					{
						velocity += 0.5f;
						acceleration += 0.005f;
					}
				}

				npc.ai[2] += 1f;
				if (reset)
				{
					npc.ai[2] = 0f;
					npc.netUpdate = true;
				}
			}

			if (expertMode)
			{
				velocity += revenge ? 5f : 3.5f;
				acceleration += revenge ? 0.035f : 0.025f;
			}

			// Move faster if inside active tiles
			int radius = 2; // 2 tile radius
			int diameter = radius * 2;
			int npcCenterX = (int)(npc.Center.X / 16f);
			int npcCenterY = (int)(npc.Center.Y / 16f);
			Rectangle area = new Rectangle(npcCenterX - radius, npcCenterY - radius, diameter, diameter);
			bool insideTiles = false;
			for (int x = area.Left; x < area.Right; x++)
			{
				for (int y = area.Top; y < area.Bottom; y++)
				{
					if (Main.tile[x, y] != null)
					{
						if (Main.tile[x, y].nactive() && Main.tileSolid[Main.tile[x, y].type] && !Main.tileSolidTop[Main.tile[x, y].type] && !TileID.Sets.Platforms[Main.tile[x, y].type])
							insideTiles = true;
					}
				}
			}

			// Slow down if close to target and not inside tiles
			if (!speedUp && !insideTiles && !charging)
			{
				velocity = 8f;
				acceleration = 0.035f;
			}

			// Detect active tiles around Polterghast
			radius = 20; // 20 tile radius
			diameter = radius * 2;
			area = new Rectangle(npcCenterX - radius, npcCenterY - radius, diameter, diameter);
			int nearbyActiveTiles = 0; // 0 to 1600
			for (int x = area.Left; x < area.Right; x++)
			{
				for (int y = area.Top; y < area.Bottom; y++)
				{
					if (Main.tile[x, y] != null)
					{
						if (Main.tile[x, y].nactive() && Main.tileSolid[Main.tile[x, y].type] && !Main.tileSolidTop[Main.tile[x, y].type] && !TileID.Sets.Platforms[Main.tile[x, y].type])
							nearbyActiveTiles++;
					}
				}
			}

			// Scale multiplier based on nearby active tiles
			float tileEnrageMult = 1f;
			if (nearbyActiveTiles < 800)
				tileEnrageMult += (800 - nearbyActiveTiles) * 0.001f; // Ranges from 1f to 1.8f

			// Used to inform clone and hooks about number of active tiles nearby
			npc.ai[3] = tileEnrageMult;

			// Increase projectile fire rate based on number of nearby active tiles
			float projectileFireRateMultiplier = MathHelper.Lerp(1f, 2f, 1f - ((tileEnrageMult - 1f) / 0.8f));

			// Increase projectile time left based on number of nearby active tiles
			int baseProjectileTimeLeft = (int)(1200f * tileEnrageMult);

			// Increase damage of projectiles and contact damage based on number of nearby active tiles
			int damageIncrease = 0;
			if (nearbyActiveTiles < 400)
				damageIncrease += (400 - nearbyActiveTiles) / 20; // Ranges from 0 to 20

			// Set DR based on phase
			float dr = phase3 ? phase3DR : phase2 ? phase2DR : phase1DR;
            npc.Calamity().DR = dr;

            Vector2 vector91 = new Vector2(num730, num731);
            float num736 = player.Center.X - vector91.X;
            float num737 = player.Center.Y - vector91.Y;

            if (despawnBoost)
            {
                num737 *= -1f;
                num736 *= -1f;
				velocity += 10f;
            }

            float num738 = (float)Math.Sqrt(num736 * num736 + num737 * num737);
            int num739 = 500;
            if (speedBoost)
                num739 += 250;
            if (expertMode)
                num739 += 150;

			// Increase speed based on nearby active tiles
			velocity *= tileEnrageMult;
			acceleration *= tileEnrageMult;

			if (num738 >= num739)
            {
                num738 = num739 / num738;
                num736 *= num738;
                num737 *= num738;
            }

            num730 += num736;
            num731 += num737;
            vector91 = new Vector2(vector.X, vector.Y);
            num736 = num730 - vector91.X;
            num737 = num731 - vector91.Y;
            num738 = (float)Math.Sqrt(num736 * num736 + num737 * num737);

            if (num738 < velocity)
            {
                num736 = npc.velocity.X;
                num737 = npc.velocity.Y;
            }
            else
            {
                num738 = velocity / num738;
                num736 *= num738;
                num737 *= num738;
            }

            Vector2 vector92 = new Vector2(vector.X, vector.Y);
            float num740 = player.Center.X - vector92.X;
            float num741 = player.Center.Y - vector92.Y;
            npc.rotation = (float)Math.Atan2(num741, num740) + MathHelper.PiOver2;

            if (npc.velocity.X < num736)
            {
                npc.velocity.X += acceleration;
                if (npc.velocity.X < 0f && num736 > 0f)
                    npc.velocity.X += acceleration * 2f;
            }
            else if (npc.velocity.X > num736)
            {
                npc.velocity.X -= acceleration;
                if (npc.velocity.X > 0f && num736 < 0f)
                    npc.velocity.X -= acceleration * 2f;
            }
            if (npc.velocity.Y < num737)
            {
                npc.velocity.Y += acceleration;
                if (npc.velocity.Y < 0f && num737 > 0f)
                    npc.velocity.Y += acceleration * 2f;
            }
            else if (npc.velocity.Y > num737)
            {
                npc.velocity.Y -= acceleration;
                if (npc.velocity.Y > 0f && num737 < 0f)
                    npc.velocity.Y -= acceleration * 2f;
            }

			// Slow down considerably if near player
			if (!speedUp && nearbyActiveTiles > 800 && !insideTiles && !charging)
			{
				if (npc.velocity.Length() > velocity)
					npc.velocity *= 0.97f;
			}

			if (!phase2 && !phase3)
            {
                npc.damage = npc.defDamage + damageIncrease * 4;
                npc.defense = npc.defDefense;

                if (speedBoost)
                {
                    npc.defense *= 2;
                    npc.damage *= 2;
                }

                if (Main.netMode != NetmodeID.MultiplayerClient && !charging)
                {
                    npc.localAI[1] += expertMode ? 1.5f : 1f;
                    if (speedBoost || CalamityWorld.bossRushActive)
                        npc.localAI[1] += 3f;

                    if (npc.localAI[1] >= 90f * projectileFireRateMultiplier)
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
                            int damage = expertMode ? 48 : 60;
                            int type = ModContent.ProjectileType<PhantomShot>();

                            if (Main.rand.NextBool(3))
                            {
                                damage = expertMode ? 60 : 70;
                                npc.localAI[1] = -30f;
                                type = ModContent.ProjectileType<PhantomBlast>();
                            }

							damage += damageIncrease;

                            if (speedBoost || npc.Calamity().enraged > 0 || (CalamityConfig.Instance.BossRushXerocCurse && CalamityWorld.bossRushActive))
                                damage *= 2;

							Vector2 vector93 = new Vector2(vector.X, vector.Y);
							float num742 = (CalamityWorld.bossRushActive ? 7f : 5f) * tileEnrageMult;
							if (speedBoost || npc.Calamity().enraged > 0 || (CalamityConfig.Instance.BossRushXerocCurse && CalamityWorld.bossRushActive))
								num742 *= 2f;

							float num743 = player.position.X + player.width * 0.5f - vector93.X;
							float num744 = player.position.Y + player.height * 0.5f - vector93.Y;
							float num745 = (float)Math.Sqrt(num743 * num743 + num744 * num744);

							num745 = num742 / num745;
							num743 *= num745;
							num744 *= num745;
							vector93.X += num743 * 3f;
							vector93.Y += num744 * 3f;

							int numProj = 4;
							int spread = 45;
							float rotation = MathHelper.ToRadians(spread);
							float baseSpeed = (float)Math.Sqrt(num743 * num743 + num744 * num744);
							double startAngle = Math.Atan2(num743, num744) - rotation / 2;
							double deltaAngle = rotation / numProj;
							double offsetAngle;
							for (int i = 0; i < numProj; i++)
							{
								offsetAngle = startAngle + deltaAngle * i;
								int proj = Projectile.NewProjectile(vector93.X, vector93.Y, baseSpeed * (float)Math.Sin(offsetAngle), baseSpeed * (float)Math.Cos(offsetAngle), type, damage, 0f, Main.myPlayer, 0f, 0f);
								Main.projectile[proj].timeLeft = type == ModContent.ProjectileType<PhantomBlast>() ? baseProjectileTimeLeft / 4 : baseProjectileTimeLeft;
							}
						}
                        else
                        {
							int damage = expertMode ? 60 : 70;
							int type = ModContent.ProjectileType<PhantomBlast>();

							damage += damageIncrease;

							if (speedBoost || npc.Calamity().enraged > 0 || (CalamityConfig.Instance.BossRushXerocCurse && CalamityWorld.bossRushActive))
								damage *= 2;

							Vector2 vector93 = new Vector2(vector.X, vector.Y);
							float num742 = (CalamityWorld.bossRushActive ? 14f : 10f) * tileEnrageMult;
							if (speedBoost || npc.Calamity().enraged > 0 || (CalamityConfig.Instance.BossRushXerocCurse && CalamityWorld.bossRushActive))
								num742 *= 2f;

							float num743 = player.position.X + player.width * 0.5f - vector93.X;
							float num744 = player.position.Y + player.height * 0.5f - vector93.Y;
							float num745 = (float)Math.Sqrt(num743 * num743 + num744 * num744);

							num745 = num742 / num745;
							num743 *= num745;
							num744 *= num745;
							vector93.X += num743 * 3f;
							vector93.Y += num744 * 3f;

							int numProj = 4;
							int spread = 60;
							float rotation = MathHelper.ToRadians(spread);
							float baseSpeed = (float)Math.Sqrt(num743 * num743 + num744 * num744);
							double startAngle = Math.Atan2(num743, num744) - rotation / 2;
							double deltaAngle = rotation / numProj;
							double offsetAngle;
							for (int i = 0; i < numProj; i++)
							{
								offsetAngle = startAngle + deltaAngle * i;
								int proj = Projectile.NewProjectile(vector93.X, vector93.Y, baseSpeed * (float)Math.Sin(offsetAngle), baseSpeed * (float)Math.Cos(offsetAngle), type, damage, 0f, Main.myPlayer, 0f, 0f);
								Main.projectile[proj].timeLeft = baseProjectileTimeLeft / 4;
							}
						}
                    }
                }
            }
            else if (!phase3)
            {
                if (npc.ai[0] == 0f)
                {
                    npc.ai[0] += 1f;

                    Main.PlaySound(SoundID.Item122, npc.position);

                    Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Polt"), 1f);
                    Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Polt2"), 1f);
                    Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Polt3"), 1f);
                    Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Polt4"), 1f);
                    Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Polt5"), 1f);

                    for (int num621 = 0; num621 < 10; num621++)
                    {
                        int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.Phantoplasm, 0f, 0f, 100, default, 2f);
                        Main.dust[num622].velocity *= 3f;
                        Main.dust[num622].noGravity = true;
                        if (Main.rand.NextBool(2))
                        {
                            Main.dust[num622].scale = 0.5f;
                            Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
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

                npc.damage = (int)(npc.defDamage * 1.2f) + damageIncrease * 4;
                npc.defense = (int)(npc.defDefense * 0.8f);

                if (speedBoost || npc.Calamity().enraged > 0 || (CalamityConfig.Instance.BossRushXerocCurse && CalamityWorld.bossRushActive))
                {
                    npc.defense *= 2;
                    npc.damage *= 2;
                }

                if (Main.netMode != NetmodeID.MultiplayerClient && !charging)
                {
                    npc.localAI[1] += expertMode ? 1.5f : 1f;
                    if (speedBoost || CalamityWorld.bossRushActive)
                        npc.localAI[1] += 3f;

                    if (npc.localAI[1] >= 150f * projectileFireRateMultiplier)
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
							int damage = expertMode ? 53 : 65;
							int type = ModContent.ProjectileType<PhantomShot2>();

							if (Main.rand.NextBool(3))
							{
								damage = expertMode ? 65 : 75;
								npc.localAI[1] = -30f;
								type = ModContent.ProjectileType<PhantomBlast2>();
							}

							damage += damageIncrease;

							if (speedBoost || npc.Calamity().enraged > 0 || (CalamityConfig.Instance.BossRushXerocCurse && CalamityWorld.bossRushActive))
								damage *= 2;

							Vector2 vector93 = new Vector2(vector.X, vector.Y);
							float num742 = (CalamityWorld.bossRushActive ? 8f : 6f) * tileEnrageMult;
							if (speedBoost || npc.Calamity().enraged > 0 || (CalamityConfig.Instance.BossRushXerocCurse && CalamityWorld.bossRushActive))
								num742 *= 2f;

							float num743 = player.position.X + player.width * 0.5f - vector93.X;
							float num744 = player.position.Y + player.height * 0.5f - vector93.Y;
							float num745 = (float)Math.Sqrt(num743 * num743 + num744 * num744);

							num745 = num742 / num745;
							num743 *= num745;
							num744 *= num745;
							vector93.X += num743 * 3f;
							vector93.Y += num744 * 3f;

							int numProj = 5;
							int spread = 60;
							float rotation = MathHelper.ToRadians(spread);
							float baseSpeed = (float)Math.Sqrt(num743 * num743 + num744 * num744);
							double startAngle = Math.Atan2(num743, num744) - rotation / 2;
							double deltaAngle = rotation / numProj;
							double offsetAngle;
							for (int i = 0; i < numProj; i++)
							{
								offsetAngle = startAngle + deltaAngle * i;
								int proj = Projectile.NewProjectile(vector93.X, vector93.Y, baseSpeed * (float)Math.Sin(offsetAngle), baseSpeed * (float)Math.Cos(offsetAngle), type, damage, 0f, Main.myPlayer, 0f, 0f);
								Main.projectile[proj].timeLeft = type == ModContent.ProjectileType<PhantomBlast2>() ? baseProjectileTimeLeft / 4 : baseProjectileTimeLeft;
							}
						}
                        else
                        {
							int damage = expertMode ? 65 : 75;
							int type = ModContent.ProjectileType<PhantomBlast2>();

							damage += damageIncrease;

							if (speedBoost || npc.Calamity().enraged > 0 || (CalamityConfig.Instance.BossRushXerocCurse && CalamityWorld.bossRushActive))
								damage *= 2;

							Vector2 vector93 = new Vector2(vector.X, vector.Y);
							float num742 = (CalamityWorld.bossRushActive ? 14f : 10f) * tileEnrageMult;
							if (speedBoost || npc.Calamity().enraged > 0 || (CalamityConfig.Instance.BossRushXerocCurse && CalamityWorld.bossRushActive))
								num742 *= 2f;

							float num743 = player.position.X + player.width * 0.5f - vector93.X;
							float num744 = player.position.Y + player.height * 0.5f - vector93.Y;
							float num745 = (float)Math.Sqrt(num743 * num743 + num744 * num744);

							num745 = num742 / num745;
							num743 *= num745;
							num744 *= num745;
							vector93.X += num743 * 3f;
							vector93.Y += num744 * 3f;

							int numProj = 5;
							int spread = 80;
							float rotation = MathHelper.ToRadians(spread);
							float baseSpeed = (float)Math.Sqrt(num743 * num743 + num744 * num744);
							double startAngle = Math.Atan2(num743, num744) - rotation / 2;
							double deltaAngle = rotation / numProj;
							double offsetAngle;
							for (int i = 0; i < numProj; i++)
							{
								offsetAngle = startAngle + deltaAngle * i;
								int proj = Projectile.NewProjectile(vector93.X, vector93.Y, baseSpeed * (float)Math.Sin(offsetAngle), baseSpeed * (float)Math.Cos(offsetAngle), type, damage, 0f, Main.myPlayer, 0f, 0f);
								Main.projectile[proj].timeLeft = baseProjectileTimeLeft / 4;
							}
						}
                    }
                }
            }
            else
            {
                npc.HitSound = SoundID.NPCHit36;

                if (!spawnGhost && expertMode)
                {
                    spawnGhost = true;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        NPC.NewNPC((int)vector.X, (int)vector.Y, ModContent.NPCType<PolterPhantom>(), 0, 0f, 0f, 0f, 0f, 255);

                        for (int I = 0; I < 3; I++)
                        {
                            int spawn = NPC.NewNPC((int)(npc.Center.X + (Math.Sin(I * 120) * 500)), (int)(npc.Center.Y + (Math.Cos(I * 120) * 500)), ModContent.NPCType<PhantomFuckYou>(), npc.whoAmI, 0, 0, 0, -1);
                            NPC npc2 = Main.npc[spawn];
                            npc2.ai[0] = I * 120;
                        }
                    }

                    Main.PlaySound(SoundID.Item122, npc.position);

                    Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Polt"), 1f);
                    Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Polt2"), 1f);
                    Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Polt3"), 1f);
                    Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Polt4"), 1f);
                    Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Polt5"), 1f);

                    for (int num621 = 0; num621 < 10; num621++)
                    {
                        int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.Phantoplasm, 0f, 0f, 100, default, 2f);
                        Main.dust[num622].velocity *= 3f;
                        Main.dust[num622].noGravity = true;
                        if (Main.rand.NextBool(2))
                        {
                            Main.dust[num622].scale = 0.5f;
                            Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
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

                npc.damage = (int)(npc.defDamage * 1.4f) + damageIncrease * 4;
                npc.defense = (int)(npc.defDefense * 0.5f);

                if (speedBoost || npc.Calamity().enraged > 0 || (CalamityConfig.Instance.BossRushXerocCurse && CalamityWorld.bossRushActive))
                {
                    npc.defense *= 2;
                    npc.damage *= 2;
                }

				npc.ai[1] += 1f;
				if (npc.ai[1] >= 210f * projectileFireRateMultiplier && Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height))
				{
					npc.ai[1] = 0f;
					if (Main.netMode != NetmodeID.MultiplayerClient && !charging)
					{
						Vector2 vector93 = new Vector2(vector.X, vector.Y);
						float num742 = (CalamityWorld.bossRushActive ? 7f : 5f) * tileEnrageMult;
						float num743 = player.position.X + player.width * 0.5f - vector93.X;
						float num744 = player.position.Y + player.height * 0.5f - vector93.Y;
						float num745 = (float)Math.Sqrt(num743 * num743 + num744 * num744);

						num745 = num742 / num745;
						num743 *= num745;
						num744 *= num745;
						vector93.X += num743 * 3f;
						vector93.Y += num744 * 3f;

						int damage = expertMode ? 53 : 65;
						damage += damageIncrease;
						int numProj = 6;
						int spread = 90;
						float rotation = MathHelper.ToRadians(spread);
						float baseSpeed = (float)Math.Sqrt(num743 * num743 + num744 * num744);
						double startAngle = Math.Atan2(num743, num744) - rotation / 2;
						double deltaAngle = rotation / numProj;
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
				}

				if (phase4)
                {
                    npc.localAI[1] += 1f;
                    if (npc.localAI[1] >= 420f)
                    {
                        npc.localAI[1] = 0f;

                        float num757 = 8f;
                        Vector2 vector94 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                        float num758 = player.position.X + player.width * 0.5f - vector94.X;
                        float num759 = Math.Abs(num758 * 0.2f);
                        float num760 = player.position.Y + player.height * 0.5f - vector94.Y;
                        if (num760 > 0f)
                            num759 = 0f;

                        num760 -= num759;
                        float num761 = (float)Math.Sqrt(num758 * num758 + num760 * num760);
                        num761 = num757 / num761;
                        num758 *= num761;
                        num760 *= num761;

                        if (NPC.CountNPCS(ModContent.NPCType<PhantomSpiritL>()) < 2 && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int num762 = NPC.NewNPC((int)vector.X, (int)vector.Y, ModContent.NPCType<PhantomSpiritL>());
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

			CalamityGlobalTownNPC.SetNewShopVariable(new int[] { NPCID.Cyborg }, CalamityWorld.downedPolterghast);

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

				if (Main.rand.NextBool(20) && DateTime.Now.Month == 4 && DateTime.Now.Day == 1)
				{
					sulfSeaBoostMessage = "Mods.CalamityMod.AprilFools2"; // Goddamn boomer duke moments
				}

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

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (npc.spriteDirection == 1)
				spriteEffects = SpriteEffects.FlipHorizontally;

			Texture2D texture2D15 = Main.npcTexture[npc.type];
			Texture2D texture2D16 = ModContent.GetTexture("CalamityMod/NPCs/Polterghast/PolterghastGlow2");
			Vector2 vector11 = new Vector2((float)(Main.npcTexture[npc.type].Width / 2), (float)(Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type] / 2));
			Color color36 = Color.White;
			float amount9 = 0.5f;
			int num153 = 7;

			if (CalamityConfig.Instance.Afterimages)
			{
				for (int num155 = 1; num155 < num153; num155 += 2)
				{
					Color color38 = lightColor;
					color38 = Color.Lerp(color38, color36, amount9);
					color38 = npc.GetAlpha(color38);
					color38 *= (float)(num153 - num155) / 15f;
					Vector2 vector41 = npc.oldPos[num155] + new Vector2((float)npc.width, (float)npc.height) / 2f - Main.screenPosition;
					vector41 -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[npc.type])) * npc.scale / 2f;
					vector41 += vector11 * npc.scale + new Vector2(0f, 4f + npc.gfxOffY);
					spriteBatch.Draw(texture2D15, vector41, npc.frame, color38, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
				}
			}

			Vector2 vector43 = npc.Center - Main.screenPosition;
			vector43 -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[npc.type])) * npc.scale / 2f;
			vector43 += vector11 * npc.scale + new Vector2(0f, 4f + npc.gfxOffY);
			spriteBatch.Draw(texture2D15, vector43, npc.frame, npc.GetAlpha(lightColor), npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			texture2D15 = ModContent.GetTexture("CalamityMod/NPCs/Polterghast/PolterghastGlow");
			Color color37 = Color.Lerp(Color.White, Color.Cyan, 0.5f);
			Color color42 = Color.Lerp(Color.White, Color.Red, 0.5f);

			if (CalamityConfig.Instance.Afterimages)
			{
				for (int num163 = 1; num163 < num153; num163++)
				{
					Color color41 = color37;
					color41 = Color.Lerp(color41, color36, amount9);
					color41 = npc.GetAlpha(color41);
					color41 *= (float)(num153 - num163) / 15f;
					Vector2 vector44 = npc.oldPos[num163] + new Vector2((float)npc.width, (float)npc.height) / 2f - Main.screenPosition;
					vector44 -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[npc.type])) * npc.scale / 2f;
					vector44 += vector11 * npc.scale + new Vector2(0f, 4f + npc.gfxOffY);
					spriteBatch.Draw(texture2D15, vector44, npc.frame, color41, npc.rotation, vector11, npc.scale, spriteEffects, 0f);

					Color color43 = color42;
					color43 = Color.Lerp(color43, color36, amount9);
					color43 = npc.GetAlpha(color43);
					color43 *= (float)(num153 - num163) / 15f;
					spriteBatch.Draw(texture2D16, vector44, npc.frame, color43, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
				}
			}

			spriteBatch.Draw(texture2D15, vector43, npc.frame, color37, npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			spriteBatch.Draw(texture2D16, vector43, npc.frame, color42, npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			return false;
		}

		public override void FindFrame(int frameHeight)
        {
			bool revenge = CalamityWorld.revenge || CalamityWorld.bossRushActive;
			bool death = CalamityWorld.death || CalamityWorld.bossRushActive;
			bool phase2 = npc.life >= npc.lifeMax * (revenge ? (death ? 0.8 : 0.5) : 0.33);
            npc.frameCounter += 1D;
            if (npc.frameCounter > 6D)
            {
                npc.frameCounter = 0D;
                npc.frame.Y += frameHeight;
            }
            if (npc.life >= npc.lifeMax * (death ? 0.9 : 0.75))
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

			player.AddBuff(BuffID.MoonLeech, 900, true);
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
                npc.position.X = npc.position.X + (npc.width / 2);
                npc.position.Y = npc.position.Y + (npc.height / 2);
                npc.width = 90;
                npc.height = 90;
                npc.position.X = npc.position.X - (npc.width / 2);
                npc.position.Y = npc.position.Y - (npc.height / 2);
                for (int num621 = 0; num621 < 10; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.Phantoplasm, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
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
