using System;
using CalamityMod.CalPlayer;
using CalamityMod.Events;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class KingSlimeJewelSapphire : ModNPC
    {
        public override string Texture => "CalamityMod/NPCs/NormalNPCs/KingSlimeJewel";

        private const int BuffDustGateValue = 60;
        private const float LightTelegraphDuration = 45f;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers bestiaryData = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, bestiaryData);
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.damage = 10;
            NPC.width = 22;
            NPC.height = 22;
            NPC.defense = 5;
            NPC.DR_NERD(0.05f);

            NPC.lifeMax = 120;
            double HPBoost = CalamityServerConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);

            NPC.knockBackResist = 0.9f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit5;
            NPC.DeathSound = SoundID.NPCDeath15;
            NPC.Calamity().VulnerableToSickness = false;
        }

        public override void AI()
        {
            // Setting this in SetDefaults will disable expert mode scaling, so put it here instead
            NPC.damage = 0;

            // Despawn
            if (!CalamityPlayer.areThereAnyDamnBosses)
            {
                NPC.life = 0;
                NPC.HitEffect();
                NPC.active = false;
                NPC.netUpdate = true;
                return;
            }

            Lighting.AddLight(NPC.Center, 0f, 0f, 0.8f);

            // Float around the player
            NPC.rotation = NPC.velocity.X / 15f;

            NPC.TargetClosest();

            float velocity = 5f;
            float acceleration = 0.1f;

            int distanceFromKingSlime = 1;
            Vector2 kingSlimeCenter = NPC.Center;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == NPCID.KingSlime)
                {
                    distanceFromKingSlime = (int)NPC.Distance(Main.npc[i].Center);
                    kingSlimeCenter = Main.npc[i].Center;
                    break;
                }
            }

            Vector2 movementTarget = kingSlimeCenter == NPC.Center ? Main.player[NPC.target].Center : kingSlimeCenter;
            if (NPC.position.Y > movementTarget.Y - 200f)
            {
                if (NPC.velocity.Y > 0f)
                    NPC.velocity.Y *= 0.98f;

                NPC.velocity.Y -= acceleration;

                if (NPC.velocity.Y > velocity)
                    NPC.velocity.Y = velocity;
            }
            else if (NPC.position.Y < movementTarget.Y - 250f)
            {
                if (NPC.velocity.Y < 0f)
                    NPC.velocity.Y *= 0.98f;

                NPC.velocity.Y += acceleration;

                if (NPC.velocity.Y < -velocity)
                    NPC.velocity.Y = -velocity;
            }

            if (NPC.Center.X > movementTarget.X + 100f)
            {
                if (NPC.velocity.X > 0f)
                    NPC.velocity.X *= 0.98f;

                NPC.velocity.X -= acceleration;

                if (NPC.velocity.X > 8f)
                    NPC.velocity.X = 8f;
            }
            if (NPC.Center.X < movementTarget.X - 100f)
            {
                if (NPC.velocity.X < 0f)
                    NPC.velocity.X *= 0.98f;

                NPC.velocity.X += acceleration;

                if (NPC.velocity.X < -8f)
                    NPC.velocity.X = -8f;
            }

            // Emit buff dust
            NPC.ai[0] += 1f;
            if (NPC.ai[0] >= BuffDustGateValue)
            {
                NPC.ai[0] = 0f;

                SoundEngine.PlaySound(SoundID.Item8, NPC.Center);

                for (int dusty = 0; dusty < 10; dusty++)
                {
                    Vector2 dustVel = (kingSlimeCenter - NPC.Center).SafeNormalize(Vector2.UnitY) * Main.rand.NextFloat(2f, 4f);
                    int sapphire = Dust.NewDust(NPC.Center, NPC.width, NPC.height, DustID.GemSapphire, 0f, 0f, 100, default, 2f);
                    Main.dust[sapphire].velocity = dustVel * Main.rand.NextFloat(1f, 2f);
                    Main.dust[sapphire].noGravity = true;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[sapphire].scale = 0.5f;
                        Main.dust[sapphire].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }

                int maxDustIterations = distanceFromKingSlime;
                int maxDust = 100;
                int dustDivisor = maxDustIterations / maxDust;
                if (dustDivisor < 2)
                    dustDivisor = 2;

                Vector2 dustLineStart = NPC.Center;
                Vector2 dustLineEnd = kingSlimeCenter;
                Vector2 currentDustPos = default;
                Vector2 spinningpoint = new Vector2(0f, -1f).RotatedByRandom(MathHelper.Pi);
                int dustSpawned = 0;
                for (int i = 0; i < maxDustIterations; i++)
                {
                    if (i % dustDivisor == 0)
                    {
                        currentDustPos = Vector2.Lerp(dustLineStart, dustLineEnd, i / (float)maxDustIterations);
                        int dust = Dust.NewDust(currentDustPos, 0, 0, DustID.GemSapphire, 0f, 0f, 100, default, 1f);
                        Main.dust[dust].position = currentDustPos;
                        Main.dust[dust].velocity = spinningpoint.RotatedBy(MathHelper.TwoPi * i / maxDustIterations) * (0.9f + Main.rand.NextFloat() * 0.2f);
                        Main.dust[dust].noGravity = true;
                        if (Main.rand.NextBool())
                        {
                            Main.dust[dust].scale = 0.5f;
                            Main.dust[dust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                        }

                        Dust dust2 = Dust.CloneDust(dust);
                        Dust dust3 = dust2;
                        dust3.scale *= 0.5f;
                        dust3 = dust2;
                        dust3.fadeIn *= 0.5f;
                        dustSpawned++;
                    }
                }

                NPC.netUpdate = true;
            }
        }

        public override Color? GetAlpha(Color drawColor)
        {
            Color initialColor = new Color(0, 0, 150);
            Color newColor = initialColor;
            Color finalColor = new Color(125, 125, 255);
            float colorTelegraphGateValue = BuffDustGateValue - LightTelegraphDuration;
            if (NPC.ai[0] > colorTelegraphGateValue)
                newColor = Color.Lerp(initialColor, finalColor, (NPC.ai[0] - colorTelegraphGateValue) / LightTelegraphDuration);
            newColor.A = (byte)(255 * NPC.Opacity);

            return newColor;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * balance);
        }

        public override bool CheckActive() => false;

        public override void HitEffect(NPC.HitInfo hit)
        {
            int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GemSapphire, hit.HitDirection, -1f, 0, default, 1f);
            Main.dust[dust].noGravity = true;

            if (NPC.life <= 0)
            {
                NPC.position = NPC.Center;
                NPC.width = NPC.height = 45;
                NPC.position.X = NPC.position.X - (NPC.width / 2);
                NPC.position.Y = NPC.position.Y - (NPC.height / 2);

                for (int i = 0; i < 2; i++)
                {
                    int rubyDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GemSapphire, 0f, 0f, 100, default, 2f);
                    Main.dust[rubyDust].noGravity = true;
                    Main.dust[rubyDust].velocity *= 3f;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[rubyDust].scale = 0.5f;
                        Main.dust[rubyDust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }

                for (int j = 0; j < 10; j++)
                {
                    int rubyDust2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GemSapphire, 0f, 0f, 100, default, 3f);
                    Main.dust[rubyDust2].noGravity = true;
                    Main.dust[rubyDust2].velocity *= 5f;
                    rubyDust2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GemSapphire, 0f, 0f, 100, default, 2f);
                    Main.dust[rubyDust2].noGravity = true;
                    Main.dust[rubyDust2].velocity *= 2f;
                }
            }
        }
    }
}
