using CalamityMod.Projectiles.Healing;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Other
{
    public class CalamitasEnchantDemon : ModNPC
    {
        public Player Target => Main.player[npc.target];
        public ref float AttackTimer => ref npc.ai[0];
        public ref float FadeAwayTimer => ref npc.ai[1];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Demon");
            Main.npcFrameCount[npc.type] = 5;
        }

        public override void SetDefaults()
        {
            npc.width = 68;
            npc.height = 68;
            npc.damage = 270;
            npc.defense = 0;

            // This is meant to be created after SCal is defeated and should be at least a little challenging.
            npc.lifeMax = 101010;
            npc.HitSound = SoundID.NPCHit47;
            npc.DeathSound = SoundID.NPCDeath18;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.value = 0f;
            npc.knockBackResist = 0f;
            npc.netAlways = true;
            npc.aiStyle = 0;
            npc.Calamity().DoesNotDisappearInBossRush = true;
            npc.Calamity().VulnerableToHeat = false;
            npc.Calamity().VulnerableToCold = true;
            npc.Calamity().VulnerableToWater = true;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale) => npc.lifeMax = 101010;

        public override void AI()
        {
            FadeAwayTimer++;

            // This npc should not attempt to locate other targets. The only one it will accept is the
            // player who summoned it in the first place.
            if (!Main.projectile.IndexInRange(npc.target) || Target.dead || !Target.active)
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
            npc.dontTakeDamage = true;
            npc.Opacity = Utils.InverseLerp(50f, 0f, FadeAwayTimer, true);

            // Become inactive after fading away completely.
            if (npc.Opacity <= 0f)
            {
                npc.active = false;
                npc.netUpdate = true;
            }

            // Slow down and fade away.
            npc.velocity = Vector2.Lerp(npc.velocity, Vector2.UnitY * npc.Opacity * -2f, 0.125f);

            FadeAwayTimer++;

            if (Main.dedServ)
                return;

            // Create some fade dust.
            for (int i = 0; i < 3; i++)
            {
                Dust magic = Dust.NewDustDirect(npc.position, npc.width, npc.height, 223);
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
                    Dust magic = Dust.NewDustDirect(npc.position, npc.width, npc.height, 223);
                    magic.velocity = -Vector2.UnitY * Main.rand.NextFloat(2.8f, 3.5f);
                    magic.scale = Main.rand.NextFloat(1f, 1.125f);
                    magic.fadeIn = 0.7f;
                    magic.noGravity = true;
                    magic.noLight = true;
                }
            }

            // Rise upward.
            npc.velocity = Vector2.UnitX * MathHelper.Lerp(-0.1f, -3.5f, FadeAwayTimer / 15f);
        }

        public void AttackTarget()
        {
            // Slow down prior to a charge.
            if (AttackTimer % 120f > 90f)
                npc.velocity *= 0.94f;

            // Otherwise attempt to move towards the player.
            else if (!npc.WithinRange(Target.Center, 150f))
            {
                Vector2 idealVelocity = npc.SafeDirectionTo(Target.Center) * 16f;
                npc.velocity = (npc.velocity * 29f + idealVelocity) / 30f;
                npc.velocity = Vector2.Lerp(npc.velocity, idealVelocity, 0.025f);
            }

            // Charge.
            if (AttackTimer % 120f == 119f)
                npc.velocity = npc.SafeDirectionTo(Target.Center) * 23.5f;

            npc.spriteDirection = (npc.velocity.X < 0f).ToDirectionInt();
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;
            if (npc.frameCounter % 5f == 4f)
                npc.frame.Y += frameHeight;

            if (npc.frame.Y >= frameHeight * Main.npcFrameCount[npc.type])
                npc.frame.Y = 0;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
                Utils.PoofOfSmoke(npc.Center);
        }

        public override void NPCLoot()
        {
            // Release 4 healing projectiles towards the target.
            for (int i = 0; i < 4; i++)
            {
                Vector2 shootVelocity = npc.SafeDirectionTo(Target.Center).RotatedByRandom(0.6f) * Main.rand.NextFloat(12f, 14f);
                Projectile.NewProjectile(npc.Center, shootVelocity, ModContent.ProjectileType<DemonHeal>(), 0, 0f, npc.target);
            }
        }
    }
}
