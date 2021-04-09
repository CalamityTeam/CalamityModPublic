using MonoMod.Cil;
using System;
using CalamityMod.Dusts;
using CalamityMod.Items.Critters;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Astral
{
	public class Twinkler : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Twinkler");
            Main.npcFrameCount[npc.type] = 8;
            Main.npcCatchable[npc.type] = true;
        }

        public override void SetDefaults()
        {
            npc.CloneDefaults(NPCID.LightningBug); //ID is 358
            npc.width = 7;
            npc.height = 5;
            aiType = NPCID.LightningBug;
            animationType = NPCID.LightningBug;
            npc.catchItem = (short)ModContent.ItemType<TwinklerItem>();
            npc.friendly = true; // prevents critter from getting slagged
            //banner = npc.type;
            //bannerItem = ModContent.ItemType<TwinklerBanner>();
        }

        public override bool? CanBeHitByItem(Player player, Item item) => true;

        public override bool? CanBeHitByProjectile(Projectile projectile) => true;

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                for (int i = 0; i < 6; i++)
                {
                    int dust = Dust.NewDust(npc.position, npc.width, npc.height, ModContent.DustType<AstralOrange>(), 2 * hitDirection, -2f);
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].scale = 1.2f * npc.scale;
                    }
                    else
                    {
                        Main.dust[dust].scale = 0.7f * npc.scale;
                    }
                }
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (CalamityGlobalNPC.AnyEvents(spawnInfo.player))
            {
                return 0f;
            }
            else if (spawnInfo.player.InAstral())
            {
                return SpawnCondition.TownCritter.Chance;
            }
            return 0f;
        }

        public override void OnCatchNPC(Player player, Item item)
        {
            try
            {
            } catch
            {
                return;
            }
        }

		public override bool Autoload(ref string name)
		{
			IL.Terraria.Wiring.HitWireSingle += HookStatue;
			return base.Autoload(ref name);
		}

		/// <summary>
		/// Change the following code sequence in Wiring.HitWireSingle
		/// num8 = (int) Utils.SelectRandom<short>(Main.rand, new short[2]
		/// {
		/// 	355,
		/// 	358
		/// });
		/// 
		/// to 
		/// 
		/// var arr = new short[2]
		/// {
		/// 	355,
		/// 	358
		/// });
		/// arr = arr.ToList().Add(id).ToArray();
		/// num8 = Utils.SelectRandom(Main.rand, arr);
		/// 
		/// </summary>
		/// <param name="il"></param>
		private void HookStatue(ILContext il)
		{
			// obtain a cursor positioned before the first instruction of the method
			// the cursor is used for navigating and modifying the il
			var c = new ILCursor(il);

			// the exact location for this hook is very complex to search for due to the hook instructions not being unique, and buried deep in control flow
			// switch statements are sometimes compiled to if-else chains, and debug builds litter the code with no-ops and redundant locals

			// in general you want to search using structure and function rather than numerical constants which may change across different versions or compile settings
			// using local variable indices is almost always a bad idea

			// we can search for
			// switch (*)
			//   case 54:
			//     Utils.SelectRandom *

			// in general you'd want to look for a specific switch variable, or perhaps the containing switch (type) { case 105:
			// but the generated IL is really variable and hard to match in this case

			// we'll just use the fact that there are no other switch statements with case 54, followed by a SelectRandom

			ILLabel[] targets = null;
			while (c.TryGotoNext(i => i.MatchSwitch(out targets)))
			{
				// some optimising compilers generate a sub so that all the switch cases start at 0
				// ldc.i4.s 51
				// sub
				// switch
				int offset = 0;
				if (c.Prev.MatchSub() && c.Prev.Previous.MatchLdcI4(out offset))
				{
					;
				}

				// get the label for case 54: if it exists
				int case54Index = 54 - offset;
				if (case54Index < 0 || case54Index >= targets.Length || !(targets[case54Index] is ILLabel target))
				{
					continue;
				}

				// move the cursor to case 54:
				c.GotoLabel(target);
				// there's lots of extra checks we could add here to make sure we're at the right spot, such as not encountering any branching instructions
				c.GotoNext(i => i.MatchCall(typeof(Utils), nameof(Utils.SelectRandom)));

				// goto next positions us before the instruction we searched for, so we can insert our array modifying code right here
				c.EmitDelegate<Func<short[], short[]>>(arr =>
				{
					// resize the array and add our custom firefly
					Array.Resize(ref arr, arr.Length+1);
					arr[arr.Length-1] = (short)ModContent.NPCType<Twinkler>();
					return arr;
				});

				// hook applied successfully
				return;
			}

			// couldn't find the right place to insert
			throw new Exception("Hook location not found, switch(*) { case 54: ...");
		}
    }
}
