using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Localization;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace CalamityMod.Cooldowns
{
    public class CooldownHandler
    {
        public static string ID => null;
        public CooldownInstance instance;

        #region Gameplay Behavior
        /// <summary>
        /// This method runs once every frame while the cooldown instance is active.
        /// </summary>
        public virtual void Tick() { }

        /// <summary>
        /// This method runs when the cooldown instance ends naturally.<br/>
        /// It is not called if the cooldown instance is deleted because the player died.
        /// </summary>
        public virtual void OnCompleted() { }

        /// <summary>
        /// Determines whether the cooldown instance can currently tick down.<br/>
        /// For example, this is useful for cooldowns that don't tick down if there are any bosses alive.<br/>
        /// You can also use it so that cooldowns which persist through death do not tick down while the player is dead.
        /// </summary>
        public virtual bool CanTickDown => true;

        /// <summary>
        /// Set this to true to make this cooldown remain even when the player dies.<br/>
        /// All cooldowns with PersistsThroughDeath set to false disappear immediately when the player dies.
        /// </summary>
        public virtual bool PersistsThroughDeath => false;

        /// <summary>
        /// Set this to true to make this cooldown persist through saves and loads.<br/>
        /// All cooldowns with SavedWithPlayer set to true are serialized into the modded player file.
        /// </summary>
        public virtual bool SavedWithPlayer => true;

        /// <summary>
        /// When the cooldown instance ends, this sound is played. Leave at <b>null</b> for no sound.
        /// </summary>
        public virtual SoundStyle? EndSound => null;

        /// <summary>
        /// Set this to true to have the cooldown instance end sound play upon expiration. Defaults to <b>true</b>
        /// </summary>
        public virtual bool ShouldPlayEndSound => true;
        #endregion

        #region Display & Rendering
        /// <summary>
        /// The name of the cooldown instance, appears when the player hovers over the indicator
        /// </summary>
        public virtual LocalizedText DisplayName => LocalizedText.Empty;

        /// <summary>
        /// Whether or not this cooldown instance should appear in the cooldown rack UI.
        /// </summary>
        public virtual bool ShouldDisplay => true;

        /// <summary>
        /// The texture of the cooldown indicator.<br/>
        /// <b>These must be 20x20 pixels when at 2x2 scale.</b>
        /// </summary>
        public virtual string Texture => "";
        /// <summary>
        /// This texture is overlaid atop the cooldown when the cooldown rack is rendering in compact mode.
        /// </summary>
        public virtual string OverlayTexture => $"{Texture}Overlay";
        /// <summary>
        /// This outline texture is rendered around the base icon texture.
        /// </summary>
        public virtual string OutlineTexture => $"{Texture}Outline";

        internal static string DefaultChargeBarTexture = "CalamityMod/Cooldowns/BarBase";
        /// <summary>
        /// The texture of this cooldown's "charge bar", or the circle rendered by shaders.<br/>
        /// By default, this is only used when the cooldown rack is rendering in expanded mode .<br/>
        /// Leave it as the default to have the charge bar be displayed as a shader-rendered circle.<br/>
        /// <b>These must be 44x44 pixels when at 2x2 scale.</b>
        /// </summary>
        public virtual string ChargeBarTexture => DefaultChargeBarTexture;
        /// <summary>
        /// The background texture of this cooldown's "charge bar", or the circle rendered by shaders.<br/>
        /// By default, this is only used when the cooldown rack is rendering in expanded mode.<br/>
        /// Leave it as the default to have nothing render underneath the charge bar itself.<br/>
        /// <b>These must be 44x44 pixels when at 2x2 scale.</b>
        /// </summary>
        public virtual string ChargeBarBackTexture => DefaultChargeBarTexture;

        /// <summary>
        /// The color used for the icon outline when rendering in expanded mode.<br/>
        /// In compact mode, this color is used for the overlay that goes above the icon.
        /// </summary>
        public virtual Color OutlineColor => Color.White;
        /// <summary>
        /// The color used for the start of the circular cooldown timer rendered by shaders.<br/>
        /// This color is only used when drawing in expanded mode and is ignored if a charge bar texture is provided.
        /// </summary>
        public virtual Color CooldownStartColor => Color.Gray;
        /// <summary>
        /// The color used for the end of the circular cooldown timer rendered by shaders.<br/>
        /// This color is only used when drawing in expanded mode and is ignored if a charge bar texture is provided.
        /// </summary>
        public virtual Color CooldownEndColor => Color.White;

        /// <summary>
        /// This method is called to render the cooldown when the cooldown rack is in expanded mode.
        /// </summary>
        public virtual void DrawExpanded(SpriteBatch spriteBatch, Vector2 position, float opacity, float scale)
        {
            Texture2D sprite = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D outline = ModContent.Request<Texture2D>(OutlineTexture).Value;
            Texture2D barBase = ModContent.Request<Texture2D>(ChargeBarTexture).Value;

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
        /// This method is called to render the cooldown when the cooldown rack is in compact mode.
        /// </summary>
        public virtual void DrawCompact(SpriteBatch spriteBatch, Vector2 position, float opacity, float scale)
        {
            Texture2D sprite = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D outline = ModContent.Request<Texture2D>(OutlineTexture).Value;
            Texture2D overlay = ModContent.Request<Texture2D>(OverlayTexture).Value;
            Color outlineColor = OutlineColor;

            //Draw the outline
            spriteBatch.Draw(outline, position, null, outlineColor * opacity, 0, outline.Size() * 0.5f, scale, SpriteEffects.None, 0f);

            //Draw the icon
            spriteBatch.Draw(sprite, position, null, Color.White * opacity, 0, sprite.Size() * 0.5f, scale, SpriteEffects.None, 0f);

            //Draw the small overlay
            int lostHeight = (int)Math.Ceiling(overlay.Height * (1 - instance.Completion));
            Rectangle crop = new Rectangle(0, lostHeight, overlay.Width, overlay.Height - lostHeight);
            spriteBatch.Draw(overlay, position + Vector2.UnitY * lostHeight * scale, crop, outlineColor * opacity * 0.9f, 0, sprite.Size() * 0.5f, scale, SpriteEffects.None, 0f);
        }

        /// <summary>
        /// Renders the circular cooldown timer with a radial shader.<br/>
        /// If the charge bar texture is defined, it is used. Otherwise it renders a flat ring which slides from the start color to the end color.
        /// </summary>
        public virtual void ApplyBarShaders(float opacity)
        {
            if (ChargeBarTexture == DefaultChargeBarTexture)
            {
                GameShaders.Misc["CalamityMod:CircularBarShader"].UseOpacity(opacity);
                GameShaders.Misc["CalamityMod:CircularBarShader"].UseSaturation(1 - instance.Completion);
                GameShaders.Misc["CalamityMod:CircularBarShader"].UseColor(CooldownStartColor);
                GameShaders.Misc["CalamityMod:CircularBarShader"].UseSecondaryColor(CooldownEndColor);
                GameShaders.Misc["CalamityMod:CircularBarShader"].Apply();
            }
            else
            {
                GameShaders.Misc["CalamityMod:CircularBarSpriteShader"].SetShaderTexture(ModContent.Request<Texture2D>(ChargeBarBackTexture));
                GameShaders.Misc["CalamityMod:CircularBarSpriteShader"].UseOpacity(opacity);
                GameShaders.Misc["CalamityMod:CircularBarSpriteShader"].UseSaturation(1 - instance.Completion);
                GameShaders.Misc["CalamityMod:CircularBarSpriteShader"].Apply();
            }
        }
        #endregion
    }
}
