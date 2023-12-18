using System.Collections.Generic;
using CalamityMod.NPCs;
using CalamityMod.NPCs.ExoMechs.Ares;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Graphics.Renderers.CalamityRenderers
{
    public class MiracleBlightRenderer : BaseRenderer
    {
        #region Fields/Properties
        /// <summary>
        /// Add NPCs to this if they break with the miracle blight shader.
        /// </summary>
        public static List<int> ExcludedNPCs => new()
        {
            // List the reason why the NPC(s) are excluded :)

            // The particle sets break with the visuals and this is the easiest way to fix this that isn't stupidly complex.
            ModContent.NPCType<AresBody>(),
            ModContent.NPCType<AresGaussNuke>(),
            ModContent.NPCType<AresLaserCannon>(),
            ModContent.NPCType<AresPlasmaFlamethrower>(),
            ModContent.NPCType<AresTeslaCannon>(),

            // Breaks with being behind tiles, and causes a funny interaction where his head goes behind his neck.
            NPCID.MoonLordCore,
            NPCID.MoonLordHand,
            NPCID.MoonLordHead
        };

        /// <summary>
        /// Used to determine whether the npc should return true in predraw or not, for exclusively drawing to the drawer target.
        /// </summary>
        public static bool ActuallyDoPreDraw
        {
            get;
            private set;
        }

        public override DrawLayer Layer => DrawLayer.NPC;

        // Unsure whether its more performant to always draw, or to check if any NPC actually has the debuff.
        public override bool ShouldDraw => true;
        #endregion

        #region Methods
        /// <summary>
        /// Checks if the provided npc is eligible to be drawn with the miracle blight visual effect.
        /// </summary>
        /// <param name="npc">The NPC to check</param>
        /// <returns>If the NPC is eligible</returns>
        public static bool ValidToDraw(NPC npc)
        {
            // Do not draw inactive npcs, or ones with weird MP types less than or equal to 0.
            if (!npc.active || npc.type <= NPCID.None)
                return false;

            // Do not draw other mod's bosses.
            if (npc.ModNPC != null && npc.ModNPC.Mod != CalamityMod.Instance && npc.boss)
                return false;

            // Don't draw excluded NPCs, or if the npc is a bestiary dummy.
            if (ExcludedNPCs.Contains(npc.type) || npc.IsABestiaryIconDummy)
                return false;

            // Safety check for weird MP bug when getting global npcs.
            if (!npc.TryGetGlobalNPC<CalamityGlobalNPC>(out var calNPC) || !npc.TryGetGlobalNPC<CalamityPolarityNPC>(out var polNPC))
                return false;

            // Do not draw if the npc does not have miracle blight, or has the polarity effect.
            if (calNPC.miracleBlight <= 0 || polNPC.CurPolarity > 0f)
                return false;

            // Do not draw if the current player has the trippy effect.
            if (Main.LocalPlayer.Calamity().trippy)
                return false;

            return true;
        }

        public override void DrawToTarget(SpriteBatch spriteBatch)
        {
            // Indicate that NPCs with miracle blight should draw.
            ActuallyDoPreDraw = true;

            // Draw every npc to a single target that should have the miracle blight visual.
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                // Extra check to ensure that index errors will not occur. If not in range, something has gone wrong and the loop should terminate.
                if (!Main.npc.IndexInRange(i))
                    break;

                NPC npc = Main.npc[i];

                if (ValidToDraw(npc))
                    Main.instance.DrawNPC(npc.whoAmI, npc.behindTiles);
            }

            // Indicate that NPCs with miracle blight should no longer draw.
            ActuallyDoPreDraw = false;
        }

        public override void DrawTarget(SpriteBatch spriteBatch)
        {
            // Apply the shader and draw the target.
            var blightShader = GameShaders.Misc["CalamityMod:MiracleBlight"];
            blightShader.SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/Neurons"), 1);
            blightShader.UseOpacity(0.7f);
            blightShader.Apply();

            Main.spriteBatch.Draw(MainTarget, Vector2.Zero, Color.White);
        }
        #endregion
    }
}
