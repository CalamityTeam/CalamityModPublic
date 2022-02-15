using CalamityMod.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;

namespace CalamityMod.UI.CooldownIndicators
{
    public class CooldownIndicator
    {
        /// <summary>
		/// Should the cooldown be displayed in the cooldown rack.
		/// </summary>
		public virtual bool DisplayMe => false;

        /// <summary>
        /// Does the cooldown need multiplayer syncing
        /// </summary>
        public virtual bool NeedsSyncing => false;

        /// <summary>
		/// The name of the cooldown, appears when the player hovers over the indicator
		/// </summary>
		public virtual string Name => "";
        /// <summary>
		/// The texture of the cooldown indicator. For the sake of consistency, please keep the icons 20x20 (in 2x2)
		/// </summary>
		public virtual string Texture => "";
        /// <summary>
		/// The overlay used above the base texture in compact mode
		/// </summary>
		public virtual string TextureOverlay => Texture + "Overlay";
        /// <summary>
		/// The outline that goes around the icon
		/// </summary>
		public virtual string TextureOutline => Texture + "Outline";
        /// <summary>
        /// The length of the cooldown
        /// </summary>
        public int Duration;
        /// <summary>
        /// Frames remaining before the cooldown ends
        /// </summary>
        public int TimeLeft;

        public float Completion => Duration != 0 ? TimeLeft / (float)Duration : 0;

        /// <summary>
        /// Set this to true to disable default drawing, thus calling CustomDraw()/CustomDrawCompact() instead.
        /// </summary>
        public virtual bool UseCustomDraw => false;
        /// <summary>
        /// Use this method if you want to handle the drawing yourself. Only called if UseCustomDraw is set to true.
        /// </summary>
        public virtual void CustomDraw(SpriteBatch spriteBatch, Vector2 position, float opacity, float scale) { }

        /// <summary>
        /// Use this method if you want to handle the drawing yourself. Only called if UseCustomDraw is set to true.
        /// </summary>
        public virtual void CustomDrawCompact(SpriteBatch spriteBatch, Vector2 position, float opacity, float scale) { }

        /// <summary>
        /// Method used to determine the color of the outline around the icon, and the overlay that goes above the icon in compact mode
        /// </summary>
        public virtual Color OutlineColor => Color.White;
        /// <summary>
        /// The color of the bar as it starts going around the icon
        /// </summary>
        public virtual Color CooldownColorStart => Color.Gray;

        /// <summary>
        /// The color of the bar as it finishes going around the icon
        /// </summary>
        public virtual Color CooldownColorEnd => Color.White;

        /// <summary>
        /// Apply the shader that creates the bar. Only called if UseCustomDraw is set to false. By default just creates a flat ring with colors going from the start to the end color.
        /// </summary>
        public virtual void ApplyBarShaders(float opacity)
        {
            GameShaders.Misc["CalamityMod:CircularBarShader"].UseOpacity(opacity);
            GameShaders.Misc["CalamityMod:CircularBarShader"].UseSaturation(1 - Completion);
            GameShaders.Misc["CalamityMod:CircularBarShader"].UseColor(CooldownColorStart);
            GameShaders.Misc["CalamityMod:CircularBarShader"].UseSecondaryColor(CooldownColorEnd);
            GameShaders.Misc["CalamityMod:CircularBarShader"].Apply();

        }

        /// <summary>
        /// Determines if the cooldown can tick down. Useful for cooldowns that don't tick down when bosses are alive for example
        /// </summary>
        /// <returns>Wether or not the cooldown should tick down</returns>
        public virtual bool CanTickDown(Player player) => true;

        /// <summary>
        /// The sound played when the cooldown time is over
        /// </summary>
        public virtual LegacySoundStyle EndSound() => null; 
    }
}
