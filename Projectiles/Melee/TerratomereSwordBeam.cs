using CalamityMod.Graphics.Primitives;
using CalamityMod.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class TerratomereSwordBeam : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public int TargetIndex = -1;
        
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = Terratomere.SlashLifetime + 1;
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = Terratomere.SlashLifetime;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            CalamityUtils.HomeInOnNPC(Projectile, true, 400f, 24f, 20f);
            Projectile.scale = Utils.GetLerpValue(0f, 8f, Projectile.timeLeft, true);

            Dust dust = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool(5) ? 131 : 294, -Projectile.velocity * Main.rand.NextFloat(0.05f, 0.3f));
            dust.noGravity = true;
            dust.scale = Main.rand.NextFloat(0.75f, 0.95f);
            if (dust.type == 131)
                dust.scale = Main.rand.NextFloat(0.55f, 0.75f);
            else
                dust.fadeIn = 0.5f;
        }

        public float SlashWidthFunction(float _) => Projectile.width * Projectile.scale * Utils.GetLerpValue(0f, 0.1f, _, true);

        public Color SlashColorFunction(float _) => Color.Turquoise;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            TargetIndex = target.whoAmI;
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<TerratomereExplosion>(), (int)(Projectile.damage * 0.5f), Projectile.knockBack, Projectile.owner);

            if (Projectile.timeLeft > 12)
                Projectile.timeLeft = 12;
            Projectile.velocity *= 0.2f;
            Projectile.damage = 0;
            Projectile.netUpdate = true;
        }

        public override void OnKill(int timeLeft)
        {
            if (Main.myPlayer == Projectile.owner && TargetIndex >= 0)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Main.npc[TargetIndex].Center, Vector2.Zero, ModContent.ProjectileType<TerratomereSlashCreator>(), Projectile.damage, Projectile.knockBack, Projectile.owner, TargetIndex, Main.rand.NextFloat(MathHelper.TwoPi));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            GameShaders.Misc["CalamityMod:ExobladePierce"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/BlobbyNoise"));
            GameShaders.Misc["CalamityMod:ExobladePierce"].UseImage2("Images/Extra_189");
            GameShaders.Misc["CalamityMod:ExobladePierce"].UseColor(Terratomere.TerraColor1);
            GameShaders.Misc["CalamityMod:ExobladePierce"].UseSecondaryColor(Terratomere.TerraColor2);

            // 17MAY2024: Ozzatron: remove Terratomere rendering its trails multiple times
            PrimitiveRenderer.RenderTrail(Projectile.oldPos, new(SlashWidthFunction, SlashColorFunction, (_) => Projectile.Size * 0.5f, shader: GameShaders.Misc["CalamityMod:ExobladePierce"]), 30);

            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Projectile.oldPos[0] == Vector2.Zero)
                return false;

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.oldPos[0] + Projectile.Size * 0.5f, Projectile.Center);
        }
    }
}
