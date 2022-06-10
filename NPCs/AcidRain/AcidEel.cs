using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Terraria.Audio;

namespace CalamityMod.NPCs.AcidRain
{
    public class AcidEel : ModNPC
    {
        public Player Target => Main.player[NPC.target];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Acid Eel");
            Main.npcFrameCount[NPC.type] = 6;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
            NPCID.Sets.TrailCacheLength[NPC.type] = 7;
        }

        public override void SetDefaults()
        {
            NPC.width = 72;
            NPC.height = 18;

            NPC.damage = 20;
            NPC.lifeMax = 80;
            NPC.defense = 4;
            NPC.knockBackResist = 0.9f;

            if (DownedBossSystem.downedPolterghast)
            {
                NPC.DR_NERD(0.05f);
                NPC.damage = 100;
                NPC.lifeMax = 3300;
                NPC.defense = 20;
                NPC.knockBackResist = 0.7f;
            }
            else if (DownedBossSystem.downedAquaticScourge)
            {
                NPC.damage = 50;
                NPC.lifeMax = 240;
            }

            NPC.value = Item.buyPrice(0, 0, 3, 32);
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.lavaImmune = false;
            NPC.noGravity = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<AcidEelBanner>();
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
				new FlavorTextBestiaryInfoElement("Along its spine runs an undulating dorsal fin which they can put to great use for their streamlined form, as they rush at prey underwater.")
            });
        }

        public override void AI()
        {
            NPC.TargetClosest(false);

            // Fall through platforms.
            NPC.Calamity().ShouldFallThroughPlatforms = true;

            // Play a slither sound from time to time.
            if (Main.rand.NextBool(480))
                SoundEngine.PlaySound(SoundID.Zombie32, NPC.Center);

            if (NPC.wet)
            {
                SwimTowardsTarget();
                return;
            }

            // Do nothing on land.
            NPC.rotation = NPC.rotation.AngleLerp(0f, 0.1f);
            NPC.velocity.X *= 0.95f;
            if (NPC.velocity.Y < 14f)
                NPC.velocity.Y += 0.15f;
        }

        public void SwimTowardsTarget()
        {
            float swimSpeed = 12f;
            if (DownedBossSystem.downedAquaticScourge)
                swimSpeed += 3f;
            if (DownedBossSystem.downedPolterghast)
                swimSpeed += 4f;

            // Swim upwards if sufficiently under water.
            bool waterAbove = false;
            for (int dy = -160; dy < 0; dy += 8)
            {
                if (Collision.WetCollision(NPC.position + Vector2.UnitY * dy, NPC.width, 16))
                {
                    waterAbove = true;
                    break;
                }
            }

            if (waterAbove)
                NPC.velocity.Y = MathHelper.Clamp(NPC.velocity.Y - 0.25f, -14f, 14f);
            else
                NPC.velocity.Y = MathHelper.Clamp(NPC.velocity.Y + 0.4f, -4f, 8f);

            // Coast in a single direction until hitting something.
            if (NPC.direction == 0)
            {
                NPC.direction = Main.rand.NextBool().ToDirectionInt();
                NPC.netUpdate = true;
            }

            // Rebound on impending collision or when near the world edge.
            bool nearWorldEdge = NPC.Center.X < (Main.offLimitBorderTiles + 2f) * 16f || NPC.Center.X > (Main.maxTilesX - Main.offLimitBorderTiles - 2f) * 16f;
            if ((CalamityUtils.DistanceToTileCollisionHit(NPC.Center, Vector2.UnitX * NPC.direction, 20) ?? 20f) < 5f || nearWorldEdge)
            {
                NPC.direction *= -1;
                if (nearWorldEdge)
                    NPC.position.X += Math.Sign(Main.maxTilesX * 8f - NPC.position.X) * 12f;

                NPC.netUpdate = true;
            }

            NPC.velocity.X = (NPC.velocity.X * 24f + NPC.direction * swimSpeed) / 25f;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ModContent.ItemType<SulfuricScale>(), 2, 1, 3);
            npcLoot.AddIf(() => DownedBossSystem.downedAquaticScourge, ModContent.ItemType<SlitheringEels>(), 20);
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter >= 5)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y >= Main.npcFrameCount[NPC.type] * frameHeight)
                    NPC.frame.Y = 0;
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            CalamityGlobalNPC.DrawGlowmask(NPC, spriteBatch, ModContent.Request<Texture2D>(Texture + "Glow").Value);
            if (NPC.velocity.Length() > 1.5f)
                CalamityGlobalNPC.DrawAfterimage(NPC, spriteBatch, drawColor, Color.Transparent, directioning: true);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 8; k++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.SulfurousSeaAcid, hitDirection, -1f, 0, default, 1f);
            if (NPC.life <= 0)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("AcidEelGore").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("AcidEelGore2").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("AcidEelGore3").Type, NPC.scale);
                }
                for (int k = 0; k < 20; k++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.SulfurousSeaAcid, hitDirection, -1f, 0, default, 1f);
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit) => target.AddBuff(ModContent.BuffType<Irradiated>(), 120);
    }
}
