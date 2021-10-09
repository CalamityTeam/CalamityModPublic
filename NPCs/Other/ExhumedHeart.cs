using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Other
{
    public class ExhumedHeart : ModNPC
    {
        public ref float Time => ref npc.ai[0];
        public Player Owner
        {
            get
            {
                if (npc.target >= 255 || npc.target < 0)
                    npc.TargetClosest();
                return Main.player[npc.target];
            }
        }
        public override void SetStaticDefaults()
		{
            DisplayName.SetDefault("Exhumed Brimstone Heart");
            Main.npcFrameCount[npc.type] = 6;
        }

        public override void SetDefaults()
        {
            npc.width = npc.height = 38;
            npc.damage = 0;
            npc.defense = 0;
            npc.lifeMax = 51740;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.value = 0f;
            npc.HitSound = SoundID.NPCHit13;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.knockBackResist = 0f;
            npc.netAlways = true;
            npc.aiStyle = -1;
            npc.Calamity().DoesNotGenerateRage = true;
            npc.Calamity().DoesNotDisappearInBossRush = true;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale) => npc.lifeMax = 51740;

        public override void AI()
        {
            npc.Opacity = Utils.InverseLerp(0f, 25f, Time, true);
            npc.Center = Owner.Center + (MathHelper.TwoPi * Time / 540f).ToRotationVector2() * 350f;
            npc.velocity = Vector2.Zero;

            Time++;
        }

		public override void FindFrame(int frameHeight)
		{
            npc.frameCounter++;
            npc.frame.Y = (int)(npc.frameCounter / 5) % Main.npcFrameCount[npc.type] * frameHeight;
		}

		public override Color? GetAlpha(Color drawColor)
		{
            Color color = Color.Purple * npc.Opacity;
            color.A = 127;
            return color;
        }
    }
}
