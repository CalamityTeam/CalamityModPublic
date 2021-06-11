using Terraria.ModLoader;

namespace CalamityMod.Localization
{
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

                new [] { "BrimmyBossText", "A protective spell has been lifted from the crags! You can now mine Charred Ore." },

                new [] { "WetWormBossText", "The sulphuric sky darkens..." },

                new [] { "PlantBossText", "The ocean depths are trembling." },

                new [] { "BabyBossText", "A plague has befallen the Jungle." },

                new [] { "MoonBossText", "The profaned flame blazes fiercely!" },
                new [] { "MoonBossText2", "Cosmic terrors are watching..." },
                new [] { "MoonBossText3", "The bloody moon beckons..." },

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
                new [] { "SCalDesparationText3", "Once you have bested me, you too will only have one path forward." },
                new [] { "SCalDesparationText4", "And that path... also has no future." },
                new [] { "SCalAcceptanceText1", "Even if he has thrown all else away, his power remains." },
                new [] { "SCalAcceptanceText2", "I have no more energy left to resent him, or you..." },
                new [] { "SCalAcceptanceText3", "It will all be in your hands now." },

                // Some of these keys are incorrect in terms of their contents, but they remain as their bases + an addon at the end to make it 
                // easier to define them.
                new [] { "SCalSummonTextRematch", "You called? If you're looking to get scorched, you've got the right person." },
                new [] { "SCalStartTextRematch", "Ha! You saw it coming this time." },
                new [] { "SCalBH2TextRematch", "Don't get cocky just because you've won once." },
                new [] { "SCalBH3TextRematch", "You know, I was expecting a house call from the Tyrant at this point. Think he died?" },
                new [] { "SCalBH3TextRematch2", "Haha, no way." },
                new [] { "SCalBrothersTextRematch", "Would you like to meet my family? Horrible, isn't it?" },
                new [] { "SCalBrothersTextRematch2", "I'm joking. They are but empty shells now. Did I frighten you?" },
                new [] { "SCalPhase2TextRematch", "Here we go again." },
                new [] { "SCalBH4TextRematch", "Remember these? I bet you've seen them a few times since the last battle... in your nightmares." },
                new [] { "SCalSeekerRingTextRematch", "It seems your skill hasn't faltered in the least." },
                new [] { "SCalBH5TextRematch", "One last go." },
                new [] { "SCalSepulcher2TextRematch", "And... you never cease to amaze." },
                new [] { "SCalDesparationText1Rematch", "You may even have a proper chance now." },
                new [] { "SCalDesparationText2Rematch", "I wonder if you've met his mechanic yet." },
                new [] { "SCalDesparationText3Rematch", "I trust you will not make the same mistakes he did." },
                new [] { "SCalDesparationText4Rematch", "Now... even I cannot see what your future holds." },

                new [] { "SCalFunnyCheatText", "Go to hell." },

                new [] { "EdgyBossText", "Don't get cocky, kid!" },
                new [] { "EdgyBossText2", "You think...you can butcher...ME!?" },
                new [] { "EdgyBossText3", "A fatal mistake!" },
                new [] { "EdgyBossText4", "Good luck recovering from that!" },
                new [] { "EdgyBossText5", "Delicious..." },
                new [] { "EdgyBossText6", "Did that hurt?" },
                new [] { "EdgyBossText7", "Nothing personal, kid." },
                new [] { "EdgyBossText8", "Are you honestly that bad at dodging?" },
                new [] { "EdgyBossText9", "Of all my segments to get hit by..." },
                new [] { "EdgyBossText10", "It's not over yet, kid!" },
                new [] { "EdgyBossText11", "A GOD DOES NOT FEAR DEATH!" },
                new [] { "EdgyBossText12", "You are no god...but I shall feast upon your essence regardless!" },
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

                new [] { "DefiledText", "Your soul is mine..." },
                new [] { "DefiledText2", "Your soul is yours once more..." },

                new [] { "IronHeartText", "Iron Heart is active, healing is disabled." },
                new [] { "IronHeartText2", "Iron Heart is not active, healing is restored." },

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

                new [] { "BossSpawnText", "Something is approaching..." },

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
