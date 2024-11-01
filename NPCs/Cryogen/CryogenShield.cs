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
    [LongDistanceNetSync(SyncWith = typeof(Cryogen))]
    public class CryogenShield : ModNPC
    {
        public static readonly SoundStyle BreakSound = new("CalamityMod/Sounds/NPCKilled/CryogenShieldBreak");

        public override void SetStaticDefaults()
        {
            this.HideFromBestiary();
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.aiStyle = -1;
            AIType = -1;
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
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.Opacity = 0f;
            NPC.HitSound = Cryogen.HitSound;
            NPC.DeathSound = BreakSound;
            if (Main.zenithWorld)
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
            NPC.HitSound = Main.zenithWorld ? SoundID.NPCHit41 : Cryogen.HitSound;
            NPC.DeathSound = Main.zenithWorld ? SoundID.NPCDeath14 : BreakSound;

            NPC.Opacity += 0.01f;
            if (NPC.Opacity >= 1f)
            {
                NPC.damage = NPC.defDamage;
                NPC.Opacity = 1f;
            }
            else
                NPC.damage = 0;

            NPC.rotation += 0.15f;

            if (NPC.type == ModContent.NPCType<CryogenShield>())
            {
                int mainCryogen = (int)NPC.ai[0];
                if (Main.npc[mainCryogen].active && Main.npc[mainCryogen].type == ModContent.NPCType<Cryogen>())
                {
                    NPC.velocity = Vector2.Zero;
                    NPC.position = Main.npc[mainCryogen].Center;
                    NPC.ai[1] = Main.npc[mainCryogen].velocity.X;
                    NPC.ai[2] = Main.npc[mainCryogen].velocity.Y;
                    NPC.ai[3] = Main.npc[mainCryogen].target;
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

            float hitboxTopLeft = Vector2.Distance(NPC.Center, targetHitbox.TopLeft());
            float hitboxTopRight = Vector2.Distance(NPC.Center, targetHitbox.TopRight());
            float hitboxBotLeft = Vector2.Distance(NPC.Center, targetHitbox.BottomLeft());
            float hitboxBotRight = Vector2.Distance(NPC.Center, targetHitbox.BottomRight());

            float minDist = hitboxTopLeft;
            if (hitboxTopRight < minDist)
                minDist = hitboxTopRight;
            if (hitboxBotLeft < minDist)
                minDist = hitboxBotLeft;
            if (hitboxBotRight < minDist)
                minDist = hitboxBotRight;

            return minDist <= (100f * NPC.scale) && NPC.Opacity >= 1f;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
            {
                if (Main.zenithWorld)
                {
                    target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 120, true);
                    target.AddBuff(ModContent.BuffType<VulnerabilityHex>(), 120, true);
                }
                else
                {
                    target.AddBuff(BuffID.Frostburn, 240, true);
                    target.AddBuff(BuffID.Chilled, 120, true);
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/NPCs/Cryogen/CryogenShield").Value;

            NPC.DrawBackglow(Main.zenithWorld ? Color.Red : Cryogen.BackglowColor, 4f, SpriteEffects.None, NPC.frame, screenPos);

            Vector2 origin = new Vector2(TextureAssets.Npc[NPC.type].Value.Width / 2, TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type] / 2);
            Vector2 drawPos = NPC.Center - screenPos;
            drawPos -= new Vector2(texture.Width, texture.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
            drawPos += origin * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            Color overlay = Main.zenithWorld ? Color.Red : drawColor;
            spriteBatch.Draw(texture, drawPos, NPC.frame, NPC.GetAlpha(overlay), NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.5f * balance);
        }

        public override void ModifyTypeName(ref string typeName)
        {
            if (Main.zenithWorld)
            {
                typeName = CalamityUtils.GetTextValue("NPCs.PyrogenShield");
            }
        }

        // Don't count towards NPC limit and don't despawn offscreen.
        public override bool CheckActive() => false;

        public override void HitEffect(NPC.HitInfo hit)
        {
            int dusttype = Main.zenithWorld ? 235 : 67;
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, dusttype, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 25; i++)
                {
                    int icyDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, dusttype, 0f, 0f, 100, default, 2f);
                    Main.dust[icyDust].velocity *= 3f;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[icyDust].scale = 0.5f;
                        Main.dust[icyDust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }

                for (int j = 0; j < 50; j++)
                {
                    int icyDust2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, dusttype, 0f, 0f, 100, default, 3f);
                    Main.dust[icyDust2].noGravity = true;
                    Main.dust[icyDust2].velocity *= 5f;
                    icyDust2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, dusttype, 0f, 0f, 100, default, 2f);
                    Main.dust[icyDust2].velocity *= 2f;
                }

                if (Main.netMode != NetmodeID.Server && !Main.zenithWorld)
                {
                    int totalGores = 16;
                    double radians = MathHelper.TwoPi / totalGores;
                    Vector2 spinningPoint = new Vector2(0f, -1f);
                    for (int k = 0; k < totalGores; k++)
                    {
                        Vector2 goreRotation = spinningPoint.RotatedBy(radians * k);
                        for (int x = 1; x <= 4; x++)
                        {
                            float randomSpread = Main.rand.Next(-200, 201) / 100f;
                            Gore.NewGore(NPC.GetSource_Death(), NPC.Center + Vector2.Normalize(goreRotation) * 80f, goreRotation * new Vector2(NPC.ai[1], NPC.ai[2]) * randomSpread, Mod.Find<ModGore>("CryoShieldGore" + x).Type, NPC.scale);
                        }
                    }
                }
            }
        }
    }
}
