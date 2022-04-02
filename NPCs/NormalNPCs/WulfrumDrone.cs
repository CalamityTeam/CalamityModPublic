using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.IO;

namespace CalamityMod.NPCs.NormalNPCs
{
    internal enum DroneAIState
    {
        Searching = 0,
        Charging = 1
    }

    public class WulfrumDrone : ModNPC
    {
        internal DroneAIState AIState
        {
            get => (DroneAIState)(int)npc.ai[0];
            set => npc.ai[0] = (int)value;
        }

        public float HorizontalChargeTime
        {
            get => npc.ai[1];
            set => npc.ai[1] = value;
        }

        public float Time
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
        public const float TotalHorizontalChargeTime = 75f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wulfrum Drone");
            Main.npcFrameCount[npc.type] = 6;
        }

        public override void SetDefaults()
        {
            aiType = -1;
            npc.aiStyle = -1;
            npc.damage = 16;
            npc.width = 32;
            npc.height = 32;
            npc.defense = 4;
            npc.lifeMax = 21;
            npc.knockBackResist = 0.35f;
            npc.value = Item.buyPrice(0, 0, 1, 20);
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath14;
            banner = npc.type;
            bannerItem = ModContent.ItemType<WulfrumDroneBanner>();
            npc.Calamity().VulnerableToSickness = false;
            npc.Calamity().VulnerableToElectricity = true;
        }

        public override void SendExtraAI(BinaryWriter writer) => writer.Write(FlyAwayTimer);

        public override void ReceiveExtraAI(BinaryReader reader) => FlyAwayTimer = reader.ReadSingle();

        public override void AI()
        {
            npc.TargetClosest(false);

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

            if (Supercharged)
            {
                SuperchargeTimer--;
            }

            if (AIState == DroneAIState.Searching)
            {
                if (npc.direction == 0)
                    npc.direction = 1;

                Vector2 destination = player.Center + new Vector2(300f * npc.direction, -90f);
                npc.velocity = Vector2.Lerp(npc.velocity, npc.SafeDirectionTo(destination) * 6f, 0.1f);
                if (npc.Distance(destination) < 40f)
                {
                    Time++;
                    npc.velocity *= 0.95f;
                    if (Time >= 40f)
                    {
                        AIState = DroneAIState.Charging;
                        npc.netUpdate = true;
                    }
                }
            }
            else
            {
                if (HorizontalChargeTime < 25)
                    npc.velocity = Vector2.Lerp(npc.velocity, npc.SafeDirectionTo(player.Center) * 6f, 0.1f);

                if (Supercharged && Main.netMode != NetmodeID.MultiplayerClient && HorizontalChargeTime % 30f == 29f)
                    Projectile.NewProjectile(npc.Center + Vector2.UnitX * 6f * npc.spriteDirection, npc.SafeDirectionTo(player.Center, Vector2.UnitY) * 6f, ProjectileID.SaucerLaser, 12, 0f);

                HorizontalChargeTime++;
                if (HorizontalChargeTime > TotalHorizontalChargeTime)
                {
                    AIState = DroneAIState.Searching;
                    HorizontalChargeTime = 0f;
                    npc.direction = (player.Center.X - npc.Center.X < 0).ToDirectionInt();
                    npc.netUpdate = true;
                }
            }

            npc.spriteDirection = (npc.velocity.X < 0).ToDirectionInt();
            npc.rotation = npc.velocity.X / 25f;

            // Generate idle dust
            if (!Main.dedServ)
            {
                Dust dust = Dust.NewDustPerfect(npc.Bottom, 229);
                dust.color = Color.Green;
                dust.scale = 0.675f;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;
            int frame = (int)(npc.frameCounter / 5) % (Main.npcFrameCount[npc.type] / 2);
            if (Supercharged)
                frame += Main.npcFrameCount[npc.type] / 2;

            npc.frame.Y = frame * frameHeight;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            float pylonMult = NPC.AnyNPCs(ModContent.NPCType<WulfrumPylon>()) ? 5.5f : 1f;
            if (spawnInfo.playerSafe || spawnInfo.player.Calamity().ZoneSulphur)
                return 0f;

            return SpawnCondition.OverworldDaySlime.Chance * (Main.hardMode ? 0.010f : 0.1f) * pylonMult;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.GrassBlades, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 15; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.GrassBlades, hitDirection, -1f, 0, default, 1f);
                }
            }
        }

        public override void NPCLoot()
        {
            DropHelper.DropItem(npc, ModContent.ItemType<WulfrumShard>(), 1, 3);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<WulfrumShard>(), Main.expertMode);
            DropHelper.DropItemChance(npc, ModContent.ItemType<WulfrumBattery>(), 0.07f);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<EnergyCore>(), Supercharged);
        }
    }
}
