using CalamityMod.Items.Placeables.Banners;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
namespace CalamityMod.NPCs.SunkenSea
{
    public class SeaFloaty : ModNPC
    {
        private bool hasBeenHit = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sea Floaty");
            Main.npcFrameCount[NPC.type] = 6;
        }

        public override void SetDefaults()
        {
            NPC.npcSlots = 0.5f;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.damage = 5;
            NPC.width = 72;
            NPC.height = 22;
            NPC.defense = 0;
            NPC.lifeMax = 50;
            NPC.knockBackResist = 0.5f;
            NPC.value = Item.buyPrice(0, 0, 0, 50);
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<SeaFloatyBanner>();
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.chaseable);
            writer.Write(hasBeenHit);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.chaseable = reader.ReadBoolean();
            hasBeenHit = reader.ReadBoolean();
        }

        public override void AI()
        {
            if (NPC.velocity.X > 0.25f)
            {
                NPC.spriteDirection = -1;
            }
            else if (NPC.velocity.X < 0.25f)
            {
                NPC.spriteDirection = 1;
            }
            if (NPC.ai[0] == 0f)
            {
                NPC.direction = 1;
                NPC.ai[0] = 1f;
            }
            NPC.velocity.X = NPC.velocity.X + (float)NPC.direction * 0.1f;
            if (NPC.velocity.X < -2.5f || NPC.velocity.X > 2.5f)
            {
                NPC.velocity.X = NPC.velocity.X * 0.95f;
            }
            if (NPC.collideX)
            {
                NPC.velocity.X = NPC.velocity.X * -1f;
                NPC.direction *= -1;
                NPC.netUpdate = true;
            }

            if (NPC.justHit && !hasBeenHit)
            {
                hasBeenHit = true;
                NPC.noTileCollide = true;
                NPC.noGravity = true;
            }
            NPC.chaseable = hasBeenHit;
            if (hasBeenHit)
            {
                NPC.TargetClosest(true);
                NPC.velocity.X = NPC.velocity.X - (float)NPC.direction * 0.5f;
                NPC.velocity.Y = NPC.velocity.Y - (float)NPC.directionY * 0.3f;
                if (NPC.velocity.X > 10f)
                {
                    NPC.velocity.X = 10f;
                }
                if (NPC.velocity.X < -10f)
                {
                    NPC.velocity.X = -10f;
                }
                if (NPC.velocity.Y > 10f)
                {
                    NPC.velocity.Y = 10f;
                }
                if (NPC.velocity.Y < -10f)
                {
                    NPC.velocity.Y = -10f;
                }
                NPC.direction *= -1;
                NPC.rotation = NPC.velocity.X * 0.1f;
                if ((double)NPC.rotation < -0.3)
                {
                    NPC.rotation = -0.3f;
                }
                if ((double)NPC.rotation > 0.3)
                {
                    NPC.rotation = 0.3f;
                    return;
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += hasBeenHit ? 0.3f : 0.15f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.Calamity().ZoneSunkenSea && spawnInfo.Water && !spawnInfo.Player.Calamity().clamity)
            {
                return SpawnCondition.CaveJellyfish.Chance * 0.45f;
            }
            return 0f;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, 68, hitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 25; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, 68, hitDirection, -1f, 0, default, 1f);
                }
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.position, NPC.velocity, Mod.Find<ModGore>("Gores/SeaFloaty/SeaFloatyGore1").Type, 1f);
                    Gore.NewGore(NPC.position, NPC.velocity, Mod.Find<ModGore>("Gores/SeaFloaty/SeaFloatyGore2").Type, 1f);
                    Gore.NewGore(NPC.position, NPC.velocity, Mod.Find<ModGore>("Gores/SeaFloaty/SeaFloatyGore3").Type, 1f);
                }
            }
        }
    }
}
