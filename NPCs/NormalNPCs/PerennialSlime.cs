using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles;  

namespace CalamityMod.NPCs
{
    public class PerennialSlime : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Perennial Slime");
            Main.npcFrameCount[npc.type] = 2;
        }

        public override void SetDefaults()
        {
            npc.aiStyle = 1;
            npc.damage = 35;
            npc.width = 40;
            npc.height = 30;
            npc.defense = 12;
            npc.lifeMax = 150;
            npc.knockBackResist = 0f;
            animationType = 81;
            npc.value = Item.buyPrice(0, 0, 10, 0);
            npc.alpha = 50;
            npc.lavaImmune = false;
            npc.noGravity = false;
            npc.noTileCollide = false;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            banner = npc.type;
            bannerItem = ModContent.ItemType<PerennialSlimeBanner>();
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.playerSafe || !NPC.downedPlantBoss || spawnInfo.player.Calamity().ZoneAbyss ||
                spawnInfo.player.Calamity().ZoneSunkenSea)
            {
                return 0f;
            }
            return SpawnCondition.Cavern.Chance * 0.08f;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            // TODO -- This dust was an invalid dust. Replaced with a random dust.
            int dustType = 115;
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, dustType, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, dustType, hitDirection, -1f, 0, default, 1f);
                }
            }
        }

        public override void NPCLoot()
        {
            Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<PerennialOre>(), Main.rand.Next(10, 27));
        }
    }
}
