using CalamityMod.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using CalamityMod.CalPlayer;
using Terraria.ModLoader;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace CalamityMod.UI.CooldownIndicators
{
    public class CooldownIndicator
    {
        public static Dictionary<string, Type> IDtoType;

        public static void Load()
        {
            Type baseCooldownType = typeof(CooldownIndicator);
            CalamityMod calamity = ModContent.GetInstance<CalamityMod>();

            IDtoType = new Dictionary<string, Type>();

            foreach (Type type in calamity.Code.GetTypes())
            {
                if (type.IsSubclassOf(baseCooldownType) && !type.IsAbstract && type != baseCooldownType)
                {
                    CooldownIndicator instance = (CooldownIndicator)FormatterServices.GetUninitializedObject(type);
                    if (instance.SyncID != "")
                        IDtoType[instance.SyncID] = type;
                }
            }
        }

        public static void Unload()
        {
            IDtoType = null;
        }


        /// <summary>
        /// The player that the cooldown is applying to
        /// </summary>
        public Player AfflictedPlayer;

        /// <summary>
        /// The ID of the cooldown for multiplayer syncing. Leave this string empty if you don't need multiplayer syncing
        /// </summary>
        public virtual string SyncID => "";

        /// <summary>
		/// Should the cooldown be displayed in the cooldown rack.
		/// </summary>
		public virtual bool DisplayMe => false;

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
        /// Set this to true to disable default drawing, thus calling CustomDraw() instead. This only applies to the expanded mode of display
        /// </summary>
        public virtual bool UseCustomDraw => false;
        /// <summary>
        /// Use this method if you want to handle the drawing yourself. Only called if UseCustomDraw is set to true.
        /// </summary>
        public virtual void CustomDraw(SpriteBatch spriteBatch, Vector2 position, float opacity, float scale)
        {
            Texture2D sprite = ModContent.GetTexture(Texture);
            Texture2D outline = ModContent.GetTexture(TextureOutline);
            Texture2D barBase = ModContent.GetTexture(ChargeBarTexture);

            //Draw the ring
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Main.UIScaleMatrix);
            ApplyBarShaders(opacity);

            spriteBatch.Draw(barBase, position, null, Color.White * opacity, 0, barBase.Size() * 0.5f, scale, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.UIScaleMatrix);

            //Draw the outline
            spriteBatch.Draw(outline, position, null, OutlineColor * opacity, 0, outline.Size() * 0.5f, scale, SpriteEffects.None, 0f);

            //Draw the icon
            spriteBatch.Draw(sprite, position, null, Color.White * opacity, 0, sprite.Size() * 0.5f, scale, SpriteEffects.None, 0f);
        }

        /// <summary>
        /// Set this to true to disable default drawing, thus calling CustomDraw()/CustomDrawCompact() instead. This only applies to the compact mode of display
        /// </summary>
        public virtual bool UseCustomDrawCompact => false;
        /// <summary>
        /// Use this method if you want to handle the drawing yourself. Only called if UseCustomDrawCompact is set to true.
        /// </summary>
        public virtual void CustomDrawCompact(SpriteBatch spriteBatch, Vector2 position, float opacity, float scale)
        {
            Texture2D sprite = ModContent.GetTexture(Texture);
            Texture2D outline = ModContent.GetTexture(TextureOutline);
            Texture2D overlay = ModContent.GetTexture(TextureOverlay);

            //Draw the outline
            spriteBatch.Draw(outline, position, null, OutlineColor * opacity, 0, outline.Size() * 0.5f, scale, SpriteEffects.None, 0f);

            //Draw the icon
            spriteBatch.Draw(sprite, position, null, Color.White * opacity, 0, sprite.Size() * 0.5f, scale, SpriteEffects.None, 0f);

            //Draw the small overlay
            int lostHeight = (int)Math.Ceiling(overlay.Height * (1 - Completion));
            Rectangle crop = new Rectangle(0, lostHeight, overlay.Width, overlay.Height - lostHeight);
            spriteBatch.Draw(overlay, position + Vector2.UnitY * lostHeight * scale, crop, OutlineColor * opacity * 0.9f, 0, sprite.Size() * 0.5f, scale, SpriteEffects.None, 0f);
        }

        /// <summary>
		/// The texture of the charge bar. If set to a different texture path than the bar base, and if UseCustomDraw is set to false , it will be used as the ring around the cooldown icon 
        /// Please keep this sprite 44x44 (In 2x2)
		/// </summary>
		public virtual string ChargeBarTexture => "CalamityMod/UI/CooldownIndicators/BarBase";

        /// <summary>
		/// The texture under the charge bar. Will be used if ChargeBarTexture is set to an actual texture. Leave this as is to have nothing below the charge bar texture
        /// Please keep this sprite 44x44 (In 2x2)
		/// </summary>
		public virtual string ChargeBarBackTexture => "CalamityMod/UI/CooldownIndicators/BarBase";

        /// <summary>
        /// Method used to determine the color of the outline around the icon, and the overlay that goes above the icon in compact mode
        /// </summary>
        public virtual Color OutlineColor => Color.White;
        /// <summary>
        /// The color of the bar as it starts going around the icon. Only used if no charge bar texture is provided
        /// </summary>
        public virtual Color CooldownColorStart => Color.Gray;

        /// <summary>
        /// The color of the bar as it finishes going around the icon. Only used if no charge bar texture is provided
        /// </summary>
        public virtual Color CooldownColorEnd => Color.White;

        /// <summary>
        /// Apply the shader that creates the bar. Only called if UseCustomDraw is set to false. By default just creates a flat ring with colors going from the start to the end color.
        /// </summary>
        public virtual void ApplyBarShaders(float opacity)
        {
            if (ChargeBarTexture == "CalamityMod/UI/CooldownIndicators/BarBase")
            {
                GameShaders.Misc["CalamityMod:CircularBarShader"].UseOpacity(opacity);
                GameShaders.Misc["CalamityMod:CircularBarShader"].UseSaturation(1 - Completion);
                GameShaders.Misc["CalamityMod:CircularBarShader"].UseColor(CooldownColorStart);
                GameShaders.Misc["CalamityMod:CircularBarShader"].UseSecondaryColor(CooldownColorEnd);
                GameShaders.Misc["CalamityMod:CircularBarShader"].Apply();
            }

            else
            {
                GameShaders.Misc["CalamityMod:CircularBarSpriteShader"].SetShaderTexture(ModContent.GetTexture(ChargeBarBackTexture));
                GameShaders.Misc["CalamityMod:CircularBarSpriteShader"].UseOpacity(opacity);
                GameShaders.Misc["CalamityMod:CircularBarSpriteShader"].UseSaturation(1 - Completion);
                GameShaders.Misc["CalamityMod:CircularBarSpriteShader"].Apply();
            }
        }

        /// <summary>
        /// Determines if the cooldown can tick down. Useful for cooldowns that don't tick down when bosses are alive for example
        /// </summary>
        /// <returns>Wether or not the cooldown should tick down</returns>
        public virtual bool CanTickDown => true;

        /// <summary>
        /// The sound played when the cooldown time is over
        /// </summary>
        public virtual LegacySoundStyle EndSound => null;

        /// <summary>
        /// Wether or not the cooldown should get reset when the player dies
        /// </summary>
        public virtual bool ResetOnDeath => true;

        public CooldownIndicator(int duration, Player player)
        {
            TimeLeft = duration;
            Duration = duration;
            AfflictedPlayer = player; 

            //Sync the cooldown if needed
            if (SyncID != "" && Main.netMode == NetmodeID.MultiplayerClient && player.whoAmI == Main.myPlayer)
            {
                player.Calamity().SyncCooldown(false, SyncID);
            }
        }

        /// <summary>
        /// What happens when the cooldown ends. Isn't called if the cooldown is removed by the player dying
        /// </summary>
        public virtual void OnCooldownEnd() { }
    }
}
