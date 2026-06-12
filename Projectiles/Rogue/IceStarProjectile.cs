using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class IceStarProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/IceStar";

        private bool initStealth = false;
        private Vector2 initialVelocity;

        public override void SetStaticDefaults() => ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.penetrate = 3;
            Projectile.aiStyle = ProjAIStyleID.ThrownProjectile;
            AIType = ProjectileID.ThrowingKnife;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.coldDamage = true;
            Projectile.MaxUpdates = 2;
            Projectile.timeLeft = 60 * Projectile.MaxUpdates; //60 effective, 120 total
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30 * Projectile.MaxUpdates; //30 effective, 60 total
        }

        public override void AI()
        {
            if (!initStealth && Projectile.Calamity().stealthStrike)
            {
                Projectile.penetrate = -1;
                Projectile.tileCollide = false;
                Projectile.timeLeft = 90 * Projectile.MaxUpdates;
                initialVelocity = Projectile.velocity;
                initStealth = true;
            }

            //Projectile.rotation = Projectile.velocity.ToRotation();

            if (Main.rand.NextBool(3))
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.IceRod, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
            }
            CalamityUtils.HomeInOnNPC(Projectile, !Projectile.tileCollide, Projectile.Calamity().stealthStrike ? 800f : 400f, 14f, 20f);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item27, Projectile.position);
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.IceRod, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => OnHitEffects();

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => OnHitEffects();

        public void OnHitEffects()
        {
            if (initStealth && Projectile.owner == Main.myPlayer && Projectile.numHits < 1)
            {
                for (int i = 0; i < 8; i++)
                {
                    Vector2 velocity = (MathHelper.TwoPi * i / 8f).ToRotationVector2() * 4f;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<KelvinCatalystStar>(), (int)(Projectile.damage * 0.8), Projectile.knockBack, Projectile.owner);
                }
            }
        }
    }
}
