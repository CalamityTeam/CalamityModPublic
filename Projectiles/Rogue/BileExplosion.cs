using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class BileExplosion : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 150;
            Projectile.height = 150;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 25;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.HitDirectionOverride = (Projectile.Center.X < target.Center.X).ToDirectionInt();
        }

        public override void AI()
        {
            Vector2 randVel = new Vector2(8, 8).RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.04f, (Main.rand.NextBool(3) ? 0.4f : 0.5f));
            Particle smoke = new HeavySmokeParticle(Projectile.Center + randVel, randVel, Color.GreenYellow, Main.rand.Next(25, 35 + 1), Main.rand.NextFloat(0.3f, 0.5f), 0.3f);
            GeneralParticleHandler.SpawnParticle(smoke);
            if (Main.rand.NextBool(2))
            {
                Color bubbleColor = Main.rand.NextBool() ? Color.DarkOliveGreen : Color.GreenYellow;
                Vector2 bubbleSpawnPos = Projectile.Center + Main.rand.NextVector2Circular(70, 70);
                Vector2 bubbleVelocity = Vector2.UnitY * Main.rand.NextFloat(-3.5f, -5f);
                Particle bubble = new DirectionalPulseRing(bubbleSpawnPos, bubbleVelocity, bubbleColor, new Vector2(0.6f, 0.8f), 0, 0.1f, 0f, 65);
                GeneralParticleHandler.SpawnParticle(bubble);
            }
            if (Projectile.Calamity().stealthStrike)
            {
                if (Main.rand.NextBool(4))
                {
                    Projectile bubble = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center + Main.rand.NextVector2Circular(70, 70), Vector2.UnitY * Main.rand.NextFloat(-3.5f, -5f), ModContent.ProjectileType<SulphuricAcidBubbleFriendly>(), (int)(Projectile.damage * 0.5), Projectile.knockBack, Projectile.owner,0,0,Projectile.Calamity().stealthStrike ? 1 : 0);
                    if (bubble.whoAmI.WithinBounds(Main.maxProjectiles))
                    {
                        bubble.DamageType = RogueDamageClass.Instance;
                        bubble.timeLeft = 45;
                    }
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(), 60);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<Irradiated>(), 60);

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, Projectile.width * 0.5f * Projectile.scale, targetHitbox);
    }
}
