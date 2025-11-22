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
    public class KingSlimeJewelEmerald : ModNPC
    {
        public override string Texture => "CalamityMod/NPCs/NormalNPCs/KingSlimeJewel";

        private const int ChargePhaseGateValue = 90;
        private const int ChargeGateValue = 60;
        private const int ChargeGateValue_Death = 40;
        private const float LightTelegraphDuration = 30f;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers bestiaryData = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, bestiaryData);
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.GetNPCDamage();
            NPC.width = 22;
            NPC.height = 22;
            NPC.defense = 15;
            NPC.DR_NERD(0.15f);

            NPC.lifeMax = 240;
            double HPBoost = CalamityServerConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);

            NPC.knockBackResist = 0.7f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit5;
            NPC.DeathSound = SoundID.NPCDeath15;
            NPC.Calamity().VulnerableToSickness = false;
        }

        public override void AI()
        {
            // Despawn
            if (!CalamityPlayer.areThereAnyDamnBosses)
            {
                NPC.life = 0;
                NPC.HitEffect();
                NPC.active = false;
                NPC.netUpdate = true;
                return;
            }

            Lighting.AddLight(NPC.Center, 0f, 0.8f, 0f);

            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead)
                NPC.TargetClosest();

            if (NPC.ai[3] == 1f)
            {
                NPC.knockBackResist = 0f;

                if (NPC.ai[0] == 0f)
                {
                    // Avoid cheap bullshit
                    NPC.damage = 0;

                    NPC.velocity *= CalamityWorld.death ? 0.94f : 0.96f;

                    NPC.ai[1] += 1f;

                    float anglularSpeed = NPC.ai[1] / (CalamityWorld.death ? ChargeGateValue_Death : ChargeGateValue);
                    anglularSpeed = 0.1f + anglularSpeed * 0.4f;
                    NPC.rotation += anglularSpeed * NPC.direction;

                    if (NPC.ai[1] >= (CalamityWorld.death ? ChargeGateValue_Death : ChargeGateValue))
                    {
                        for (int dusty = 0; dusty < 10; dusty++)
                        {
                            Vector2 dustVel = NPC.SafeDirectionTo(Main.player[NPC.target].Center + Main.player[NPC.target].velocity * 20f, -Vector2.UnitY) * Main.rand.NextFloat(-4f, -1f);
                            int emerald = Dust.NewDust(NPC.Center, NPC.width, NPC.height, DustID.GemEmerald, 0f, 0f, 100, default, 2f);
                            Main.dust[emerald].velocity = dustVel * Main.rand.NextFloat(1f, 2f);
                            Main.dust[emerald].noGravity = true;
                            if (Main.rand.NextBool())
                            {
                                Main.dust[emerald].scale = 0.5f;
                                Main.dust[emerald].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                            }
                        }

                        SoundEngine.PlaySound(SoundID.Item38, NPC.Center);

                        NPC.ai[0] = 1f;
                        NPC.ai[1] = 0f;
                        NPC.netUpdate = true;
                    }
                }
                else if (NPC.ai[0] == 1f)
                {
                    // Set damage
                    NPC.damage = NPC.defDamage;

                    float chargeSpeed = CalamityWorld.death ? 24f : 16f;
                    NPC.velocity = NPC.SafeDirectionTo(Main.player[NPC.target].Center + Main.player[NPC.target].velocity * 20f, -Vector2.UnitY) * chargeSpeed;
                    NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;

                    NPC.ai[0] = 2f;
                    NPC.ai[1] = 0f;

                    NPC.netSpam = 0;
                    NPC.netUpdate = true;
                }
                else if (NPC.ai[0] == 2f)
                {
                    // Set damage
                    NPC.damage = NPC.defDamage;

                    NPC.ai[1] += 1f;
                    if (NPC.ai[1] >= (CalamityWorld.death ? ChargeGateValue_Death : ChargeGateValue))
                    {
                        // Avoid cheap bullshit
                        NPC.damage = 0;

                        for (int dusty = 0; dusty < 10; dusty++)
                        {
                            Vector2 dustVel = Main.rand.NextVector2CircularEdge(5f, 5f);
                            int emerald = Dust.NewDust(NPC.Center, NPC.width, NPC.height, DustID.GemEmerald, 0f, 0f, 100, default, 2f);
                            Main.dust[emerald].velocity = dustVel * Main.rand.NextFloat(1f, 2f);
                            Main.dust[emerald].noGravity = true;
                            if (Main.rand.NextBool())
                            {
                                Main.dust[emerald].scale = 0.5f;
                                Main.dust[emerald].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                            }
                        }

                        SoundEngine.PlaySound(SoundID.Item8, NPC.Center);

                        NPC.ai[0] = 0f;
                        NPC.ai[1] = 0f;
                        NPC.ai[3] = 0f;
                        NPC.netUpdate = true;

                        NPC.velocity = Vector2.Zero;
                    }
                }
            }
            else
            {
                // Avoid cheap bullshit
                NPC.damage = 0;

                NPC.knockBackResist = 0.7f;

                NPC.rotation = NPC.velocity.X / 15f;

                float velocity = 5f;
                float acceleration = 0.2f;

                if (NPC.position.Y > Main.player[NPC.target].position.Y - 200f)
                {
                    if (NPC.velocity.Y > 0f)
                        NPC.velocity.Y *= 0.98f;

                    NPC.velocity.Y -= acceleration;

                    if (NPC.velocity.Y > velocity)
                        NPC.velocity.Y = velocity;
                }
                else if (NPC.position.Y < Main.player[NPC.target].position.Y - 300f)
                {
                    if (NPC.velocity.Y < 0f)
                        NPC.velocity.Y *= 0.98f;

                    NPC.velocity.Y += acceleration;

                    if (NPC.velocity.Y < -velocity)
                        NPC.velocity.Y = -velocity;
                }

                if (NPC.Center.X > Main.player[NPC.target].Center.X + 200f)
                {
                    if (NPC.velocity.X > 0f)
                        NPC.velocity.X *= 0.98f;

                    NPC.velocity.X -= acceleration;

                    if (NPC.velocity.X > 8f)
                        NPC.velocity.X = 8f;
                }
                if (NPC.Center.X < Main.player[NPC.target].Center.X - 200f)
                {
                    if (NPC.velocity.X < 0f)
                        NPC.velocity.X *= 0.98f;

                    NPC.velocity.X += acceleration;

                    if (NPC.velocity.X < -8f)
                        NPC.velocity.X = -8f;
                }

                NPC.ai[2] += 1f;
                if (NPC.ai[2] >= ChargePhaseGateValue)
                {
                    NPC.ai[2] = 0f;
                    NPC.ai[3] = 1f;
                    NPC.netUpdate = true;
                }
            }
        }

        public override Color? GetAlpha(Color drawColor)
        {
            Color initialColor = new Color(0, 150, 0);
            Color newColor = initialColor;
            Color finalColor = new Color(125, 255, 125);
            float colorTelegraphGateValue = (CalamityWorld.death ? ChargeGateValue_Death : ChargeGateValue) - LightTelegraphDuration;
            if (NPC.ai[1] > colorTelegraphGateValue)
                newColor = Color.Lerp(initialColor, finalColor, (NPC.ai[1] - colorTelegraphGateValue) / LightTelegraphDuration);
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
            int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GemEmerald, hit.HitDirection, -1f, 0, default, 1f);
            Main.dust[dust].noGravity = true;

            if (NPC.life <= 0)
            {
                NPC.position = NPC.Center;
                NPC.width = NPC.height = 45;
                NPC.position.X = NPC.position.X - (NPC.width / 2);
                NPC.position.Y = NPC.position.Y - (NPC.height / 2);

                for (int i = 0; i < 2; i++)
                {
                    int emeraldDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GemEmerald, 0f, 0f, 100, default, 2f);
                    Main.dust[emeraldDust].noGravity = true;
                    Main.dust[emeraldDust].velocity *= 3f;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[emeraldDust].scale = 0.5f;
                        Main.dust[emeraldDust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }

                for (int j = 0; j < 10; j++)
                {
                    int emeraldDust2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GemEmerald, 0f, 0f, 100, default, 3f);
                    Main.dust[emeraldDust2].noGravity = true;
                    Main.dust[emeraldDust2].velocity *= 5f;
                    emeraldDust2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GemEmerald, 0f, 0f, 100, default, 2f);
                    Main.dust[emeraldDust2].noGravity = true;
                    Main.dust[emeraldDust2].velocity *= 2f;
                }
            }
        }
    }
}
