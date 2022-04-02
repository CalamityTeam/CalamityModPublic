using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class WulfrumHovercraft : ModNPC
    {
        internal enum HovercraftAIState
        {
            Searching = 0,
            Hover = 1,
            Slowdown = 2,
            SwoopDownward = 3
        }

        public float StunTime;

        internal HovercraftAIState AIState
        {
            get => (HovercraftAIState)(int)npc.ai[0];
            set => npc.ai[0] = (int)value;
        }

        public float SubphaseTime
        {
            get => npc.ai[1];
            set => npc.ai[1] = value;
        }

        public float SearchDirection
        {
            get => npc.ai[2];
            set => npc.ai[2] = value;
        }

        public float SuperchargeTimer
        {
            get => npc.ai[3];
            set => npc.ai[3] = value;
        }

        public bool Supercharged => SuperchargeTimer > 0;
        public ref float FlyAwayTimer => ref npc.localAI[0];

        public const float StunTimeMax = 45f;
        public const float SearchXOffset = 345f;
        public const float TotalSubphaseTime = 110f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wulfrum Hovercraft");
            Main.npcFrameCount[npc.type] = 12;
        }

        public override void SetDefaults()
        {
            aiType = -1;
            npc.aiStyle = -1;
            npc.damage = 15;
            npc.width = 40;
            npc.height = 38;
            npc.defense = 4;
            npc.lifeMax = 20;
            npc.value = Item.buyPrice(0, 0, 1, 50);
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath14;
            npc.noGravity = true;
            npc.noTileCollide = true;
            banner = npc.type;
            bannerItem = ModContent.ItemType<WulfrumHovercraftBanner>();
            npc.Calamity().VulnerableToSickness = false;
            npc.Calamity().VulnerableToElectricity = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(StunTime);
            writer.Write(FlyAwayTimer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            StunTime = reader.ReadSingle();
            FlyAwayTimer = reader.ReadSingle();
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;
            int frame = (int)(npc.frameCounter / 5) % (Main.npcFrameCount[npc.type] / 2);
            if (Supercharged)
                frame += Main.npcFrameCount[npc.type] / 2;

            npc.frame.Y = frame * frameHeight;
        }

        public override void AI()
        {
            npc.knockBackResist = 0.1f;

            Player player = Main.player[npc.target];

            bool farFromPlayer = npc.Distance(player.Center) > 960f;
            bool obstanceInFrontOfPlayer = !Collision.CanHitLine(npc.position, npc.width, npc.height, player.position, player.width, player.height);

            if (npc.target < 0 || npc.target >= 255 || farFromPlayer || obstanceInFrontOfPlayer || player.dead || !player.active)
            {
                npc.TargetClosest(false);
                player = Main.player[npc.target];
                farFromPlayer = npc.Distance(player.Center) > 960f;
                obstanceInFrontOfPlayer = !Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height);
                // Fly away if there is no living target, or the closest target is too far away.
                if (player.dead || !player.active || farFromPlayer || obstanceInFrontOfPlayer)
                {
                    if (FlyAwayTimer > 360)
                    {
                        npc.velocity = Vector2.Lerp(npc.velocity, Vector2.UnitY * -8f, 0.1f);
                        npc.rotation = npc.rotation.AngleTowards(0f, MathHelper.ToRadians(15f));
                        npc.noTileCollide = true;
                    }
                    else
                    {
                        npc.velocity *= 0.96f;
                        npc.rotation = npc.rotation.AngleTowards(0f, MathHelper.ToRadians(15f));
                        FlyAwayTimer++;
                    }
                    return;
                }
            }

            FlyAwayTimer = Utils.Clamp(FlyAwayTimer - 3, 0, 180);

            npc.noTileCollide = !farFromPlayer;

            Lighting.AddLight(npc.Center - Vector2.UnitY * 8f, Color.Lime.ToVector3() * 1.5f);

            if (StunTime > 0)
            {
                if (!Main.dedServ && Main.rand.NextBool(4))
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Dust.NewDustPerfect(npc.Center + Main.rand.NextVector2Circular(8f, 8f), 226).scale = 0.7f;
                    }
                }

                npc.rotation = npc.rotation.AngleTowards(0f, MathHelper.ToRadians(15f));
                if (StunTime > StunTimeMax - 15)
                    npc.velocity *= 0.6f;
                else
                    npc.knockBackResist = 2.4f;

                StunTime--;
                return;
            }

            if (SearchDirection == 0f)
            {
                if (Math.Abs(player.Center.X + SearchXOffset - npc.Center.X) < Math.Abs(player.Center.X - SearchXOffset - npc.Center.X))
                    SearchDirection = 1f;
                else
                    SearchDirection = -1f;

                npc.netUpdate = true;
            }

            if (AIState == HovercraftAIState.Searching || AIState == HovercraftAIState.Hover)
            {
                Vector2 destination = player.Center + new Vector2(SearchXOffset * SearchDirection, -160f);
                npc.velocity = npc.SafeDirectionTo(destination, Vector2.UnitY) * (Supercharged ? 7f : 5f);
                if (AIState == HovercraftAIState.Hover)
                {
                    destination = player.Center + new Vector2(SearchXOffset * -SearchDirection, -160f);
                    npc.velocity = npc.SafeDirectionTo(destination, Vector2.UnitY) * (Supercharged ? 5.5f : 4f);
                }

                npc.rotation = npc.velocity.X / 16f;
                if (npc.Distance(destination) < 50f)
                {
                    if (AIState == HovercraftAIState.Searching)
                    {
                        AIState = HovercraftAIState.Slowdown;
                    }
                    else
                    {
                        AIState = HovercraftAIState.SwoopDownward;
                    }
                    npc.netUpdate = true;
                }
            }

            if (AIState == HovercraftAIState.Slowdown)
            {
                SubphaseTime++;
                if (SubphaseTime < 30f)
                {
                    npc.velocity *= 0.96f;
                }
                else
                {
                    AIState = HovercraftAIState.Hover;
                    SubphaseTime = 0f;
                    npc.netUpdate = true;
                }
            }

            if (AIState == HovercraftAIState.SwoopDownward)
            {
                npc.rotation = 0f;
                float swoopType = Supercharged ? TotalSubphaseTime - 40f : TotalSubphaseTime;
                float swoopSlowdownTime = Supercharged ? 10f : 45f;
                Vector2 swoopVelocity = Vector2.UnitY.RotatedBy(MathHelper.Pi * SubphaseTime / swoopType * -SearchDirection) * (Supercharged ? 11f : 8.5f);

                SubphaseTime++;
                if (SubphaseTime < swoopSlowdownTime)
                {
                    swoopVelocity *= MathHelper.Lerp(1f, 0.75f, Utils.InverseLerp(45f, 0f, SubphaseTime));
                }
                if (SubphaseTime >= swoopType - swoopSlowdownTime)
                {
                    swoopVelocity *= MathHelper.Lerp(1f, 0.75f, Utils.InverseLerp(swoopType - 45f, swoopType, SubphaseTime));
                }
                swoopVelocity.Y *= 0.5f;

                npc.velocity = Vector2.Lerp(npc.velocity, swoopVelocity, 0.2f);

                if (SubphaseTime >= swoopType)
                {
                    AIState = HovercraftAIState.Searching;
                    SearchDirection = 0f;
                    SubphaseTime = 0f;
                    npc.netUpdate = true;
                }

                npc.rotation = npc.velocity.X / 12f;
            }

            npc.spriteDirection = (npc.velocity.X < 0).ToDirectionInt();
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            float pylonMult = NPC.AnyNPCs(ModContent.NPCType<WulfrumPylon>()) ? 5.5f : 1f;
            if (spawnInfo.playerSafe || spawnInfo.player.Calamity().ZoneSulphur)
                return 0f;

            return SpawnCondition.OverworldDaySlime.Chance * (Main.hardMode ? 0.015f : 0.1f) * pylonMult;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (!Main.dedServ)
            {
                for (int k = 0; k < 5; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 3, hitDirection, -1f, 0, default, 1f);
                }
                if (npc.life <= 0)
                {
                    for (int k = 0; k < 20; k++)
                    {
                        Dust.NewDust(npc.position, npc.width, npc.height, 3, hitDirection, -1f, 0, default, 1f);
                    }
                }
            }
        }

        public override void OnHitByItem(Player player, Item item, int damage, float knockback, bool crit)
        {
            StunTime = StunTimeMax;
            npc.netUpdate = true;
        }

        public override void NPCLoot()
        {
            DropHelper.DropItem(npc, ModContent.ItemType<WulfrumShard>(), 2, 3);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<EnergyCore>(), Supercharged);
            DropHelper.DropItemChance(npc, ModContent.ItemType<WulfrumBattery>(), 0.07f);
        }
    }
}
