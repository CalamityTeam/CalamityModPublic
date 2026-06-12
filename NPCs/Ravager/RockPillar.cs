using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Events;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Ravager
{
    public class RockPillar : ModNPC
    {
        public static readonly SoundStyle HitSound = new("CalamityMod/Sounds/NPCHit/RavagerRockPillarHit", 3);
        public override void SetStaticDefaults()
        {
            this.HideFromBestiary();
            NPCID.Sets.ImmuneToAllBuffs[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.damage = 90; // 180
            NPC.width = 60;
            NPC.height = 300;
            NPC.defense = 50;
            NPC.DR_NERD(0.3f);
            NPC.chaseable = false;
            NPC.alpha = 255;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.HitSound = RavagerBody.PillarSound;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToWater = true;
            NPCID.Sets.ImmuneToAllBuffs[Type] = true;
            NPC.lifeMax = 1800;
        }

        //Disable dmg when stationary vertically or before jumping
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            if (NPC.velocity.Y == 0 || NPC.ai[0] == 0f)
                return false;
            return base.CanHitPlayer(target, ref cooldownSlot);
        }
        public override void AI()
        {
            if (NPC.lifeMax > 1800)
                NPC.lifeMax = 1800;
            if (NPC.life > NPC.lifeMax)
                NPC.life = NPC.lifeMax;
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
                // Avoid cheap bullshit
                NPC.damage = 0;

                NPC.alpha -= 10;
                if (NPC.alpha < 0)
                    NPC.alpha = 0;
            }
            else
            {
            }

            if (NPC.ai[0] == 0)
            {
                if (NPC.velocity.Y == 0f)
                {
                    if (NPC.ai[1] >= 2)
                    {
                        SoundEngine.PlaySound(SoundID.Item62, NPC.Center);

                        for (int i = 0; i < 10; i++)
                        {
                            int rockDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Iron, 0f, 0f, 100, default, 2f);
                            Main.dust[rockDust].velocity *= 3f;
                            if (Main.rand.NextBool())
                            {
                                Main.dust[rockDust].scale = 0.5f;
                                Main.dust[rockDust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                            }
                        }
                        for (int j = 0; j < 10; j++)
                        {
                            int rockDust2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Stone, 0f, 0f, 100, default, 3f);
                            Main.dust[rockDust2].noGravity = true;
                            Main.dust[rockDust2].velocity *= 5f;
                            rockDust2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Iron, 0f, 0f, 100, default, 2f);
                            Main.dust[rockDust2].velocity *= 2f;
                        }

                        NPC.noTileCollide = true;
                        if (NPC.rotation == 0)
                            NPC.velocity.X = 12 * NPC.direction;
                        NPC.velocity.Y = -28.5f;
                        NPC.ai[0] = 1f;
                        NPC.ai[1] = 0;
                        NPC.damage = NPC.defDamage;
                        if (DownedBossSystem.downedProvidence && !BossRushEvent.BossRushActive)
                            NPC.damage = (int)(NPC.defDamage * 1.5);
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
        public override bool? CanFallThroughPlatforms() => NPC.ai[0] != 0 || ((NPC.alpha > 10) || (NPC.target >= 0 && Main.player[NPC.target].position.Y > NPC.position.Y + NPC.height));
        public override bool CheckActive() => false;

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage <= 0)
                return;

            target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 240);
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
                    int rockDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Iron, 0f, 0f, 100, default, 2f);
                    Main.dust[rockDust].velocity *= 3f;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[rockDust].scale = 0.5f;
                        Main.dust[rockDust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int j = 0; j < 30; j++)
                {
                    int rockDust2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Stone, 0f, 0f, 100, default, 3f);
                    Main.dust[rockDust2].noGravity = true;
                    Main.dust[rockDust2].velocity *= 5f;
                    rockDust2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Iron, 0f, 0f, 100, default, 2f);
                    Main.dust[rockDust2].velocity *= 2f;
                }

                if (!Main.dedServ)
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
                    int rockDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Iron, 0f, 0f, 100, default, 2f);
                    Main.dust[rockDust].velocity *= 3f;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[rockDust].scale = 0.5f;
                        Main.dust[rockDust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int j = 0; j < 2; j++)
                {
                    int rockDust2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Stone, 0f, 0f, 100, default, 3f);
                    Main.dust[rockDust2].noGravity = true;
                    Main.dust[rockDust2].velocity *= 5f;
                    rockDust2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Iron, 0f, 0f, 100, default, 2f);
                    Main.dust[rockDust2].velocity *= 2f;
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
                modifiers.FinalDamage.Flat += item.pick - 1;
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
