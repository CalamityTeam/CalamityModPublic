using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Weapons.Rogue;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Rogue
{
    public class Honeycomb : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/HardenedHoneycomb";

        private const float radius = 15f;

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.aiStyle = ProjAIStyleID.ThrownProjectile;
            Projectile.timeLeft = 300;
            AIType = ProjectileID.ThrowingKnife;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override void AI()
        {
            Player player = Main.player[Main.myPlayer];
            Vector2 posDiff = player.Center - Projectile.Center;
            if (posDiff.Length() <= radius)
            {
                player.AddBuff(BuffID.Honey, 300);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SpawnFragments();
            Projectile.penetrate--;
            if (Projectile.penetrate <= 0)
            {
                Projectile.Kill();
            }
            else
            {
                if (Projectile.velocity.X != oldVelocity.X)
                {
                    Projectile.velocity.X = -oldVelocity.X;
                }
                if (Projectile.velocity.Y != oldVelocity.Y)
                {
                    Projectile.velocity.Y = -oldVelocity.Y;
                }
            }
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SpawnFragments();
            Projectile.velocity.X = -Projectile.velocity.X;
            Projectile.velocity.Y = -Projectile.velocity.Y;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            SpawnFragments();
            Projectile.velocity.X = -Projectile.velocity.X;
            Projectile.velocity.Y = -Projectile.velocity.Y;
        }

        public void SpawnFragments()
        {
            int split = 0;
            while (split < 2)
            {
                //Calculate the velocity of the projectile
                float shardspeedX = -Projectile.velocity.X * Main.rand.NextFloat(.5f, .7f) + Main.rand.NextFloat(-3f, 3f);
                float shardspeedY = -Projectile.velocity.Y * Main.rand.Next(50, 70) * 0.01f + Main.rand.Next(-8, 9) * 0.2f;
                //Prevents the projectile speed from being too low
                if (shardspeedX < 2f && shardspeedX > -2f)
                {
                    shardspeedX += -Projectile.velocity.X;
                }
                if (shardspeedY > 2f && shardspeedY < 2f)
                {
                    shardspeedY += -Projectile.velocity.Y;
                }

                //Spawn the projectile
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + shardspeedX, Projectile.position.Y + shardspeedY, shardspeedX, shardspeedY, ModContent.ProjectileType<HoneycombFragment>(), Projectile.damage, Projectile.knockBack, Projectile.owner, Main.rand.Next(3), 0f);
                split += 1;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath1, Projectile.position);
            //Dust on impact
            int dust_splash = 0;
            while (dust_splash < 9)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 9, -Projectile.velocity.X * 0.15f, -Projectile.velocity.Y * 0.15f, 159, default, 1.5f);
                dust_splash += 1;
            }
        }
    }
}
