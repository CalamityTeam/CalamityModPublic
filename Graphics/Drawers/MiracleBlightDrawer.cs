using System.Collections.Generic;
using CalamityMod.NPCs.ExoMechs.Ares;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Graphics.Drawers
{
    public class MiracleBlightDrawer : BaseDrawer
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

        public override DrawerLayer Layer => DrawerLayer.NPC;

        // Unsure whether its more performant to always draw, or to check if any NPC actually has the debuff.
        public override bool ShouldDraw => true;
        #endregion

        #region Methods
        // Only draw if:
        // - A Vanilla or Calamity NPC OR another mod's non boss npc.
        // - The NPC is active.
        // - The NPC has miracle blight.
        // - The current player does not have the shrooms effect.
        // - The current NPC isnt in an excluded list of NPCs.
        // - The current NPC does not have the polarity effect.
        // If this seems really excessive, it's because global complex effects like this are a pain in the ass to implement without having a million
        // broken edge cases.
        public static bool ValidToDraw(NPC npc) =>
            (npc.ModNPC == null || npc.ModNPC.Mod == CalamityMod.Instance || !npc.boss) && npc.active && npc.Calamity().miracleBlight > 0
                && !(Main.LocalPlayer.Calamity().trippy && !npc.IsABestiaryIconDummy) && !ExcludedNPCs.Contains(npc.type) && npc.PolarityNPC().CurPolarity == 0;

        public override void DrawToTarget(SpriteBatch spriteBatch)
        {
            // Indicate that NPCs with miracle blight should draw.
            ActuallyDoPreDraw = true;

            // Draw every npc to a single target that should have the miracle blight visual.
            foreach (NPC npc in Main.npc)
            {
                if (ValidToDraw(npc))
                    Main.instance.DrawNPC(npc.whoAmI, npc.behindTiles);
            }

            // Indicate that NPCs with miracle blight should no longer draw.
            ActuallyDoPreDraw = false;
        }

        public override void DrawTarget(SpriteBatch spriteBatch)
        {
            // Apply the shader and draw the target.
            MiscShaderData blightShader = GameShaders.Misc["CalamityMod:MiracleBlight"];
            blightShader.SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/Neurons"), 1);
            blightShader.UseOpacity(0.7f);
            blightShader.Shader.Parameters["uWorldPosition"]?.SetValue(Main.screenPosition);
            blightShader.Apply();

            Main.spriteBatch.Draw(MainTarget, Vector2.Zero, Color.White);
        }
        #endregion
    }
}
