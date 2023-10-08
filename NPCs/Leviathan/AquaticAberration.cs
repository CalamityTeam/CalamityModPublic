using CalamityMod.Events;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace CalamityMod.NPCs.Leviathan
{
    public class AquaticAberration : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 7;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0);
            value.Position.X += 25;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.aiStyle = -1;
            NPC.GetNPCDamage();
            NPC.width = 50;
            NPC.height = 50;
            NPC.defense = 14;
            NPC.lifeMax = 600;
            if (BossRushEvent.BossRushActive)
            {
                NPC.lifeMax = 10000;
            }
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.canGhostHeal = false;
            AIType = -1;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToSickness = true;
            NPC.Calamity().VulnerableToElectricity = true;
            NPC.Calamity().VulnerableToWater = false;

            if (Main.getGoodWorld)
                NPC.scale *= 1.3f;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            int associatedNPCType = ModContent.NPCType<Leviathan>();
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[associatedNPCType], quickUnlock: true);

            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Ocean,
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.AquaticAberration")
            });
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.15f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override void AI()
        {
            if (CalamityGlobalNPC.leviathan < 0 || !Main.npc[CalamityGlobalNPC.leviathan].active)
            {
                NPC.life = 0;
                NPC.HitEffect();
                NPC.active = false;
                NPC.netUpdate = true;
                return;
            }

            bool bossRush = BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || bossRush;
            bool revenge = CalamityWorld.revenge || bossRush;
            bool expertMode = Main.expertMode || bossRush;

            NPC.TargetClosest(false);

            NPC.rotation = NPC.velocity.ToRotation();
            if (Math.Sign(NPC.velocity.X) != 0)
                NPC.spriteDirection = -Math.Sign(NPC.velocity.X);
            if (NPC.rotation < -MathHelper.PiOver2)
                NPC.rotation += MathHelper.Pi;
            if (NPC.rotation > MathHelper.PiOver2)
                NPC.rotation -= MathHelper.Pi;
            NPC.spriteDirection = Math.Sign(NPC.velocity.X);

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

            float inertia = bossRush ? 24f : death ? 26f : revenge ? 27f : expertMode ? 28f : 30f;
            if (!sirenAlive || leviathanInPhase4)
                inertia *= 0.75f;

            float num1006 = 0.111111117f * inertia;
            if (NPC.ai[0] == 0f)
            {
                float scaleFactor6 = bossRush ? 14f : death ? 12f : revenge ? 11f : expertMode ? 10f : 8f;
                if (!sirenAlive || leviathanInPhase4)
                    scaleFactor6 *= 1.25f;

                Vector2 center4 = NPC.Center;
                Vector2 center5 = Main.player[NPC.target].Center;
                Vector2 vector126 = center5 - center4;
                Vector2 vector127 = vector126 - Vector2.UnitY * 300f * NPC.scale;
                float num1013 = vector126.Length();
                vector126 = Vector2.Normalize(vector126) * scaleFactor6;
                vector127 = Vector2.Normalize(vector127) * scaleFactor6;
                bool flag64 = Collision.CanHit(NPC.Center, 1, 1, Main.player[NPC.target].Center, 1, 1);
                if (NPC.ai[3] >= 120f)
                {
                    flag64 = true;
                }
                float num1014 = 8f;
                flag64 = flag64 && vector126.ToRotation() > MathHelper.Pi / num1014 && vector126.ToRotation() < MathHelper.Pi - MathHelper.Pi / num1014;
                if (num1013 > 800f * NPC.scale || !flag64)
                {
                    NPC.velocity.X = (NPC.velocity.X * (inertia - 1f) + vector127.X) / inertia;
                    NPC.velocity.Y = (NPC.velocity.Y * (inertia - 1f) + vector127.Y) / inertia;
                    if (!flag64)
                    {
                        NPC.ai[3] += 1f;
                        if (NPC.ai[3] == 120f)
                        {
                            NPC.netUpdate = true;
                        }
                    }
                    else
                    {
                        NPC.ai[3] = 0f;
                    }
                }
                else
                {
                    NPC.ai[0] = 1f;
                    NPC.ai[2] = vector126.X;
                    NPC.ai[3] = vector126.Y;
                    NPC.netUpdate = true;
                }
            }
            else if (NPC.ai[0] == 1f)
            {
                NPC.velocity *= 0.8f;
                NPC.ai[1] += 1f;
                if (NPC.ai[1] >= 5f)
                {
                    NPC.ai[0] = 2f;
                    NPC.ai[1] = 0f;
                    NPC.netUpdate = true;
                    Vector2 velocity = new Vector2(NPC.ai[2], NPC.ai[3]);
                    velocity.Normalize();
                    velocity *= (!sirenAlive || leviathanInPhase4) ? 12f : 10f;
                    NPC.velocity = velocity;
                }
            }
            else if (NPC.ai[0] == 2f)
            {
                NPC.ai[1] += 1f;
                bool flag65 = NPC.Center.Y + 50f > Main.player[NPC.target].Center.Y;
                if ((NPC.ai[1] >= 90f && flag65) || NPC.velocity.Length() < ((!sirenAlive || leviathanInPhase4) ? 10f : 8f))
                {
                    NPC.ai[0] = 3f;
                    NPC.ai[1] = 45f;
                    NPC.ai[2] = 0f;
                    NPC.ai[3] = 0f;
                    NPC.velocity /= 2f;
                    NPC.netUpdate = true;
                }
                else
                {
                    Vector2 center6 = NPC.Center;
                    Vector2 center7 = Main.player[NPC.target].Center;
                    Vector2 vec2 = center7 - center6;
                    vec2.Normalize();
                    if (vec2.HasNaNs())
                    {
                        vec2 = new Vector2((float)NPC.direction, 0f);
                    }
                    NPC.velocity = (NPC.velocity * (inertia - 1f) + vec2 * (NPC.velocity.Length() + num1006)) / inertia;
                }
            }
            else if (NPC.ai[0] == 3f)
            {
                NPC.ai[1] -= (!sirenAlive || leviathanInPhase4) ? 1.5f : 1f;
                if (NPC.ai[1] <= 0f)
                {
                    NPC.ai[0] = 0f;
                    NPC.ai[1] = 0f;
                    NPC.netUpdate = true;
                }
                NPC.velocity *= 0.98f;
            }

            if (death)
            {
                float pushVelocity = 0.5f;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].active)
                    {
                        if (i != NPC.whoAmI && Main.npc[i].type == NPC.type)
                        {
                            if (Vector2.Distance(NPC.Center, Main.npc[i].Center) < 80f * NPC.scale)
                            {
                                if (NPC.position.X < Main.npc[i].position.X)
                                    NPC.velocity.X -= pushVelocity;
                                else
                                    NPC.velocity.X += pushVelocity;

                                if (NPC.position.Y < Main.npc[i].position.Y)
                                    NPC.velocity.Y -= pushVelocity;
                                else
                                    NPC.velocity.Y += pushVelocity;
                            }
                        }
                    }
                }
            }
        }

        public override void OnKill()
        {
            int closestPlayer = Player.FindClosest(NPC.Center, 1, 1);
            if (Main.rand.NextBool(4) && Main.player[closestPlayer].statLife < Main.player[closestPlayer].statLifeMax2)
                Item.NewItem(NPC.GetSource_Loot(), (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ItemID.Heart);

            // Explode into bubbles on gfb
            if (Main.zenithWorld)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = 0; i < Main.rand.Next(1, 5); i++)
                    {
                        int spawn = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, NPCID.DetonatingBubble);
                        Main.npc[spawn].target = NPC.target;
                        Main.npc[spawn].velocity = new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7));
                        Main.npc[spawn].netUpdate = true;
                        Main.npc[spawn].ai[3] = Main.rand.Next(80, 121) / 100f;
                    }
                }
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(BuffID.Bleeding, 240, true);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);
                }
            }
        }
    }
}
