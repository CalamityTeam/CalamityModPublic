using CalamityMod.BiomeManagers;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.Crags
{
    public class CharredSlime : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Charred Slime");
            Main.npcFrameCount[NPC.type] = 2;
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = 1;
            AIType = NPCID.LavaSlime;
            NPC.damage = 40;
            NPC.width = 40;
            NPC.height = 30;
            NPC.defense = 10;
            NPC.lifeMax = 250;
            NPC.knockBackResist = 0f;
            AnimationType = NPCID.CorruptSlime;
            NPC.value = Item.buyPrice(0, 0, 5, 0);
            NPC.alpha = 50;
            NPC.lavaImmune = true;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            if (DownedBossSystem.downedProvidence)
            {
                NPC.damage = 80;
                NPC.defense = 20;
                NPC.lifeMax = 3500;
            }
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<CharredSlimeBanner>();
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToWater = true;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<BrimstoneCragsBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {

				// Will move to localization whenever that is cleaned up.
				new FlavorTextBestiaryInfoElement("Lava slimes are the only ones that manage to survive in the heat of hell, having replaced their moisture with liquid rock. That said, their mannerisms are the same, and they seek out and devour anything they can.")
            });
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (!DownedBossSystem.downedBrimstoneElemental)
            {
                return 0f;
            }
            return spawnInfo.Player.Calamity().ZoneCalamity ? 0.08f : 0f;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 120, true);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Brimstone, hitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Brimstone, hitDirection, -1f, 0, default, 1f);
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ModContent.ItemType<CharredOre>(), 1, 10, 26);
            npcLoot.AddIf(() => Main.hardMode, ModContent.ItemType<EssenceofChaos>(), 3);
            npcLoot.AddIf(() => DownedBossSystem.downedProvidence, ModContent.ItemType<Bloodstone>(), 2);
        }
    }
}
