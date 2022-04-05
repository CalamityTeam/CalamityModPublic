using Terraria.ModLoader;

namespace CalamityMod.Localization
{
    // TODO - At some point change this to use proper localization files.
    public class CalamityLocalization
    {
        private static string[][] _localizations;

        public static void Load()
        {
            _localizations = new[]
            {
                new [] { "SkyOreText", "The ground is glittering with cyan light." },
                new [] { "IceOreText", "The ice caves are crackling with frigid energy." },
                new [] { "PlantOreText", "Energized plant matter has formed in the underground." },
                new [] { "TreeOreText", "Fossilized tree bark is bursting through the jungle's mud." },
                new [] { "AuricOreText", "A godly aura has blessed the world's caverns." },
                new [] { "FutureOreText", "A cold and dark energy has materialized in space." },

                new [] { "UglyBossText", "The Sunken Sea trembles..." },

                new [] { "SteelSkullBossText", "A blood red inferno lingers in the night..." },

                new [] { "WetWormBossText", "The sulphuric sky darkens..." },

                new [] { "PlantBossText", "The ocean depths are trembling." },

                new [] { "BabyBossText", "A plague has befallen the Jungle." },

                new [] { "MoonBossText", "The profaned flame blazes fiercely!" },
                new [] { "MoonBossText2", "Cosmic terrors are watching..." },

                new [] { "PlagueBossText", "PLAGUE NUKE BARRAGE ARMED, PREPARING FOR LAUNCH!!!" },
                new [] { "PlagueBossText2", "MISSILES LAUNCHED, TARGETING ROUTINE INITIATED!!!" },

                new [] { "ProfanedBossText", "The air is burning..." },
                new [] { "ProfanedBossText2", "Shrieks are echoing from the dungeon." },
                new [] { "ProfanedBossText3", "The calamitous beings have been inundated with bloodstone." },
                new [] { "ProfanedBossText4", "The Profaned Goddess has recognised your devotion to purity!" },

                new [] { "GhostBossText", "The abyssal spirits have been disturbed." },
                new [] { "GhostBossText2", "Wails echo through the dilapidated dungeon halls..." },
                new [] { "GhostBossText3", "Long-dead prisoners seek their zealous revenge..." },
                new [] { "GhostBossText4", "The souls released stir the acidic storms..." },

                new [] { "SCalSummonText", "Do you enjoy going through hell?" },
                new [] { "SCalStartText", "You should have just died..." },
                new [] { "SCalBH2Text", "It wasn't too long ago you barely managed to defeat my doppelganger. Quite the failure, wasn't it?" },
                new [] { "SCalBH3Text", "You've harnessed great power, but you wield it for no one but yourself." },
                new [] { "SCalBrothersText", "Would you like to meet my family? Horrible, isn't it?" },
                new [] { "SCalPhase2Text", "You will suffer greatly." },
                new [] { "SCalBH4Text", "It's absurd to even think of trying to get away. As long as you live, you will suffer." },
                new [] { "SCalSeekerRingText", "An upstart who recklessly stole and killed their way to power. I wonder, who does that remind me of...?" },
                new [] { "SCalBH5Text", "You have no stake in this battle. No one gave you any say in this matter!" },
                new [] { "SCalSepulcher2Text", "Once the dust has settled and only one remains, if it is you, what value will this have had?!" },
                new [] { "SCalDesparationText1", "Just stop!" },
                new [] { "SCalDesparationText2", "I have no future if I lose here." },
                new [] { "SCalDesparationText3", "Once you have bested me, you will only have one path forward." },
                new [] { "SCalDesparationText4", "And that path... also has no future." },
                new [] { "SCalAcceptanceText1", "Even if he has thrown all else away, his power remains." },
                new [] { "SCalAcceptanceText2", "I have no more energy left to resent him, or you..." },
                new [] { "SCalAcceptanceText3", "It will all be in your hands now." },

                // Some of these keys are incorrect in terms of their contents, but they remain as their bases + an addon at the end to make it
                // easier to define them.
                new [] { "SCalSummonTextRematch", "If you’re looking for some fourth-degree burns, you’ve got the right person." },
                new [] { "SCalStartTextRematch", "When the time comes, would you like to join my creation?" },
                new [] { "SCalBH2TextRematch", "You’ll still have to work hard for this victory." },
                new [] { "SCalBH3TextRematch", "I haven’t had such an interesting target dummy to test my magic on in a long time." },
                new [] { "SCalBrothersTextRematch", "Empty shells of their former selves. I doubt even a scrap of their spirits remain." },
                new [] { "SCalPhase2TextRematch", "Here we go again." },
                new [] { "SCalBH4TextRematch", "I wonder if you’ve seen these in your nightmares since our first battle?" },
                new [] { "SCalSeekerRingTextRematch", "Your skill hasn't faltered in the least." },
                new [] { "SCalBH5TextRematch", "Isn't this exciting?" },
                new [] { "SCalSepulcher2TextRematch", "Here comes the crawling tomb, one last time." },
                new [] { "SCalDesparationText1Rematch", "A terrific display, I concede this match to you." },
                new [] { "SCalDesparationText2Rematch", "No doubt you will face enemies stronger than I." },
                new [] { "SCalDesparationText3Rematch", "I trust you will not make the same mistakes he did." },
                new [] { "SCalDesparationText4Rematch", "I can’t imagine what your future holds now." },

                new [] { "EdgyBossText", "Don't get cocky, kid!" },
                new [] { "EdgyBossText3", "A fatal mistake!" },
                new [] { "EdgyBossText4", "Good luck recovering from that!" },
                new [] { "EdgyBossText5", "Delicious..." },
                new [] { "EdgyBossText6", "Did that hurt?" },
                new [] { "EdgyBossText7", "Nothing personal, kid." },
                new [] { "EdgyBossText8", "Are you honestly that bad at dodging?" },
                new [] { "EdgyBossText9", "Of all my segments to get hit by..." },
                new [] { "EdgyBossText10", "It's not over yet, kid!" },
                new [] { "EdgyBossText11", "A GOD DOES NOT FEAR DEATH!" },
                new [] { "EdgyBossText12", "You are no god... but I shall feast upon your essence regardless!" },
                new [] { "DoGBossText", "The frigid moon shimmers brightly." },
                new [] { "DoGBossText2", "The harvest moon glows eerily." },

                new [] { "AstralText", "A star has fallen from the heavens!" },
                new [] { "AureusBossText", "The astral enemies have been empowered!" },
                new [] { "AureusBossText2", "A faint ethereal click can be heard from the dungeon." },
                new [] { "AstralBossText", "The seal of the stars has been broken! You can now mine Astral Ore." },
                new [] { "DeusText", "A star-spawned horror tunnels through the astral infection." },
                new [] { "DeusAltarRejectNightText", "The god of the stars rejects your offering. The ritual can only be performed at night." },

                new [] { "CalamitasBossText", "You underestimate my power..." },
                new [] { "CalamitasBossText2", "The brothers have been reborn!" },
                new [] { "CalamitasBossText3", "Impressive child, most impressive..." },

                new [] { "SandSharkText", "Something stirs in the warm desert sands..." },
                new [] { "SandSharkText2", "An enormous apex predator approaches..." },
                new [] { "SandSharkText3", "The desert sand shifts intensely!" },

                new [] { "CryogenBossText", "Cryogen is derping out!" },

                new [] { "DraedonIntroductionText1", "I have waited long for this moment." },
                new [] { "DraedonIntroductionText2", "Your nature fascinates me, for I do not understand it." },
                new [] { "DraedonIntroductionText3", "You will face my creations which have surpassed gods." },
                new [] { "DraedonIntroductionText4", "And you will show me your disposition through battle." },
                new [] { "DraedonIntroductionText5", "Now, choose." },
                new [] { "DraedonExoPhase1Text1", "Designs improved with time and knowledge are the essence of my work." },
                new [] { "DraedonExoPhase1Text2", "Through no other method can I approach perfection." },
                new [] { "DraedonExoPhase2Text1", "Your performance falls neatly within the margins of error." },
                new [] { "DraedonExoPhase2Text2", "That is quite satisfactory. We will proceed to the next phase of testing." },
                new [] { "DraedonExoPhase3Text1", "Ever since I was alerted to your presence, I have processed your battles in order to make my machines stronger." },
                new [] { "DraedonExoPhase3Text2", "Even now, I monitor your actions. Nothing should escape the bounds of my calculations." },
                new [] { "DraedonExoPhase4Text1", "Curious. Very curious." },
                new [] { "DraedonExoPhase4Text2", "You progress steadily against more difficult challenges." },
                new [] { "DraedonExoPhase5Text1", "Your nature remains unknown to me. This will not do." },
                new [] { "DraedonExoPhase5Text2", "...I sought perfection. Fate must favor irony, for that must have been my first mistake." },
                new [] { "DraedonExoPhase6Text1", "Absurd." },
                new [] { "DraedonExoPhase6Text2", "I will no longer let my calculations impede my observation of this battle." },
                new [] { "DraedonExoPhase6Text3", "I shall show you the full fury of my final machine." },
                new [] { "DraedonAresEnrageText", "How foolish. You cannot escape." },
                new [] { "DraedonResummonText", "Make your choice." },
                new [] { "DraedonEndText1", "An unknown factor-a catalyst is what you are." },
                new [] { "DraedonEndText2", "Nearly as alien as I, to this land and its history." },
                new [] { "DraedonEndText3", "...Excuse my introspection. I must gather my thoughts after that display." },
                new [] { "DraedonEndText4", "This land has become stale and stiff with blood that has been spilled until now." },
                new [] { "DraedonEndText5", "You have also spilled blood, but it may be enough to usher a new age... Of what, I do not know. But it is something I am eager to see." },
                new [] { "DraedonEndText6", "Now. You wish to reach the Tyrant. I cannot assist you in that." },
                new [] { "DraedonEndText7", "It is not a matter of spite, for I would wish nothing more than to observe such a conflict." },
                new [] { "DraedonEndText8", "But you have managed before. You will find a way eventually." },
                new [] { "DraedonEndText9", "I must acknowledge your triumph, but I will return now to my machinery." },
                new [] { "DraedonEndKillAttemptText", "...Quite unnecessary." },

                new [] { "HardmodeOreTier1Text", "Your world has been blessed with Cobalt and Palladium!" },
                new [] { "HardmodeOreTier2Text", "Your world has been blessed with Mythril and Orichalcum!" },
                new [] { "HardmodeOreTier3Text", "Your world has been blessed with Adamantite and Titanium!" },
                new [] { "HardmodeOreTier4Text", "The hallow has been blessed with consecrated stone!" },

                new [] { "BloodMoonText", "The Blood Moon is rising..." },

                new [] { "DargonBossText", "The dark sun awaits." },
                new [] { "DargonBossText2", "My dragon deems you an unworthy opponent. You must acquire the power of the dark sun to witness his true power." },

                new [] { "RevengeText", "Revengeance is active." },
                new [] { "RevengeText2", "Revengeance is not active." },

                new [] { "DeathText", "Death is active; enjoy the fun." },
                new [] { "DeathText2", "Death is not active; not fun enough for you?" },

                new [] { "MaliceText", "Malice is active; witness the horror!" },
                new [] { "MaliceText2", "Malice is not active." },

                new [] { "ArmageddonText", "Bosses will now kill you instantly." },
                new [] { "ArmageddonText2", "Bosses will no longer kill you instantly." },
                new [] { "ArmageddonDodgeDisable", "All of your dodges are now disabled." },
                new [] { "ArmageddonDodgeEnable", "All of your dodges are now re-enabled." },

                new [] { "ChangingTheRules", "You cannot change the rules now." },

                new [] { "FlameText", "The air is getting warmer around you." },

                new [] { "BossRushStartText", "Hmm? Ah, another contender. Very well, may the ritual commence!" },
                new [] { "BossRushTierOneEndText", "Hmm? Oh, you're still alive. Unexpected, but don't get complacent just yet." },
                new [] { "BossRushTierTwoEndText", "Hmm? Persistent aren't you? Perhaps you have some hope of prosperity, unlike past challengers." },
                new [] { "BossRushTierThreeEndText", "Hmm? Your perseverance is truly a trait to behold. You've come further than even the demigods in such a short time." },
                new [] { "BossRushTierThreeEndText2", "May your skills remain sharp for the last challenges." },
                new [] { "BossRushTierFourEndText", "Hmm? So you've made it to the final tier, a remarkable feat enviable by even the mightiest of the gods." },
                new [] { "BossRushTierFourEndText2", "Go forth and conquer 'til the ritual's end!" },
                new [] { "BossRushTierFiveEndText", "Hmm? You expected a reward beyond this mere pebble? Patience, the true reward will become apparent in time..." },

                new [] { "MeleeLevelUp", "Melee weapon proficiency level up!" },
                new [] { "MeleeLevelUpFinal", "Melee weapon proficiency maxed out!" },
                new [] { "RangedLevelUp", "Ranged weapon proficiency level up!" },
                new [] { "RangedLevelUpFinal", "Ranged weapon proficiency maxed out!" },
                new [] { "MagicLevelUp", "Magic weapon proficiency level up!" },
                new [] { "MagicLevelUpFinal", "Magic weapon proficiency maxed out!" },
                new [] { "SummonLevelUp", "Summoner weapon proficiency level up!" },
                new [] { "SummonLevelUpFinal", "Summoner weapon proficiency maxed out!" },
                new [] { "RogueLevelUp", "Rogue weapon proficiency level up!" },
                new [] { "RogueLevelUpFinal", "Rogue weapon proficiency maxed out!" },

                new [] { "OpenSunkenSea", "The depths of the underground desert are rumbling..." },
                new [] { "SandstormTrigger", "The desert wind is blowing furiously!" },

                new [] { "ThirdSageBlessingText", "You have been blessed by the Third Sage!" },
                new [] { "ThirdSageBlessingText2", "The Third Sage has rescinded its blessing..." },

                new [] { "AprilFools", "The LORDE is approaching..." },
                new [] { "AprilFools2", "A boomer awaits..." }, //possible alternative to GhostBossText4

                new [] { "AcidRainStart", "A toxic downpour falls over the wasteland seas!" },
                new [] { "AcidRainEnd", "The sulphuric skies begin to clear..." },

                new [] { "DefenseDamage", "Defense Damage" },

                new [] { "Tier1ArsenalRecipeCondition", "View an unencrypted schematic from the lab near the Sunken Sea" },
                new [] { "Tier2ArsenalRecipeCondition", "Decrypt a schematic from the lab in the large planetoid in the sky" },
                new [] { "Tier3ArsenalRecipeCondition", "Decrypt a schematic from the lab deep within the jungle" },
                new [] { "Tier4ArsenalRecipeCondition", "Decrypt a schematic from the lab near the edges of the underworld" },
                new [] { "Tier5ArsenalRecipeCondition", "Decrypt a schematic from the lab deep within the frozen caverns" },
            };
        }

        public static void Unload()
        {
            _localizations = null;
        }

        public static void AddLocalizations()
        {
            Load();
            foreach (string[] localization in _localizations)
            {
                ModTranslation text = ModContent.GetInstance<CalamityMod>().CreateTranslation(localization[0]);
                text.SetDefault(localization[1]);
                ModContent.GetInstance<CalamityMod>().AddTranslation(text);
            }
            Unload();
        }
    }
}
