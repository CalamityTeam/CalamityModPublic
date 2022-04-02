using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Potions;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class Frogfish : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Frogfish");
            Main.npcFrameCount[npc.type] = 4;
        }

        public override void SetDefaults()
        {
            npc.chaseable = false;
            npc.damage = 25;
            npc.width = 60;
            npc.height = 50;
            npc.defense = 10;
            npc.lifeMax = 80;
            npc.aiStyle = -1;
            aiType = -1;
            npc.value = Item.buyPrice(0, 0, 0, 80);
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.knockBackResist = 0.5f;
            banner = npc.type;
            bannerItem = ModContent.ItemType<FrogfishBanner>();
            npc.chaseable = false;
            npc.Calamity().VulnerableToHeat = false;
            npc.Calamity().VulnerableToSickness = false;
            npc.Calamity().VulnerableToElectricity = true;
            npc.Calamity().VulnerableToWater = false;
        }

        public override void AI()
        {
            npc.spriteDirection = (npc.direction > 0) ? 1 : -1;
            int num = 200;
            if (npc.ai[2] == 0f)
            {
                npc.alpha = num;
                npc.TargetClosest(true);
                if (!Main.player[npc.target].dead && (Main.player[npc.target].Center - npc.Center).Length() < 170f &&
                    Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                {
                    npc.ai[2] = -16f;
                }
                if (npc.velocity.X != 0f || npc.velocity.Y < 0f || npc.velocity.Y > 2f || npc.justHit)
                {
                    npc.ai[2] = -16f;
                }
                return;
            }
            if (npc.ai[2] < 0f)
            {
                if (npc.alpha > 0)
                {
                    npc.alpha -= num / 16;
                    if (npc.alpha < 0)
                    {
                        npc.alpha = 0;
                    }
                }
                npc.ai[2] += 1f;
                if (npc.ai[2] == 0f)
                {
                    npc.ai[2] = 1f;
                    npc.velocity.X = (float)(npc.direction * 2);
                }
                return;
            }
            npc.alpha = 0;
            if (npc.ai[2] == 1f)
            {
                npc.chaseable = true;
                CalamityAI.PassiveSwimmingAI(npc, mod, 0, 0f, 0.15f, 0.15f, 3.5f, 1.5f, 0.1f);
            }
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(BuffID.Venom, 180, true);
        }

        public override void FindFrame(int frameHeight)
        {
            if (!npc.wet)
            {
                npc.frameCounter = 0.0;
                return;
            }
            npc.frameCounter += 0.15f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.playerSafe || spawnInfo.player.Calamity().ZoneSulphur)
            {
                return 0f;
            }
            return SpawnCondition.OceanMonster.Chance * 0.2f;
        }

        public override void NPCLoot()
        {
            DropHelper.DropItemChance(npc, ModContent.ItemType<AnechoicCoating>(), 2, 1, 1);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 25; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
                }
            }
        }
    }
}
