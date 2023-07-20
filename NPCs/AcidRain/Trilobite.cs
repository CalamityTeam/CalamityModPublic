using System;
using CalamityMod.BiomeManagers;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Projectiles.Enemy;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.AcidRain
{
    public class Trilobite : ModNPC
    {
        public Player Target => Main.player[NPC.target];
        public ref float SpikeShootCountdown => ref NPC.ai[0];
        public const float MinSpeedLungePrompt = 0.5f;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 8;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
            NPCID.Sets.TrailCacheLength[NPC.type] = 5;
        }

        public override void SetDefaults()
        {
            NPC.width = 36;
            NPC.height = 38;
            NPC.aiStyle = AIType = -1;

            NPC.damage = 45;
            NPC.lifeMax = 300;
            NPC.defense = 15;
            NPC.DR_NERD(0.25f);

            if (DownedBossSystem.downedPolterghast)
            {
                NPC.damage = 80;
                NPC.lifeMax = 4200;
                NPC.defense = 30;
            }

            NPC.knockBackResist = 0.2f;
            NPC.value = Item.buyPrice(0, 0, 4, 0);
            NPC.lavaImmune = false;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit42;
            NPC.DeathSound = SoundID.NPCDeath27;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<TrilobiteBanner>();
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
				new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.Trilobite")
            });
        }

        public override void AI()
        {
            NPC.TargetClosest(false);
            if (!NPC.wet)
            {
                GetStuckOnLand();
                return;
            }

            // If this NPC doesn't have enough momentum, lunge towards its target.
            if (NPC.velocity.Length() < MinSpeedLungePrompt)
            {
                NPC.TargetClosest();
                float lungeSpeed = DownedBossSystem.downedPolterghast ? 18.5f : 15f;

                NPC.velocity = NPC.SafeDirectionTo(Target.Center, -Vector2.UnitY) * lungeSpeed;
                NPC.velocity.X *= 1.6f;
                NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
                NPC.netUpdate = true;
                return;
            }

            // Accelerate quickly through the water horizontally.
            if (Math.Abs(NPC.velocity.X) < 20f)
                NPC.velocity.X += NPC.direction * 0.02f;

            NPC.rotation = NPC.velocity.X * 0.4f;

            // Slow down in terms of horizontal speed if there is little vertical speed.
            if (Math.Abs(NPC.velocity.Y) < 0.9f)
                NPC.velocity.X *= 0.96f;

            // Lunge towards the target if there is little horizontal speed but also a little bit of vertical speed.
            else if (Main.netMode != NetmodeID.MultiplayerClient && Math.Abs(NPC.velocity.X) < 3.5f)
            {
                float speedX = 18f;
                float speedY = 9f;
                if (DownedBossSystem.downedPolterghast)
                {
                    speedX = 22f;
                    speedY = 11f;
                }
                NPC.velocity = NPC.SafeDirectionTo(Target.Center, -Vector2.UnitY) * new Vector2(speedX, speedY);
                NPC.netUpdate = true;
            }
            NPC.velocity.Y *= 0.98f;
        }

        public void GetStuckOnLand()
        {
            if (SpikeShootCountdown > 0)
                SpikeShootCountdown--;
            NPC.rotation += NPC.velocity.X * 0.1f;

            // If not moving vertically, slow down hotizontally.
            if (NPC.velocity.Y == 0f)
            {
                NPC.velocity.X *= 0.99f;
                if (Math.Abs(NPC.velocity.X) < 0.01f)
                    NPC.velocity.X = 0f;
            }

            // Fall and cap the vertical speed.
            if (NPC.velocity.Y > 13f)
            {
                NPC.velocity.Y = 13f;
                NPC.netUpdate = true;
            }
            else
                NPC.velocity.Y += 0.3f;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            LeadingConditionRule postPolter = npcLoot.DefineConditionalDropSet(() => DownedBossSystem.downedPolterghast);
            postPolter.Add(ModContent.ItemType<CorrodedFossil>(), 15, 1, 3, !DownedBossSystem.downedPolterghast);
            postPolter.AddFail(ModContent.ItemType<CorrodedFossil>(), 3, 1, 3, DownedBossSystem.downedPolterghast);
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.velocity.Length() > 0.5f)
                CalamityGlobalNPC.DrawAfterimage(NPC, spriteBatch, drawColor, Color.Transparent, directioning: true);
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            // No need for a netmode check- this code is only called by 1 client, the owner of the projectile that hit the NPC.
            if (SpikeShootCountdown <= 0f)
            {
                SoundEngine.PlaySound(SoundID.NPCDeath11, NPC.Center);
                int projDamage = DownedBossSystem.downedPolterghast ? 35 : DownedBossSystem.downedAquaticScourge ? 29 : 21;
                if (Main.expertMode)
                    projDamage = (int)Math.Round(projDamage * 0.8);

                Vector2 spikeVelocity = -NPC.velocity.RotatedByRandom(0.18f);
                if (Main.zenithWorld) // more true to the original concept art.
                {
                    spikeVelocity = -projectile.velocity;
                    spikeVelocity.Normalize();
                    spikeVelocity *= DownedBossSystem.downedPolterghast ? 8 : 5;
                }
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + Main.rand.NextVector2Unit() * NPC.Size * 0.7f, spikeVelocity, ModContent.ProjectileType<TrilobiteSpike>(), projDamage, 3f);
                SpikeShootCountdown = Main.rand.Next(50, 65);
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

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.SulfurousSeaAcid, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("TrilobiteGore").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("TrilobiteGore2").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("TrilobiteGore3").Type, NPC.scale);
                }
                for (int k = 0; k < 30; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.SulfurousSeaAcid, hit.HitDirection, -1f, 0, default, 1f);
                }
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(ModContent.BuffType<Irradiated>(), 180);
        }
    }
}
