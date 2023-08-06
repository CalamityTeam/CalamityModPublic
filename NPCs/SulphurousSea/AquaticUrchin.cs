using CalamityMod.BiomeManagers;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Weapons.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace CalamityMod.NPCs.SulphurousSea
{
    public class AquaticUrchin : ModNPC
    {
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0);
            value.Position.Y += 12;
            value.PortraitPositionYOverride = 32f;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.damage = Main.hardMode ? 50 : 25;
            NPC.width = 20;
            NPC.height = 20;
            NPC.defense = 10;
            NPC.lifeMax = Main.hardMode ? 300 : 50;
            NPC.knockBackResist = 0.8f;
            NPC.value = Item.buyPrice(0, 0, 0, 80);
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath15;
            NPC.behindTiles = true;
            NPC.npcSlots = 0.3333f;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<AquaticUrchinBanner>();
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.waterMovementSpeed = 1f;
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToElectricity = true;
            NPC.Calamity().VulnerableToWater = false;
            SpawnModBiomes = new int[2] { ModContent.GetInstance<SulphurousSeaBiome>().Type, ModContent.GetInstance<AbyssLayer1Biome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.AquaticUrchin")
            });
        }

        public static void DoUrchinAI(NPC npc)
        {
            // Simply stick to tiles.
            if (Main.netMode != NetmodeID.MultiplayerClient && npc.localAI[0] == 0f)
            {
                GenSearch search = Main.rand.NextBool() ? new Searches.Down(300) : new Searches.Up(300);
                if (WorldUtils.Find(npc.Center.ToTileCoordinates(), Searches.Chain(search, new Conditions.IsSolid()), out Point result))
                {
                    npc.Center = result.ToWorldCoordinates();
                    npc.netUpdate = true;
                }
                npc.localAI[0] = 1f;
            }

            // Drift slowly over tiles.
            float speedFactor = 0.018f;

            npc.collideX = MathHelper.Distance(npc.position.X, npc.oldPosition.X) < 0.01f;
            npc.collideY = MathHelper.Distance(npc.position.Y, npc.oldPosition.Y) < 0.01f;
            npc.noGravity = Collision.SolidCollision(npc.TopLeft - Vector2.One * 12, npc.width + 6, npc.height + 6);
            if (npc.ai[1] == 0f)
            {
                if (npc.direction == 0)
                    npc.direction = 1;

                npc.rotation += npc.direction * npc.directionY * speedFactor * 0.006f;
                if (npc.collideY)
                    npc.ai[0] = 2f;

                if (!npc.collideY && npc.ai[0] == 2f)
                {
                    npc.direction = -npc.direction;
                    npc.ai[1] = 1f;
                    npc.ai[0] = 1f;
                }
                if (npc.collideX)
                {
                    npc.directionY = -npc.directionY;
                    npc.ai[1] = 1f;
                }
            }
            else
            {
                npc.rotation -= npc.direction * npc.directionY * 0.006f;
                if (npc.collideX)
                    npc.ai[0] = 2f;

                if (!npc.collideX && npc.ai[0] == 2f)
                {
                    npc.directionY = -npc.directionY;
                    npc.ai[1] = 0f;
                    npc.ai[0] = 1f;
                }
                if (npc.collideY)
                {
                    npc.direction = -npc.direction;
                    npc.ai[1] = 0f;
                }
            }
            npc.velocity.X = npc.direction * speedFactor;

            if (npc.noGravity)
                npc.velocity.Y = npc.directionY * speedFactor;

            // Use blazing wheel movement.
            Vector2 position = npc.Center - Vector2.One * 6f;
            npc.velocity = Collision.noSlopeCollision(position, npc.velocity, 12, 12, true, true);
        }

        public override void AI() => DoUrchinAI(NPC);

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(ModContent.BuffType<Irradiated>(), 120, true);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.PlayerSafe)
                return 0f;

            if ((spawnInfo.Player.Calamity().ZoneSulphur || spawnInfo.Player.Calamity().ZoneAbyssLayer1) && spawnInfo.Water && NPC.CountNPCS(ModContent.NPCType<AquaticUrchin>()) < 12)
                return 2.2f;

            return 0f;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ModContent.ItemType<UrchinStinger>(), 1, 30, 50);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);
                }
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("AquaticUrchin").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("AquaticUrchin2").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("AquaticUrchin3").Type, 1f);
                }
            }
        }
    }
}
