using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using CalamityMod.Projectiles.Enemy;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Sounds;

namespace CalamityMod.NPCs.AcidRain
{
    public class NuclearToad : ModNPC
    {
        public Player Target => Main.player[NPC.target];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nuclear Toad");
            Main.npcFrameCount[NPC.type] = 5;
        }

        public override void SetDefaults()
        {
            NPC.width = 62;
            NPC.height = 34;
            NPC.defense = 4;

            NPC.aiStyle = AIType = -1;

            NPC.damage = 15;
            NPC.lifeMax = 60;
            NPC.defense = 3;

            if (DownedBossSystem.downedPolterghast)
            {
                NPC.damage = 80;
                NPC.lifeMax = 2750;
                NPC.defense = 15;
            }
            else if (DownedBossSystem.downedAquaticScourge)
            {
                NPC.damage = 35;
                NPC.lifeMax = 200;
            }

            NPC.knockBackResist = 0.7f;
            NPC.value = Item.buyPrice(0, 0, 5, 0);
            NPC.lavaImmune = false;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<NuclearToadBanner>();
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToElectricity = true;
            NPC.Calamity().VulnerableToWater = false;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				//BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.AcidRainTier1,
				//BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.AcidRainTier2,
                //BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.AcidRainTier3,

				// Will move to localization whenever that is cleaned up.
				new FlavorTextBestiaryInfoElement("When threatened underwater they rapidly inflate and bob to the surface. As a last resort, they forcefully burst themselves. Their selfless goal is the prolonged survival of their kin.")
            });
        }

        public override void AI()
        {
            NPC.TargetClosest(false);

            // Slow down over time on the X axis (to prevent endless sliding as a result of KB).
            NPC.velocity.X *= 0.96f;

            // Bob on the top of the water.
            if (NPC.wet)
            {
                if (NPC.velocity.Y > 2f)
                    NPC.velocity.Y *= 0.9f;
                NPC.velocity.Y -= 0.16f;
                if (NPC.velocity.Y < -4f)
                    NPC.velocity.Y = -4f;
            }
            else
            {
                if (NPC.velocity.Y < -2f)
                    NPC.velocity.Y *= 0.9f;
                NPC.velocity.Y += 0.16f;
                if (NPC.velocity.Y > 3f)
                    NPC.velocity.Y = 3f;
                NPC.ai[0] = 5f;
            }

            // Make a frog croak sound from time to time.
            if (Main.rand.NextBool(480))
                SoundEngine.PlaySound(CommonCalamitySounds.GetZombieSound(13), NPC.Center);

            float explodeDistance = DownedBossSystem.downedAquaticScourge ? 295f : 195f;
            if (DownedBossSystem.downedPolterghast)
                explodeDistance = 470f;

            // Explode if a player gets too close.
            if (NPC.WithinRange(Target.Center, explodeDistance))
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int damage = Main.expertMode ? DownedBossSystem.downedAquaticScourge ? 21 : 8 : DownedBossSystem.downedAquaticScourge ? 27 : 10;
                    float speed = Main.rand.NextFloat(6f, 9f);
                    if (DownedBossSystem.downedPolterghast)
                    {
                        speed *= 1.8f;
                        damage = Main.expertMode ? 36 : 45;
                    }

                    for (int i = 0; i < 7; i++)
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Main.rand.NextVector2CircularEdge(speed, speed), ModContent.ProjectileType<NuclearToadGoo>(), damage, 1f);
                }
                SoundEngine.PlaySound(SoundID.DD2_KoboldExplosion, NPC.Center);
                NPC.life = 0;
                NPC.HitEffect();
                NPC.active = false;
                NPC.netUpdate = true;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter >= 4)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y >= Main.npcFrameCount[NPC.type] * frameHeight)
                    NPC.frame.Y = 0;
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 8; k++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.SulfurousSeaAcid, hitDirection, -1f, 0, default, 1f);

            if (NPC.life <= 0)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("NuclearToadGore1").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("NuclearToadGore2").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("NuclearToadGore3").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("NuclearToadGore4").Type, NPC.scale);
                }
                for (int i = 0; i < 25; i++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.SulfurousSeaAcid, Main.rand.NextFloat(-2f, 2f), -1f, 0, default, 1f);
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ModContent.ItemType<SulfuricScale>(), 2, 1, 3);
            npcLoot.AddIf(() => !DownedBossSystem.downedAquaticScourge, ModContent.ItemType<CausticCroakerStaff>(), 20, 1, 3);
            npcLoot.AddIf(() => DownedBossSystem.downedAquaticScourge, ModContent.ItemType<CausticCroakerStaff>(), 100, 1, 3);
        }

        public override void OnHitPlayer(Player target, int damage, bool crit) => target.AddBuff(ModContent.BuffType<Irradiated>(), 120);

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) => CalamityGlobalNPC.DrawGlowmask(NPC, spriteBatch, ModContent.Request<Texture2D>(Texture + "Glow").Value, true);
    }
}
