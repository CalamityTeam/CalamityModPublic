using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class UmbraphileBoom : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";

        public const int Lifetime = 35; // 7 animation frames, 12 FPS

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 7;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 58;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = Lifetime;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Projectile.frameCounter++;
            Projectile.frame = Projectile.frameCounter / 5;

            if (Projectile.frameCounter > Lifetime)
                Projectile.Kill();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.dayTime || Main.rand.NextBool(3)) //100% during day, 33.33% chance at night
                target.AddBuff(BuffID.Daybreak, 30);

            if (!Main.dayTime || Main.rand.NextBool(3)) //100% at night, 33.33% chance during day
                target.AddBuff(ModContent.BuffType<Nightwither>(), 30);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (Main.dayTime || Main.rand.NextBool(3)) //100% during day, 33.33% chance at night
                target.AddBuff(BuffID.Daybreak, 30);

            if (!Main.dayTime || Main.rand.NextBool(3)) //100% at night, 33.33% chance during day
                target.AddBuff(ModContent.BuffType<Nightwither>(), 30);
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D glow = ModContent.Request<Texture2D>(Texture + "Glow").Value;
            Rectangle frame = glow.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, frame, Color.White, Projectile.rotation, frame.Size() * 0.5f, Projectile.scale, SpriteEffects.None);
        }
    }
}
