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
            Main.npcFrameCount[NPC.type] = 4;
        }

        public override void SetDefaults()
        {
            NPC.lavaImmune = true;
            NPC.aiStyle = 3;
            NPC.damage = 50;
            NPC.width = 18;
            NPC.height = 40;
            NPC.defense = 16;
            NPC.lifeMax = 80;
            NPC.knockBackResist = 0.5f;
            animationType = NPCID.ZombieXmas;
            AIType = NPCID.ChaosElemental;
            NPC.value = Item.buyPrice(0, 0, 10, 0);
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath50;
            if (CalamityWorld.downedProvidence)
            {
                NPC.damage = 100;
                NPC.defense = 30;
                NPC.lifeMax = 3000;
            }
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<CultistAssassinBanner>();
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToSickness = true;
            NPC.Calamity().VulnerableToWater = true;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return (spawnInfo.Player.Calamity().ZoneCalamity || spawnInfo.Player.ZoneDungeon) && Main.hardMode ? 0.04f : 0f;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 120, true);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
                }
                Gore.NewGore(NPC.position, NPC.velocity, Mod.GetGoreSlot("Gores/CultistAssassinGores/CultistAssassin"), NPC.scale);
                Gore.NewGore(NPC.position, NPC.velocity, Mod.GetGoreSlot("Gores/CultistAssassinGores/CultistAssassin2"), NPC.scale);
                Gore.NewGore(NPC.position, NPC.velocity, Mod.GetGoreSlot("Gores/CultistAssassinGores/CultistAssassin3"), NPC.scale);
            }
        }

        public override void NPCLoot()
        {
            DropHelper.DropItemCondition(NPC, ModContent.ItemType<Bloodstone>(), CalamityWorld.downedProvidence, 2, 1, 1);
            DropHelper.DropItemCondition(NPC, ModContent.ItemType<EssenceofChaos>(), Main.hardMode, 3, 1, 1);
        }
    }
}
