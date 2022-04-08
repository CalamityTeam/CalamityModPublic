using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Potions;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.SulphurousSea
{
    public class Flounder : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flounder");
            Main.npcFrameCount[NPC.type] = 4;
        }

        public override void SetDefaults()
        {
            NPC.chaseable = false;
            NPC.damage = 10;
            NPC.width = 42;
            NPC.height = 32;
            NPC.defense = 15;
            NPC.lifeMax = 40;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.value = Item.buyPrice(0, 0, 0, 80);
            NPC.HitSound = SoundID.NPCHit50;
            NPC.DeathSound = SoundID.NPCDeath53;
            NPC.knockBackResist = 0.35f;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<FlounderBanner>();
            NPC.chaseable = false;
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToElectricity = true;
            NPC.Calamity().VulnerableToWater = false;
        }

        public override void AI()
        {
            NPC.spriteDirection = (NPC.direction > 0) ? 1 : -1;
            int num = 200;
            if (NPC.ai[2] == 0f)
            {
                NPC.alpha = num;
                NPC.TargetClosest(true);
                if (!Main.player[NPC.target].dead && (Main.player[NPC.target].Center - NPC.Center).Length() < 170f &&
                    Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height))
                {
                    NPC.ai[2] = -16f;
                }
                if (NPC.velocity.X != 0f || NPC.velocity.Y < 0f || NPC.velocity.Y > 2f || NPC.justHit)
                {
                    NPC.ai[2] = -16f;
                }
                return;
            }
            if (NPC.ai[2] < 0f)
            {
                if (NPC.alpha > 0)
                {
                    NPC.alpha -= num / 16;
                    if (NPC.alpha < 0)
                    {
                        NPC.alpha = 0;
                    }
                }
                NPC.ai[2] += 1f;
                if (NPC.ai[2] == 0f)
                {
                    NPC.ai[2] = 1f;
                    NPC.velocity.X = (float)(NPC.direction * 2);
                }
                return;
            }
            NPC.alpha = 0;
            if (NPC.ai[2] == 1f)
            {
                NPC.chaseable = true;
                CalamityAI.PassiveSwimmingAI(NPC, Mod, 0, 0f, 0.1f, 0.1f, 2f, 1f, 0.1f);
            }
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<Irradiated>(), 120);
        }

        public override void FindFrame(int frameHeight)
        {
            if (!NPC.wet)
            {
                NPC.frameCounter = 0.0;
                return;
            }
            NPC.frameCounter += 0.15f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.PlayerSafe)
            {
                return 0f;
            }
            if (spawnInfo.Player.Calamity().ZoneSulphur && spawnInfo.Water)
            {
                return 0.2f;
            }
            return 0f;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ModContent.ItemType<AnechoicCoating>(), 2, 1, 1);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 15; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
                }
            }
        }
    }
}
