using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Astral
{
    public class Hiveling : ModNPC
    {
        private static Texture2D glowmask;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hiveling");
            if (!Main.dedServ)
                glowmask = ModContent.GetTexture("CalamityMod/NPCs/Astral/HivelingGlow");
            Main.npcFrameCount[npc.type] = 4;
        }

        public override void SetDefaults()
        {
            npc.width = 50;
            npc.height = 40;
            npc.aiStyle = -1;
            npc.damage = 30;
            npc.defense = 0;
            npc.lifeMax = 150;
            npc.DeathSound = mod.GetLegacySoundSlot(SoundType.NPCKilled, "Sounds/NPCKilled/AstralEnemyDeath");
            npc.knockBackResist = 0.5f;
            npc.noGravity = true;
            npc.value = Item.buyPrice(0, 0, 5, 0);
            if (CalamityWorld.downedAstrageldon)
            {
                npc.damage = 50;
                npc.defense = 8;
                npc.knockBackResist = 0.4f;
                npc.lifeMax = 220;
            }
            npc.Calamity().VulnerableToHeat = true;
            npc.Calamity().VulnerableToSickness = false;
        }

        public override void AI()
        {
            if (npc.ai[1] == 0f)
            {
                npc.velocity *= 0.97f;

                npc.TargetClosest(false);
                if (Main.player[npc.target].dead)
                {
                    npc.TargetClosest(false);
                }
                Player targ = Main.player[npc.target];

                if (Collision.CanHit(npc.position, npc.width, npc.height, targ.position, targ.width, targ.height) || Vector2.Distance(npc.Center, targ.MountedCenter) < 320f)
                {
                    npc.ai[1] = 1f;
                }
            }
            else
            {
                CalamityGlobalNPC.DoFlyingAI(npc, (CalamityWorld.death ? 4.5f : 3f), (CalamityWorld.death ? 0.075f : 0.05f), 200f);
                Player myTarget = Main.player[npc.target];
                Vector2 toTarget = myTarget.Center - npc.Center;
                if (!myTarget.dead && myTarget.active)
                {
                    npc.spriteDirection = npc.direction = (toTarget.X > 0).ToDirectionInt();
                }
                else
                {
                    npc.spriteDirection = npc.direction = (npc.velocity.X > 0).ToDirectionInt();
                }
                if (npc.spriteDirection == 1)
                    npc.rotation += MathHelper.Pi;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 0.05f + npc.velocity.Length() * 0.667f;
            if (npc.frameCounter >= 8)
            {
                npc.frameCounter = 0;
                npc.frame.Y += frameHeight;
                if (npc.frame.Y > npc.height * 2)
                {
                    npc.frame.Y = 0;
                }
            }

            //DO DUST
            Dust d = CalamityGlobalNPC.SpawnDustOnNPC(npc, 30, frameHeight, ModContent.DustType<AstralOrange>(), new Rectangle(16, 8, 6, 6), Vector2.Zero, 0.3f, true);
            if (d != null)
            {
                d.customData = 0.04f;
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            //draw glowmask
            spriteBatch.Draw(glowmask, npc.Center - Main.screenPosition + new Vector2(0, 12), npc.frame, Color.White * 0.6f, npc.rotation, new Vector2(15, 10), 1f, npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.soundDelay == 0)
            {
                npc.soundDelay = 15;
                switch (Main.rand.Next(3))
                {
                    case 0:
                        Main.PlaySound(mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/NPCHit/AstralEnemyHit"), npc.Center);
                        break;
                    case 1:
                        Main.PlaySound(mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/NPCHit/AstralEnemyHit2"), npc.Center);
                        break;
                    case 2:
                        Main.PlaySound(mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/NPCHit/AstralEnemyHit3"), npc.Center);
                        break;
                }
            }

            CalamityGlobalNPC.DoHitDust(npc, hitDirection, (Main.rand.Next(0, Math.Max(0, npc.life)) == 0) ? 5 : ModContent.DustType<AstralEnemy>(), 1f, 3, 20);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return 0f;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 60, true);
        }
    }
}
