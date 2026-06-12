using CalamityMod.Events;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Ravager
{
    public class FlamePillar : ModNPC
    {
        public static readonly SoundStyle HitSound = new("CalamityMod/Sounds/NPCHit/RavagerRockPillarHit", 3);
        public static Asset<Texture2D> GlowTexture;

        public override void SetStaticDefaults()
        {
            this.HideFromBestiary();
            NPCID.Sets.ImmuneToAllBuffs[Type] = true;
            Main.npcFrameCount[Type] = 4;
            if (!Main.dedServ)
            {
                GlowTexture = ModContent.Request<Texture2D>(Texture + "Glow", AssetRequestMode.AsyncLoad);
            }
        }

        public static int FlameDamage = 30; // 120
        public static int PostProviFlameBuff = 20; // +80 = 200

        public override void SetDefaults()
        {
            NPC.damage = 75; // 150
            NPC.width = 40;
            NPC.height = 150;
            NPC.chaseable = false;
            NPC.lifeMax = 1250;
            NPC.alpha = 255;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.HitSound = HitSound;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToWater = true;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.15f;
            NPC.frameCounter %= Main.npcFrameCount[Type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override void AI()
        {
            if (NPC.lifeMax > 1250)
                NPC.lifeMax = 1250;
            if (NPC.life > NPC.lifeMax)
                NPC.life = NPC.lifeMax;
            // Avoid cheap bullshit
            NPC.damage = 0;

            bool provy = DownedBossSystem.downedProvidence;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            if (CalamityGlobalNPC.scavenger < 0 || !Main.npc[CalamityGlobalNPC.scavenger].active)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    NPC.StrikeInstantKill();

                return;
            }

            if (NPC.timeLeft < 1800)
                NPC.timeLeft = 1800;

            Lighting.AddLight((int)((NPC.position.X + (NPC.width / 2)) / 16f), (int)((NPC.position.Y + (NPC.height / 2)) / 16f), 0f, 0.5f, 0.5f);

            if (NPC.alpha > 0)
            {
                NPC.alpha -= 5;
                if (NPC.alpha < 0)
                    NPC.alpha = 0;
            }

            if (NPC.ai[0] == 0f)
            {
                NPC.ai[1] += 1f;
                if (NPC.ai[1] >= 60f)
                {
                    NPC.ai[0] = 1f;
                    NPC.ai[1] = 180f;
                }
            }
            else if (NPC.ai[0] == 1f)
            {
                if (NPC.ai[1] >= 0f)
                {
                    NPC.ai[1] -= 1f;
                    NPC.localAI[0] += 1f;
                    if (NPC.localAI[0] % (death ? 45f : 60f) == 0f)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            float speedY = -12f;
                            float speedX = 0f;
                            switch ((int)NPC.ai[2])
                            {
                                case 0:
                                    break;
                                case 1:
                                    speedX = 2f;
                                    break;
                                case 2:
                                    speedX = -2f;
                                    break;
                                default:
                                    break;
                            }
                            Vector2 velocity = new Vector2(speedX, speedY);
                            int type = ModContent.ProjectileType<RavagerFlame>();
                            NPC.SimpleStrikeNPC(NPC.lifeMax/4,0,false,noPlayerInteraction:true);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocity, type, FlameDamage + (provy ? PostProviFlameBuff : 0), 0f, Main.myPlayer, 0f, 0f);
                        }

                        NPC.ai[2] += 1f;
                        NPC.localAI[0] = 0f;
                        NPC.ForceNetUpdate(false);
                    }
                }
                else
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        NPC.StrikeInstantKill();
                }
            }
        }
        public override bool? CanFallThroughPlatforms() => NPC.target >= 0 && Main.player[NPC.target].position.Y > NPC.position.Y + NPC.height;

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture2D15 = TextureAssets.Npc[Type].Value;
            Vector2 halfSizeTexture = new Vector2(TextureAssets.Npc[Type].Value.Width / 2, TextureAssets.Npc[Type].Value.Height / 2);
            Vector2 drawLocation = NPC.Center - screenPos;
            drawLocation -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[Type]) * NPC.scale / 2f;
            drawLocation += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);

            spriteBatch.Draw(texture2D15, drawLocation, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

            texture2D15 = GlowTexture.Value;
            Color flameBlue = Color.Lerp(Color.White, Color.Cyan, 0.5f);

            spriteBatch.Draw(texture2D15, drawLocation, NPC.frame, flameBlue, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

            return false;
        }

        public override bool CheckActive() => false;

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                NPC.position.X = NPC.position.X + (NPC.width / 2);
                NPC.position.Y = NPC.position.Y + (NPC.height / 2);
                NPC.width = 50;
                NPC.height = 180;
                NPC.position.X = NPC.position.X - (NPC.width / 2);
                NPC.position.Y = NPC.position.Y - (NPC.height / 2);
                for (int i = 0; i < 30; i++)
                {
                    int iceFlame = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.IceTorch, 0f, 0f, 100, default, 2f);
                    Main.dust[iceFlame].velocity *= 3f;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[iceFlame].scale = 0.5f;
                        Main.dust[iceFlame].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int j = 0; j < 30; j++)
                {
                    int rockDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Stone, 0f, 0f, 100, default, 3f);
                    Main.dust[rockDust].noGravity = true;
                    Main.dust[rockDust].velocity *= 5f;
                    rockDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Iron, 0f, 0f, 100, default, 2f);
                    Main.dust[rockDust].velocity *= 2f;
                }
            }
            else
            {
                for (int i = 0; i < 2; i++)
                {
                    int iceFlame = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Iron, 0f, 0f, 100, default, 2f);
                    Main.dust[iceFlame].velocity *= 3f;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[iceFlame].scale = 0.5f;
                        Main.dust[iceFlame].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int j = 0; j < 2; j++)
                {
                    int rockDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Stone, 0f, 0f, 100, default, 3f);
                    Main.dust[rockDust].noGravity = true;
                    Main.dust[rockDust].velocity *= 5f;
                    rockDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Iron, 0f, 0f, 100, default, 2f);
                    Main.dust[rockDust].velocity *= 2f;
                }
            }
        }

        public override void ModifyHitByItem(Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            if (item.pick > 0)
            {
                modifiers.FlatBonusDamage += -10000;
                modifiers.FinalDamage.Flat += item.pick - 1;
                modifiers.SetCrit();
            }
            else
            {
                modifiers.SetMaxDamage(1);
                modifiers.DisableCrit();
                modifiers.HideCombatText();
            }
            base.ModifyHitByItem(player, item, ref modifiers);
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            var item = Main.player[projectile.owner].HeldItem;
            if (item.pick > 0 && !projectile.CountsAsClass<SummonDamageClass>())
            {
                modifiers.FlatBonusDamage += -10000;
                modifiers.FinalDamage.Flat += item.pick-1;
                modifiers.SetCrit();
            }
            else
            {
                modifiers.SetMaxDamage(1);
                modifiers.DisableCrit();
                modifiers.HideCombatText();
            }
            base.ModifyHitByProjectile(projectile, ref modifiers);
        }
    }
}
