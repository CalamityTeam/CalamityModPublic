using CalamityMod.Events;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.SlimeGod
{
    public class CorruptSlimeSpawn : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers();
            value.PortraitPositionYOverride = -32f;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = NPCAIStyleID.Bat;
            NPC.GetNPCDamage();
            NPC.width = 40;
            NPC.height = 30;
            if (CalamityWorld.LegendaryMode && CalamityWorld.revenge)
                NPC.scale = 2f;

            NPC.defense = 6;
            NPC.lifeMax = BossRushEvent.BossRushActive ? 10000 : (CalamityWorld.LegendaryMode && CalamityWorld.revenge) ? 360 : 180;
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.knockBackResist = 0f;
            AnimationType = 121;
            NPC.Opacity = 0.8f;
            NPC.lavaImmune = false;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.canGhostHeal = false;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToSickness = false;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            int associatedNPCType = ModContent.NPCType<SplitEbonianPaladin>();
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[associatedNPCType], quickUnlock: true);

            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCrimson,
				new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.CorruptSlimeSpawn")
            });
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient && NPC.life <= 0)
            {
                Vector2 spawnAt = NPC.Center + new Vector2(0f, (float)NPC.height / 2f);
                NPC.NewNPC(NPC.GetSource_Loot(), (int)spawnAt.X, (int)spawnAt.Y, ModContent.NPCType<CorruptSlimeSpawn2>());
            }

            Color dustColor = Color.Lavender;
            dustColor.A = 150;
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, 4, hit.HitDirection, -1f, NPC.alpha, dustColor, 1f);
            }
        }
        public override void OnKill()
        {
            if (Main.zenithWorld && Main.netMode != NetmodeID.MultiplayerClient)
            {
                int type = ModContent.ProjectileType<ShadeNimbusHostile>();
                int damage = NPC.GetProjectileDamage(type);
                Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, Vector2.Zero, type, damage, 0f, Main.myPlayer);
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(BuffID.Weak, 90, true);
        }
    }
}
