using CalamityMod.Items.Placeables.Banners;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.SunkenSea
{
    public class SeaSerpent5 : ModNPC
    {
        public override LocalizedText DisplayName => CalamityUtils.GetText("NPCs.SeaSerpent1.DisplayName");
        public override void SetStaticDefaults()
        {
            this.HideFromBestiary();
        }

        public override void SetDefaults()
        {
            NPC.damage = 20;
            NPC.width = 36; //42
            NPC.height = 30; //40
            NPC.defense = 30;
            NPC.lifeMax = 3000;
            NPC.aiStyle = -1; //new
            AIType = -1; //new
            NPC.knockBackResist = 0f;
            NPC.alpha = 255;
            NPC.behindTiles = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.netAlways = true;
            NPC.dontCountMe = true;
            NPC.chaseable = false;
            Banner = ModContent.NPCType<SeaSerpent1>();
            BannerItem = ModContent.ItemType<SeaSerpentBanner>();
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToSickness = true;
            NPC.Calamity().VulnerableToElectricity = true;
            NPC.Calamity().VulnerableToWater = false;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        public override void AI()
        {
            Lighting.AddLight(NPC.Center, (255 - NPC.alpha) * 0f / 255f, (255 - NPC.alpha) * 0.30f / 255f, (255 - NPC.alpha) * 0.30f / 255f);

            if (NPC.ai[2] > 0f)
                NPC.realLife = (int)NPC.ai[2];

            // Check if other segments are still alive, if not, die
            bool shouldDespawn = true;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<SeaSerpent1>())
                {
                    shouldDespawn = false;
                    break;
                }
            }
            if (!shouldDespawn)
            {
                if (NPC.ai[1] <= 0f)
                    shouldDespawn = true;
                else if (Main.npc[(int)NPC.ai[1]].life <= 0)
                    shouldDespawn = true;
            }
            if (shouldDespawn)
            {
                NPC.life = 0;
                NPC.HitEffect(0, 10.0);
                NPC.checkDead();
                NPC.active = false;
            }

            if (Main.npc[(int)NPC.ai[1]].alpha < 128)
            {
                NPC.alpha -= 42;
                if (NPC.alpha < 0)
                    NPC.alpha = 0;
            }

            Vector2 segmentPosition = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
            float targetXDist = Main.player[NPC.target].position.X + (float)(Main.player[NPC.target].width / 2);
            float targetYDist = Main.player[NPC.target].position.Y + (float)(Main.player[NPC.target].height / 2);
            targetXDist = (float)((int)(targetXDist / 16f) * 16);
            targetYDist = (float)((int)(targetYDist / 16f) * 16);
            segmentPosition.X = (float)((int)(segmentPosition.X / 16f) * 16);
            segmentPosition.Y = (float)((int)(segmentPosition.Y / 16f) * 16);
            targetXDist -= segmentPosition.X;
            targetYDist -= segmentPosition.Y;
            float targetDistance = (float)System.Math.Sqrt((double)(targetXDist * targetXDist + targetYDist * targetYDist));
            if (NPC.ai[1] > 0f && NPC.ai[1] < (float)Main.npc.Length)
            {
                try
                {
                    segmentPosition = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
                    targetXDist = Main.npc[(int)NPC.ai[1]].position.X + (float)(Main.npc[(int)NPC.ai[1]].width / 2) - segmentPosition.X;
                    targetYDist = Main.npc[(int)NPC.ai[1]].position.Y + (float)(Main.npc[(int)NPC.ai[1]].height / 2) - segmentPosition.Y;
                } catch
                {
                }
                NPC.rotation = (float)System.Math.Atan2((double)targetYDist, (double)targetXDist) + 1.57f;
                targetDistance = (float)System.Math.Sqrt((double)(targetXDist * targetXDist + targetYDist * targetYDist));
                int segmentWidth = NPC.width;
                targetDistance = (targetDistance - (float)segmentWidth) / targetDistance;
                targetXDist *= targetDistance;
                targetYDist *= targetDistance;
                NPC.velocity = Vector2.Zero;
                NPC.position.X = NPC.position.X + targetXDist;
                NPC.position.Y = NPC.position.Y + targetYDist;

                if (targetXDist < 0f)
                    NPC.spriteDirection = -1;
                else if (targetXDist > 0f)
                    NPC.spriteDirection = 1;
            }
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, 37, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 10; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, 37, hit.HitDirection, -1f, 0, default, 1f);
                }
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("SeaSerpentGore5").Type, 1f);
                }
            }
        }
    }
}
