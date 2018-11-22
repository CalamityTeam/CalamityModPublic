using System;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod
{
	public class ModSupport
    {	
		public static object Call(params object[] args)
		{
			if (args.Length <= 0 || !(args[0] is string)) return new Exception("CalamityMod Error: NO METHOD NAME! First param MUST be a method name!");
			string methodName = (string)args[0];		
			if (methodName.Equals("Downed")) //returns a Func which will return a downed value based on name.
			{
				Func<string, bool> downed = (name) => 
				{
					name = name.ToLower();
					switch (name)
					{
						default: return false;
						case "desertscourge": return CalamityWorld.downedDesertScourge;
						case "crabulon": return CalamityWorld.downedCrabulon;
						case "hivemind": return CalamityWorld.downedHiveMind;
						case "perforator":
						case "perforators": return CalamityWorld.downedPerforator;
						case "slimegod": return CalamityWorld.downedSlimeGod;
						case "cryogen": return CalamityWorld.downedCryogen;
						case "brimstoneelemental": return CalamityWorld.downedBrimstoneElemental;
						case "calamitas": return CalamityWorld.downedCalamitas;
						case "leviathan": return CalamityWorld.downedLeviathan;
						case "astrumdeus": return CalamityWorld.downedStarGod;
						case "plaguebringer": return CalamityWorld.downedPlaguebringer;
						case "ravager": return CalamityWorld.downedScavenger;
						case "guardians":  return CalamityWorld.downedGuardians;
						case "providence": return CalamityWorld.downedProvidence;
						case "polterghast": return CalamityWorld.downedPolterghast;
						case "sentinelany": return (CalamityWorld.downedSentinel1 || CalamityWorld.downedSentinel2 || CalamityWorld.downedSentinel3);
						case "sentinelall": return (CalamityWorld.downedSentinel1 && CalamityWorld.downedSentinel2 && CalamityWorld.downedSentinel3);
						case "sentinel1": return CalamityWorld.downedSentinel1;
						case "sentinel2": return CalamityWorld.downedSentinel2;
						case "sentinel3": return CalamityWorld.downedSentinel3;
						case "devourerofgods": return CalamityWorld.downedDoG;
						case "bumblebirb": return CalamityWorld.downedBumble;
						case "yharon": return CalamityWorld.downedYharon;
						case "supremecalamitas": return CalamityWorld.downedSCal;		
					}
				};
				return downed;
			}
			else
			if (methodName.Equals("InZone")) //returns a Func which will return a zone value based on player and name.
			{
				Func<Player, string, bool> inZone = (p, name) => { return ModSupport.InZone(p, name); };
				return inZone;
			}
			return new Exception("CalamityMod Error: NO METHOD FOUND: " + methodName);
		}

        public static bool InZone(Player p, string zoneName)
        {
			Mod calamity = ModLoader.GetMod("CalamityMod");
			zoneName = zoneName.ToLower();
            switch (zoneName)
            {
                case "calamity": return p.GetModPlayer<CalamityPlayer>(calamity).ZoneCalamity;
                case "astral": return p.GetModPlayer<CalamityPlayer>(calamity).ZoneAstral;
            }
            return false;
        }
    }
}