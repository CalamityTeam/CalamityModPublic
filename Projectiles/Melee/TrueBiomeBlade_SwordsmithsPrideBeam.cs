using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using static CalamityMod.CalamityUtils;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Buffs.StatDebuffs;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class SwordsmithsPrideBeam : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public NPC target;
        public Player Owner => Main.player[Projectile.owner];
        public override string Texture => "CalamityMod/Projectiles/Melee/TrueBiomeBlade_SwordsmithsPrideBeam";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.aiStyle = ProjAIStyleID.Beam;
            AIType = ProjectileID.LightBeam;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 80;
            Projectile.extraUpdates = 1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {

            if (target == null)
            {
                foreach (Projectile proj in Main.projectile)
                {
                    if (proj.active && proj.type == ProjectileType<PurityProjectionSigil>() && proj.owner == Owner.whoAmI)
                    {
                        target = Main.npc[(int)proj.ai[0]];
                        break;
                    }
                }
            }

            else if ((Projectile.Center - target.Center).Length() >= (Projectile.Center + Projectile.velocity - target.Center).Length() && CalamityUtils.AngleBetween(Projectile.velocity, target.Center - Projectile.Center) < MathHelper.PiOver4) //Home in
            {
                Projectile.timeLeft = 70; //Remain alive
                float angularTurnSpeed = MathHelper.ToRadians(MathHelper.Lerp(15, 2.5f, MathHelper.Clamp(Projectile.Distance(target.Center) / 10f, 0f, 1f)));
                float idealDirection = Projectile.AngleTo(target.Center);
                float updatedDirection = Projectile.velocity.ToRotation().AngleTowards(idealDirection, angularTurnSpeed);
                Projectile.velocity = updatedDirection.ToRotationVector2() * Projectile.velocity.Length();
            }

            Lighting.AddLight(Projectile.Center, 0.75f, 1f, 0.24f);
            int dustParticle = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 75, 0f, 0f, 100, default, 0.9f);
            Main.dust[dustParticle].noGravity = true;
            Main.dust[dustParticle].velocity *= 0.5f;
            Main.dust[dustParticle].velocity += Projectile.velocity * 0.1f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft > 75)
                return false;

            DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);

            Main.spriteBatch.End(); //Haha sup babe what if i restarted the spritebatch way too many times haha /blushes
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;

            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Color.Lerp(Color.HotPink, Color.GreenYellow, (float)Math.Sin(Main.GlobalTimeWrappedHourly * 2f)), Projectile.rotation, tex.Size() * 0.5f, 1f, 0f, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item43, Projectile.Center);
            for (int i = 0; i <= 15; i++)
            {
                Vector2 displace = (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * (-0.5f + (i / 15f)) * 88f;
                int dustParticle = Dust.NewDust(Projectile.Center + displace, Projectile.width, Projectile.height, 75, 0f, 0f, 100, default, 2f);
                Main.dust[dustParticle].noGravity = true;
                Main.dust[dustParticle].velocity = Projectile.oldVelocity;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            int debuffTime = 90;
            target.AddBuff(BuffType<ArmorCrunch>(), debuffTime);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Owner.HeldItem.ModItem is OmegaBiomeBlade sword && Main.rand.NextFloat() <= OmegaBiomeBlade.WhirlwindAttunement_SwordBeamProc)
                sword.OnHitProc = true;
        }

        public override Color? GetAlpha(Color lightColor) => Color.Lerp(Color.HotPink, Color.GreenYellow, (float)Math.Sin(Main.GlobalTimeWrappedHourly * 2f));
    }
}
