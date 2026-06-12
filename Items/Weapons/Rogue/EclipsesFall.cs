using System;
using System.Collections.Generic;
using System.Linq;
using CalamityMod.DataStructures;
using CalamityMod.Graphics.Primitives;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using CalamityMod.Utilities.Daybreak;
using CalamityMod.Utilities.Daybreak.Buffers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;

#region Item
namespace CalamityMod.Items.Weapons.Rogue
{
    public class EclipsesFall : RogueWeapon
    {
        /// <summary>
        /// Base damage of the spear, dynamically updated to match rogue stats.
        /// This is reduced if the spear has less fragments than MaxFragmentCount
        /// </summary>
        public static int EclipseSpearBaseDmg => 1250;
        /// <summary>
        /// How much damage fragments and sparks should do
        /// </summary>
        public static float FragmentDmgMult => 0.33f;
        /// <summary>
        /// How many fragments are spawned per spear hit
        /// </summary>
        public static int FragmentCount => 2;
        /// <summary>
        /// How many fragments can exist before they start homing in.
        /// Also the required amount of fragments for 100% base dmg on Eclipse Spear
        /// </summary>
        public static int MaxFragmentCount => 20;

        public override void SetDefaults()
        {
            Item.width = 82;
            Item.height = 88;
            Item.damage = 1000;
            Item.knockBack = 3.5f;
            Item.useAnimation = Item.useTime = 24;
            Item.autoReuse = true;
            Item.DamageType = RogueDamageClass.Instance;
            Item.shootSpeed = 32f;
            Item.shoot = ModContent.ProjectileType<EclipsesFall_Javelin>();
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.UseSound = SoundID.Item1;
            Item.value = CalamityGlobalItem.RarityDarkBlueBuyPrice;
            Item.rare = ModContent.RarityType<BurnishedAuric>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int proj = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            if (proj.WithinBounds(Main.maxProjectiles))
                Main.projectile[proj].Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
            return false;
        }
        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI) => Item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.Request<Texture2D>(Texture + "Glow").Value);

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Vega>().
                AddIngredient<AuricBar>(5).
                AddIngredient<DarksunFragment>(15).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
#endregion

namespace CalamityMod.Projectiles.Rogue
{
    #region Main Javelin
    public class EclipsesFall_Javelin : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/EclipsesFall";
        private int SplitProjDamage => (int)(Projectile.damage * EclipsesFall.FragmentDmgMult);

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.MaxUpdates = 2;
            Projectile.penetrate = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 6 * Projectile.MaxUpdates;
            Projectile.timeLeft = 150 * Projectile.MaxUpdates;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            if (Main.rand.NextBool(5))
            {
                Vector2 trailPos = Projectile.Center + Vector2.UnitY.RotatedBy(Projectile.rotation) * Main.rand.NextFloat(-16f, 16f);
                float trailScale = Main.rand.NextFloat(0.8f, 1.2f);
                Color trailColor = Main.rand.NextBool() ? Color.Indigo : Color.DarkOrange;
                Particle eclipseTrail = new SparkParticle(trailPos, Projectile.velocity * 0.2f, false, 60, trailScale, trailColor);
                GeneralParticleHandler.SpawnParticle(eclipseTrail);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            int onHitCount = EclipsesFall.FragmentCount + 4;
            float spread = 20f;
            int projectileDamage = SplitProjDamage;
            float kb = 5f;
            int sparkID = ModContent.ProjectileType<EclipsesFall_LightSpark>();
            int starID = ModContent.ProjectileType<EclipsesFall_LightFragment>();
            for (int i = 0; i < onHitCount; i++)
            {
                int projID = i < EclipsesFall.FragmentCount ? starID : sparkID;
                Vector2 velocity = Projectile.oldVelocity.RotateRandom(MathHelper.ToRadians(spread)) * 0.5f;
                float speed = Main.rand.NextFloat(1.5f, 2f);
                float moveDuration = Main.rand.Next(5, 15);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity * speed, projID, projectileDamage, kb, Projectile.owner, 0f, moveDuration, 20);
            }

