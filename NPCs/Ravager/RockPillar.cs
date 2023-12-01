using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.NPCs.Ravager
{
    public class RockPillar : ModNPC
    {
        public static readonly SoundStyle HitSound = new("CalamityMod/Sounds/NPCHit/RavagerRockPillarHit", 3);
        public override void SetStaticDefaults()
        {
            this.HideFromBestiary();
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.GetNPCDamage();
            NPC.width = 60;
            NPC.height = 300;
            NPC.defense = 50;
            NPC.DR_NERD(0.3f);
            NPC.chaseable = false;
            NPC.canGhostHeal = false;
            NPC.lifeMax = DownedBossSystem.downedProvidence ? 20000 : 5000;
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.alpha = 255;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.HitSound = RavagerBody.PillarSound;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToWater = true;
        }

        public override void AI()
        {
            if (CalamityGlobalNPC.scavenger < 0 || !Main.npc[CalamityGlobalNPC.scavenger].active)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    NPC.StrikeInstantKill();

                return;
            }

            if (NPC.timeLeft < 1800)
                NPC.timeLeft = 1800;

            if (NPC.alpha > 0)
            {
                NPC.damage = 0;

                NPC.alpha -= 10;
                if (NPC.alpha < 0)
                    NPC.alpha = 0;
            }
            else
            {
                if (DownedBossSystem.downedProvidence && !BossRushEvent.BossRushActive)
                    NPC.damage = (int)(NPC.defDamage * 1.5);
                else
                    NPC.damage = NPC.defDamage;
            }

            if (NPC.ai[0] == 0f)
            {
                if (NPC.velocity.Y == 0f)
                {
                    if (NPC.ai[1] == -1f)
                    {
                        SoundEngine.PlaySound(SoundID.Item62, NPC.Center);

                        for (int i = 0; i < 10; i++)
                        {
                            int rockDust = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.Iron, 0f, 0f, 100, default, 2f);
                            Main.dust[rockDust].velocity *= 3f;
                            if (Main.rand.NextBool())
                            {
                                Main.dust[rockDust].scale = 0.5f;
                                Main.dust[rockDust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                            }
                        }
                        for (int j = 0; j < 10; j++)
                        {
                            int rockDust2 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.Stone, 0f, 0f, 100, default, 3f);
                            Main.dust[rockDust2].noGravity = true;
                            Main.dust[rockDust2].velocity *= 5f;
                            rockDust2 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.Iron, 0f, 0f, 100, default, 2f);
                            Main.dust[rockDust2].velocity *= 2f;
                        }

                        NPC.noTileCollide = true;
                        NPC.velocity.X = (BossRushEvent.BossRushActive ? 15 : 12) * NPC.direction;
                        NPC.velocity.Y = -28.5f;
                        NPC.ai[0] = 1f;
                        NPC.ai[1] = 0f;
                    }
                }
            }
            else
            {
                if (NPC.velocity.Y == 0f || Vector2.Distance(NPC.Center, Main.npc[CalamityGlobalNPC.scavenger].Center) > 2800f)
                {
                    SoundEngine.PlaySound(SoundID.Item14, NPC.Center);
                    NPC.ai[0] = 0f;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        NPC.StrikeInstantKill();

                    return;
                }
                else
                {
                    NPC.velocity.Y += 0.2f;

                    if (NPC.velocity.Y >= 0f && !Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                        NPC.noTileCollide = false;
                }
            }
        }

        public override bool CheckActive() => false;

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage <= 0)
                return;

            target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 240, true);
            SoundEngine.PlaySound(SoundID.Item14, NPC.Center);
            NPC.ai[0] = 0f;

            if (Main.netMode != NetmodeID.MultiplayerClient)
                NPC.StrikeInstantKill();
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                NPC.position.X = NPC.position.X + (NPC.width / 2);
                NPC.position.Y = NPC.position.Y + (NPC.height / 2);
                NPC.width = 80;
                NPC.height = 360;
                NPC.position.X = NPC.position.X - (NPC.width / 2);
                NPC.position.Y = NPC.position.Y - (NPC.height / 2);
                for (int i = 0; i < 30; i++)
                {
                    int rockDust = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.Iron, 0f, 0f, 100, default, 2f);
                    Main.dust[rockDust].velocity *= 3f;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[rockDust].scale = 0.5f;
                        Main.dust[rockDust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int j = 0; j < 30; j++)
                {
                    int rockDust2 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.Stone, 0f, 0f, 100, default, 3f);
                    Main.dust[rockDust2].noGravity = true;
                    Main.dust[rockDust2].velocity *= 5f;
                    rockDust2 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.Iron, 0f, 0f, 100, default, 2f);
                    Main.dust[rockDust2].velocity *= 2f;
                }

                if (Main.netMode != NetmodeID.Server)
                {
                    float y = NPC.height / 6f;
                    float randomVelocityScale = 0.25f;
                    for (int i = 0; i < 2; i++)
                    {
                        Vector2 randomVelocity = NPC.velocity * Main.rand.NextFloat() * randomVelocityScale;
                        Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity + randomVelocity, Mod.Find<ModGore>("RockPillar").Type, 1f);
                        Gore.NewGore(NPC.GetSource_Death(), NPC.position + Vector2.UnitY * y, NPC.velocity + randomVelocity, Mod.Find<ModGore>("RockPillar2").Type, 1f);
                        Gore.NewGore(NPC.GetSource_Death(), NPC.position + Vector2.UnitY * y * 2f, NPC.velocity + randomVelocity, Mod.Find<ModGore>("RockPillar3").Type, 1f);
                        Gore.NewGore(NPC.GetSource_Death(), NPC.position + Vector2.UnitY * y * 3f, NPC.velocity + randomVelocity, Mod.Find<ModGore>("RockPillar4").Type, 1f);
                        Gore.NewGore(NPC.GetSource_Death(), NPC.position + Vector2.UnitY * y * 4f, NPC.velocity + randomVelocity, Mod.Find<ModGore>("RockPillar5").Type, 1f);
                        Gore.NewGore(NPC.GetSource_Death(), NPC.position + Vector2.UnitY * y * 5f, NPC.velocity + randomVelocity, Mod.Find<ModGore>("RockPillar6").Type, 1f);
                    }
                }
            }
            else
            {
                for (int i = 0; i < 2; i++)
                {
                    int rockDust = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.Iron, 0f, 0f, 100, default, 2f);
                    Main.dust[rockDust].velocity *= 3f;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[rockDust].scale = 0.5f;
                        Main.dust[rockDust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int j = 0; j < 2; j++)
                {
                    int rockDust2 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.Stone, 0f, 0f, 100, default, 3f);
                    Main.dust[rockDust2].noGravity = true;
                    Main.dust[rockDust2].velocity *= 5f;
                    rockDust2 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.Iron, 0f, 0f, 100, default, 2f);
                    Main.dust[rockDust2].velocity *= 2f;
                }
            }
        }
    }
}
