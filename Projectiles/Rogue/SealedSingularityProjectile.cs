using System;
using CalamityMod.Dusts;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.NPCs;
using CalamityMod.NPCs.CeaselessVoid;
using CalamityMod.Particles;
using CalamityMod.Systems.Graphic.PixelationSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    [PierceResistException]
    public class SealedSingularityProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/SealedSingularity";

        //Trans flag colors with name containing "girlswag" or having a gender change pot in inventory at donor request 
        public static bool DonorColor => Main.LocalPlayer.name.Contains("girlswag") || Main.LocalPlayer.HasItemInAnyInventory(ItemID.GenderChangePotion);
        public static Color BorderColor => DonorColor ? Color.Lerp(Color.Pink, Color.LightBlue, (MathF.Sin(Main.GlobalTimeWrappedHourly * 3) + 1) * 0.5f) : new Color(59, 2, 120);

        public static Color RandomColor
        {
            get
            {
                if (DonorColor)
                {
                    switch (Main.rand.Next(3))
                    {
                        case 0: return Color.Pink;
                        case 1: return Color.LightBlue;
                        case 2: return Color.White;
                    }
                }

                switch (Main.rand.Next(4))
                {
                    case 0: return new Color(91, 47, 113);
                    case 1: return new Color(119, 74, 165);
                    case 2: return new Color(58, 58, 58);
                    case 3: return new Color(123, 115, 142);
                }
                return Color.White;
            }
        }
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 3;
            ProjectileID.Sets.TrailingMode[Type] = 0;
            OutlineTexVoid = null; //this ensures the outline tex is gotten properly
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
            Projectile.MaxUpdates = 2;
            Projectile.timeLeft = 300;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        int targetID = -1;

        NPC target => Main.npc[targetID];
        ref float Timer => ref Projectile.ai[0];
        ref float TimerMax => ref Projectile.ai[1];
        ref float AIState => ref Projectile.ai[2];
        bool Stealth => Projectile.Calamity().stealthStrike;

        int bounceCooldown = 0;
        public override void AI()
        {
            if (AIState == 1)
                Lighting.AddLight(Projectile.Center, BorderColor.ToVector3() * (Stealth ? 4 : 2) * Timer / TimerMax);
            else if (AIState == 2)
                Lighting.AddLight(Projectile.Center, BorderColor.ToVector3() * (Stealth ? 4 : 2));
            Timer++;
            bounceCooldown--;

            if (AIState == 0)
            {
                Projectile.rotation += 0.175f * Projectile.direction;
                if (Projectile.timeLeft < 280)
                    Projectile.velocity.Y += 0.22f;

                if (TimerMax - Timer < 30)
                    Projectile.velocity *= 0.925f;
            }
            if (AIState == 1)
            {
                if (Timer % 30 == 0) //reset hit immunity every 30 frames & reset the pierce falloff too
                {
                    Projectile.ResetLocalNPCHitImmunity();
                    Projectile.numHits = 0;
                }
                Projectile.timeLeft++;
                Projectile.rotation += 0.175f * Projectile.direction;
                Projectile.velocity = new Vector2(
                    MathF.Sin(Timer),
                    MathF.Sin(Timer * 0.7f)
                    );
                if (Stealth)
                {
                    Projectile.velocity *= 1 + Math.Clamp(Timer / TimerMax, 0f, 1f) * 2f;
                }
                if (Timer % 10 == 0 && Main.myPlayer == Projectile.owner && Timer < TimerMax - (Stealth ? 120 : 90))
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + new Vector2(Stealth ? 380 : 255, 0).RotatedByRandom(MathHelper.TwoPi), Vector2.Zero, ModContent.ProjectileType<SealedSingularityRock>(), 0, 0, Projectile.owner, Projectile.whoAmI);
                    var v = new Vector2(Stealth ? 380 : 255, 0).RotatedByRandom(MathHelper.TwoPi);
                    Dust.NewDustPerfect(Projectile.Center + v, DustID.Clentaminator_Purple, -v / 100f);
                }
                if (TimerMax - Timer == 330)
                {
                    SoundEngine.PlaySound(CeaselessVoid.BuildupSound with { pitch = 0.75f, MaxInstances = 10, Volume = 0.2f }, Projectile.Center);
                }

            }

            if (AIState == 2)
            {
                if (targetID == -1 || !target.active || !target.CanBeChasedBy())
                {
                    Timer = TimerMax;
                }
                else
                {
                    Projectile.velocity = Projectile.DirectionTo(target.Center) * MathHelper.Clamp(MathF.Pow((Timer - 30) / 8f, 2), 0f, 32f) * (Timer - 30).DirectionalSign();
                }
            }

            if (Timer > TimerMax && AIState == 0)
            {
                var sizee = Stealth ? 900 : 600;
                Projectile.tileCollide = false;
                Timer = 0;
                TimerMax = Stealth ? 600 : 300;
                Projectile.timeLeft += 180;
                AIState = 1;
            }
            if (Timer > TimerMax && AIState == 1)
            {
                AIState = 2;
                Timer = 0;
                TimerMax = 300;
                Projectile.ResetLocalNPCHitImmunity();
                Projectile.numHits = 0;
                if (Main.myPlayer == Projectile.owner)
                    for (int index = 0; index < 3; ++index)
                    {
                        float SpeedX = -Projectile.velocity.X * Main.rand.Next(40, 70) * 0.01f + Main.rand.Next(-20, 21) * 0.4f;
                        float SpeedY = -Projectile.velocity.Y * Main.rand.Next(40, 70) * 0.01f + Main.rand.Next(-20, 21) * 0.4f;
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + SpeedX, Projectile.Center.Y + SpeedY, SpeedX, SpeedY, ModContent.ProjectileType<SealedSingularityGore>(), 20, 0f, Projectile.owner, index, 0f);
                    }
                Projectile.velocity = Vector2.Zero;
                SoundEngine.PlaySound(CeaselessVoid.DeathSound with { pitch = 1f, Volume = Stealth ? 0.5f : 0.2f }, Projectile.Center);
            }

            if (Timer >= TimerMax && AIState == 2)
            {
                Projectile.Resize(300, 300);
                Projectile.numHits = 0;
                Projectile.ResetLocalNPCHitImmunity();
                Projectile.Damage();

                for (var i = 0; i < (Stealth ? 80 : 40); i++)
                {
                    var d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<LemonNadeExplodeDust>(), Main.rand.NextVector2CircularEdge(15, 15) * Main.rand.NextFloat(0.25f, Stealth ? 1.5f : 1f), Scale: Main.rand.NextFloat(0.5f, 1.5f), newColor: RandomColor);
                }
                var ringTimer = Stealth ? 50 : 35;
                GeneralParticleHandler.SpawnParticle(new CustomPulse(Projectile.Center, Vector2.Zero, RandomColor, "CalamityMod/Particles/BloomRing", new Vector2(0.1f, 0.85f), MathHelper.PiOver2, -4f, 4f, ringTimer));
                if (Stealth)
                {
                    GeneralParticleHandler.SpawnParticle(new CustomPulse(Projectile.Center, Vector2.Zero, RandomColor, "CalamityMod/Particles/BloomRing", new Vector2(0.2f, 0.7f), MathHelper.PiOver4, -4f, 4f, ringTimer - 15));
                    GeneralParticleHandler.SpawnParticle(new CustomPulse(Projectile.Center, Vector2.Zero, RandomColor, "CalamityMod/Particles/BloomRing", new Vector2(0.2f, 0.7f), -MathHelper.PiOver4, -4f, 4f, ringTimer - 15));
                }

                SoundEngine.PlaySound(SoundID.Item62 with { pitch = 1f }, Projectile.Center);
                SoundEngine.PlaySound(SoundID.Item111 with { pitch = 0.5f }, Projectile.Center);

                Projectile.Kill();
            }
        }

        private static Texture2D OutlineTex
        {
            get
            {
                if (field == null)
                {
                    var texture = TextureAssets.Projectile[ModContent.ProjectileType<SealedSingularityProjectile>()].Value;
                    field = new Texture2D(Main.graphics.GraphicsDevice, texture.Width, texture.Height);

                    var BaseArray = new Color[field.Width * field.Height];
                    var ColorArray = new Color[field.Width * field.Height];
                    texture.GetData(BaseArray);
                    for (var i = 0; i < BaseArray.Length; i++)
                    {
                        ColorArray[i] = new Color(255, 255, 255) * (((float)BaseArray[i].A) / 255f);
                    }
                    field.SetData(ColorArray);
                }
                return field;
            }
            set;
        }

        private static Texture2D OutlineTexVoid
        {
            get
            {
                if (field == null)
                {
                    var texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Rogue/SealedSingularityBlackhole", AssetRequestMode.ImmediateLoad).Value;
                    field = new Texture2D(Main.graphics.GraphicsDevice, texture.Width, texture.Height);

                    var BaseArray = new Color[field.Width * field.Height];
                    var ColorArray = new Color[field.Width * field.Height];
                    texture.GetData(BaseArray);
                    for (var i = 0; i < BaseArray.Length; i++)
                    {
                        ColorArray[i] = new Color(255, 255, 255) * (((float)BaseArray[i].A) / 255f);
                    }
                    field.SetData(ColorArray);
                }
                return field;
            }
            set;
        }
        private static Asset<Texture2D> VoidTex => field ??= ModContent.Request<Texture2D>("CalamityMod/Projectiles/Rogue/SealedSingularityBlackhole");
        public override bool PreDraw(ref Color lightColor)
        {
            if (AIState == 1)
            {
                for (float i = 0; i < MathHelper.TwoPi; i += MathHelper.PiOver2)
                {
                    var pos = Projectile.Center - Main.screenPosition;
                    float rot = Projectile.rotation;
                    var frame = TextureAssets.Projectile[Type].Frame(1, 1, 0, 0);
                    float scale = 1f;
                    float borderOp = MathF.Pow(Projectile.Opacity, 2);
                    Main.spriteBatch.Draw(OutlineTex, pos + new Vector2(6 * Timer / TimerMax, 0).RotatedBy(i), frame, BorderColor * borderOp, rot, frame.Size() * 0.5f, scale, SpriteEffects.None, 0);
                }
                PixelationManager.AddPixelatedDrawer((matrix) => DrawAuraOutside(this, matrix), Enums.GeneralDrawLayer.AfterNPCs);
            }
            if (AIState == 2)
            {
                var frame = VoidTex.Frame(1, 7, 0, (int)(Timer * 0.2f % 7));
                for (float i = 0; i < MathHelper.TwoPi; i += MathHelper.PiOver2)
                {
                    Main.EntitySpriteDraw(OutlineTexVoid, Projectile.Center - Main.screenPosition + new Vector2(2, 0).RotatedBy(i), frame, BorderColor, 0, frame.Size() * 0.5f, Projectile.scale, 0);
                }
                Main.EntitySpriteDraw(VoidTex.Value, Projectile.Center - Main.screenPosition, frame, Color.White, 0, frame.Size() * 0.5f, Projectile.scale, 0);
                return false;
            }
            Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, TextureAssets.Projectile[Type].Size() * 0.5f, Projectile.scale, 0);
            return false;
        }
        private static void DrawAuraOutside(SealedSingularityProjectile mproj, Matrix matrix)
        {

            Vector2 drawPosition = mproj.Projectile.Center - Main.screenPosition;

            //Draw the outer particles
            Main.spriteBatch.EnterShaderRegion(matrix: matrix);
            GameShaders.Misc["CalamityMod:OtherworldBarrierDistortion"].UseOpacity((mproj.Stealth ? 4 : 2) * (mproj.Timer / mproj.TimerMax));
            GameShaders.Misc["CalamityMod:OtherworldBarrierDistortion"].UseSaturation(0.1f);
            GameShaders.Misc["CalamityMod:OtherworldBarrierDistortion"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/Neurons"), 1);
            GameShaders.Misc["CalamityMod:OtherworldBarrierDistortion"].Apply();
            Texture2D telegraphBase = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomRing").Value;
            Main.EntitySpriteDraw(telegraphBase, drawPosition, null, Color.White * mproj.Projectile.Opacity * 0.25f * Math.Clamp(mproj.Timer / 15f, 0f, 1f), mproj.Projectile.whoAmI, telegraphBase.Size() / 2f, (mproj.Stealth ? 900f : 600f) * mproj.Projectile.Opacity / telegraphBase.Width * Math.Clamp(1 - (mproj.Timer - mproj.TimerMax + 15) / 15f, 0f, 1f), 0, 0);
            Main.EntitySpriteDraw(telegraphBase, drawPosition, null, BorderColor * mproj.Projectile.Opacity * 0.5f * Math.Clamp(mproj.Timer / 15f, 0f, 1f), mproj.Projectile.whoAmI, telegraphBase.Size() / 2f, (mproj.Stealth ? 900f : 600f) * mproj.Projectile.Opacity / telegraphBase.Width * Math.Clamp(1 - (mproj.Timer - mproj.TimerMax + 15) / 15f, 0f, 1f), 0, 0);
            Main.spriteBatch.ExitShaderRegion(matrix: matrix);
        }

        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            modifiers.SourceDamage /= Main.masterMode ? 2f : Main.expertMode ? 1.5f : 1;
            modifiers.SourceDamage *= 0.044f;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

            switch (AIState)
            {
                case 0:
                    {
                        if (bounceCooldown <= 0)
                        {
                            Projectile.velocity *= -1f;
                            bounceCooldown = 5;
                        }
                        if (TimerMax - Timer > 30)
                            Timer = TimerMax - 30;
                        goto case 1;
                    }
                case 1:
                    {
                        if (target.Calamity().IsArmored())
                            return;
                        if (targetID == -1 || !this.target.active || !this.target.CanBeChasedBy())
                        {
                            targetID = target.whoAmI;
                        }
                        else
                        {
                            if (this.target.life < target.life)
                            {
                                targetID = target.whoAmI;
                            }
                        }
                        if (AIState == 1)
                        {
                            target.MoveNPC(target.DirectionTo(Projectile.Center), Stealth ? 18 : 4, true);
                        }
                        return;
                    }
                case 2:
                    {
                        if (targetID == target.whoAmI)
                            Timer = TimerMax;
                        return;
                    }
            }
            return;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            switch (AIState)
            {
                case 1:
                    modifiers.SourceDamage /= Stealth ? 15 : 25;
                    modifiers.SourceDamage *= MathF.Pow(1 - SealedSingularity.FalloffPerTargetHitByAura, Projectile.numHits);
                    return;
                case 2:
                    modifiers.SourceDamage *= Stealth ? 5 : 2;
                    modifiers.SourceDamage *= MathF.Pow(1 - SealedSingularity.FallofPerTargetHitByBomb, Projectile.numHits);
                    return;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (AIState == 1)
            {
                return CalamityUtils.CircularHitboxCollision(projHitbox.Center(), (Stealth ? 900 : 600) * 0.4f, targetHitbox);
            }
            return base.Colliding(projHitbox, targetHitbox);
        }

        // Make it bounce on tiles.
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.width < 100)
            {
                SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
                Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            }

            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.velocity.X = -oldVelocity.X;
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.velocity.Y = -oldVelocity.Y;
            }
            Projectile.velocity *= 0.75f;
            return false;
        }
    }

}
