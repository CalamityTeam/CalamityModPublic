using CalamityMod.Events;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.HiveMind
{
    public class DarkHeart : ModNPC
    {
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NeedsExpertScaling[Type] = true;
            Main.npcFrameCount[Type] = 4;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }

        public override void SetDefaults()
        {
            NPC.damage = 0; // No contact damage
            NPC.width = 32;
            NPC.height = 32;
            NPC.defense = 2;

            NPC.lifeMax = 75;
            if (BossRushEvent.BossRushActive)
                NPC.lifeMax = 1800;
            if (Main.getGoodWorld)
                NPC.lifeMax *= 3;

            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0.4f;
            NPC.noGravity = true;
            NPC.HitSound = SoundID.NPCHit13;
            NPC.DeathSound = SoundID.NPCDeath21;
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToSickness = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[ModContent.NPCType<HiveMind>()], quickUnlock: true);
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.UndergroundCorruption,
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.DarkHeart")
            });
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.15f;
            NPC.frameCounter %= Main.npcFrameCount[Type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override void AI()
        {
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            // Float around the player
            NPC.rotation = NPC.velocity.X / 20f;

            // Get a target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            // Despawn if Hive Mind isn't present
            if (CalamityGlobalNPC.hiveMind == -1)
            {
                NPC.life = 0;
                NPC.HitEffect();
                NPC.checkDead();
                NPC.active = false;
            }

            float velocity = Main.getGoodWorld ? 10f : death ? 7f : revenge ? 6f : 4f;
            float acceleration = Main.getGoodWorld ? 0.5f : death ? 0.35f : revenge ? 0.3f : 0.2f;
            float deceleration = Main.getGoodWorld ? 0.9f : death ? 0.95f : revenge ? 0.96f : 0.98f;

            if (NPC.position.Y > Main.player[NPC.target].position.Y - 400f)
            {
                if (NPC.velocity.Y > 0f)
                    NPC.velocity.Y *= deceleration;

                NPC.velocity.Y = MathHelper.Clamp(NPC.velocity.Y - acceleration, -velocity * 1.5f, velocity);
            }
            else if (NPC.position.Y < Main.player[NPC.target].position.Y - 450f)
            {
                if (NPC.velocity.Y < 0f)
                    NPC.velocity.Y *= deceleration;

                NPC.velocity.Y = MathHelper.Clamp(NPC.velocity.Y + acceleration, -velocity, velocity * 1.5f);
            }

            bool dropRain = NPC.Bottom.Y < Main.player[NPC.target].position.Y - 350f && Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height);
            float distanceX = death ? 200f : 400f;
            if (NPC.Center.X > Main.player[NPC.target].Center.X + distanceX)
            {
                dropRain = false;

                if (NPC.velocity.X > 0f)
                    NPC.velocity.X *= deceleration;

                NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X - acceleration, -velocity * 1.5f, velocity);
            }
            if (NPC.Center.X < Main.player[NPC.target].Center.X - distanceX)
            {
                dropRain = false;

                if (NPC.velocity.X < 0f)
                    NPC.velocity.X *= deceleration;

                NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X + acceleration, -velocity, velocity * 1.5f);
            }

            if (dropRain && Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.ai[0] += 1f;
                float rainDropRate = Main.getGoodWorld ? 10f : death ? 15f : revenge ? 20f : 30f;
                if (NPC.ai[0] >= rainDropRate)
                {
                    NPC.ai[0] = 0f;
                    int shaderainXPos = (int)(NPC.position.X + 10f + Main.rand.Next(NPC.width - 20));
                    int shaderainYos = (int)(NPC.position.Y + NPC.height + 4f);
                    int type = ModContent.ProjectileType<ShaderainHostile>();
                    float randomXVelocity = Main.getGoodWorld ? Main.rand.NextFloat() * 5f : 0f;
                    float velocityY = 8f;
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), shaderainXPos, shaderainYos, randomXVelocity, velocityY, type, HiveMind.ShaderainDamage, 0f, Main.myPlayer);
                }
            }
        }

        public override void OnKill()
        {
            int closestPlayer = Player.FindClosest(NPC.Center, 1, 1);
            if (Main.player[closestPlayer].statLife < Main.player[closestPlayer].statLifeMax2)
                Item.NewItem(NPC.GetSource_Loot(), (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ItemID.Heart);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 3; k++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Demonite, hit.HitDirection, -1f, 0, default, 1f);

            if (NPC.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Demonite, hit.HitDirection, -1f, 0, default, 1f);
            }
        }
    }
}
