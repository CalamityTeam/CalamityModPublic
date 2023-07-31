using CalamityMod.Cooldowns;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    [LegacyName("DukeScales")]
    public class OldDukeScales : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";

        public static int RecoverTime = 120;
        public static int DashFatigueIncrease = 240;
        public static int MaxFatigue = 1200; // This is also the time that it takes the player to recover from the loss of all stamina.

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 26;
            Item.accessory = true;
            Item.value = CalamityGlobalItem.Rarity13BuyPrice;
            Item.rare = ModContent.RarityType<PureGreen>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            OldDukeScalesPlayer modPlayer = player.GetModPlayer<OldDukeScalesPlayer>();
            modPlayer.OldDukeScalesOn = true;
        }
    }

    public class OldDukeScalesPlayer : ModPlayer
    {
        public bool OldDukeScalesOn = false;
        public bool IsTired = false;
        public bool HasBoostedDashFirstFrame = false;
        private SoundStyle TiredSound => new SoundStyle("CalamityMod/Sounds/Custom/OldDukeHuff") { PitchVariance = .1f, Volume = .8f };

        public int Fatigue = 0;
        public int RecoverTimer = 0;

        public override void PostUpdateMiscEffects()
        {
            if (OldDukeScalesOn)
            {
                // Show the bar if the player doesn't have it.
                if (!Player.HasCooldown(OldDukeScalesFatigue.ID))
                    Player.AddCooldown(OldDukeScalesFatigue.ID, OldDukeScales.MaxFatigue);
                
                if (!IsTired)
                {
                    // +10% DR.
                    Player.endurance += 0.1f;
                    // +10% Max Movement Speed.
                    Player.maxRunSpeed *= 1.1f;
                    // +10% Max Acceleration.
                    Player.accRunSpeed *= 1.1f;

                    // If the player has dashed, increase that dash's velocity,
                    // the fatigue and sets the time before the fatigue can decrease.
                    if (Player.dashDelay == -1)
                    {
                        if (!HasBoostedDashFirstFrame)
                        {
                            RecoverTimer = OldDukeScales.RecoverTime;
                            Fatigue += OldDukeScales.DashFatigueIncrease;

                            Player.velocity.X *= 1.25f;

                            HasBoostedDashFirstFrame = true;
                        }
                    }
                    else
                        HasBoostedDashFirstFrame = false;

                    // If the player has reached max fatigue, the player becomes tired.
                    if (Fatigue >= OldDukeScales.MaxFatigue)
                    {
                        SoundEngine.PlaySound(TiredSound, Player.Center);
                        IsTired = true;
                    }
                }
            }

            if (IsTired)
            {
                // -30% Movement Speed.
                Player.moveSpeed -= 0.3f;

                // If the player dashes, that dash is 50% slower.
                if (Player.dashDelay == -1)
                {
                    if (!HasBoostedDashFirstFrame)
                    {
                        Player.velocity.X *= 0.5f;

                        HasBoostedDashFirstFrame = true;
                    }
                }
                else
                    HasBoostedDashFirstFrame = false;

                // If the fatigue has worn off, the player is back to normal.
                if (Fatigue <= 0)
                    IsTired = false;
            }

            //
            // This code below the condition for when the accesory is equipped
            // is a prevention to not exploit the item and only get the good bonuses.
            //

            // The cooldown doens't act as a normal cooldown, we use the fatigue as the time left of the cooldown.
            if (Player.Calamity().cooldowns.TryGetValue(OldDukeScalesFatigue.ID, out var cooldown))
            {
                cooldown.timeLeft = Fatigue;

                // The cooldown handler doesn't like values higher than 600 frames.
                // So we update the duration to a higher value every frame.
                cooldown.duration = OldDukeScales.MaxFatigue;
            }

            // If there's any amount of recovery before decreasing the fatigue, decrease this.
            if (RecoverTimer > 0)
                RecoverTimer--;

            // If the player has recovered, start decreasing the fatigue.
            if (Fatigue > 0 && RecoverTimer <= 0)
                Fatigue -= Player.StandingStill() ? 2 : 1;

            // The fatigue cannot go higher than the max.
            if (Fatigue >= OldDukeScales.MaxFatigue)
                Fatigue = OldDukeScales.MaxFatigue;
        }

        public override void ResetEffects()
        {
            OldDukeScalesOn = false;
        }

        public override void UpdateDead()
        {
            Fatigue = 0;
            RecoverTimer = 0;
        }
    }
}
