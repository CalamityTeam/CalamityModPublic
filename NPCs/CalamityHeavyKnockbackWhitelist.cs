using System;
using System.Collections.Generic;
using System.Reflection;
using CalamityMod.Balancing;
using CalamityMod.Items.Accessories;
using CalamityMod.Systems.Collections;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.NPCs
{
    public sealed class CalamityHeavyKnockbackWhitelist : GlobalNPC
    {
        // General rule of thumb I used for this is to not put it on anything that is a stationary entity (EX: Tesla Turrets, Antlions, Hive Cysts)
        // No putting it on any bosses, though boss minions are fair game when applicable (no stuff like Scal hearts obviously, sending them into the void would be bad)
        internal static HashSet<int> whitelistNPC;
        public override void Load()
        {
            whitelistNPC = new();
        }
        public override void Unload()
        {
            whitelistNPC?.Clear();
            whitelistNPC = null;
        }
        public override void SetStaticDefaults()
        {
            // Add vanilla enemies to the whitelist
            whitelistNPC.Add(NPCID.AncientCultistSquidhead);
            whitelistNPC.Add(NPCID.DesertDjinn);
            whitelistNPC.Add(NPCID.DD2DrakinT2);
            whitelistNPC.Add(NPCID.DD2DrakinT3);
            whitelistNPC.Add(NPCID.GrayGrunt);
            whitelistNPC.Add(NPCID.HeadlessHorseman);
            whitelistNPC.Add(NPCID.LihzahrdCrawler);
            whitelistNPC.Add(NPCID.MartianProbe);
            whitelistNPC.Add(NPCID.MartianWalker);
            whitelistNPC.Add(NPCID.Paladin);
            whitelistNPC.Add(NPCID.PirateCaptain);
            whitelistNPC.Add(NPCID.ShadowFlameApparition);
            whitelistNPC.Add(NPCID.ThePossessed);
            whitelistNPC.Add(NPCID.EyeballFlyingFish);
            whitelistNPC.Add(NPCID.Yeti);
            whitelistNPC.Add(NPCID.ZombieMerman);
            whitelistNPC.Add(NPCID.FungiBulb);
            whitelistNPC.Add(NPCID.GiantFungiBulb);
            whitelistNPC.Add(NPCID.ManEater);
            whitelistNPC.Add(NPCID.Snatcher);
            whitelistNPC.Add(NPCID.BloodEelHead);
            whitelistNPC.Add(NPCID.BoneSerpentHead);
            whitelistNPC.Add(NPCID.DevourerHead);
            whitelistNPC.Add(NPCID.DiggerHead);
            whitelistNPC.Add(NPCID.DuneSplicerHead);
            whitelistNPC.Add(NPCID.GiantWormHead);
            whitelistNPC.Add(NPCID.LeechHead);
            whitelistNPC.Add(NPCID.StardustWormHead);
            whitelistNPC.Add(NPCID.CultistDragonHead);
            whitelistNPC.Add(NPCID.TombCrawlerHead);
            whitelistNPC.Add(NPCID.SeekerHead);
            whitelistNPC.Add(NPCID.WyvernHead);
            whitelistNPC.Add(NPCID.Creeper); // Here because they gain KB immunity in multiplayer
            whitelistNPC.Add(NPCID.TheHungry); // Here because they gain KB immunity in multiplayer
            whitelistNPC.Add(NPCID.TheHungryII); // Here because they gain KB immunity in multiplayer
            whitelistNPC.Add(NPCID.DD2DarkMageT1);
            whitelistNPC.Add(NPCID.DD2DarkMageT3);
            whitelistNPC.Add(NPCID.DD2OgreT2);
            whitelistNPC.Add(NPCID.DD2OgreT3);
            whitelistNPC.Add(NPCID.MourningWood);
            whitelistNPC.Add(NPCID.Pumpking);
            whitelistNPC.Add(NPCID.Everscream);
            whitelistNPC.Add(NPCID.SantaNK1);
            whitelistNPC.Add(NPCID.IceQueen);
            whitelistNPC.Add(NPCID.MartianSaucerCore);
            whitelistNPC.Add(NPCID.Sharkron);
            whitelistNPC.Add(NPCID.Sharkron2);
            // Below are enemies Calamity gives KB immunity, but that aren't KB immune in vanilla
            whitelistNPC.Add(NPCID.Mothron);
            whitelistNPC.Add(NPCID.BigMimicCorruption);
            whitelistNPC.Add(NPCID.BigMimicCrimson);
            whitelistNPC.Add(NPCID.BigMimicHallow);
            whitelistNPC.Add(NPCID.BigMimicJungle);
            whitelistNPC.Add(NPCID.SolarCorite);
            whitelistNPC.Add(NPCID.CrimsonAxe);
            whitelistNPC.Add(NPCID.CursedHammer);
            whitelistNPC.Add(NPCID.EnchantedSword);
            whitelistNPC.Add(NPCID.GiantTortoise);
            whitelistNPC.Add(NPCID.IceTortoise);
            whitelistNPC.Add(NPCID.GoblinSummoner);
            whitelistNPC.Add(NPCID.Butcher);

            var npcs = GetContent<ModNPC>();
            foreach (var npc in npcs)
            {
                try
                {
                    var type = npc.GetType();
                    var whitelisted = type.GetCustomAttribute<HeavyKnockbackWhitelistedAttribute>();
                    if (whitelisted != null)
                    {
                        int npcType = npc.Type;
                        whitelistNPC.Add(npcType);
                    }
                }
                catch (Exception e)
                {
                    CalamityMod.Log.Error($"Exception thrown while evaluating type \"{npc.FullName}\": {e}");
                }
            }
        }
    }
}
