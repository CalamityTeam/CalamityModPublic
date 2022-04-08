using CalamityMod.CalPlayer;
using CalamityMod.Events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.UI
{
    public class BossRushUI : InvasionProgressUI
    {
        public override int SecondaryDigitPrecision => 1;
        public override bool IsActive => BossRushEvent.BossRushActive;
        public override float CompletionRatio
        {
            get
            {
                float invasionBasedCompletion = BossRushEvent.BossRushStage / (float)BossRushEvent.Bosses.Count;
                if (!CalamityPlayer.areThereAnyDamnBosses || !BossRushEvent.Bosses.IndexInRange(BossRushEvent.BossRushStage))
                    return invasionBasedCompletion;

                int bossIndex = NPC.FindFirstNPC(BossRushEvent.CurrentlyFoughtBoss);

                if (!Main.npc.IndexInRange(bossIndex))
                    return invasionBasedCompletion;

                // Yes, this could be cached, but I personally don't see the need for it. In a typical situation, there won't be two of the same boss active during Boss Rush.
                NPC currentBoss = Main.npc[bossIndex];
                float bossBasedCompletion = 1f - currentBoss.life / (float)currentBoss.lifeMax;
                return invasionBasedCompletion + bossBasedCompletion / BossRushEvent.Bosses.Count;
            }
        }
        public override string InvasionName => "Boss Rush";
        public override Color InvasionBarColor => Color.DarkSlateBlue;
        public override Texture2D IconTexture => ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/UI/BossRushIcon").Value;

        public static float EvaluationLifeRatioFromNPCTypes(params int[] types)
        {
            int totalLife = 0;
            int totalLifeMax = 0;
            for (int i = 0; i < Main.npc.Length; i++)
            {
                if (!Main.npc[i].active)
                    continue;
                if (types.Contains(Main.npc[i].type))
                {
                    totalLife += Main.npc[i].life;
                    totalLifeMax += Main.npc[i].lifeMax;
                }
            }

            // Avoid division by zero, just in case.
            return totalLifeMax == 0 ? 0f : totalLife / (float)totalLifeMax;
        }
    }
}
