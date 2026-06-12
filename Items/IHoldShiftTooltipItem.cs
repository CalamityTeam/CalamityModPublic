using Microsoft.Xna.Framework;
using Terraria.Localization;

namespace CalamityMod.Items
{
    // This interface is used on items where you can hold SHIFT for an expanded tooltip.
    // All actual implementation of this behavior is done in CalamityGlobalItemTooltip.
    public interface IHoldShiftTooltipItem
    {
        #region Default Keys and Colors
        /// <summary>
        /// Internal ID for the TooltipLine which contains Calamity items' extended tooltips when holding SHIFT.
        /// </summary>
        internal const string ExtensionTooltipID = "CalamityMod:HoldShiftTooltip";

        /// <summary>
        /// Internal ID for the TooltipLine which contains Calamity items' "Hold SHIFT" extension indicators.
        /// </summary>
        internal const string ExtensionIndicatorTooltipID = "CalamityMod:HoldShiftExtensionIndicator";

        /// <summary>
        /// Internal ID for the TooltipLine which contains Calamity items' flavor tooltips.<br />
        /// Flavor tooltips on items which do not use this interface will be present in standard vanilla tooltip lines.
        /// </summary>
        internal const string FlavorTooltipID = "CalamityMod:FlavorTooltip";
        
        /// <summary>
        /// The lang/localization key of the default tooltip extension indicator message.<br />
        /// This key is used for items which don't replace their tooltips when holding SHIFT.
        /// </summary>
        public const string DefaultExtensionIndicatorKey = "UI.HoldShiftTooltipExtensionIndicator";

        /// <summary>
        /// The lang/localization key of the default tooltip replacement indicator message.<br />
        /// This key is used for items which replace their tooltips entirely when holding SHIFT.
        /// </summary>
        public const string DefaultReplacementIndicatorKey = "UI.HoldShiftTooltipReplacementIndicator";

        /// <summary>
        /// The default color of the default tooltip extension indicator message.
        /// </summary>
        public static readonly Color DefaultExtensionIndicatorColor = new Color(184, 184, 184); // #B8B8B8
        
        /// <summary>
        /// The default lang/localization key which is queried to show an item's tooltip extension.
        /// </summary>
        public const string DefaultTooltipExtensionKey = "HoldShiftTooltip";

        /// <summary>
        /// The default lang/localization key which is queried to show an item's flavor tooltip.
        /// </summary>
        public const string DefaultFlavorTooltipKey = "FlavorTooltip";
        #endregion

        /// <summary>
        /// Whether holding SHIFT for the tooltip extension hides the normal tooltip. Typically this is false.
        /// </summary>
        public virtual bool HidesNormalTooltip => false;

        #region Extension Indicator Properties
        /// <summary>
        /// Whether the "Hold SHIFT for more" extension indicator appears by default. Should essentially always be true.<br />
        /// You can set this to false for "secret" easter egg tooltips, like the one on Midas Prime.
        /// </summary>
        public virtual bool ShowExtensionIndicator => true;

        /// <summary>
        /// The lang/localization key of this item's tooltip extension indicator message.
        /// </summary>
        public virtual string ExtensionIndicatorKey => HidesNormalTooltip ? DefaultReplacementIndicatorKey : DefaultExtensionIndicatorKey;

        /// <summary>
        /// The color of this item's tooltip extension indicator message. If set to <b>null</b>, the indicator will not be colored.
        /// </summary>
        public virtual Color? ExtensionIndicatorColor => DefaultExtensionIndicatorColor;
        #endregion

        #region Tooltip Extension Properties
        /// <summary>
        /// The localization text object itself. Override to allow text to be formatted.
        /// </summary>
        public virtual LocalizedText TooltipExtensionText => LocalizedText.Empty;

        /// <summary>
        /// The lang/localization key which this item uses for its tooltip extension.
        /// </summary>
        public virtual string TooltipExtensionKey => DefaultTooltipExtensionKey;

        /// <summary>
        /// The color of this item's tooltip extension. If set to <b>null</b>, the tooltip extension will not be colored.
        /// </summary>
        public virtual Color? TooltipExtensionColor => null;
        #endregion

        #region Flavor Tooltip Properties
        /// <summary>
        /// Whether or not this item should have a separate flavor tooltip. Flavor tooltips are rendered below tooltip extensions for vanilla consistency.<br />
        /// Flavor tooltips are optional. If an item only needs a flavor tooltip and not a Hold SHIFT tooltip, just add it to the normal tooltip and don't use this interface.
        /// </summary>
        public virtual bool HasFlavorTooltip => false;

        /// <summary>
        /// The lang/localization key which this item uses for its flavor tooltip.
        /// </summary>
        public virtual string FlavorTooltipKey => DefaultFlavorTooltipKey;

        /// <summary>
        /// The color of this item's flavor tooltip. If set to <b>null</b>, the flavor tooltip will not be colored.
        /// </summary>
        public virtual Color? FlavorTooltipColor => null;
        #endregion
    }
}
