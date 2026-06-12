using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Events;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.HiveMind
{
    public class DankCreeper : ModNPC
    {
        public override void SetStaticDefaults()
        {
            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }
        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.damage = 24; // 48
            NPC.width = 70;
            NPC.height = 70;
            NPC.defense = 6;

            NPC.lifeMax = 120;
            if (BossRushEvent.BossRushActive)
                NPC.lifeMax = 2000;
            if (Main.getGoodWorld)
            {
                NPC.lifeMax *= 3;
                NPC.reflectsProjectiles = true;
            }

            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0.3f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToSickness = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[ModContent.NPCType<HiveMind>()], quickUnlock: true);
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.UndergroundCorruption,
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.DankCreeper")
            });
        }

        public override void AI()
        {
            // Avoid cheap bullshit
            NPC.damage = 0;

            // Get a target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            bool death = CalamityWorld.death;
            bool revenge = CalamityWorld.revenge;
            float speed = death ? 15f : revenge ? 13f : 11f;

            if (NPC.ai[1] < 90f)
                NPC.ai[1] += 1f;

            speed = MathHelper.Lerp(3f, speed, NPC.ai[1] / 90f);

            NPC.rotation = NPC.velocity.X * 0.05f;

            Vector2 targetDirection = new Vector2(NPC.Center.X + (NPC.direction * 20), NPC.Center.Y + 6f);
            Vector2 targetLocation = Main.player[NPC.target].Center;

            // Fly above if below 25% HP and burst to spawn a Rain Cloud when high enough
            bool killYourself = NPC.life / (float)NPC.lifeMax < 0.25f;
            if (killYourself && (Main.expertMode || BossRushEvent.BossRushActive))
            {
                targetLocation -= Vector2.UnitY * 400f;
                if (NPC.Distance(targetLocation) < 80f)
                {
                    NPC.life = 0;
                    NPC.HitEffect();
                    NPC.checkDead();
                    return;
                }
            }

            if (!killYourself)
            {
                NPC.ai[0] -= 1f;
                bool dash = NPC.Distance(targetLocation) < 200f;
                if (dash || NPC.ai[0] > 0f)
                {
                    // Set damage
                    NPC.damage = NPC.defDamage;

                    if (dash)
                        NPC.ai[0] = 20f;

                    if (NPC.velocity.X < 0f)
                        NPC.direction = -1;
                    else
                        NPC.direction = 1;

                    return;
                }
            }

            float inertia = (NPC.Distance(targetLocation) < 300f || killYourself) ? 8f : NPC.Distance(targetLocation) < 400f ? 20f : 50f;
            Vector2 idealVelocity = (targetLocation - targetDirection).SafeNormalize(Vector2.UnitX * NPC.direction) * speed;
            NPC.velocity = (NPC.velocity * inertia + idealVelocity) / (inertia + 1f);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage < 0)
                return;

            target.AddBuff(ModContent.BuffType<BrainRot>(), 120);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Glass, hit.HitDirection, -1f, 0, default, 1f);

            if (NPC.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Glass, hit.HitDirection, -1f, 0, default, 1f);

                if (!Main.dedServ)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("DankCreeperGore").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("DankCreeperGore2").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("DankCreeperGore3").Type, 1f);
                }
            }
        }

        public override void OnKill()
        {
            int closestPlayer = Player.FindClosest(NPC.Center, 1, 1);
            if (Main.rand.NextBool(4) && Main.player[closestPlayer].statLife < Main.player[closestPlayer].statLifeMax2)
                Item.NewItem(NPC.GetSource_Loot(), (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ItemID.Heart);

            if ((Main.expertMode || BossRushEvent.BossRushActive) && Main.netMode != NetmodeID.MultiplayerClient)
            {
                int type = ModContent.ProjectileType<ShadeNimbusHostile>();
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, type, HiveMind.ShaderainDamage, 0f, Main.myPlayer);
            }
        }
    }
}
