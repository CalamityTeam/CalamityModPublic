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

                new [] { "SupremeBossText3", "Alright, let's get started. Not sure why you're bothering." }, // start
                new [] { "SupremeBossText4", "You seem so confident, even though you are painfully ignorant of what has yet to transpire." }, //75%
                new [] { "SupremeBossText5", "Everything was going well until you came along." }, //75%
                new [] { "SupremeBossText6", "Brothers, could you assist me for a moment? This ordeal is growing tiresome." }, //45%
                new [] { "SupremeBossText", "Don't worry, I still have plenty of tricks left." }, //40%
                new [] { "SupremeBossText7", "Hmm...perhaps I should let the little ones out to play for a while." },//30%
                new [] { "SupremeBossText8", "Impressive...but still not good enough!" }, //20%
                new [] { "SupremeBossText9", "I'm just getting started!" }, //10%
                new [] { "SupremeBossText20", "How are you still alive!?" }, //8%
                new [] { "SupremeBossText21", "Just stop!" }, //6%
                new [] { "SupremeBossText22", "Even if you defeat me you would still have the lord to contend with!" }, //4%
                new [] { "SupremeBossText23", "He has never lost a battle!" }, //2%
                new [] { "SupremeBossText24", "Not even I could defeat him! What hope do you have!?" }, //1%
                new [] { "SupremeBossText25", "He has grown far stronger since we last fought...you stand no chance." }, //1% after 10 seconds
                new [] { "SupremeBossText26", "Well...I suppose this is the end..." }, //1% after 15 seconds
                new [] { "SupremeBossText27", "Perhaps one of these times I'll change my mind..." }, //1% after first win
                new [] { "SupremeBossText28", "You aren't hurting as much as I'd like...are you cheating?" }, //not taking enough damage
                new [] { "SupremeBossText2", "Go to hell." }, //cheater
                new [] { "SupremeBossText10", "At long last I am free...for a time. I'll keep coming back, just like you. Until we meet again, farewell." }, //end after 20 seconds
                new [] { "SupremeBossText11", "Do you enjoy going through hell?" }, //rebattle after killing once
                new [] { "SupremeBossText12", "Don't get me wrong, I like pain too, but you're just ridiculous." }, //rebattle after killing four times
                new [] { "SupremeBossText13", "You must enjoy dying more than most people, huh?" }, //rebattle five deaths
                new [] { "SupremeBossText14", "Do you have a fetish for getting killed or something?" }, //rebattle twenty deaths
                new [] { "SupremeBossText15", "Alright, I'm done counting. You probably died this much just to see what I'd say." }, //rebattle fifty deaths
                new [] { "SupremeBossText16", "You didn't die at all huh? Welp, you probably cheated. Do it again, for real this time...but here's your reward I guess." }, //die 0 times
                new [] { "SupremeBossText17", "One death? That's it? ...I guess you earned this then." },  //die 1 time and win
                new [] { "SupremeBossText18", "Two deaths, nice job. Here's your reward." }, //die 2 times and win
                new [] { "SupremeBossText19", "Third time's the charm. Here's a special reward." }, //die 3 times and win

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

                new [] { "BloodMoonText", "The Blood Moon is rising..." },

                new [] { "DargonBossText", "The dark sun awaits." },
                new [] { "DargonBossText2", "My dragon deems you an unworthy opponent. You must acquire the power of the dark sun to witness his true power." },

                new [] { "RevengeText", "Revengeance is active." },
                new [] { "RevengeText2", "Revengeance is not active." },

                new [] { "DeathText", "Death is active, enjoy the fun." },
                new [] { "DeathText2", "Death is not active, not fun enough for you?" },

                new [] { "ArmageddonText", "Bosses will now kill you instantly." },
                new [] { "ArmageddonText2", "Bosses will no longer kill you instantly." },

                new [] { "DefiledText", "Your soul is mine..." },
                new [] { "DefiledText2", "Your soul is yours once more..." },

                new [] { "IronHeartText", "Iron Heart is active, healing is disabled." },
                new [] { "IronHeartText2", "Iron Heart is not active, healing is restored." },

                new [] { "ChangingTheRules", "Now is not the time to change the rules of this game!" },

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
                new [] { "AcidRainEnd", "The sulphuric skies begin to clear..." }
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
