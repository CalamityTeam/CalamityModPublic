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
            Main.projFrames[Projectile.type] = 7;
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.tileCollide = false;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor);
            return false;
        }

        public override void AI()
        {
            // Update animation
            if (Projectile.timeLeft % 5 == 0)
            {
                Projectile.frame++;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
            {
                Projectile.frame = 0;
            }

            //Succcc
            float projCenX = Projectile.Center.X;
            float projCenY = Projectile.Center.Y;
            float maxDistance = Projectile.Calamity().stealthStrike ? 1000f : 500f;
            float succPower = Projectile.Calamity().stealthStrike ? 0.25f : 0.1f;
            for (int index = 0; index < Main.maxNPCs; index++)
            {
                NPC npc = Main.npc[index];
                if (npc.CanBeChasedBy(Projectile, false) && Collision.CanHit(Projectile.Center, 1, 1, npc.Center, 1, 1))
                {
                    if (CalamityGlobalNPC.ShouldAffectNPC(npc) || npc.type == ModContent.NPCType<SuperDummyNPC>())
                    {
                        float extraDistance = (npc.width / 2) + (npc.height / 2);
                        if (Vector2.Distance(npc.Center, Projectile.Center) < (maxDistance + extraDistance))
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

            Projectile.ai[0]++;
            if (Projectile.ai[0] > 300f)
            {
                Projectile.scale *= 0.95f;
                Projectile.Opacity *= 0.95f;
                Projectile.height = (int)(Projectile.height * Projectile.scale);
                Projectile.width = (int)(Projectile.width * Projectile.scale);
            }
            if (Projectile.scale <= 0.05f)
            {
                Projectile.Kill();
            }
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Blackout, 300);
        }
    }
}
