using System.Collections.Generic;
using System.Linq;
using System.Text;
using CalamityMod.Balancing;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static CalamityMod.CalamityUtils;

namespace CalamityMod.Items.Accessories.Wings
{
    public abstract class BaseWings : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories.Wings";

        /// <summary>
        /// Bonus vertical acceleration per frame while player velocity is below 0.<br/>
        /// Defaults to 0.5f.
        /// </summary>
        public virtual float BonusAscentWhileFalling => 0.5f;

        /// <summary>
        /// Bonus vertical acceleration per frame while player velocity is below a velocity threshold determined by RisingSpeedThreshold.<br/>
        /// Defaults to 0.1f.
        /// </summary>
        public virtual float BonusAscentWhileRising => 0.1f;

        /// <summary>
        /// Vertical velocity threshold for activating bonus acceleration from BonusAscentWhileRising when multiplied by the player's jump speed.<br/>
        /// Defaults to 0.5f.
        /// </summary>
        public virtual float RisingSpeedThreshold => 0.5f;

        /// <summary>
        /// Max vertical velocity threshold when multiplied by the player's jump speed.<br/>
        /// Defaults to 1.5f.
        /// </summary>
        public virtual float MaxAscentSpeed => 1.5f;

        /// <summary>
        /// Base vertical acceleration per frame.<br/>
        /// Defaults to 0.1f.
        /// </summary>
        public virtual float BaseAscent => 0.1f;

        public override void SetDefaults()
        {
            Item.accessory = true;
        }

        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            if (Item.wingSlot == -1) return;
            ascentWhenFalling = BonusAscentWhileFalling;
            ascentWhenRising = BonusAscentWhileRising;
            maxCanAscendMultiplier = RisingSpeedThreshold;
            maxAscentMultiplier = MaxAscentSpeed;
            constantAscend = BaseAscent;

