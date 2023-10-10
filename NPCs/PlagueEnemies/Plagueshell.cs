using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using Terraria.Audio;
using CalamityMod.Sounds;

namespace CalamityMod.NPCs.PlagueEnemies
{
    public class Plagueshell : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 8;
        }

        public override void SetDefaults()
        {
            NPC.npcSlots = 2f;
            NPC.damage = 80;
            NPC.aiStyle = NPCAIStyleID.GiantTortoise;
            NPC.width = 46;
            NPC.height = 32;
            NPC.defense = 32;
            NPC.lifeMax = 800;
            NPC.knockBackResist = 0.2f;
            AnimationType = NPCID.GiantTortoise;
            NPC.value = Item.buyPrice(0, 0, 20, 0);
            NPC.HitSound = SoundID.NPCHit24;
            NPC.noGravity = false;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<PlagueshellBanner>();
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToSickness = false;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Jungle, 
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.UndergroundJungle,
				new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.Plagueshell")
            });
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Plague, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                SoundEngine.PlaySound(CommonCalamitySounds.PlagueBoomSound, NPC.Center);
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Plague, hit.HitDirection, -1f, 0, default, 1f);
                }
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Plagueshell").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Plagueshell2").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Plagueshell3").Type, 1f);
                }
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.PlayerSafe || !NPC.downedGolemBoss || spawnInfo.Player.Calamity().ZoneSunkenSea)
            {
                return 0f;
            }
            return SpawnCondition.HardmodeJungle.Chance * 0.09f;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(ModContent.BuffType<Plague>(), 90, true);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) => npcLoot.Add(ModContent.ItemType<PlagueCellCanister>(), 1, 3, 4);
    }
}
