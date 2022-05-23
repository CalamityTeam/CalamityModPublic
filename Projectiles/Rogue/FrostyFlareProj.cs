using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class FrostyFlareProj : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/FrostyFlare";

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.coldDamage = true;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Frosty Flare");
        }

        public override void AI()
        {
            if (Projectile.owner != Main.myPlayer)
                return;

            bool shoot = false;
            if (Projectile.timeLeft % 30f == 0f)
            {
                if (Projectile.owner == Main.myPlayer)
                    shoot = true;
            }

            if (Projectile.ai[0] == 0f)
            {
                Projectile.velocity.X *= 0.99f;
                Projectile.velocity.Y += 0.25f;
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

                if (shoot)
                {
                    Vector2 vel = new Vector2(Main.rand.Next(-300, 301), Main.rand.Next(500, 801));
                    Vector2 pos = Projectile.Center - vel;
                    vel.X += Main.rand.Next(-50, 51);
                    vel.Normalize();
                    vel *= 30f;
                    int shard = Projectile.NewProjectile(Projectile.GetSource_FromThis(), pos, vel + Projectile.velocity / 4f, ModContent.ProjectileType<FrostShardFriendly>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    Main.projectile[shard].alpha = Projectile.alpha;
                }

                int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 172);
                Main.dust[index2].noGravity = true;
            }
            else
            {
                Projectile.ignoreWater = true;
                Projectile.tileCollide = false;
                int id = (int)Projectile.ai[1];
                if (id >= 0 && id < Main.maxNPCs && Main.npc[id].active && !Main.npc[id].dontTakeDamage)
                {
                    Projectile.Center = Main.npc[id].Center - Projectile.velocity * 2f;
                    Projectile.gfxOffY = Main.npc[id].gfxOffY;

                    if (shoot)
                    {
                        Vector2 vel = new Vector2(Main.rand.Next(-300, 301), Main.rand.Next(500, 801));
                        Vector2 pos = Main.npc[id].Center - vel;
                        vel.X += Main.rand.Next(-50, 51);
                        vel.Normalize();
                        vel *= 30f;
                        int shard = Projectile.NewProjectile(Projectile.GetSource_FromThis(), pos, vel + Main.npc[id].velocity, ModContent.ProjectileType<FrostShardFriendly>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                        Main.projectile[shard].alpha = Projectile.alpha;
                    }
                }
                else
                {
                    Projectile.Kill();
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 180);
            target.AddBuff(ModContent.BuffType<GlacialState>(), 30);
            target.immune[Projectile.owner] = 0;
            Projectile.ai[0] = 1f;
            Projectile.ai[1] = target.whoAmI;
            Projectile.velocity = target.Center - Projectile.Center;
            Projectile.velocity *= 0.75f;
            Projectile.netUpdate = true;

            const int maxFlares = 5;
            int flaresFound = 0;
            int oldestFlare = -1;
            int oldestFlareTimeLeft = 300;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].owner == Main.myPlayer && Main.projectile[i].type == Projectile.type && i != Projectile.whoAmI && Main.projectile[i].ai[1] == target.whoAmI)
                {
                    flaresFound++;
                    if (Main.projectile[i].timeLeft < oldestFlareTimeLeft)
                    {
                        oldestFlareTimeLeft = Main.projectile[i].timeLeft;
                        oldestFlare = Main.projectile[i].whoAmI;
                    }
                    if (flaresFound >= maxFlares)
                        break;
                }
            }
            if (flaresFound >= maxFlares && oldestFlare >= 0)
            {
                Main.projectile[oldestFlare].Kill();
            }
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 180);
            target.AddBuff(ModContent.BuffType<GlacialState>(), 30);
        }

        public override bool? CanDamage() => Projectile.ai[0] == 0f ? null : false;
    }
}
