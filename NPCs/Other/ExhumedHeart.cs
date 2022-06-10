using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Other
{
    public class ExhumedHeart : ModNPC
    {
        public ref float Time => ref NPC.ai[0];
        public Player Owner
        {
            get
            {
                if (NPC.target >= 255 || NPC.target < 0)
                    NPC.TargetClosest();
                return Main.player[NPC.target];
            }
        }
        public override void SetStaticDefaults()
        {
            this.HideFromBestiary();
            DisplayName.SetDefault("Exhumed Brimstone Heart");
            Main.npcFrameCount[NPC.type] = 6;
        }

        public override void SetDefaults()
        {
            NPC.width = NPC.height = 38;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.lifeMax = 51740;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.value = 0f;
            NPC.HitSound = SoundID.NPCHit13;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0f;
            NPC.netAlways = true;
            NPC.aiStyle = -1;
            NPC.Calamity().DoesNotGenerateRage = true;
            NPC.Calamity().DoesNotDisappearInBossRush = true;
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToWater = true;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale) => NPC.lifeMax = 51740;

        public override void AI()
        {
            NPC.Opacity = Utils.GetLerpValue(0f, 25f, Time, true);
            NPC.Center = Owner.Center + (MathHelper.TwoPi * Time / 540f).ToRotationVector2() * 350f;
            NPC.velocity = Vector2.Zero;

            if (!Owner.active || Owner.dead ||
                Owner.ownedProjectileCounts[ModContent.ProjectileType<SepulcherMinion>()] <= 0)
            {
                NPC.life = 0;
                NPC.HitEffect();
                NPC.checkDead();
                NPC.active = false;
            }

            Time++;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            NPC.frame.Y = (int)(NPC.frameCounter / 5) % Main.npcFrameCount[NPC.type] * frameHeight;
        }

        public override Color? GetAlpha(Color drawColor)
        {
            Color color = Color.Purple * NPC.Opacity;
            color.A = 127;
            return color;
        }
    }
}
