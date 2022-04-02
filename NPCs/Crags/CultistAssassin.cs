using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.World;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.Crags
{
    public class CultistAssassin : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cultist Assassin");
            Main.npcFrameCount[npc.type] = 4;
        }

        public override void SetDefaults()
        {
            npc.lavaImmune = true;
            npc.aiStyle = 3;
            npc.damage = 50;
            npc.width = 18;
            npc.height = 40;
            npc.defense = 16;
            npc.lifeMax = 80;
            npc.knockBackResist = 0.5f;
            animationType = NPCID.ZombieXmas;
            aiType = NPCID.ChaosElemental;
            npc.value = Item.buyPrice(0, 0, 10, 0);
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath50;
            if (CalamityWorld.downedProvidence)
            {
                npc.damage = 100;
                npc.defense = 30;
                npc.lifeMax = 3000;
            }
            banner = npc.type;
            bannerItem = ModContent.ItemType<CultistAssassinBanner>();
            npc.Calamity().VulnerableToHeat = false;
            npc.Calamity().VulnerableToCold = true;
            npc.Calamity().VulnerableToSickness = true;
            npc.Calamity().VulnerableToWater = true;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return (spawnInfo.player.Calamity().ZoneCalamity || spawnInfo.player.ZoneDungeon) && Main.hardMode ? 0.04f : 0f;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 120, true);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
                }
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/CultistAssassinGores/CultistAssassin"), npc.scale);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/CultistAssassinGores/CultistAssassin2"), npc.scale);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/CultistAssassinGores/CultistAssassin3"), npc.scale);
            }
        }

        public override void NPCLoot()
        {
            DropHelper.DropItemCondition(npc, ModContent.ItemType<Bloodstone>(), CalamityWorld.downedProvidence, 2, 1, 1);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<EssenceofChaos>(), Main.hardMode, 3, 1, 1);
        }
    }
}