            SoundEngine.PlaySound(SoundID.Item62 with { Volume = SoundID.Item62.Volume * 0.6f }, Projectile.position);
            SoundEngine.PlaySound(SoundID.Item68 with { Volume = SoundID.Item68.Volume * 0.2f }, Projectile.position);
            SoundEngine.PlaySound(SoundID.Item122 with { Volume = SoundID.Item122.Volume * 0.4f }, Projectile.position);


            List<Projectile> frags = new();
            var p = -2;
            foreach (var item in Main.ActiveProjectiles)
            {

                if (item.type == ModContent.ProjectileType<EclipsesFall_StealthSpear>() && item.owner == Projectile.owner && item.Opacity > 0.1f)
                {
                    if (Projectile.Calamity().stealthStrike && item.timeLeft > 600 * item.MaxUpdates)
                    {
                        if (item.timeLeft < 1120 * item.MaxUpdates)
                        {
                            item.timeLeft = 60 * item.MaxUpdates;
                            item.ai[0] = target.whoAmI;
                            continue;
                        }
                        item.timeLeft = 1200 * item.MaxUpdates;
                        p = item.whoAmI;
                    }
                    item.ai[0] = target.whoAmI;
                }
                if (item.type == ModContent.ProjectileType<EclipsesFall_LightFragment>() && item.owner == Projectile.owner && item.ai[0] > -2)
                {
                    frags.Add(item);
                }
            }
            if (Projectile.Calamity().stealthStrike && p < 0)
                for (int i = 0; i < 1; i++)
                {
                    int projID = ModContent.ProjectileType<EclipsesFall_StealthSpear>();
                    Vector2 velocity = Vector2.Zero;
                    var proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity.SafeNormalize(default) * 64f, projID, Projectile.damage, kb, Projectile.owner, target.whoAmI, 0, Math.Min(20, frags.Count(x => x.ai[0] == 0)));
                    proj.rotation = Projectile.rotation;
                    p = proj.whoAmI;
                }


            frags = frags.OrderBy(x => x.timeLeft).ToList();
            int toRemove = frags.Count(x => x.ai[0] == 0) - EclipsesFall.MaxFragmentCount;
            foreach (var item in frags)
            {
                if (toRemove > 0 && item.ai[0] == 0)
                {
                    item.ai[0] = -2;
                    item.timeLeft = EclipsesFall_LightFragment.lifetime;
                    toRemove--;
                    continue;
                }
                if (Projectile.Calamity().stealthStrike)
                {
                    if (item.ai[0] == 0)
                    {
                        item.ai[1] = Main.rand.Next(5, 15);
                        item.ai[2] = 20;
                    }
                    item.ai[0] = p + 1;
                    item.timeLeft = EclipsesFall_LightFragment.lifetime;
                    item.damage = 0;
                    item.netUpdate = true;
                }
            }


        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() * 0.5f, Projectile.scale, SpriteEffects.None);
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D glow = ModContent.Request<Texture2D>(Texture + "Glow").Value;
            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, glow.Size() * 0.5f, Projectile.scale, SpriteEffects.None);
        }
    }
    #endregion

    #region Stealth Projectiles

    [PierceResistException]
    public class EclipsesFall_StealthSpear : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";

        bool initialized = false;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        (Vector2 start, Vector2 end) LinePos = (new(), new());
        float LineWidth = 0;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 48;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.MaxUpdates = 1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 1;
            Projectile.timeLeft = 1200 * Projectile.MaxUpdates;
            Projectile.ContinuouslyUpdateDamageStats = true;
        }
        public override void AI()
        {
            if (!initialized)
            {
                Projectile.ai[0] = -1;
                Projectile.localAI[0] = Projectile.ai[2];
                Projectile.ai[2] = 0;
                initialized = true;
            }

            Projectile.originalDamage = (int)(EclipsesFall.EclipseSpearBaseDmg * (Projectile.ai[2] / (float)EclipsesFall.MaxFragmentCount));
            if (Projectile.ai[0] >= 0 && Projectile.ai[2] >= Projectile.localAI[0] && Projectile.Opacity > 0)
            {
                LinePos.end = Projectile.position;
                Projectile.velocity = Projectile.DirectionTo(Main.npc[(int)Projectile.ai[0]].Center) * 64;
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
                Projectile.Center = Main.npc[(int)Projectile.ai[0]].Center + Projectile.velocity;
                Projectile.Damage();
                Projectile.ai[0] = -1;
                LineWidth = 1f;
                LinePos.start = Projectile.Center;
                Projectile.localAI[1] = 0;
            }


            LineWidth -= 0.05f;
            Projectile.localAI[1]++;
            if (Projectile.localAI[1] > 60)
            {
                var dis = Projectile.Distance(Main.player[Projectile.owner].Center);
                Projectile.velocity += Projectile.DirectionTo(Main.player[Projectile.owner].Center) * dis / 320f;
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;

            }
            Projectile.velocity *= 0.9f;
        }
        public override bool PreDraw(ref Color lightColor)
        {

            if (LineWidth > 0)
            {
                var device = Main.instance.GraphicsDevice;
                using var lease = RenderTargetPool.Shared.Rent(
                    device,
                    Main.screenWidth / 2,
                    Main.screenHeight / 2,
                    RenderTargetDescriptor.Default
                );

                using (Main.spriteBatch.Scope())
                {
                    using (lease.Scope(clearColor: Color.Transparent))
                    {

                        List<Vector2> posList = [];
                        //For the prim to render properly I need to divide the distance between the positions into a couple points. Just using start and end doesn't render.
                        for (var i = 0; i <= 2; i++)
                        {
                            posList.Add(Vector2.Lerp(Projectile.position, LinePos.end, i / 2f));
                        }
                        var pos = posList.ToArray();

                        GameShaders.Misc["CalamityMod:ImpFlameTrail"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/ScarletDevilStreak"));
                        PrimitiveRenderer.RenderTrail(pos, new(FireWidthFunction, FireColorFunction, (_, _) => Projectile.Size * 0.5f, smoothen: true, pixelate: false, shader: GameShaders.Misc["CalamityMod:ImpFlameTrail"], useUnscaledMatrices: true), pos.Length);
                        GameShaders.Misc["CalamityMod:ImpFlameTrail"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/SylvestaffStreak"));
                        PrimitiveRenderer.RenderTrail(pos, new(FireCoreWidthFunction, FireCoreColorFunction, (_, _) => Projectile.Size * 0.5f, smoothen: true, pixelate: false, shader: GameShaders.Misc["CalamityMod:ImpFlameTrail"], useUnscaledMatrices: true), pos.Length);
                    }

                    Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                    Main.spriteBatch.Draw(lease.Target, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);
                    Main.spriteBatch.End();

                }
            }
            var tex = TextureAssets.Item[ModContent.ItemType<EclipsesFall>()];
            if (Projectile.Opacity > 0.5f)
                Main.EntitySpriteDraw(tex.Value, Projectile.Center - Main.screenPosition, null, Color.White * Projectile.Opacity, Projectile.rotation, tex.Size() * 0.5f, Projectile.scale, 0);
            using (Main.spriteBatch.Scope())
            {
                Main.spriteBatch.Begin(default, BlendState.NonPremultiplied, null, null, null, null, Main.Transform);
                Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, null, Color.White * Projectile.Opacity * (Projectile.localAI[0] > 0 ? Projectile.ai[2] / Projectile.localAI[0] : 1), Projectile.rotation, TextureAssets.Projectile[Type].Size() * 0.5f, Projectile.scale, 0);
                Main.spriteBatch.End();
            }
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float _ = 0;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.oldPosition + new Vector2(Projectile.width, Projectile.height) * 0.5f, Projectile.Center, 48f, ref _))
            {
                return true;
            }

            return base.Colliding(projHitbox, targetHitbox);
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.ai[0] < 0)
                return false;
            return null;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            //Half damage to any NPC that isn't the primary target
            if (Projectile.ai[0] != target.whoAmI)
                modifiers.SourceDamage *= 0.5f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

            if (Projectile.timeLeft < 600 * Projectile.MaxUpdates && Projectile.ai[0] == target.whoAmI)
            {
                Projectile.Opacity = 0;
                Projectile.Center = target.Center;
                Projectile.timeLeft = 60 * Projectile.MaxUpdates;
                Projectile.velocity = new(0, 1E-05f);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<EclipsesFall_StealthSpearExplosion>(), Projectile.damage, 0, Projectile.owner);
            }
            Projectile.netUpdate = true;
            if (Projectile.ai[0] == target.whoAmI)
                SoundEngine.PlaySound(SarosPossession.FiringSound with { Pitch = -1f, Volume = 0.75f, }, Projectile.Center);
        }

        public override void OnKill(int timeLeft)
        {
            if (Projectile.Opacity > 0)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<EclipsesFall_StealthSpearExplosion>(), Projectile.damage, 0, Projectile.owner);
            return;
        }

        public float FireWidthFunction(float completion, Vector2 vertexPos)
        {
            return 96 * LineWidth * MathHelper.Clamp((Projectile.ai[2] / 20f), 0.25f, 1f);
        }

        public Color FireColorFunction(float completion, Vector2 vertexPos)
        {
            Color mainColor = Color.Lerp(new Color(238, 226, 153), new Color(255, 191, 73), (MathF.Sin(completion * MathHelper.TwoPi + Main.GlobalTimeWrappedHourly * 5) + 1) * 0.5f);
            return mainColor * MathF.Pow(1 - completion * 1.1f, 0.5f);
        }

        public float FireCoreWidthFunction(float completion, Vector2 vertexPos)
        {
            return 32 * LineWidth * MathHelper.Clamp((Projectile.ai[2] / 20f), 0.25f, 1f);
        }

        public Color FireCoreColorFunction(float completion, Vector2 vertexPos)
        {
            return Color.Black * MathF.Pow(1 - completion * 1.1f, 0.5f);
        }
    }
    public class EclipsesFall_StealthSpearExplosion : ModProjectile, IAdditiveDrawer, ILocalizedModType
    {
        //Based on Terratomere explosion
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 520;
            Projectile.friendly = true;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = false;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 150;
            Projectile.MaxUpdates = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.scale = 0.2f;
            Projectile.hide = true;
        }

        public override void AI()
        {
            // Play an explosion sound on the first frame of this projectile's existence.
            if (Projectile.localAI[0] == 0f)
            {
                SoundEngine.PlaySound(SubsumingVortex.ExplosionSound with { Volume = 0.6f }, Projectile.Center);
                Projectile.localAI[0] = 1f;
            }

            // Emit a strong white light.
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 1.5f);

            // Determine frames. Once the maximum frame is reached the projectile dies.
            Projectile.frameCounter++;
            if (Projectile.frameCounter % 5 == 4)
                Projectile.frame++;
            if (Projectile.frame >= 17)
                Projectile.Kill();

            // Exponentially accelerate.
            Projectile.scale *= 1.013f;
            Projectile.Opacity = Utils.GetLerpValue(5f, 36f, Projectile.timeLeft, true);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return base.PreDraw(ref lightColor);
        }

        public void AdditiveDraw(SpriteBatch spriteBatch)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D lightTexture = ModContent.Request<Texture2D>("CalamityMod/Skies/XerocLight").Value;
            Rectangle frame = texture.Frame(3, 6, Projectile.frame / 6, Projectile.frame % 6);
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 origin = frame.Size() * 0.5f;

            for (int i = 0; i < 2; i++)
            {
                Vector2 lightDrawPosition = drawPosition + (MathHelper.TwoPi * i / 36f + Main.GlobalTimeWrappedHourly * 5f).ToRotationVector2() * Projectile.scale * 12f;
                Color lightBurstColor = CalamityUtils.MulticolorLerp(Projectile.timeLeft / 144f, Terratomere.TerraColor1, Terratomere.TerraColor2);
                lightBurstColor = Color.Lerp(lightBurstColor, Color.White, 0.4f) * Projectile.Opacity * 0.24f;
                Main.spriteBatch.Draw(lightTexture, lightDrawPosition, null, lightBurstColor, 0f, lightTexture.Size() * 0.5f, Projectile.scale * 1.32f, SpriteEffects.None, 0);
            }
            if (Projectile.timeLeft < 149)
                using (Main.spriteBatch.Scope())
                {
                    Main.spriteBatch.Begin(default, BlendState.NonPremultiplied, null, null, null, null, Main.Transform);
                    Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, Color.White, Projectile.rotation, frame.Size() * 0.5f, 1.6f, 0);
                    Main.spriteBatch.End();
                }
        }
    }
    #endregion

    #region Shatter Projectiles
    public class EclipsesFall_LightFragment : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public static int lifetime => 1200;
        Color? color = null;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 32;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 3;
            Projectile.timeLeft = lifetime;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
            Projectile.MaxUpdates = 2;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Projectile.rotation += Projectile.direction * 0.02f;
            if (Projectile.ai[0] < -1)
            {
                Projectile.penetrate = 1;
                Projectile.damage = Projectile.originalDamage;
                Projectile.stopsDealingDamageAfterPenetrateHits = false;
                var target = Projectile.FindTargetWithinRange(4000);
                if (target is not null)
                {
                    Projectile.velocity += Projectile.DirectionTo(target.Center) * 2;
                    Projectile.velocity *= 0.95f;
                }
            }

            if (Projectile.ai[0] == 0f || Projectile.ai[2] > 0)
            {
                if (Projectile.timeLeft < (lifetime - Projectile.ai[1]) && Projectile.ai[2] >= 0)
                {
                    Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero);
                    Projectile.velocity *= Projectile.ai[2];
                    Projectile.ai[2]--;
                }
            }
            else
            {
                if (Main.projectile.IndexInRange((int)Projectile.ai[0] - 1) && Main.projectile[(int)Projectile.ai[0] - 1].active)
                {
                    var proj = Main.projectile[(int)Projectile.ai[0] - 1];
                    if (Projectile.timeLeft > 100)
                        Projectile.timeLeft = 100;
                    Projectile.Center = Vector2.Lerp(Projectile.Center, proj.Center, (1 - (Projectile.timeLeft / 100f)));
                    Projectile.velocity = new(0, 1E-05f);
                    if (Projectile.Distance(proj.Center) < 16)
                    {
                        proj.ai[2]++;
                        proj.netUpdate = true;
                        if (proj.ai[2] == proj.localAI[0])
                            SoundEngine.PlaySound(SarosPossession.SpawnSound with { Pitch = 1f, Volume = 1f, }, Projectile.Center);
                        Projectile.active = false;

                    }
                }
            }
        }
        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.ai[0] < -1 && target.Calamity().IsArmored())
                return false;
            return null;
        }

        public override void OnKill(int timeLeft)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
                Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<EclipsesFall_LightExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End(out var ss);
            var device = Main.instance.GraphicsDevice;
            using var lease = RenderTargetPool.Shared.Rent(
                device,
                Main.screenWidth / 2,
                Main.screenHeight / 2,
                RenderTargetDescriptor.Default
            );
            using (lease.Scope(clearColor: Color.Transparent))
            {
                var list = Projectile.oldPos.Take(16).ToArray();

                GameShaders.Misc["CalamityMod:ImpFlameTrail"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/ScarletDevilStreak"));
                PrimitiveRenderer.RenderTrail(list, new(FireWidthFunction, FireColorFunction, (_, _) => Projectile.Size * 0.5f, smoothen: true, pixelate: false, shader: GameShaders.Misc["CalamityMod:ImpFlameTrail"], useUnscaledMatrices: true), 32);

                GameShaders.Misc["CalamityMod:ImpFlameTrail"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/ScarletDevilStreak"));
                PrimitiveRenderer.RenderTrail(list, new(FireCoreWidthFunction, FireCoreColorFunction, (_, _) => Projectile.Size * 0.5f, smoothen: true, pixelate: false, shader: GameShaders.Misc["CalamityMod:ImpFlameTrail"], useUnscaledMatrices: true), 32);
            }
            float dis = Projectile.position.Distance(Projectile.oldPos.Last()) / 32;
            if (dis > 1)
            {
                dis = 1f;
            }
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            Main.spriteBatch.Draw(lease.Target, Vector2.Zero, null, Color.White * dis, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);
            Main.spriteBatch.End();

            Main.spriteBatch.Begin(ss);

            using (Main.spriteBatch.Scope())
            {
                Main.spriteBatch.Begin(default, BlendState.NonPremultiplied, null, null, null, null, Main.Transform);
                Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, TextureAssets.Projectile[Type].Size() * 0.5f, Projectile.scale, 0);
                Main.spriteBatch.End();
            }
            return false;
        }

        // Matches Saros Possesion sunfire with slight edits
        public float FireWidthFunction(float completion, Vector2 vertexPos)
        {
            float width;
            float maxBodyWidth = 24f * Projectile.scale;
            float curveRatio = 0.2f;
            var positions = Projectile.oldPos.ToList();
            positions.RemoveAll(x => x == Vector2.Zero);
            if (completion < curveRatio)
                width = MathF.Pow(completion / curveRatio, 0.5f) * maxBodyWidth;
            else
                width = Utils.Remap(completion, curveRatio, 1f, maxBodyWidth, 0f);

            // Pulse inwards and outwards over time.
            float pulseInterpolant = MathF.Cos(MathHelper.Pi * completion - Main.GlobalTimeWrappedHourly * 20f) * 0.5f + 0.5f;
            float additionalPulseWidth = MathHelper.Lerp(0f, 12f, pulseInterpolant);
            return (width + additionalPulseWidth) * positions.Count() / (float)ProjectileID.Sets.TrailCacheLength[Type];
        }

        public Color FireColorFunction(float completion, Vector2 vertexPos)
        {
            Color mainColor = new Color(238, 226, 153);
            return Color.Lerp(mainColor, Color.Transparent, completion);
        }

        public float FireCoreWidthFunction(float completion, Vector2 vertexPos)
        {
            float width;
            float maxBodyWidth = Projectile.scale * 16;
            float curveRatio = 0.25f;
            var positions = Projectile.oldPos.ToList();
            positions.RemoveAll(x => x == Vector2.Zero);

            if (completion < curveRatio)
                width = MathF.Sin(completion / curveRatio * MathHelper.PiOver2) * maxBodyWidth + curveRatio;
            else
                width = Utils.Remap(completion, curveRatio, 1f, maxBodyWidth, 0f);
            return width * positions.Count() / (float)ProjectileID.Sets.TrailCacheLength[Type];
        }

        public Color FireCoreColorFunction(float completion, Vector2 vertexPos)
        {
            Color mainColor = new Color(255, 191, 73);
            return mainColor;
        }
    }
    public class EclipsesFall_LightExplosion : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";

        public const int Lifetime = 35; // 7 animation frames, 12 FPS

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 7;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 102;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = RogueDamageClass.Instance;
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

        public override void PostDraw(Color lightColor)
        {
            Texture2D glow = ModContent.Request<Texture2D>(Texture + "Glow").Value;
            Rectangle frame = glow.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, frame, Color.White, Projectile.rotation, frame.Size() * 0.5f, Projectile.scale, SpriteEffects.None);
        }
    }
    public class EclipsesFall_LightSpark : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public static int lifetime = 150;
        Color? color = null;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 32;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 3;
            Projectile.timeLeft = lifetime;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.localAI[0] = 20f;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
            Projectile.MaxUpdates = 2;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Projectile.rotation += Projectile.direction * Projectile.ai[0];

            if (Projectile.timeLeft < (lifetime - Projectile.ai[1]) && Projectile.localAI[0] >= 0)
            {
                Projectile.velocity.Normalize();
                Projectile.velocity *= Projectile.localAI[0];
                Projectile.localAI[0]--;
                Projectile.timeLeft++;
            }

            if (Projectile.localAI[0] == 0)
            {
                Projectile.velocity = new(0, 1E-05f);
                Projectile.damage = 0;
                float dis = Projectile.position.Distance(Projectile.oldPos.Take(16).Last());
                if (dis < 0.1f)
                    Projectile.Kill();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            color ??= Color.Lerp(Color.OrangeRed, new Color(255, 191, 73), Main.rand.NextFloat(0.25f, 0.5f));

            Main.spriteBatch.End(out var ss);
            var device = Main.instance.GraphicsDevice;
            using var lease = RenderTargetPool.Shared.Rent(
                device,
                Main.screenWidth / 2,
                Main.screenHeight / 2,
                RenderTargetDescriptor.Default
            );
            using (lease.Scope(clearColor: Color.Transparent))
            {
                var list = Projectile.oldPos.Take(16);

                GameShaders.Misc["CalamityMod:ImpFlameTrail"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/ScarletDevilStreak"));
                PrimitiveRenderer.RenderTrail(list.ToArray(), new(FireWidthFunction, FireColorFunction, (_, _) => Projectile.Size * 0.5f, smoothen: true, pixelate: false, shader: GameShaders.Misc["CalamityMod:ImpFlameTrail"], useUnscaledMatrices: true), 32);

                GameShaders.Misc["CalamityMod:ImpFlameTrail"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/ScarletDevilStreak"));
                PrimitiveRenderer.RenderTrail(list.ToArray(), new(FireCoreWidthFunction, FireCoreColorFunction, (_, _) => Projectile.Size * 0.5f, smoothen: true, pixelate: false, shader: GameShaders.Misc["CalamityMod:ImpFlameTrail"], useUnscaledMatrices: true), 32);
            }
            float dis = Projectile.position.Distance(Projectile.oldPos.Take(16).Last()) / 128f - 0.1f;
            if (dis > 1)
            {
                dis = 1f;
            }
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            Main.spriteBatch.Draw(lease.Target, Vector2.Zero, null, Color.White * dis, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);
            Main.spriteBatch.End();

            Main.spriteBatch.Begin(ss);

            return false;
        }

        public float FireWidthFunction(float completion, Vector2 vertexPos)
        {
            float width;
            float maxBodyWidth = 24f * Projectile.scale;
            float curveRatio = 0.2f;
            var positions = Projectile.oldPos.ToList();
            positions.RemoveAll(x => x == Vector2.Zero);
            if (completion < curveRatio)
                width = MathF.Pow(completion / curveRatio, 0.5f) * maxBodyWidth;
            else
                width = Utils.Remap(completion, curveRatio, 1f, maxBodyWidth, 0f);

            // Pulse inwards and outwards over time.
            float pulseInterpolant = MathF.Cos(MathHelper.Pi * completion - Main.GlobalTimeWrappedHourly * 20f) * 0.5f + 0.5f;
            float additionalPulseWidth = MathHelper.Lerp(0f, 12f, pulseInterpolant);
            return (width + additionalPulseWidth) * positions.Count() / (float)ProjectileID.Sets.TrailCacheLength[Type];
        }

        public Color FireColorFunction(float completion, Vector2 vertexPos)
        {
            Color mainColor = new Color(255, 191, 73);
            return Color.Lerp(mainColor, Color.Transparent, completion);
        }

        public float FireCoreWidthFunction(float completion, Vector2 vertexPos)
        {
            float width;
            float maxBodyWidth = Projectile.scale * 16;
            float curveRatio = 0.25f;
            var positions = Projectile.oldPos.ToList();
            positions.RemoveAll(x => x == Vector2.Zero);

            if (completion < curveRatio)
                width = MathF.Sin(completion / curveRatio * MathHelper.PiOver2) * maxBodyWidth + curveRatio;
            else
                width = Utils.Remap(completion, curveRatio, 1f, maxBodyWidth, 0f);
            return width * positions.Count() / (float)ProjectileID.Sets.TrailCacheLength[Type];
        }

        public Color FireCoreColorFunction(float completion, Vector2 vertexPos)
        {
            Color mainColor = color.Value;
            return mainColor;
        }
    }
    #endregion
}
