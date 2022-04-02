using CalamityMod.NPCs;
using CalamityMod.NPCs.NormalNPCs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class SealedSingularityBlackhole : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blackhole");
            Main.projFrames[projectile.type] = 7;
        }

        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 40;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.Calamity().rogue = true;
            projectile.tileCollide = false;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 10;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor);
            return false;
        }

        public override void AI()
        {
            // Update animation
            if (projectile.timeLeft % 5 == 0)
            {
                projectile.frame++;
            }
            if (projectile.frame >= Main.projFrames[projectile.type])
            {
                projectile.frame = 0;
            }

            //Succcc
            float projCenX = projectile.Center.X;
            float projCenY = projectile.Center.Y;
            float maxDistance = projectile.Calamity().stealthStrike ? 1000f : 500f;
            float succPower = projectile.Calamity().stealthStrike ? 0.25f : 0.1f;
            for (int index = 0; index < Main.maxNPCs; index++)
            {
                NPC npc = Main.npc[index];
                if (npc.CanBeChasedBy(projectile, false) && Collision.CanHit(projectile.Center, 1, 1, npc.Center, 1, 1))
                {
                    if (CalamityGlobalNPC.ShouldAffectNPC(npc) || npc.type == ModContent.NPCType<SuperDummyNPC>())
                    {
                        float extraDistance = (npc.width / 2) + (npc.height / 2);
                        if (Vector2.Distance(npc.Center, projectile.Center) < (maxDistance + extraDistance))
                        {
                            if (npc.position.X < projCenX)
                            {
                                npc.velocity.X += succPower;
                            }
                            else
                            {
                                npc.velocity.X -= succPower;
                            }
                            if (npc.position.Y < projCenY)
                            {
                                npc.velocity.Y += succPower;
                            }
                            else
                            {
                                npc.velocity.Y -= succPower;
                            }
                        }
                    }
                }
            }

            projectile.ai[0]++;
            if (projectile.ai[0] > 300f)
            {
                projectile.scale *= 0.95f;
                projectile.Opacity *= 0.95f;
                projectile.height = (int)(projectile.height * projectile.scale);
                projectile.width = (int)(projectile.width * projectile.scale);
            }
            if (projectile.scale <= 0.05f)
            {
                projectile.Kill();
            }
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Blackout, 300);
        }
    }
}
