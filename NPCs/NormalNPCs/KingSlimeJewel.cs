using CalamityMod.Events;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.CalPlayer;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class KingSlimeJewel : ModNPC
    {
        private const int BoltShootGateValue = 75;
        private const int BoltShootGateValue_Death = 60;
        private const int BoltShootGateValue_BossRush = 45;
        private const float LightTelegraphDuration = 45f;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers bestiaryData = new NPCID.Sets.NPCBestiaryDrawModifiers(0) { Hide = true }; 
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, bestiaryData);
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.damage = 10;
            NPC.width = 22;
            NPC.height = 22;
            NPC.defense = 10;
            NPC.DR_NERD(0.1f);
            NPC.lifeMax = 140;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.canGhostHeal = false;
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

            // Float around the player
            NPC.rotation = NPC.velocity.X / 15f;

            NPC.TargetClosest(true);

            float velocity = 2f;
            float acceleration = 0.1f;

            if (NPC.position.Y > Main.player[NPC.target].position.Y - 350f)
            {
                if (NPC.velocity.Y > 0f)
                    NPC.velocity.Y *= 0.98f;

                NPC.velocity.Y -= acceleration;

                if (NPC.velocity.Y > velocity)
                    NPC.velocity.Y = velocity;
            }
            else if (NPC.position.Y < Main.player[NPC.target].position.Y - 400f)
            {
                if (NPC.velocity.Y < 0f)
                    NPC.velocity.Y *= 0.98f;

                NPC.velocity.Y += acceleration;

                if (NPC.velocity.Y < -velocity)
                    NPC.velocity.Y = -velocity;
            }

            if (NPC.Center.X > Main.player[NPC.target].Center.X + 100f)
            {
                if (NPC.velocity.X > 0f)
                    NPC.velocity.X *= 0.98f;

                NPC.velocity.X -= acceleration;

                if (NPC.velocity.X > 8f)
                    NPC.velocity.X = 8f;
            }
            if (NPC.Center.X < Main.player[NPC.target].Center.X - 100f)
            {
                if (NPC.velocity.X < 0f)
                    NPC.velocity.X *= 0.98f;

                NPC.velocity.X += acceleration;

                if (NPC.velocity.X < -8f)
                    NPC.velocity.X = -8f;
            }

            // Fire projectiles
            NPC.ai[0] += 1f;
            if (NPC.ai[0] >= (BossRushEvent.BossRushActive ? BoltShootGateValue_BossRush : CalamityWorld.death ? BoltShootGateValue_Death : BoltShootGateValue))
            {
                NPC.ai[0] = 0f;

                Vector2 npcPos = new Vector2(NPC.Center.X, NPC.Center.Y);
                float xDist = Main.player[NPC.target].Center.X - npcPos.X;
                float yDist = Main.player[NPC.target].Center.Y - npcPos.Y;
                Vector2 projVector = new Vector2(xDist, yDist);
                float projLength = projVector.Length();

                float speed = 10f;
                int type = ModContent.ProjectileType<JewelProjectile>();

                projLength = speed / projLength;
                projVector.X *= projLength;
                projVector.Y *= projLength;
                npcPos.X += projVector.X * 2f;
                npcPos.Y += projVector.Y * 2f;

                for (int dusty = 0; dusty < 10; dusty++)
                {
                    Vector2 dustVel = projVector;
                    dustVel.Normalize();
                    int ruby = Dust.NewDust(NPC.Center, NPC.width, NPC.height, 90, dustVel.X, dustVel.Y, 100, default, 2f);
                    Main.dust[ruby].velocity *= 1.5f;
                    Main.dust[ruby].noGravity = true;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[ruby].scale = 0.5f;
                        Main.dust[ruby].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }

                SoundEngine.PlaySound(SoundID.Item8, NPC.Center);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int damage = NPC.GetProjectileDamage(type);
                    if (CalamityWorld.death || BossRushEvent.BossRushActive)
                    {
                        int numProj = 3;
                        float rotation = MathHelper.ToRadians(9);
                        for (int i = 0; i < numProj; i++)
                        {
                            Vector2 perturbedSpeed = projVector.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(numProj - 1)));
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), npcPos, perturbedSpeed, type, damage, 0f, Main.myPlayer);
                        }
                    }
                    else
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), npcPos, projVector, type, damage, 0f, Main.myPlayer);
                }
            }
        }

        public override Color? GetAlpha(Color drawColor)
        {
            Color initialColor = Color.DarkRed;
            Color newColor = initialColor;
            Color finalColor = Color.White;
            float colorTelegraphGateValue = (BossRushEvent.BossRushActive ? BoltShootGateValue_BossRush : CalamityWorld.death ? BoltShootGateValue_Death : BoltShootGateValue) - LightTelegraphDuration;
            if (NPC.ai[0] > colorTelegraphGateValue)
                newColor = Color.Lerp(initialColor, finalColor, (NPC.ai[0] - colorTelegraphGateValue) / LightTelegraphDuration);
            newColor.A = (byte)(255 * NPC.Opacity);

            return newColor;
        }

        public override bool CheckActive() => false;

        public override void HitEffect(NPC.HitInfo hit)
        {
            Dust.NewDust(NPC.position, NPC.width, NPC.height, 90, hit.HitDirection, -1f, 0, default, 1f);
            if (NPC.life <= 0)
            {
                NPC.position.X = NPC.position.X + (NPC.width / 2);
                NPC.position.Y = NPC.position.Y + (NPC.height / 2);
                NPC.width = 45;
                NPC.height = 45;
                NPC.position.X = NPC.position.X - (NPC.width / 2);
                NPC.position.Y = NPC.position.Y - (NPC.height / 2);
                for (int i = 0; i < 2; i++)
                {
                    int rubyDust = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 90, 0f, 0f, 100, default, 2f);
                    Main.dust[rubyDust].velocity *= 3f;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[rubyDust].scale = 0.5f;
                        Main.dust[rubyDust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int j = 0; j < 10; j++)
                {
                    int rubyDust2 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 90, 0f, 0f, 100, default, 3f);
                    Main.dust[rubyDust2].noGravity = true;
                    Main.dust[rubyDust2].velocity *= 5f;
                    rubyDust2 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 90, 0f, 0f, 100, default, 2f);
                    Main.dust[rubyDust2].velocity *= 2f;
                }
            }
        }
    }
}
