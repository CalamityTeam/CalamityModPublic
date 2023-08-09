using CalamityMod.Events;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.NPCs.SupremeCalamitas
{
    public class SepulcherBodyEnergyBall : ModNPC
    {
        private bool setAlpha = false;
        public NPC AheadSegment => Main.npc[(int)NPC.ai[1]];
        public NPC HeadSegment => Main.npc[(int)NPC.ai[2]];
        public ref float AttackTimer => ref NPC.localAI[0];
        public override LocalizedText DisplayName => CalamityUtils.GetText("NPCs.SepulcherHead.DisplayName");
        public override void SetStaticDefaults()
        {
            this.HideFromBestiary();
            Main.npcFrameCount[NPC.type] = 5;
        }

        public override void SetDefaults()
        {
            NPC.damage = 0;
            NPC.npcSlots = 5f;
            NPC.width = 20;
            NPC.height = 20;
            NPC.lifeMax = CalamityWorld.revenge ? 345000 : 300000;
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.scale *= Main.expertMode ? 1.35f : 1.2f;
            NPC.alpha = 255;
            NPC.chaseable = false;
            NPC.behindTiles = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.canGhostHeal = false;
            NPC.netAlways = true;
            NPC.dontCountMe = true;

            CalamityGlobalNPC global = NPC.Calamity();
            global.DR = 0.999999f;
            global.unbreakableDR = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(AttackTimer);
            writer.Write(setAlpha);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            AttackTimer = reader.ReadSingle();
            setAlpha = reader.ReadBoolean();
        }

        public override void AI()
        {
            if (NPC.ai[2] > 0f)
                NPC.realLife = (int)NPC.ai[2];

            bool shouldDie = false;
            if (NPC.ai[1] <= 0f)
                shouldDie = true;
            else if (AheadSegment.life <= 0 || !AheadSegment.active || NPC.life <= 0)
                shouldDie = true;

            if (shouldDie)
            {
                NPC.life = 0;
                NPC.HitEffect(0, 10.0);
                NPC.checkDead();
            }

            if (AheadSegment.alpha < 128 && !setAlpha)
            {
                if (NPC.alpha != 0)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Dust fire = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, 182, 0f, 0f, 100, default, 2f);
                        fire.noGravity = true;
                        fire.noLight = true;
                    }
                }
                NPC.alpha -= 42;
                if (NPC.alpha <= 0)
                {
                    setAlpha = true;
                    NPC.alpha = 0;
                }
            }
            else
                NPC.alpha = HeadSegment.alpha;

            AttackTimer += BossRushEvent.BossRushActive ? 1.5f : 1f;
            float attackgate = !HeadSegment.Calamity().unbreakableDR && Main.zenithWorld ? 450f : 900f;
            if (AttackTimer >= attackgate)
            {
                AttackTimer = 0f;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int type = ModContent.ProjectileType<BrimstoneBarrage>();
                    int damage = NPC.GetProjectileDamage(type);
                    int totalProjectiles = 4;
                    float radians = MathHelper.TwoPi / totalProjectiles;
                    Vector2 spinningPoint = Vector2.Normalize(new Vector2(-1f, -1f));
                    for (int k = 0; k < totalProjectiles; k++)
                    {
                        Vector2 velocity = spinningPoint.RotatedBy(radians * k);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocity, type, damage, 0f, Main.myPlayer);
                    }
                    NPC.netUpdate = true;
                }
                SoundEngine.PlaySound(SupremeCalamitas.BrimstoneShotSound, NPC.Center);
            }

            if (Main.npc.IndexInRange((int)NPC.ai[1]))
            {
                Vector2 offsetToAheadSegment = AheadSegment.Center - NPC.Center;
                NPC.rotation = offsetToAheadSegment.ToRotation() + MathHelper.PiOver2;
                NPC.velocity = Vector2.Zero;
                NPC.Center = AheadSegment.Center - offsetToAheadSegment.SafeNormalize(Vector2.UnitY) * 34f;
                NPC.spriteDirection = (offsetToAheadSegment.X > 0f).ToDirectionInt();
            }
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            NPC.frame.Y = ((int)(NPC.frameCounter / 5) + NPC.whoAmI) % Main.npcFrameCount[NPC.type] * frameHeight;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient && NPC.life <= 0)
            {
                for (int i = 0; i < Main.rand.Next(1, 3 + 1); i++)
                {
                    if (!Main.rand.NextBool(3))
                        continue;

                    Vector2 soulVelocity = -Vector2.UnitY.RotatedByRandom(0.53f) * Main.rand.NextFloat(2.5f, 4f);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, soulVelocity, ModContent.ProjectileType<SepulcherSoul>(), 0, 0f);
                }
            }
        }

        public override bool CheckActive()
        {
            return false;
        }
    }
}
