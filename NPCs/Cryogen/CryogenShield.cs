using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Cryogen
{
    public class CryogenShield : ModNPC
    {
        public static readonly SoundStyle BreakSound = new("CalamityMod/Sounds/NPCKilled/CryogenShieldBreak");

        public override void SetStaticDefaults()
        {
            this.HideFromBestiary();
            DisplayName.SetDefault("Cryogen's Shield");
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.canGhostHeal = false;
            NPC.noTileCollide = true;
            NPC.coldDamage = true;
            NPC.GetNPCDamage();
            NPC.width = 216;
            NPC.height = 216;
            NPC.scale *= (CalamityWorld.death || BossRushEvent.BossRushActive || Main.getGoodWorld) ? 0.8f : 1f;
            NPC.DR_NERD(0.4f);
            NPC.lifeMax = CalamityWorld.death ? 700 : 1400;
            if (BossRushEvent.BossRushActive)
            {
                NPC.lifeMax = 10000;
            }
            NPC.alpha = 255;
            NPC.HitSound = Cryogen.HitSound;
            NPC.DeathSound = BreakSound;
            if (CalamityWorld.getFixedBoi)
            {
                NPC.Calamity().VulnerableToHeat = false;
                NPC.Calamity().VulnerableToCold = true;
                NPC.Calamity().VulnerableToWater = true;
            }
            else
            {
                NPC.Calamity().VulnerableToHeat = true;
                NPC.Calamity().VulnerableToCold = false;
                NPC.Calamity().VulnerableToSickness = false;
            }
        }

        public override void AI()
        {
            NPC.HitSound = CalamityWorld.getFixedBoi ? SoundID.NPCHit41 : Cryogen.HitSound;
            NPC.DeathSound = CalamityWorld.getFixedBoi ? SoundID.NPCDeath14 : BreakSound;

            NPC.alpha -= 3;
            if (NPC.alpha < 0)
                NPC.alpha = 0;

            NPC.rotation += 0.15f;

            if (NPC.type == ModContent.NPCType<CryogenShield>())
            {
                int num989 = (int)NPC.ai[0];
                if (Main.npc[num989].active && Main.npc[num989].type == ModContent.NPCType<Cryogen>())
                {
                    NPC.velocity = Vector2.Zero;
                    NPC.position = Main.npc[num989].Center;
                    NPC.ai[1] = Main.npc[num989].velocity.X;
                    NPC.ai[2] = Main.npc[num989].velocity.Y;
                    NPC.ai[3] = Main.npc[num989].target;
                    NPC.position.X = NPC.position.X - (NPC.width / 2);
                    NPC.position.Y = NPC.position.Y - (NPC.height / 2);
                    return;
                }
                NPC.life = 0;
                NPC.HitEffect();
                NPC.active = false;
                NPC.netUpdate = true;
            }
        }

        // Can only hit the target if within certain distance
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            Rectangle targetHitbox = target.Hitbox;

            float dist1 = Vector2.Distance(NPC.Center, targetHitbox.TopLeft());
            float dist2 = Vector2.Distance(NPC.Center, targetHitbox.TopRight());
            float dist3 = Vector2.Distance(NPC.Center, targetHitbox.BottomLeft());
            float dist4 = Vector2.Distance(NPC.Center, targetHitbox.BottomRight());

            float minDist = dist1;
            if (dist2 < minDist)
                minDist = dist2;
            if (dist3 < minDist)
                minDist = dist3;
            if (dist4 < minDist)
                minDist = dist4;

            return minDist <= 100f * NPC.scale && NPC.alpha == 0;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            if (damage > 0)
            {
                if (CalamityWorld.getFixedBoi)
                {
                    player.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 240, true);
                    player.AddBuff(ModContent.BuffType<VulnerabilityHex>(), 120, true);
                }
                else
                {
                    player.AddBuff(BuffID.Frostburn, 240, true);
                    player.AddBuff(BuffID.Chilled, 120, true);
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/NPCs/Cryogen/CryogenShield").Value;

            Vector2 origin = new Vector2(TextureAssets.Npc[NPC.type].Value.Width / 2, TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type] / 2);
            Vector2 drawPos = NPC.Center - screenPos;
            drawPos -= new Vector2(texture.Width, texture.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
            drawPos += origin * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            Color overlay = CalamityWorld.getFixedBoi ? Color.Red : drawColor;
            spriteBatch.Draw(texture, drawPos, NPC.frame, NPC.GetAlpha(overlay), NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.5f * bossLifeScale);
        }

        public override void ModifyTypeName(ref string typeName)
        {
            if (CalamityWorld.getFixedBoi)
            {
                typeName = "Pyrogen's Shield";
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            int dusttype = CalamityWorld.getFixedBoi ? 235 : 67;
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, dusttype, hitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int num621 = 0; num621 < 25; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, dusttype, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }

                for (int num623 = 0; num623 < 50; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, dusttype, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, dusttype, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }

                if (Main.netMode != NetmodeID.Server && !CalamityWorld.getFixedBoi)
                {
                    int totalGores = 16;
                    double radians = MathHelper.TwoPi / totalGores;
                    Vector2 spinningPoint = new Vector2(0f, -1f);
                    for (int k = 0; k < totalGores; k++)
                    {
                        Vector2 vector255 = spinningPoint.RotatedBy(radians * k);
                        for (int x = 1; x <= 4; x++)
                        {
                            float randomSpread = Main.rand.Next(-200, 201) / 100f;
                            Gore.NewGore(NPC.GetSource_Death(), NPC.Center + Vector2.Normalize(vector255) * 80f, vector255 * new Vector2(NPC.ai[1], NPC.ai[2]) * randomSpread, Mod.Find<ModGore>("CryoShieldGore" + x).Type, NPC.scale);
                        }
                    }
                }
            }
        }
    }
}
