using CalamityMod.BiomeManagers;
using CalamityMod.Dusts;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Projectiles.Enemy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.NPCs.AcidRain
{
    public class GammaSlime : ModNPC
    {
        public float DustAngleMultiplier1;
        public float DustAngleMultiplier2;
        public Player Target => Main.player[NPC.target];
        public float LaserTelegraphPower => Utils.GetLerpValue(540f, 480f, LaserShootCountdown, true);
        public float LaserTelegraphLength => MathHelper.Lerp(20f, 550f, LaserTelegraphPower);
        public float LaserTelegraphOpacity => MathHelper.Lerp(0.3f, 0.9f, LaserTelegraphPower);
        public ref float GammaAcidShootTimer => ref NPC.ai[0];
        public ref float LaserShootCountdown => ref NPC.ai[1];

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 2;
        }

        public override void SetDefaults()
        {
            NPC.width = 40;
            NPC.height = 44;

            NPC.damage = 110;
            NPC.lifeMax = 5000;
            NPC.DR_NERD(0.15f);
            NPC.defense = 25;

            NPC.aiStyle = AIType = -1;

            NPC.knockBackResist = 0f;
            AnimationType = NPCID.CorruptSlime;
            NPC.value = Item.buyPrice(0, 0, 8, 30);
            NPC.alpha = 50;
            NPC.lavaImmune = false;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<GammaSlimeBanner>();
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
				new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.GammaSlime")
            });
        }

        public override void AI()
        {
            // Release green light at the position of the slime.
            Lighting.AddLight((int)NPC.Center.X / 16, (int)NPC.Center.Y / 16, 0.6f, 0.8f, 0.6f);

            NPC.TargetClosest(false);

            if (NPC.velocity.Y == 0f && LaserShootCountdown <= 0f && !Target.npcTypeNoAggro[NPC.type])
            {
                NPC.velocity.X *= 0.8f;

                GammaAcidShootTimer++;

                // Jump into the air and release gamma acid upward.
                if (GammaAcidShootTimer % 30f == 29f)
                {
                    NPC.velocity.Y -= MathHelper.Clamp(Math.Abs(Target.Center.Y - NPC.Center.Y) / 16f, 5f, 15f);
                    NPC.velocity.X = NPC.SafeDirectionTo(Target.Center).X * 16f;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            float angle = MathHelper.TwoPi / 5f * i;
                            if (GammaAcidShootTimer % 60f == 58f)
                                angle += MathHelper.PiOver2;
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, angle.ToRotationVector2() * 7f, ModContent.ProjectileType<GammaAcid>(), Main.expertMode ? 36 : 45, 3f);
                        }
                    }
                    NPC.netUpdate = true;
                }
            }
            else
                NPC.velocity.X *= 0.9935f;

            if (LaserShootCountdown > 0f)
                LaserShootCountdown--;

            // Stop moving for a bit after the laser.
            if (LaserShootCountdown > 240f)
            {
                NPC.velocity.X *= 0.95f;
                NPC.velocity.Y += 0.2f;
            }

            // Hold energy for laser
            if (LaserShootCountdown > 480f)
            {
                float scale = LaserShootCountdown < 530f ? 2.25f : 1.65f;
                Vector2 destination = NPC.Top + new Vector2(0f, 6f);
                Dust dust = Dust.NewDustPerfect(destination + Main.rand.NextVector2CircularEdge(12f, 12f), (int)CalamityDusts.SulfurousSeaAcid);
                dust.velocity = Vector2.Normalize(destination - dust.position) * 3f;
                dust.scale = scale;
                dust.noGravity = true;

                if (LaserShootCountdown <= 540f)
                {
                    for (float i = NPC.Top.Y + 4f; i >= NPC.Top.Y + 4f - LaserTelegraphLength; i -= 8f)
                    {
                        float angle = i / 24f;
                        Vector2 spawnPosition = new Vector2(NPC.Center.X, i);
                        dust = Dust.NewDustPerfect(spawnPosition, (int)CalamityDusts.SulfurousSeaAcid);
                        dust.scale = 1.5f;
                        dust.velocity = Vector2.UnitX * (float)Math.Cos(angle) * (1f - LaserTelegraphPower) * 4f;
                        dust.noGravity = true;
                    }
                }
            }

            // Release a gamma laser upward.
            if (LaserShootCountdown == 480f)
            {
                DustAngleMultiplier1 = Main.rand.NextFloat(3f);
                DustAngleMultiplier2 = Main.rand.NextFloat(4f);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    SoundEngine.PlaySound(SoundID.Zombie104, NPC.Center);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, -Vector2.UnitY, ModContent.ProjectileType<GammaBeam>(), Main.expertMode ? 96 : 120, 4f, Main.myPlayer, 0f, NPC.whoAmI);
                }
                NPC.netUpdate = true;
            }

            // Very complex particle effects while releasing the beam
            else if (LaserShootCountdown >= 300f)
            {
                float angle = LaserShootCountdown / 30f % MathHelper.TwoPi;
                float horizontalSpeed = (float)Math.Sin(angle * DustAngleMultiplier1) * (float)Math.Cos(angle) * 4.5f;
                float verticalSpeed = (float)Math.Cos(angle * DustAngleMultiplier1) * (float)Math.Sin(angle) * 2f;
                Vector2 velocity = new Vector2(horizontalSpeed, verticalSpeed);
                Dust dust = Dust.NewDustPerfect(NPC.Center + angle.ToRotationVector2() * 8f, (int)CalamityDusts.SulfurousSeaAcid);
                dust.velocity = velocity;
                dust.scale = (float)Math.Cos(angle) + 2f;
                dust.noGravity = true;

                dust = Dust.NewDustPerfect(NPC.Center + angle.ToRotationVector2() * 8f, (int)CalamityDusts.SulfurousSeaAcid);
                dust.velocity = -velocity;
                dust.scale = (float)Math.Cos(angle) + 2f;
                dust.noGravity = true;
            }

            // Randomly prepare to shoot a laser.
            if (Main.netMode != NetmodeID.MultiplayerClient && Math.Abs(Target.Center.X - NPC.Center.X) < 250f && Target.Center.X < NPC.Center.X &&
                LaserShootCountdown == 0f && Main.rand.NextBool(110) && !Target.npcTypeNoAggro[NPC.type])
            {
                LaserShootCountdown = 600f;
                NPC.netUpdate = true;
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 10; k++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.SulfurousSeaAcid, hit.HitDirection, -1f, 0, default, 1f);
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.SulfurousSeaAcid, hit.HitDirection, -1f, 0, default, 1f);
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("GammaSlimeGore").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("GammaSlimeGore2").Type, NPC.scale);
                }
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (LaserShootCountdown >= 480f && LaserShootCountdown <= 540f)
            {
                Vector2 laserBottom = NPC.Top + Vector2.UnitY * 4f;
                Vector2 laserTop = laserBottom - Vector2.UnitY * LaserTelegraphLength;
                Utils.DrawLine(spriteBatch, laserBottom, laserTop, Color.Lerp(Color.Lime, Color.Transparent, LaserTelegraphOpacity));
            }
            CalamityGlobalNPC.DrawGlowmask(NPC, spriteBatch, ModContent.Request<Texture2D>(Texture + "Glow").Value);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(ModContent.BuffType<Irradiated>(), 180);
        }
    }
}
