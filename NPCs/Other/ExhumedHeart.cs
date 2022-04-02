using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using System;
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
            npc.Calamity().VulnerableToHeat = false;
            npc.Calamity().VulnerableToCold = true;
            npc.Calamity().VulnerableToWater = true;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale) => npc.lifeMax = 51740;

        public override void AI()
        {
            npc.Opacity = Utils.InverseLerp(0f, 25f, Time, true);
            npc.Center = Owner.Center + (MathHelper.TwoPi * Time / 540f).ToRotationVector2() * 350f;
            npc.velocity = Vector2.Zero;

            if (!Owner.active || Owner.dead ||
                Owner.ownedProjectileCounts[ModContent.ProjectileType<SepulcherMinion>()] <= 0)
            {
                npc.life = 0;
                npc.HitEffect();
                npc.checkDead();
                npc.active = false;
            }

            Time++;
        }

        public float PrimitiveWidthFunction(float completionRatio)
        {
            float widthInterpolant = Utils.InverseLerp(0f, 0.16f, completionRatio, true) * Utils.InverseLerp(1f, 0.84f, completionRatio, true);
            widthInterpolant = (float)Math.Pow(widthInterpolant, 8D);
            float baseWidth = MathHelper.Lerp(4f, 1f, widthInterpolant);
            float pulseWidth = MathHelper.Lerp(0f, 3.2f, (float)Math.Pow(Math.Sin(Main.GlobalTime * 2.6f + npc.whoAmI * 1.3f + completionRatio), 16D));
            return baseWidth + pulseWidth;
        }

        public Color PrimitiveColorFunction(float completionRatio)
        {
            float colorInterpolant = MathHelper.SmoothStep(0f, 1f, Utils.InverseLerp(0f, 0.34f, completionRatio, true) * Utils.InverseLerp(1.07f, 0.66f, completionRatio, true));
            return Color.Lerp(Color.DarkRed * 0.7f, Color.Red, colorInterpolant) * 0.425f;
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
