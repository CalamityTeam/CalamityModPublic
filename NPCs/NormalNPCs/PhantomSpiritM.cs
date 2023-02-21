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
    public class PhantomSpiritM : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phantom Spirit");
            Main.npcFrameCount[NPC.type] = 5;
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.damage = 90;
            NPC.width = 32;
            NPC.height = 80;
            NPC.scale *= 1.1f;
            NPC.defense = 25;
            NPC.lifeMax = 2000;
            NPC.knockBackResist = 0.1f;
            AIType = -1;
            NPC.value = Item.buyPrice(0, 0, 40, 0);
            NPC.HitSound = SoundID.NPCHit36;
            NPC.DeathSound = SoundID.NPCDeath39;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            Banner = ModContent.NPCType<PhantomSpirit>();
            BannerItem = ModContent.ItemType<PhantomSpiritBanner>();
            NPC.Calamity().VulnerableToSickness = false;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheDungeon,

				// Will move to localization whenever that is cleaned up.
				new FlavorTextBestiaryInfoElement("Spirits which turned upon one another, cannibalizing others in their mindless frustrated frenzies. There is precedent in the past, for spirits which have devoured enough of others, to grow astoundingly in power.")
            });
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.15f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override void AI()
        {
            float speed = CalamityWorld.death ? 18f : CalamityWorld.revenge ? 15.75f : 13.5f;
            CalamityAI.DungeonSpiritAI(NPC, Mod, speed, -MathHelper.PiOver2);
            int num822 = Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Phantoplasm, 0f, 0f, 0, default, 1f);
            Dust dust = Main.dust[num822];
            dust.velocity *= 0.1f;
            dust.scale = 1.3f;
            dust.noGravity = true;
            return;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Phantoplasm, hitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int num288 = 0; num288 < 50; num288++)
                {
                    int num289 = Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Phantoplasm, NPC.velocity.X, NPC.velocity.Y, 0, default, 1f);
                    Dust dust = Main.dust[num289];
                    dust.velocity *= 2f;
                    dust.noGravity = true;
                    dust.scale = 1.4f;
                }
            }
        }

        public override Color? GetAlpha(Color drawColor)
        {
            return new Color(200, 200, 200, 0);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) => npcLoot.Add(ModContent.ItemType<Phantoplasm>(), 1, 1, 3);
    }
}
