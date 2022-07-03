using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer;
using CalamityMod.Dusts;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class RadiantExplosion : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private bool updatedTime = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Explosion");
        }

        public override void SetDefaults()
        {
            Projectile.width = 150;
            Projectile.height = 150;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 10;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override void AI()
        {
            if (Projectile.ai[0] == 1f || (Projectile.Calamity().stealthStrike && !updatedTime))
            {
                Projectile.timeLeft = 100;
                Projectile.ai[0] = 0f;
                updatedTime = true;
            }

            if (Projectile.timeLeft >= (updatedTime ? 80 : 6))
            {
                for (int i = 0; i < 5; i++)
                {
                    int num469 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, ModContent.DustType<AstralBlue>(), 0f, 0f, 100, default, 1.5f);
                    Main.dust[num469].noGravity = true;
                    Main.dust[num469].velocity *= 0f;
                }
                for (int i = 0; i < 5; i++)
                {
                    int num469 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, default, 1.5f);
                    Main.dust[num469].noGravity = true;
                    Main.dust[num469].velocity *= 0f;
                }
            }

            if (Projectile.Calamity().stealthStrike)
            {
                float num472 = Projectile.Center.X;
                float num473 = Projectile.Center.Y;
                float num474 = 600f;
                for (int num475 = 0; num475 < Main.maxNPCs; num475++)
                {
                    if (Main.npc[num475].CanBeChasedBy(Projectile, false) && Collision.CanHit(Projectile.Center, 1, 1, Main.npc[num475].Center, 1, 1) && !CalamityPlayer.areThereAnyDamnBosses)
                    {
                        float npcCenterX = Main.npc[num475].position.X + (float)(Main.npc[num475].width / 2);
                        float npcCenterY = Main.npc[num475].position.Y + (float)(Main.npc[num475].height / 2);
                        float num478 = Math.Abs(Projectile.position.X + (float)(Projectile.width / 2) - npcCenterX) + Math.Abs(Projectile.position.Y + (float)(Projectile.height / 2) - npcCenterY);
                        if (num478 < num474)
                        {
                            if (Main.npc[num475].position.X < num472)
                            {
                                Main.npc[num475].velocity.X += 0.25f;
                            }
                            else
                            {
                                Main.npc[num475].velocity.X -= 0.25f;
                            }
                            if (Main.npc[num475].position.Y < num473)
                            {
                                Main.npc[num475].velocity.Y += 0.25f;
                            }
                            else
                            {
                                Main.npc[num475].velocity.Y -= 0.25f;
                            }
                        }
                    }
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 120);
            OnHitEffect(target.Center);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 120);
            OnHitEffect(target.Center);
        }

        private void OnHitEffect(Vector2 targetPos)
        {
            var source = Projectile.GetSource_FromThis();
            for (int n = 0; n < 3; n++)
            {
                int projType = Utils.SelectRandom(Main.rand, new int[]
                {
                    ModContent.ProjectileType<AstralStar>(),
                    ProjectileID.HallowStar,
                    ModContent.ProjectileType<FallenStarProj>()
                });
                Projectile star = CalamityUtils.ProjectileRain(source, targetPos, 400f, 100f, 500f, 800f, 25f, projType, (int)(Projectile.damage * 0.75), 5f, Projectile.owner);
                if (star.whoAmI.WithinBounds(Main.maxProjectiles))
                {
                    star.DamageType = RogueDamageClass.Instance;
                    star.ai[0] = 2f;
                }
            }
        }
    }
}
