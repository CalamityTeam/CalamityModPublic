using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.NormalNPCs
{
    public class PhantomSpirit : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phantom Spirit");
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
            animationType = NPCID.DungeonSpirit;
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

        public override void AI()
        {
            float speed = CalamityWorld.death ? 22f : 17f;
            CalamityAI.DungeonSpiritAI(NPC, Mod, speed, -MathHelper.PiOver2);
            int num822 = Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Phantoplasm, 0f, 0f, 0, default, 1f);
            Dust dust = Main.dust[num822];
            dust.velocity *= 0.1f;
            dust.scale = 1.3f;
            dust.noGravity = true;
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

        public override void NPCLoot()
        {
            DropHelper.DropItem(NPC, ModContent.ItemType<Phantoplasm>());
        }
    }
}
