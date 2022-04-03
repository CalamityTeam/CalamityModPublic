using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.SupremeCalamitas
{
    public class SCalWormBody : ModNPC
    {
        private bool setAlpha = false;
        public NPC AheadSegment => Main.npc[(int)NPC.ai[1]];
        public NPC HeadSegment => Main.npc[(int)NPC.ai[2]];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sepulcher");
        }

        public override void SetDefaults()
        {
            NPC.damage = 0; //70
            NPC.npcSlots = 5f;
            NPC.width = NPC.height = 48;
            NPC.defense = 0;
            CalamityGlobalNPC global = NPC.Calamity();
            global.DR = 0.999999f;
            global.unbreakableDR = true;
            NPC.lifeMax = CalamityWorld.revenge ? 345000 : 300000;
            NPC.aiStyle = -1; //new
            aiType = -1; //new
            NPC.knockBackResist = 0f;
            NPC.scale = 1.2f;
            if (Main.expertMode)
            {
                NPC.scale = 1.35f;
            }
            NPC.alpha = 255;
            NPC.chaseable = false;
            NPC.behindTiles = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.canGhostHeal = false;
            NPC.netAlways = true;
            NPC.dontCountMe = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[3]);
            writer.Write(setAlpha);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[3] = reader.ReadSingle();
            setAlpha = reader.ReadBoolean();
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        public override void AI()
        {
            if (NPC.ai[2] > 0f)
            {
                NPC.realLife = (int)NPC.ai[2];
            }

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

            if (Main.npc.IndexInRange((int)NPC.ai[1]))
            {
                Vector2 offsetToAheadSegment = AheadSegment.Center - NPC.Center;
                NPC.rotation = offsetToAheadSegment.ToRotation() + MathHelper.PiOver2;
                NPC.velocity = Vector2.Zero;
                NPC.Center = AheadSegment.Center - offsetToAheadSegment.SafeNormalize(Vector2.UnitY) * 52f;
                NPC.spriteDirection = (offsetToAheadSegment.X > 0f).ToDirectionInt();
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

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture2D15 = NPC.localAI[3] / 2f % 2f == 0f ? ModContent.Request<Texture2D>("CalamityMod/NPCs/SupremeCalamitas/SCalWormBodyAlt") : Main.npcTexture[NPC.type];
            Vector2 vector11 = new Vector2((float)(Main.npcTexture[NPC.type].Width / 2), (float)(Main.npcTexture[NPC.type].Height / 2));

            Vector2 vector43 = NPC.Center - Main.screenPosition;
            vector43 -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height)) * NPC.scale / 2f;
            vector43 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.Draw(texture2D15, vector43, NPC.frame, NPC.GetAlpha(lightColor), NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);

            return false;
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override bool PreNPCLoot()
        {
            return false;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                int variant = (int)(NPC.localAI[3] / 2f % 2f);
                if (variant == 0)
                {
                    for (int i = 1; i <= 9; i++)
                    {
                        if (!Main.rand.NextBool(3))
                            continue;

                        Vector2 goreSpawnPosition = NPC.Center;
                        Gore.NewGorePerfect(goreSpawnPosition, Main.rand.NextVector2Circular(2f, 2f), Mod.GetGoreSlot($"Gores/SupremeCalamitas/SepulcherBody1_Gore{i}"), NPC.scale);
                    }
                }
                else
                {
                    for (int i = 1; i <= 7; i++)
                    {
                        if (!Main.rand.NextBool(3))
                            continue;

                        Vector2 goreSpawnPosition = NPC.Center;
                        Gore.NewGorePerfect(goreSpawnPosition, Main.rand.NextVector2Circular(2f, 2f), Mod.GetGoreSlot($"Gores/SupremeCalamitas/SepulcherBody2_Gore{i}"), NPC.scale);
                    }
                }
            }
        }
    }
}
