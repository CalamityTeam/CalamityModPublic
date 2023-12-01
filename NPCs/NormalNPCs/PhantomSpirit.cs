using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.NormalNPCs
{
    public class PhantomSpirit : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 3;
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.damage = 70;
            NPC.width = 16;
            NPC.height = 16;
            NPC.defense = 10;
            NPC.lifeMax = 1000;
            NPC.knockBackResist = 0.1f;
            AnimationType = NPCID.DungeonSpirit;
            AIType = -1;
            NPC.value = Item.buyPrice(0, 0, 20, 0);
            NPC.HitSound = SoundID.NPCHit36;
            NPC.DeathSound = SoundID.NPCDeath39;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<PhantomSpiritBanner>();
            NPC.Calamity().VulnerableToSickness = false;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheDungeon,
				new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.PhantomSpirit")
            });
        }

        public override void AI()
        {
            float speed = CalamityWorld.death ? 22f : CalamityWorld.revenge ? 19.5f : 17f;
            CalamityAI.DungeonSpiritAI(NPC, Mod, speed, -MathHelper.PiOver2);
            int polterDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Polterplasm, 0f, 0f, 0, default, 1f);
            Dust dust = Main.dust[polterDust];
            dust.velocity *= 0.1f;
            dust.scale = 1.3f;
            dust.noGravity = true;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Polterplasm, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 50; i++)
                {
                    int hitPolterDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Polterplasm, NPC.velocity.X, NPC.velocity.Y, 0, default, 1f);
                    Dust dust = Main.dust[hitPolterDust];
                    dust.velocity *= 2f;
                    dust.noGravity = true;
                    dust.scale = 1.4f;
                }
            }
        }

        public override Color? GetAlpha(Color drawColor) => new Color(200, 200, 200, 0);

        public override void ModifyNPCLoot(NPCLoot npcLoot) => npcLoot.Add(ModContent.ItemType<Polterplasm>());
    }
}