            AdditionalFlightMovement(player, ref ascentWhenFalling, ref ascentWhenRising, ref maxCanAscendMultiplier, ref maxAscentMultiplier, ref constantAscend);
        }

        /// <summary>
        /// Addition for any deviations in regular wing movement.<br/>
        /// This is typically for UP boost or hovers.
        /// </summary>
        public virtual void AdditionalFlightMovement(Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend) { }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            if (Item.wingSlot == -1) return;
            WingStats stats = new();
            if (Item.type == ModContent.ItemType<MoonWalkers>())
                stats = ArmorIDs.Wing.Sets.Stats[MoonWalkers.wingSlot];
            else if (Item.type == ModContent.ItemType<VoidStriders>())
                stats = ArmorIDs.Wing.Sets.Stats[VoidStriders.wingSlot];
            else if (Item.type == ModContent.ItemType<SeraphTracers>())
                stats = ArmorIDs.Wing.Sets.Stats[SeraphTracers.wingSlot];
            else stats = ArmorIDs.Wing.Sets.Stats[Item.wingSlot];
            int time = stats.FlyTime;
            float run = stats.AccRunSpeedOverride;
            float rAcc = stats.AccRunAccelerationMult * 0.08f;
            bool hover = stats.HasDownHoverStats;
            float hSpeed = stats.DownHoverSpeedOverride;
            float hAcc = stats.DownHoverAccelerationMult * 0.08f;
            float baseJumpSpeed = (CalamityServerConfig.Instance.FasterJumpSpeed ? BalancingConstants.ConfigBoostedBaseJumpSpeed : 5.01f) + 1f;
            StringBuilder sb = new StringBuilder(512);
            sb.Append('\n');
            if (Main.keyState.PressingShift())
            {
                sb.Append(GetText($"Common.WingStatsFull").Format(time.FramesToSeconds(),
                HorizontalSpeedText(run), run.ToMph(),
                VerticalSpeedText(MaxAscentSpeed), (MaxAscentSpeed * baseJumpSpeed).ToMph(),
                HorizontalAccelerationText(stats.AccRunAccelerationMult), rAcc.ToMphps(),
                VerticalAccelerationText(BaseAscent), BaseAscent.ToMphps(),
                (BaseAscent + BonusAscentWhileRising).ToMphps(), (RisingSpeedThreshold * baseJumpSpeed).ToMph(),
                (BaseAscent + BonusAscentWhileFalling).ToMphps()));
                if (hover)
                {
                    sb.Append('\n');
                    sb.Append(GetText($"Common.WingStatsHover").Format(hSpeed.ToMph(), hAcc.ToMphps()));
                }
            }
            else
            {
                sb.Append(GetText($"Common.WingStats").Format(time.FramesToSeconds(), HorizontalSpeedText(run), VerticalSpeedText(MaxAscentSpeed),
                HorizontalAccelerationText(stats.AccRunAccelerationMult), VerticalAccelerationText(BaseAscent)));
                sb.Append('\n');
                sb.Append($"[c/B8B8B8:{GetTextValue("UI.HoldShiftTooltipExtensionIndicator")}]");                
            }

            // Add stats below the common "Allows flight" line
            TooltipLine wingTooltip = list.FirstOrDefault(x => x.Text == Language.GetTextValue("CommonItemTooltip.FlightAndSlowfall") && x.Mod == "Terraria");
            if (wingTooltip != null)
                wingTooltip.Text += sb.ToString();
        }

        public static string HorizontalSpeedText(float speed)
        {
            string key = "Average";
            Vector3 hsl = new Vector3(0.167f, 0.65f, 0.61f);
            if (speed > 9f)
            {
                key = "Excellent";
                hsl.X = 0.334f;
                hsl.Y = 1f;
            }
            else if (speed >= 8f)
            {
                key = "Great";
                hsl.X = 0.334f;
            }
            else if (speed >= 7f)
            {
                key = "Good";
                hsl.X = 0.334f;
                hsl.Y = 0.35f;
            }
            else if (speed <= 3f)
            {
                key = "Awful";
                hsl.X = 0f;
            }
            else if (speed <= 6f)
            {
                key = "Poor";
                hsl.X = 0f;
                hsl.Y = 0.35f;
            }
            return $"[c/{Main.hslToRgb(hsl).Hex3()}:{GetTextValue($"Common.Stats{key}")}]";
        }

        public static string VerticalSpeedText(float speed)
        {
            string key = "Average";
            Vector3 hsl = new Vector3(0.167f, 0.65f, 0.61f);
            if (speed > 2.5f)
            {
                key = "Excellent";
                hsl.X = 0.334f;
                hsl.Y = 1f;
            }
            else if (speed >= 2f)
            {
                key = "Great";
                hsl.X = 0.334f;
            }
            else if (speed >= 1.65f)
            {
                key = "Good";
                hsl.X = 0.334f;
                hsl.Y = 0.35f;
            }
            else if (speed <= 1f)
            {
                key = "Awful";
                hsl.X = 0f;
            }
            else if (speed <= 1.35f)
            {
                key = "Poor";
                hsl.X = 0f;
                hsl.Y = 0.35f;
            }
            return $"[c/{Main.hslToRgb(hsl).Hex3()}:{GetTextValue($"Common.Stats{key}")}]";
        }

        public static string HorizontalAccelerationText(float acc)
        {
            string key = "Average";
            Vector3 hsl = new Vector3(0.167f, 0.65f, 0.61f);
            if (acc > 2.5f)
            {
                key = "Excellent";
                hsl.X = 0.334f;
                hsl.Y = 1f;
            }
            else if (acc >= 2f)
            {
                key = "Great";
                hsl.X = 0.334f;
            }
            else if (acc >= 1.25f)
            {
                key = "Good";
                hsl.X = 0.334f;
                hsl.Y = 0.35f;
            }
            else if (acc <= 0.5f)
            {
                key = "Awful";
                hsl.X = 0f;
            }
            else if (acc <= 0.75f)
            {
                key = "Poor";
                hsl.X = 0f;
                hsl.Y = 0.35f;
            }
            return $"[c/{Main.hslToRgb(hsl).Hex3()}:{GetTextValue($"Common.Stats{key}")}]";
        }

        public static string VerticalAccelerationText(float acc)
        {
            string key = "Average";
            Vector3 hsl = new Vector3(0.167f, 0.65f, 0.61f);
            if (acc > 0.2f)
            {
                key = "Excellent";
                hsl.X = 0.334f;
                hsl.Y = 1f;
            }
            else if (acc >= 0.135f)
            {
                key = "Great";
                hsl.X = 0.334f;
            }
            else if (acc >= 0.11f)
            {
                key = "Good";
                hsl.X = 0.334f;
                hsl.Y = 0.35f;
            }
            else if (acc <= 0.05f)
            {
                key = "Awful";
                hsl.X = 0f;
            }
            else if (acc <= 0.09f)
            {
                key = "Poor";
                hsl.X = 0f;
                hsl.Y = 0.35f;
            }
            return $"[c/{Main.hslToRgb(hsl).Hex3()}:{GetTextValue($"Common.Stats{key}")}]";
        }
    }
}
