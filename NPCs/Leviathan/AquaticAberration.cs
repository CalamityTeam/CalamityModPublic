using CalamityMod.Events;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Leviathan
{
	public class AquaticAberration : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aquatic Aberration");
            Main.npcFrameCount[npc.type] = 7;
        }

        public override void SetDefaults()
        {
			npc.Calamity().canBreakPlayerDefense = true;
			npc.aiStyle = -1;
			npc.GetNPCDamage();
			npc.width = 50;
            npc.height = 50;
            npc.defense = 14;
            npc.lifeMax = 800;
            if (BossRushEvent.BossRushActive)
            {
                npc.lifeMax = 10000;
            }
            npc.knockBackResist = 0f;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.canGhostHeal = false;
            aiType = -1;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            banner = npc.type;
            bannerItem = ModContent.ItemType<AquaticAberrationBanner>();
			npc.Calamity().VulnerableToHeat = false;
			npc.Calamity().VulnerableToSickness = true;
			npc.Calamity().VulnerableToElectricity = true;
			npc.Calamity().VulnerableToWater = false;
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
			if (CalamityGlobalNPC.leviathan < 0 || !Main.npc[CalamityGlobalNPC.leviathan].active)
			{
				npc.active = false;
				npc.netUpdate = true;
				return;
			}

			bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
			bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
			bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
			bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;

			npc.TargetClosest(false);

            npc.rotation = npc.velocity.ToRotation();
            if (Math.Sign(npc.velocity.X) != 0)
                npc.spriteDirection = -Math.Sign(npc.velocity.X);
            if (npc.rotation < -MathHelper.PiOver2)
                npc.rotation += MathHelper.Pi;
            if (npc.rotation > MathHelper.PiOver2)
                npc.rotation -= MathHelper.Pi;
            npc.spriteDirection = Math.Sign(npc.velocity.X);

			// Percent life remaining
			float lifeRatio = Main.npc[CalamityGlobalNPC.leviathan].life / (float)Main.npc[CalamityGlobalNPC.leviathan].lifeMax;

			// Phases
			bool leviathanInPhase4 = lifeRatio < 0.2f;

			bool sirenAlive = false;
			if (CalamityGlobalNPC.siren != -1)
				sirenAlive = Main.npc[CalamityGlobalNPC.siren].active;

			if (CalamityGlobalNPC.siren != -1)
			{
				if (Main.npc[CalamityGlobalNPC.siren].active)
				{
					if (Main.npc[CalamityGlobalNPC.siren].damage == 0)
						sirenAlive = false;
				}
			}

			float inertia = malice ? 24f : death ? 26f : revenge ? 27f : expertMode ? 28f : 30f;
			if (!sirenAlive || leviathanInPhase4)
				inertia *= 0.75f;

            float num1006 = 0.111111117f * inertia;
            if (npc.ai[0] == 0f)
            {
                float scaleFactor6 = malice ? 14f : death ? 12f : revenge ? 11f : expertMode ? 10f : 8f;
				if (!sirenAlive || leviathanInPhase4)
					scaleFactor6 *= 1.25f;

                Vector2 center4 = npc.Center;
                Vector2 center5 = Main.player[npc.target].Center;
                Vector2 vector126 = center5 - center4;
                Vector2 vector127 = vector126 - Vector2.UnitY * 300f;
                float num1013 = vector126.Length();
                vector126 = Vector2.Normalize(vector126) * scaleFactor6;
                vector127 = Vector2.Normalize(vector127) * scaleFactor6;
                bool flag64 = Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1);
                if (npc.ai[3] >= 120f)
                {
                    flag64 = true;
                }
                float num1014 = 8f;
                flag64 = flag64 && vector126.ToRotation() > MathHelper.Pi / num1014 && vector126.ToRotation() < MathHelper.Pi - MathHelper.Pi / num1014;
                if (num1013 > 800f || !flag64)
                {
                    npc.velocity.X = (npc.velocity.X * (inertia - 1f) + vector127.X) / inertia;
                    npc.velocity.Y = (npc.velocity.Y * (inertia - 1f) + vector127.Y) / inertia;
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
                    velocity *= (!sirenAlive || leviathanInPhase4) ? 12f : 10f;
                    npc.velocity = velocity;
                }
            }
            else if (npc.ai[0] == 2f)
            {
                npc.ai[1] += 1f;
				bool flag65 = npc.Center.Y + 50f > Main.player[npc.target].Center.Y;
				if ((npc.ai[1] >= 90f && flag65) || npc.velocity.Length() < ((!sirenAlive || leviathanInPhase4) ? 10f : 8f))
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
                    Vector2 center6 = npc.Center;
                    Vector2 center7 = Main.player[npc.target].Center;
                    Vector2 vec2 = center7 - center6;
                    vec2.Normalize();
                    if (vec2.HasNaNs())
                    {
                        vec2 = new Vector2((float)npc.direction, 0f);
                    }
                    npc.velocity = (npc.velocity * (inertia - 1f) + vec2 * (npc.velocity.Length() + num1006)) / inertia;
                }
            }
            else if (npc.ai[0] == 3f)
            {
                npc.ai[1] -= (!sirenAlive || leviathanInPhase4) ? 1.5f : 1f;
                if (npc.ai[1] <= 0f)
                {
                    npc.ai[0] = 0f;
                    npc.ai[1] = 0f;
                    npc.netUpdate = true;
                }
                npc.velocity *= 0.98f;
            }

            if (death)
            {
                float pushVelocity = 0.5f;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].active)
                    {
                        if (i != npc.whoAmI && Main.npc[i].type == npc.type)
                        {
                            if (Vector2.Distance(npc.Center, Main.npc[i].Center) < 80f)
                            {
                                if (npc.position.X < Main.npc[i].position.X)
                                    npc.velocity.X -= pushVelocity;
                                else
                                    npc.velocity.X += pushVelocity;

                                if (npc.position.Y < Main.npc[i].position.Y)
                                    npc.velocity.Y -= pushVelocity;
                                else
                                    npc.velocity.Y += pushVelocity;
                            }
                        }
                    }
                }
            }
        }

		public override bool PreNPCLoot()
		{
			if (!CalamityWorld.revenge)
			{
				int closestPlayer = Player.FindClosest(npc.Center, 1, 1);
				if (Main.rand.Next(4) == 0 && Main.player[closestPlayer].statLife < Main.player[closestPlayer].statLifeMax2)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Heart);
			}

			return false;
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.playerSafe || spawnInfo.player.Calamity().ZoneSulphur || (!NPC.downedPlantBoss && !CalamityWorld.downedCalamitas))
			{
				return 0f;
			}
			return SpawnCondition.OceanMonster.Chance * 0.02f;
		}

		public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(BuffID.Bleeding, 120, true);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
                }
            }
        }
    }
}
