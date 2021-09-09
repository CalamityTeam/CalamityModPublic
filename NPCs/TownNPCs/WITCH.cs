using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.Items;
using CalamityMod.Projectiles.Magic;
using CalamityMod.UI.CalamitasEnchants;
using CalamityMod.World;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using static Terraria.ModLoader.ModContent;
using SCalBoss = CalamityMod.NPCs.SupremeCalamitas.SupremeCalamitas;

namespace CalamityMod.NPCs.TownNPCs
{
	[AutoloadHead]
	public class WITCH : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Brimstone Witch");

			Main.npcFrameCount[npc.type] = 27;
			NPCID.Sets.ExtraFramesCount[npc.type] = 9;
			NPCID.Sets.AttackFrameCount[npc.type] = 4;
			NPCID.Sets.DangerDetectRange[npc.type] = 700;
			NPCID.Sets.AttackType[npc.type] = 1;
			NPCID.Sets.AttackTime[npc.type] = 30;
			NPCID.Sets.AttackAverageChance[npc.type] = 5;
		}

		public override void SetDefaults()
		{
			npc.townNPC = true;
			npc.friendly = true;
			npc.lavaImmune = true;
			npc.width = 18;
			npc.height = 40;
			npc.aiStyle = 7;
			npc.damage = 10;

			// You should not be able to kill SCal under any typical circumstances.
			npc.lifeMax = 1000000;

			npc.defense = 120;
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath6;
			npc.knockBackResist = 0.8f;
			animationType = NPCID.Wizard;
		}

		public override bool CanTownNPCSpawn(int numTownNPCs, int money) => CalamityWorld.downedSCal && !NPC.AnyNPCs(NPCType<SCalBoss>());

		public override string TownNPCName() => "Calamitas";

		// For Mrrp when he works on this.
		// These comments can be removed once he's done modifying this and understands how it works.

		// The way this works is by having an RNG based on weights.
		// With certain conditions (such as if a blood moon is happening) you can add possibilities
		// to the RNG via textSelector.Add("text", weight);
		// Text can always appear assuming the weight is greater than 0 and there's no if condition deciding whether it can.
		// The higher the weight is, the more likely it is to be selected from all the choices.
		// To give an example of this, assume you have two possibilities:
		// "a" with a weight of 1, and "b" with a weight of 5. The chance of "a" being displayed would be
		// 1/6, while "b" wold have a 5/6 chance of being displayed.
		// If only one possibility exists it will be the only thing that is displayed, regardless of weight.
		public override string GetChat()
		{
			WeightedRandom<string> textSelector = new WeightedRandom<string>(Main.rand);
			textSelector.Add("Test", 0.95);

			// Select a possibility from the RNG and choose it as the thing that should be said.
			string thingToSay = textSelector.Get();

			// Have a flat chance (1/4444) to simply ignore the above selection and say something humorous instead.
			if (Main.rand.NextBool(4444))
				thingToSay = "Mrrp is cringe.";

			return thingToSay;
		}

		public override void SetChatButtons(ref string button, ref string button2) => button = "Enchant";

		public override void OnChatButtonClicked(bool firstButton, ref bool shop)
		{
			if (firstButton)
			{
				Main.playerInventory = true;
				Main.LocalPlayer.Calamity().newCalamitasInventory = false;
				CalamitasEnchantUI.NPCIndex = npc.whoAmI;
				CalamitasEnchantUI.CurrentlyViewing = true;

				if (!Main.LocalPlayer.Calamity().GivenBrimstoneLocus)
				{
					DropHelper.DropItem(npc, ItemType<BrimstoneLocus>());
					Main.LocalPlayer.Calamity().GivenBrimstoneLocus = true;
				}

				shop = false;
			}
		}

		// Make this Town NPC teleport to the Queen statue when triggered.
		public override bool CanGoToStatue(bool toKingStatue) => !toKingStatue;

		public override void TownNPCAttackStrength(ref int damage, ref float knockback)
		{
			damage = 50;
			knockback = 10f;
		}

		public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
		{
			cooldown = 10;
			randExtraCooldown = 50;
		}

        public override bool PreAI()
        {
			// Disappear if the SCal boss is active. She's supposed to be the boss.
			// However, this doesn't happen in Boss Rush; the SCal there is a silent puppet created by Xeroc, not SCal herself.
			if (NPC.AnyNPCs(NPCType<SCalBoss>()) && !BossRushEvent.BossRushActive)
            {
				npc.active = false;
				npc.netUpdate = true;
				return false;
            }
            return true;
        }

        //public override void TownNPCAttackMagic(ref float auraLightMultiplier)	

        public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
		{
			projType = ProjectileType<SeethingDischargeBrimstoneHellblast>();
			attackDelay = 1;
		}

		public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
		{
			multiplier = 2f;
		}

		// Explode into red dust on death.
		public override void HitEffect(int hitDirection, double damage)
		{
			if (npc.life <= 0)
			{
				npc.position = npc.Center;
				npc.width = npc.height = 50;
				npc.position.X -= npc.width / 2;
				npc.position.Y -= npc.height / 2;
				for (int i = 0; i < 5; i++)
				{
					int brimstone = Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 2f);
					Main.dust[brimstone].velocity *= 3f;
					if (Main.rand.NextBool(2))
					{
						Main.dust[brimstone].scale = 0.5f;
						Main.dust[brimstone].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
					}
				}

				for (int i = 0; i < 10; i++)
				{
					int fire = Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 3f);
					Main.dust[fire].noGravity = true;
					Main.dust[fire].velocity *= 5f;

					fire = Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 2f);
					Main.dust[fire].velocity *= 2f;
				}
			}
		}
	}
}
