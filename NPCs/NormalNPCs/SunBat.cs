using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.NormalNPCs
{
    public class SunBat : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sun Bat");
            Main.npcFrameCount[npc.type] = 7;
        }

        public override void SetDefaults()
        {
            npc.lavaImmune = true;
            npc.aiStyle = -1;
            aiType = -1;
            npc.damage = 35;
            npc.width = 26;
            npc.height = 20;
            npc.defense = 10;
            npc.lifeMax = 200;
            npc.knockBackResist = 0.65f;
            npc.value = Item.buyPrice(0, 0, 5, 0);
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath4;
            banner = npc.type;
            bannerItem = ModContent.ItemType<SunBatBanner>();
            npc.Calamity().VulnerableToHeat = false;
            npc.Calamity().VulnerableToCold = true;
            npc.Calamity().VulnerableToSickness = true;
            npc.Calamity().VulnerableToWater = true;
        }

        public override void AI()
        {
            CalamityGlobalAI.BuffedBatAI(npc, mod);
        }

        public override void FindFrame(int frameHeight)
        {
            if (npc.velocity.X > 0f)
                npc.spriteDirection = 1;
            if (npc.velocity.X < 0f)
                npc.spriteDirection = -1;

            npc.rotation = npc.velocity.X * 0.1f;

            npc.frameCounter += 0.15f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.playerSafe || !Main.hardMode || spawnInfo.player.Calamity().ZoneAbyss ||
                spawnInfo.player.Calamity().ZoneSunkenSea)
            {
                return 0f;
            }
            return SpawnCondition.Underground.Chance * 0.12f;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<HolyFlames>(), 120, true);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 64, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 64, hitDirection, -1f, 0, default, 1f);
                }
            }
        }

        public override void NPCLoot()
        {
            DropHelper.DropItemChance(npc, ModContent.ItemType<EssenceofCinder>(), 3);
        }
    }
}
