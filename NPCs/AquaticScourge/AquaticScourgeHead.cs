using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Events;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.NPCs.TownNPCs;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Armor.Vanity;

namespace CalamityMod.NPCs.AquaticScourge
{
	[AutoloadBossHead]
    public class AquaticScourgeHead : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aquatic Scourge");
        }

        public override void SetDefaults()
        {
            npc.npcSlots = 16f;
			npc.GetNPCDamage();
			npc.Calamity().canBreakPlayerDefense = true;
			npc.width = 90;
            npc.height = 90;
            npc.defense = 10;
			npc.DR_NERD(0.05f);
            npc.aiStyle = -1;
            aiType = -1;
            npc.LifeMaxNERB(80000, 92000, 1000000);
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            npc.lifeMax += (int)(npc.lifeMax * HPBoost);
            npc.knockBackResist = 0f;
            npc.value = Item.buyPrice(0, 12, 0, 0);
            npc.behindTiles = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.netAlways = true;
            bossBag = ModContent.ItemType<AquaticScourgeBag>();

			if (CalamityWorld.death || BossRushEvent.BossRushActive || CalamityWorld.malice)
				npc.scale = 1.2f;
			else if (CalamityWorld.revenge)
				npc.scale = 1.15f;
			else if (Main.expertMode)
				npc.scale = 1.1f;
        }

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(npc.chaseable);
			writer.Write(npc.localAI[1]);
			for (int i = 0; i < 4; i++)
				writer.Write(npc.Calamity().newAI[i]);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			npc.chaseable = reader.ReadBoolean();
			npc.localAI[1] = reader.ReadSingle();
			for (int i = 0; i < 4; i++)
				npc.Calamity().newAI[i] = reader.ReadSingle();
		}

        public override void AI()
        {
			if (npc.justHit || npc.life <= npc.lifeMax * 0.99 || BossRushEvent.BossRushActive)
                music = CalamityMod.Instance.GetMusicFromMusicMod("AquaticScourge") ?? MusicID.Boss2;

			CalamityAI.AquaticScourgeAI(npc, mod, true);
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (npc.spriteDirection == 1)
				spriteEffects = SpriteEffects.FlipHorizontally;

			Texture2D texture2D15 = Main.npcTexture[npc.type];
			Vector2 vector11 = new Vector2(Main.npcTexture[npc.type].Width / 2, Main.npcTexture[npc.type].Height / 2);

			Vector2 vector43 = npc.Center - Main.screenPosition;
			vector43 -= new Vector2(texture2D15.Width, texture2D15.Height) * npc.scale / 2f;
			vector43 += vector11 * npc.scale + new Vector2(0f, npc.gfxOffY);
			Color color = npc.GetAlpha(lightColor);

			if (npc.Calamity().newAI[3] > 480f && (CalamityWorld.revenge || BossRushEvent.BossRushActive || CalamityWorld.malice))
				color = Color.Lerp(color, Color.SandyBrown, MathHelper.Clamp((npc.Calamity().newAI[3] - 480f) / 180f, 0f, 1f));

			spriteBatch.Draw(texture2D15, vector43, npc.frame, color, npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			return false;
		}

		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            Rectangle targetHitbox = target.Hitbox;

            float dist1 = Vector2.Distance(npc.Center, targetHitbox.TopLeft());
            float dist2 = Vector2.Distance(npc.Center, targetHitbox.TopRight());
            float dist3 = Vector2.Distance(npc.Center, targetHitbox.BottomLeft());
            float dist4 = Vector2.Distance(npc.Center, targetHitbox.BottomRight());

            float minDist = dist1;
            if (dist2 < minDist)
                minDist = dist2;
            if (dist3 < minDist)
                minDist = dist3;
            if (dist4 < minDist)
                minDist = dist4;

            return minDist <= 50f;
        }

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            if (projectile.minion && !projectile.Calamity().overridesMinionDamagePrevention)
                return npc.Calamity().newAI[0] == 1f;

            return null;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.playerSafe)
                return 0f;

            if (spawnInfo.player.Calamity().ZoneSulphur && spawnInfo.water)
            {
                if (!NPC.AnyNPCs(ModContent.NPCType<AquaticScourgeHead>()))
                    return 0.01f;
            }

            return 0f;
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ModContent.ItemType<SulphurousSand>();
        }

        public override bool SpecialNPCLoot()
        {
            int closestSegmentID = DropHelper.FindClosestWormSegment(npc,
                ModContent.NPCType<AquaticScourgeHead>(),
                ModContent.NPCType<AquaticScourgeBody>(),
                ModContent.NPCType<AquaticScourgeBodyAlt>(),
                ModContent.NPCType<AquaticScourgeTail>());
            npc.position = Main.npc[closestSegmentID].position;
            return false;
        }

        public override void NPCLoot()
        {
            DropHelper.DropBags(npc);

			// Legendary drops for Aquatic Scourge
			DropHelper.DropItemCondition(npc, ModContent.ItemType<SeasSearing>(), true, CalamityWorld.malice);
			DropHelper.DropItemCondition(npc, ModContent.ItemType<DeepDiver>(), true, CalamityWorld.malice);

			DropHelper.DropItem(npc, ItemID.GreaterHealingPotion, 8, 14);
			DropHelper.DropItemChance(npc, ModContent.ItemType<AquaticScourgeTrophy>(), 10);
			DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeAquaticScourge>(), true, !CalamityWorld.downedAquaticScourge);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeSulphurSea>(), true, !CalamityWorld.downedAquaticScourge);

			CalamityGlobalTownNPC.SetNewShopVariable(new int[] { ModContent.NPCType<SEAHOE>() }, CalamityWorld.downedAquaticScourge);

			// All other drops are contained in the bag, so they only drop directly on Normal
			if (!Main.expertMode)
            {
                // Materials
                DropHelper.DropItem(npc, ModContent.ItemType<VictoryShard>(), 11, 20);
                DropHelper.DropItem(npc, ItemID.Coral, 5, 9);
                DropHelper.DropItem(npc, ItemID.Seashell, 5, 9);
                DropHelper.DropItem(npc, ItemID.Starfish, 5, 9);

                // Weapons
                float w = DropHelper.NormalWeaponDropRateFloat;
                DropHelper.DropEntireWeightedSet(npc,
                    DropHelper.WeightStack<SubmarineShocker>(w),
                    DropHelper.WeightStack<Barinautical>(w),
                    DropHelper.WeightStack<Downpour>(w),
                    DropHelper.WeightStack<DeepseaStaff>(w),
                    DropHelper.WeightStack<ScourgeoftheSeas>(w)
                );

                // Equipment
                DropHelper.DropItemChance(npc, ModContent.ItemType<AeroStone>(), 9);
                DropHelper.DropItemChance(npc, ModContent.ItemType<CorrosiveSpine>(), 9);

                // Vanity
                DropHelper.DropItemChance(npc, ModContent.ItemType<AquaticScourgeMask>(), 7);

                // Fishing
                DropHelper.DropItem(npc, ModContent.ItemType<BleachedAnglingKit>());
            }

            // If Aquatic Scourge has not yet been killed, notify players of buffed Acid Rain
            if (!CalamityWorld.downedAquaticScourge)
            {
                if (!Main.player[Main.myPlayer].dead && Main.player[Main.myPlayer].active)
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/MaulerRoar"), (int)Main.player[Main.myPlayer].position.X, (int)Main.player[Main.myPlayer].position.Y);

                string sulfSeaBoostKey = "Mods.CalamityMod.WetWormBossText";
                Color sulfSeaBoostColor = AcidRainEvent.TextColor;

                CalamityUtils.DisplayLocalizedText(sulfSeaBoostKey, sulfSeaBoostColor);
                //set a timer for acid rain to start after 10 seconds
                CalamityWorld.forceRainTimer = 601;
            }

            // Mark Aquatic Scourge as dead
            CalamityWorld.downedAquaticScourge = true;
            CalamityNetcode.SyncWorld();
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
			npc.damage = (int)(npc.damage * npc.GetExpertDamageMultiplier());
		}

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);

            if (npc.life <= 0)
            {
                for (int k = 0; k < 15; k++)
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);

                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/AquaticScourgeGores/ASHead"), 1f);
            }
        }

        public override bool CheckActive()
        {
            if (npc.Calamity().newAI[0] == 1f && !Main.player[npc.target].dead && npc.Calamity().newAI[1] != 1f)
                return false;

            return true;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<Irradiated>(), 360, true);
        }
    }
}
