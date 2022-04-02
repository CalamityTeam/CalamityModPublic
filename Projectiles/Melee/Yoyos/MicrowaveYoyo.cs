using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee.Yoyos
{
    public class MicrowaveYoyo : ModProjectile
    {
        private const float Radius = 100f;
        private SoundEffectInstance mmmmmm = null;
        private bool spawnedAura = false;
        public int soundCooldown = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Microwave");
            ProjectileID.Sets.YoyosLifeTimeMultiplier[projectile.type] = -1f;
            ProjectileID.Sets.YoyosMaximumRange[projectile.type] = 450f;
            ProjectileID.Sets.YoyosTopSpeed[projectile.type] = 14f;

            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.aiStyle = 99;
            projectile.width = 16;
            projectile.height = 16;
            projectile.scale = 1f;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.penetrate = -1;
            projectile.MaxUpdates = 2;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 20;
        }

        public override void SendExtraAI(BinaryWriter writer) => writer.Write(soundCooldown);
        public override void ReceiveExtraAI(BinaryReader reader) => soundCooldown = reader.ReadInt32();

        public override void AI()
        {
            // Sound is done manually, so that it can loop correctly.
            if (mmmmmm is null)
            {
                mmmmmm = ModContent.GetSound("CalamityMod/Sounds/Custom/MMMMMMMMMMMMM").CreateInstance();
                mmmmmm.IsLooped = true;
                CalamityUtils.ApplySoundStats(ref mmmmmm, projectile.Center);
                Main.PlaySoundInstance(mmmmmm);
            }
            else if (!mmmmmm.IsDisposed)
                CalamityUtils.ApplySoundStats(ref mmmmmm, projectile.Center);

            // Spawn invisible but damaging aura projectile
            if (projectile.owner == Main.myPlayer && !spawnedAura)
            {
                Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<MicrowaveAura>(), (int)(projectile.damage * 0.35), projectile.knockBack, projectile.owner, projectile.identity, 0f);
                spawnedAura = true;
            }

            // Decrement sound cooldown
            if (soundCooldown > 0 && projectile.FinalExtraUpdate())
                soundCooldown--;

            // Dust circle appears for all players, even though the aura projectile is only spawned by the owner
            int numDust = (int)(0.2f * MathHelper.TwoPi * Radius);
            float angleIncrement = MathHelper.TwoPi / (float)numDust;
            Vector2 dustOffset = new Vector2(Radius, 0f);
            dustOffset = dustOffset.RotatedByRandom(MathHelper.TwoPi);
            for (int i = 0; i < numDust; i++)
            {
                dustOffset = dustOffset.RotatedBy(angleIncrement);
                int dustType = Utils.SelectRandom(Main.rand, new int[]
                {
                        ModContent.DustType<AstralOrange>(),
                        ModContent.DustType<AstralBlue>()
                });
                int dust = Dust.NewDust(projectile.Center, 1, 1, dustType);
                Main.dust[dust].position = projectile.Center + dustOffset;
                Main.dust[dust].fadeIn = 1f;
                Main.dust[dust].velocity *= 0.2f;
                Main.dust[dust].scale = 0.1599999999f;
            }

            // Delete the projectile if it is farther than 200 blocks away from the player
            if ((projectile.position - Main.player[projectile.owner].position).Length() > 3200f)
                projectile.Kill();
        }

        public override void Kill(int timeLeft)
        {
            mmmmmm?.Stop();
            mmmmmm?.Dispose();
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Rectangle frame = new Rectangle(0, 0, 20, 16);
            spriteBatch.Draw(ModContent.GetTexture("CalamityMod/Projectiles/Melee/Yoyos/MicrowaveYoyoGlow"), projectile.Center - Main.screenPosition, frame, Color.White, projectile.rotation, projectile.Size / 2, 1f, SpriteEffects.None, 0f);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 240);
            if (target.life <= 0 && soundCooldown <= 0)
            {
                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/MicrowaveBeep"), (int)projectile.Center.X, (int)projectile.Center.Y);
                soundCooldown = 45;
            }
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 240);
            if (target.statLife <= 0 && soundCooldown <= 0)
            {
                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/MicrowaveBeep"), (int)projectile.Center.X, (int)projectile.Center.Y);
                soundCooldown = 45;
            }
        }
    }
}
