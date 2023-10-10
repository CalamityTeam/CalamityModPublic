using CalamityMod.BiomeManagers;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using CalamityMod.Projectiles.Enemy;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.SulphurousSea
{
    public class BelchingCoral : ModNPC
    {
        public const float CheckDistance = 480f;
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0);
            value.Position.Y += 4;
            value.PortraitPositionYOverride = 24f;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
        }

        public override void SetDefaults()
        {
            NPC.noGravity = true;
            NPC.damage = 45;
            NPC.width = 54;
            NPC.height = 42;
            NPC.defense = 25;
            NPC.lifeMax = 1000;
            NPC.aiStyle = AIType = -1;
            NPC.value = Item.buyPrice(0, 0, 3, 50);
            NPC.HitSound = SoundID.NPCHit42;
            NPC.DeathSound = SoundID.NPCDeath5;
            NPC.knockBackResist = 0f;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<BelchingCoralBanner>();
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToElectricity = true;
            NPC.Calamity().VulnerableToWater = false;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<SulphurousSeaBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
				new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.BelchingCoral")
            });
        }

        public override void AI()
        {
            NPC.velocity.Y += 0.25f;
            NPC.TargetClosest(false);
            Player player = Main.player[NPC.target];
            if (Math.Abs(player.Center.X - NPC.Center.X) < CheckDistance && player.Bottom.Y < NPC.Top.Y)
            {
                if (NPC.ai[0]++ % 35f == 34f && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 velocity = new Vector2(Main.rand.NextFloat(-6f, 6f), Main.rand.NextFloat(-11f, -6f));
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Top + new Vector2(0f, 6f), velocity, ModContent.ProjectileType<BelchingCoralSpike>(), 27, 3f);
                }
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.PlayerSafe || !spawnInfo.Player.Calamity().ZoneSulphur || !DownedBossSystem.downedAquaticScourge)
            {
                return 0f;
            }
            return 0.085f;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            // Rarer to encourage fighting Acid Rain to obtain the fossils
            npcLoot.Add(ModContent.ItemType<CorrodedFossil>(), 15);
            npcLoot.Add(ModContent.ItemType<BelchingSaxophone>(), 10);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(ModContent.BuffType<Irradiated>(), 180);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 10; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.SulfurousSeaAcid, hit.HitDirection, -1f, 0, default, 1f);
                }
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("BelchingCoralGore").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("BelchingCoralGore2").Type, NPC.scale);
                }
            }
        }
    }
}
