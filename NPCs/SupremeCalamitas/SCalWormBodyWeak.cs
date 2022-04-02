using CalamityMod.Events;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.SupremeCalamitas
{
    public class SCalWormBodyWeak : ModNPC
    {
        private bool setAlpha = false;
        public NPC AheadSegment => Main.npc[(int)npc.ai[1]];
        public NPC HeadSegment => Main.npc[(int)npc.ai[2]];
        public ref float AttackTimer => ref npc.localAI[0];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brimstone Heart");
            Main.npcFrameCount[npc.type] = 5;
        }

        public override void SetDefaults()
        {
            npc.damage = 0;
            npc.npcSlots = 5f;
            npc.width = 20;
            npc.height = 20;
            npc.lifeMax = CalamityWorld.revenge ? 345000 : 300000;
            npc.aiStyle = -1;
            aiType = -1;
            npc.knockBackResist = 0f;
            npc.scale = Main.expertMode ? 1.35f : 1.2f;
            npc.alpha = 255;
            npc.chaseable = false;
            npc.behindTiles = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.canGhostHeal = false;
            npc.netAlways = true;
            npc.dontCountMe = true;

            CalamityGlobalNPC global = npc.Calamity();
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
            if (npc.ai[2] > 0f)
                npc.realLife = (int)npc.ai[2];

            bool shouldDie = false;
            if (npc.ai[1] <= 0f)
                shouldDie = true;
            else if (AheadSegment.life <= 0 || !AheadSegment.active || npc.life <= 0)
                shouldDie = true;

            if (shouldDie)
            {
                npc.life = 0;
                npc.HitEffect(0, 10.0);
                npc.checkDead();
            }

            if (AheadSegment.alpha < 128 && !setAlpha)
            {
                if (npc.alpha != 0)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Dust fire = Dust.NewDustDirect(npc.position, npc.width, npc.height, 182, 0f, 0f, 100, default, 2f);
                        fire.noGravity = true;
                        fire.noLight = true;
                    }
                }
                npc.alpha -= 42;
                if (npc.alpha <= 0)
                {
                    setAlpha = true;
                    npc.alpha = 0;
                }
            }
            else
                npc.alpha = HeadSegment.alpha;

            AttackTimer += (CalamityWorld.malice || BossRushEvent.BossRushActive) ? 1.5f : 1f;
            if (AttackTimer >= 900f)
            {
                AttackTimer = 0f;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int type = ModContent.ProjectileType<BrimstoneBarrage>();
                    int damage = npc.GetProjectileDamage(type);
                    int totalProjectiles = 4;
                    float radians = MathHelper.TwoPi / totalProjectiles;
                    Vector2 spinningPoint = Vector2.Normalize(new Vector2(-1f, -1f));
                    for (int k = 0; k < totalProjectiles; k++)
                    {
                        Vector2 velocity = spinningPoint.RotatedBy(radians * k);
                        Projectile.NewProjectile(npc.Center, velocity, type, damage, 0f, Main.myPlayer);
                    }
                    npc.netUpdate = true;
                }
                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/SCalSounds/BrimstoneShoot"), npc.Center);
            }

            if (Main.npc.IndexInRange((int)npc.ai[1]))
            {
                Vector2 offsetToAheadSegment = AheadSegment.Center - npc.Center;
                npc.rotation = offsetToAheadSegment.ToRotation() + MathHelper.PiOver2;
                npc.velocity = Vector2.Zero;
                npc.Center = AheadSegment.Center - offsetToAheadSegment.SafeNormalize(Vector2.UnitY) * 34f;
                npc.spriteDirection = (offsetToAheadSegment.X > 0f).ToDirectionInt();
            }
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (CalamityLists.projectileDestroyExceptionList.TrueForAll(x => projectile.type != x))
            {
                if (projectile.penetrate == -1 && !projectile.minion)
                {
                    projectile.penetrate = 1;
                }
                else if (projectile.penetrate >= 1)
                {
                    projectile.penetrate = 1;
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;
            npc.frame.Y = ((int)(npc.frameCounter / 5) + npc.whoAmI) % Main.npcFrameCount[npc.type] * frameHeight;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient && npc.life <= 0)
            {
                for (int i = 0; i < Main.rand.Next(1, 3 + 1); i++)
                {
                    if (!Main.rand.NextBool(3))
                        continue;

                    Vector2 soulVelocity = -Vector2.UnitY.RotatedByRandom(0.53f) * Main.rand.NextFloat(2.5f, 4f);
                    Projectile.NewProjectile(npc.Center, soulVelocity, ModContent.ProjectileType<SepulcherSoul>(), 0, 0f);
                }
            }
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override bool PreNPCLoot()
        {
            return false;
        }
    }
}
