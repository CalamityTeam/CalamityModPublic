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
    public class RadiantExplosion : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private bool updatedTime = false;

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
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 35; // Hits 3 times when stealth
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
                    int dusty = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, ModContent.DustType<AstralBlue>(), 0f, 0f, 100, default, 1.5f);
                    Main.dust[dusty].noGravity = true;
                    Main.dust[dusty].velocity *= 0f;
                }
                for (int i = 0; i < 5; i++)
                {
                    int dusty = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, default, 1.5f);
                    Main.dust[dusty].noGravity = true;
                    Main.dust[dusty].velocity *= 0f;
                }
            }

            if (Projectile.Calamity().stealthStrike)
            {
                float projX = Projectile.Center.X;
                float projY = Projectile.Center.Y;
                for (int j = 0; j < Main.maxNPCs; j++)
                {
                    if (Main.npc[j].CanBeChasedBy(Projectile, false) && Collision.CanHit(Projectile.Center, 1, 1, Main.npc[j].Center, 1, 1) && !CalamityPlayer.areThereAnyDamnBosses)
                    {
                        float npcCenterX = Main.npc[j].position.X + (float)(Main.npc[j].width / 2);
                        float npcCenterY = Main.npc[j].position.Y + (float)(Main.npc[j].height / 2);
                        float targetDist = Math.Abs(Projectile.position.X + (float)(Projectile.width / 2) - npcCenterX) + Math.Abs(Projectile.position.Y + (float)(Projectile.height / 2) - npcCenterY);
                        if (targetDist < 600f)
                        {
                            if (Main.npc[j].position.X < projX)
                            {
                                Main.npc[j].velocity.X += 0.25f;
                            }
                            else
                            {
                                Main.npc[j].velocity.X -= 0.25f;
                            }
                            if (Main.npc[j].position.Y < projY)
                            {
                                Main.npc[j].velocity.Y += 0.25f;
                            }
                            else
                            {
                                Main.npc[j].velocity.Y -= 0.25f;
                            }
                        }
                    }
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 120);
            OnHitEffect(target.Center);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
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
                    ProjectileID.StarCloakStar,
                    ProjectileID.StarCannonStar
                });
                Projectile star = CalamityUtils.ProjectileRain(source, targetPos, 400f, 100f, 500f, 800f, 25f, projType, (int)(Projectile.damage * 0.75), Projectile.knockBack * 0.75f, Projectile.owner);
                if (star.whoAmI.WithinBounds(Main.maxProjectiles))
                {
                    star.DamageType = RogueDamageClass.Instance;
                    star.ai[0] = 2f;
                }
            }
        }
    }
}
