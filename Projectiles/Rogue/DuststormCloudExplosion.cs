using CalamityMod.Dusts;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.NPCs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class DuststormCloudExplosion : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";

        public override string Texture => "CalamityMod/Projectiles/Rogue/DuststormCloud";
        public override void SetDefaults()
        {
            Projectile.width = 150;
            Projectile.height = 150;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 1;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 12;
        }

        Color color = Color.White;
        public override void AI()
        {
            if (color == Color.White)
            {
                color = DuststormCloud.RandomColor;
            }
            SoundEngine.PlaySound(SoundID.DD2_CrystalCartImpact);
            for (var i = 0; i < 80; i++) {
                var d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<LemonNadeExplodeDust>(), Main.rand.NextVector2CircularEdge(8, 8) * Main.rand.NextFloat(0.2f,1.2f), newColor: DuststormCloud.RandomColor, Scale: 0.75f);
                d.customData = 0.75f;
                }
        }

        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            modifiers.SourceDamage /= Main.masterMode ? 2f : Main.expertMode ? 1.5f : 1;
            modifiers.SourceDamage *= 0.1f;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, null, color * Projectile.Opacity, Projectile.rotation, TextureAssets.Projectile[Type].Size() * 0.5f, 0.08f * Projectile.scale, 0);
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, Projectile.width, targetHitbox);
    }
}
