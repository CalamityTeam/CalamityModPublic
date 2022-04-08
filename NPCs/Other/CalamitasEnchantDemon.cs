using CalamityMod.Projectiles.Healing;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Other
{
    public class CalamitasEnchantDemon : ModNPC
    {
        public Player Target => Main.player[NPC.target];
        public ref float AttackTimer => ref NPC.ai[0];
        public ref float FadeAwayTimer => ref NPC.ai[1];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Demon");
            Main.npcFrameCount[NPC.type] = 5;
        }

        public override void SetDefaults()
        {
            NPC.width = 68;
            NPC.height = 68;
            NPC.damage = 270;
            NPC.defense = 0;

            // This is meant to be created after SCal is defeated and should be at least a little challenging.
            NPC.lifeMax = 101010;
            NPC.HitSound = SoundID.NPCHit47;
            NPC.DeathSound = SoundID.NPCDeath18;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.value = 0f;
            NPC.knockBackResist = 0f;
            NPC.netAlways = true;
            NPC.aiStyle = 0;
            NPC.Calamity().DoesNotDisappearInBossRush = true;
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToWater = true;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale) => NPC.lifeMax = 101010;

        public override void AI()
        {
            FadeAwayTimer++;

            // This npc should not attempt to locate other targets. The only one it will accept is the
            // player who summoned it in the first place.
            if (!Main.projectile.IndexInRange(NPC.target) || Target.dead || !Target.active)
            {
                FadeAway();
                return;
            }

            if (FadeAwayTimer < 15f)
            {
                DoInitializationAI();
                return;
            }

            AttackTarget();
        }

        public void FadeAway()
        {
            NPC.dontTakeDamage = true;
            NPC.Opacity = Utils.GetLerpValue(50f, 0f, FadeAwayTimer, true);

            // Become inactive after fading away completely.
            if (NPC.Opacity <= 0f)
            {
                NPC.active = false;
                NPC.netUpdate = true;
            }

            // Slow down and fade away.
            NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.UnitY * NPC.Opacity * -2f, 0.125f);

            FadeAwayTimer++;

            if (Main.dedServ)
                return;

            // Create some fade dust.
            for (int i = 0; i < 3; i++)
            {
                Dust magic = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, 223);
                magic.velocity = -Vector2.UnitY * Main.rand.NextFloat(2.8f, 3.5f);
                magic.scale = Main.rand.NextFloat(1f, 1.125f);
                magic.fadeIn = 0.4f;
                magic.noGravity = true;
                magic.noLight = true;
            }
        }

        public void DoInitializationAI()
        {
            // Create a burst of magic on the first frame.
            if (!Main.dedServ && FadeAwayTimer == 1f)
            {
                for (int i = 0; i < 50; i++)
                {
                    Dust magic = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, 223);
                    magic.velocity = -Vector2.UnitY * Main.rand.NextFloat(2.8f, 3.5f);
                    magic.scale = Main.rand.NextFloat(1f, 1.125f);
                    magic.fadeIn = 0.7f;
                    magic.noGravity = true;
                    magic.noLight = true;
                }
            }

            // Rise upward.
            NPC.velocity = Vector2.UnitX * MathHelper.Lerp(-0.1f, -3.5f, FadeAwayTimer / 15f);
        }

        public void AttackTarget()
        {
            // Slow down prior to a charge.
            if (AttackTimer % 120f > 90f)
                NPC.velocity *= 0.94f;

            // Otherwise attempt to move towards the player.
            else if (!NPC.WithinRange(Target.Center, 150f))
            {
                Vector2 idealVelocity = NPC.SafeDirectionTo(Target.Center) * 16f;
                NPC.velocity = (NPC.velocity * 29f + idealVelocity) / 30f;
                NPC.velocity = Vector2.Lerp(NPC.velocity, idealVelocity, 0.025f);
            }

            // Charge.
            if (AttackTimer % 120f == 119f)
                NPC.velocity = NPC.SafeDirectionTo(Target.Center) * 23.5f;

            NPC.spriteDirection = (NPC.velocity.X < 0f).ToDirectionInt();
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter % 5f == 4f)
                NPC.frame.Y += frameHeight;

            if (NPC.frame.Y >= frameHeight * Main.npcFrameCount[NPC.type])
                NPC.frame.Y = 0;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
                Utils.PoofOfSmoke(NPC.Center);
        }

        public override void OnKill()
        {
            // Release 4 healing projectiles towards the target.
            for (int i = 0; i < 4; i++)
            {
                Vector2 shootVelocity = NPC.SafeDirectionTo(Target.Center).RotatedByRandom(0.6f) * Main.rand.NextFloat(12f, 14f);
                Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, shootVelocity, ModContent.ProjectileType<DemonHeal>(), 0, 0f, NPC.target);
            }
        }
    }
}
