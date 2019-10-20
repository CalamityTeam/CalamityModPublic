using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.Items.Placeables.Banners;
namespace CalamityMod.NPCs
{
    public class SeaFloaty : ModNPC
    {
        private bool hasBeenHit = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sea Floaty");
            Main.npcFrameCount[npc.type] = 6;
        }

        public override void SetDefaults()
        {
            npc.npcSlots = 0.5f;
            npc.aiStyle = -1;
            aiType = -1;
            npc.damage = 5;
            npc.width = 72;
            npc.height = 22;
            npc.defense = 0;
            npc.lifeMax = 50;
            npc.knockBackResist = 0.5f;
            npc.value = Item.buyPrice(0, 0, 0, 50);
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            banner = npc.type;
            bannerItem = ModContent.ItemType<SeaFloatyBanner>();
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(npc.chaseable);
            writer.Write(hasBeenHit);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            npc.chaseable = reader.ReadBoolean();
            hasBeenHit = reader.ReadBoolean();
        }

        public override void AI()
        {
            if (npc.velocity.X > 0.25f)
            {
                npc.spriteDirection = -1;
            }
            else if (npc.velocity.X < 0.25f)
            {
                npc.spriteDirection = 1;
            }
            if (npc.ai[0] == 0f)
            {
                npc.direction = 1;
                npc.ai[0] = 1f;
            }
            npc.velocity.X = npc.velocity.X + (float)npc.direction * 0.1f;
            if (npc.velocity.X < -2.5f || npc.velocity.X > 2.5f)
            {
                npc.velocity.X = npc.velocity.X * 0.95f;
            }
            if (npc.collideX)
            {
                npc.velocity.X = npc.velocity.X * -1f;
                npc.direction *= -1;
                npc.netUpdate = true;
            }

            if (npc.justHit && !hasBeenHit)
            {
                hasBeenHit = true;
                npc.noTileCollide = true;
                npc.noGravity = true;
            }
            npc.chaseable = hasBeenHit;
            if (hasBeenHit)
            {
                npc.TargetClosest(true);
                npc.velocity.X = npc.velocity.X - (float)npc.direction * 0.5f;
                npc.velocity.Y = npc.velocity.Y - (float)npc.directionY * 0.3f;
                if (npc.velocity.X > 10f)
                {
                    npc.velocity.X = 10f;
                }
                if (npc.velocity.X < -10f)
                {
                    npc.velocity.X = -10f;
                }
                if (npc.velocity.Y > 10f)
                {
                    npc.velocity.Y = 10f;
                }
                if (npc.velocity.Y < -10f)
                {
                    npc.velocity.Y = -10f;
                }
                npc.direction *= -1;
                npc.rotation = npc.velocity.X * 0.1f;
                if ((double)npc.rotation < -0.3)
                {
                    npc.rotation = -0.3f;
                }
                if ((double)npc.rotation > 0.3)
                {
                    npc.rotation = 0.3f;
                    return;
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += hasBeenHit ? 0.3f : 0.15f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.player.Calamity().ZoneSunkenSea && spawnInfo.water && !spawnInfo.player.Calamity().clamity)
            {
                return SpawnCondition.CaveJellyfish.Chance * 0.45f;
            }
            return 0f;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 68, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 25; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 68, hitDirection, -1f, 0, default, 1f);
                }
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/SeaFloaty/SeaFloatyGore1"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/SeaFloaty/SeaFloatyGore2"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/SeaFloaty/SeaFloatyGore3"), 1f);
            }
        }
    }
}
