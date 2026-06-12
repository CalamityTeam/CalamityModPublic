using System;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.NormalNPCs
{
    [HeavyKnockbackWhitelisted]
    public class AuroraSpirit : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 5;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                SpriteDirection = -1,
                PortraitPositionYOverride = -20f
            };
            value.Position.X += 4f;
            value.Position.Y -= 4f;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = NPCAIStyleID.AncientVision;
            NPC.damage = 40;
            NPC.width = 40;
            NPC.height = 24;
            NPC.defense = 8;
            NPC.alpha = 100;
            NPC.lifeMax = 200;
            NPC.value = Item.buyPrice(silver: 1);
            NPC.knockBackResist = 0f;
            NPC.HitSound = SoundID.NPCHit5;
            NPC.DeathSound = SoundID.NPCDeath15;
            NPC.coldDamage = true;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<AuroraSpiritBanner>();
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToCold = false;
            NPC.Calamity().VulnerableToSickness = false;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Snow,
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.AuroraSpirit")
            });
        }

        public override void FindFrame(int frameHeight)
        {
            int currentFrame = 1;
            if (!Main.dedServ)
            {
                if (TextureAssets.Npc[Type].Value == null)
                    return;
                currentFrame = TextureAssets.Npc[Type].Value.Height / Main.npcFrameCount[Type];
            }

            if (!NPC.IsABestiaryIconDummy)
            {
                if (NPC.velocity.X < 0f)
                    NPC.direction = -1;
                else
                    NPC.direction = 1;
                if (NPC.direction == 1)
                    NPC.spriteDirection = 1;
                if (NPC.direction == -1)
                    NPC.spriteDirection = -1;
                NPC.rotation = (float)Math.Atan2((double)NPC.velocity.Y * (double)NPC.direction, (double)NPC.velocity.X * (double)NPC.direction);
            }

            NPC.frameCounter++;
            if (NPC.frameCounter > 4)
            {
                NPC.frame.Y += currentFrame;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y / currentFrame >= Main.npcFrameCount[Type])
                NPC.frame.Y = 0;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.Player.ZoneSnow &&
                spawnInfo.Player.ZoneOverworldHeight &&
                !spawnInfo.Player.PillarZone() &&
                !spawnInfo.Player.ZoneDungeon &&
                !spawnInfo.Player.InSunkenSea() &&
                Main.hardMode && !spawnInfo.PlayerInTown && !spawnInfo.Player.ZoneOldOneArmy && !Main.snowMoon && !Main.pumpkinMoon ? 0.03f : 0f;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
            {
                target.AddBuff(BuffID.Frostburn, 120);
                target.AddBuff(BuffID.Chilled, 60);
            }
        }

        public override void AI()
        {
            Lighting.AddLight(NPC.Center, 0.02f, 0.7f, 0.7f);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.IceRod, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.IceRod, hit.HitDirection, -1f, 0, default, 1f);
                }
                if (!Main.dedServ)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("CryoSpirit").Type, 1f);
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) => npcLoot.Add(ModContent.ItemType<EssenceofEleum>());
    }
}
