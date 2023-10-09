using CalamityMod.BiomeManagers;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using CalamityMod.Projectiles.Enemy;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Weapons.Summon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using System;

namespace CalamityMod.NPCs.AcidRain
{
    public class NuclearToad : ModNPC
    {
        public Player Target => Main.player[NPC.target];

        public static float ExplosionStartRadius
        {
            get
            {
                float explodeDistance = DownedBossSystem.downedAquaticScourge ? 360f : 270f;
                if (DownedBossSystem.downedPolterghast)
                    explodeDistance = 560f;
                return explodeDistance;
            }
        }

        public ref float ExplosionTimer => ref NPC.ai[0];

        public const float ExplosionTelegraphTime = 120f;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 5;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0);
            value.Position.Y += 8;
            value.PortraitPositionYOverride = 28f;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
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
            SpawnModBiomes = new int[1] { ModContent.GetInstance<AcidRainBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.NuclearToad")
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
            }

            // Make a frog croak sound from time to time.
            if (Main.rand.NextBool(480))
                SoundEngine.PlaySound(SoundID.Zombie13, NPC.Center);

            // Explode if a player gets too close.
            if (NPC.WithinRange(Target.Center, ExplosionStartRadius) || ExplosionTimer >= 1f)
                ExplosionTimer++;

            if (ExplosionTimer >= ExplosionTelegraphTime)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int damage = Main.expertMode ? DownedBossSystem.downedAquaticScourge ? 21 : 8 : DownedBossSystem.downedAquaticScourge ? 27 : 10;
                    float speed = Main.rand.NextFloat(8f, 12f);
                    if (DownedBossSystem.downedPolterghast)
                    {
                        speed *= 1.8f;
                        damage = Main.expertMode ? 36 : 45;
                    }

                    for (int i = 0; i < 7; i++)
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, -Vector2.UnitY.RotatedByRandom(0.79f) * speed * Main.rand.NextFloat(0.8f, 1f), ModContent.ProjectileType<NuclearToadGoo>(), damage, 1f);
                }
                SoundEngine.PlaySound(SoundID.DD2_KoboldExplosion, NPC.Center);
                NPC.life = 0;
                NPC.HitEffect();
                NPC.active = false;
                NPC.netUpdate = true;
            }
        }

        public override Color? GetAlpha(Color drawColor)
        {
            float explosionInterpolant = ExplosionTimer / ExplosionTelegraphTime;
            float fadeToRed = (float)Math.Abs(Math.Sin(MathHelper.Pi * Math.Pow(explosionInterpolant, 3D) * 6f));
            Color c = Color.Lerp(drawColor, new(232, 40, 12, 0), fadeToRed * 0.75f);
            return c * NPC.Opacity;
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

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 8; k++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.SulfurousSeaAcid, hit.HitDirection, -1f, 0, default, 1f);

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
            npcLoot.Add(ModContent.ItemType<SulphuricScale>(), 2, 1, 3);
            LeadingConditionRule postAS = npcLoot.DefineConditionalDropSet(() => DownedBossSystem.downedAquaticScourge);
            postAS.Add(ModContent.ItemType<CausticCroakerStaff>(), 100, hideLootReport: !DownedBossSystem.downedAquaticScourge);
            postAS.AddFail(ModContent.ItemType<CausticCroakerStaff>(), 20, hideLootReport: DownedBossSystem.downedAquaticScourge);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(ModContent.BuffType<Irradiated>(), 120);
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) => CalamityGlobalNPC.DrawGlowmask(NPC, spriteBatch, ModContent.Request<Texture2D>(Texture + "Glow").Value, true);
    }
}
