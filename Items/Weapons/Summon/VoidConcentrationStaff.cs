using System;
using System.Collections.Generic;
using System.Linq;
using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.DataStructures;
using CalamityMod.Dusts;
using CalamityMod.Graphics.Primitives;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.CeaselessVoid;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Rarities;
using CalamityMod.Systems.Collections;
using CalamityMod.Utilities.Daybreak;
using CalamityMod.Utilities.Daybreak.Buffers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    #region Item
    public class VoidConcentrationStaff : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Summon";
        public static SummonTag summonTag = new SummonTag()
        {
            MultiplicativeTagDamage = 0.3f, //Percentage of damage dealt added by the void. This is doubled by summons. This was tested - 1.5x was too weak to be worth 4 slots.
            AllowsWhipStacking = true,
            TagOnHit = tagOnHit,
            TagModifyHitEffects = SummonTag.BlankTagModifyHit,
            AutoDrawTooltip = false
        };

        public static void tagOnHit(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone) => npc.GetGlobalNPC<VoidTagNPC>().StoredDamage += (int)(damageDone);
        public override void SetStaticDefaults()
        {
            ItemID.Sets.StaffMinionSlotsRequired[Type] = 4f;
            summonTag.TagItem = Type;
        }

        public override void SetDefaults()
        {
            Item.width = 52;
            Item.height = 72;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.UseSound = SoundID.DD2_EtherianPortalOpen;
            Item.DamageType = DamageClass.Summon;
            Item.mana = 10;
            Item.damage = 200;
            Item.knockBack = 4f;
            Item.useAnimation = Item.useTime = 24;
            Item.shoot = ModContent.ProjectileType<VoidConcentrationMinion>();
            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                foreach (var item in Main.ActiveProjectiles)
                {
                    if (item.type == ModContent.ProjectileType<VoidConcentrationMark>())
                        item.Kill();
                }
                if (player.ownedProjectileCounts[ModContent.ProjectileType<VoidConcentrationMinion>()] == 0)
                    Projectile.NewProjectileDirect(source, player.Center, Vector2.Zero, type, damage, knockback, player.whoAmI);
            }
            else
            {
                player.MinionNPCTargetAim(true);
                if (player.MinionAttackTargetNPC >= 0)
                {
                    var target = Main.npc[player.MinionAttackTargetNPC];
                    foreach (var item in Main.ActiveProjectiles)
                    {
                        if (item.type == ModContent.ProjectileType<VoidConcentrationMark>())
                            item.Kill();
                    }
                    Projectile.NewProjectileDirect(source, target.Center, Vector2.Zero, ModContent.ProjectileType<VoidConcentrationMark>(), damage, knockback, player.whoAmI, target.whoAmI);
                }
            }
            return false;
        }
        public override bool AltFunctionUse(Player player) => true;
    }

    #endregion
}

namespace CalamityMod.Projectiles.Summon
{

    #region Left Click
    public class VoidConcentrationMinion : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public override void SetDefaults()
        {
            Projectile.width = 46;
            Projectile.height = 80;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 4f;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
        }
        public override bool? CanHitNPC(NPC target) => false;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            player.AddBuff(ModContent.BuffType<VoidConcentrationBuff>(), 3600);

            if (player.dead)
            {
                modPlayer.VoidConcentrationStaff = false;
            }
            if (modPlayer.VoidConcentrationStaff)
            {
                Projectile.timeLeft = 2;
            }

            Projectile.Center = player.Center;

            //Spawn proj code taken from Ceaseless Void
            int numPerRing = 7;
            int numRings = 3;
            int spacing = 360 / numPerRing;
            int distance2 = 10;
            if (Main.netMode != NetmodeID.MultiplayerClient && player.ownedProjectileCounts[ModContent.ProjectileType<VoidConcentrationMark>()] + player.ownedProjectileCounts[ModContent.ProjectileType<VoidConcentrationDarkEnergy>()] <= 0)
            {
                for (int i = 0; i < numPerRing; i++)
                {
                    for (int j = 0; j < numRings; j++)
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), new Vector2((Projectile.Center.X + (MathF.Sin(i * spacing) * distance2)), (int)(Projectile.Center.Y + (MathF.Cos(i * spacing) * distance2))), Vector2.Zero, ModContent.ProjectileType<VoidConcentrationDarkEnergy>(), Projectile.damage, Projectile.knockBack, Projectile.owner, i * spacing, j);
                }
            }

        }
    }
    public class VoidConcentrationDarkEnergy : ModProjectile, ILocalizedModType
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public new string LocalizationCategory => "Projectiles.Summon";
        private double minDistance => 10;
        private double distance { get; set; } = 10;
        private double maxDistance => 400;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 8;
            ProjectileID.Sets.MinionShot[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 10;
            Projectile.penetrate = -1;
            Projectile.MaxUpdates = 2;
            Projectile.tileCollide = false;
            Projectile.scale = 1f;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            texture = TextureAssets.Npc[ModContent.NPCType<DarkEnergy>()].Value;
            var gt = DarkEnergy.GlowTexture.Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
            var frame = texture.Frame(1, 8, 0, Projectile.frame);
            Main.EntitySpriteDraw(texture, drawPos, frame, lightColor, Projectile.rotation, frame.Size() * 0.5f, Projectile.scale * 0.5f, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(gt, drawPos, frame, Color.White, Projectile.rotation, frame.Size() * 0.5f, Projectile.scale * 0.5f, SpriteEffects.None, 0);
            return false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (player.ownedProjectileCounts[ModContent.ProjectileType<VoidConcentrationMinion>()] - player.ownedProjectileCounts[ModContent.ProjectileType<VoidConcentrationMark>()] <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, (int)CalamityDusts.PurpleCosmilite, 0, -1f, 0, default, 1f);
                    Main.dust[dust].noGravity = true;
                }
                Projectile.Kill();
                return;
            }
            else
            {
                Projectile.timeLeft++;
            }


            if (Projectile.FinalExtraUpdate())
                Projectile.frameCounter++;
            if (Projectile.frameCounter > 6)
            {
                Projectile.frame++;
                if (Projectile.frame >= Main.projFrames[Type])
                    Projectile.frame = 0;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame == 1)
                Projectile.frame++;

            #region Movement Code (Taken from Ceaseless Void)

            Projectile.velocity = Vector2.Zero;
            double rateOfChangeIncrease = (maxDistance / 80) - 1D;
            double rateOfChange = ((Projectile.ai[1] * 0.5f) + 3.2D) * (maxDistance / 960);
            if (Projectile.ai[2] == 0f)
            {
                distance += rateOfChange;
                if (distance >= maxDistance)
                {
                    distance = maxDistance;
                    Projectile.ai[2] = 1f;
                }
            }
            else
            {
                distance -= rateOfChange;
                if (distance <= minDistance)
                {
                    distance = minDistance;
                    Projectile.ai[2] = 0f;
                }
            }

            // Rotation velocity
            float minRotationVelocity = 0.5f;
            float rotationVelocityIncrease = 0.15f;
            rotationVelocityIncrease += rotationVelocityIncrease * (Projectile.ai[1] * 0.5f);

            // Rotate around Ceaseless Void
            double degrees = Projectile.ai[0];
            double radians = degrees * (Math.PI / 180);
            Projectile.position.X = player.Center.X - (int)(Math.Cos(radians) * distance) - Projectile.width / 2;
            Projectile.position.Y = player.Center.Y - (int)(Math.Sin(radians) * distance) - Projectile.height / 2;
            Projectile.ai[0] += minRotationVelocity + rotationVelocityIncrease;
            #endregion
        }
    }

    #endregion

    #region Right Click
    public class VoidConcentrationMark : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
            ProjectileID.Sets.MinionShot[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 54;
            Projectile.height = 50;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 600;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.scale = 1f;
            Projectile.DamageType = DamageClass.Summon;
        }
        public override bool? CanHitNPC(NPC target) => false;
        public override void AI()
        {
            if (!Main.npc[(int)Projectile.ai[0]].active)
            {
                Projectile.Kill();
                return;
            }
            Player owner = Main.player[Projectile.owner];

            Projectile.Center = Main.npc[(int)Projectile.ai[0]].Center;
            Main.npc[(int)Projectile.ai[0]].AddBuff(ModContent.BuffType<VoidConcentrationSummonTagBuff>(), 5);
            if (Projectile.timeLeft % 60 == 0)
            {
                var w = Main.npc[(int)Projectile.ai[0]].width + 200;
                var v1 = Main.rand.NextVector2CircularEdge(1, 1);
                for (var i = 0; i < 3; i++)
                {
                    var v = v1.RotatedBy(MathHelper.TwoPi * (i / 3f));
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center + 200 * v, v.RotatedBy(MathHelper.PiOver2) * 10, ModContent.ProjectileType<VoidConcentrationBeam>(), (int)(Projectile.damage * 0.4f), Projectile.knockBack, Projectile.owner);
                        Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center + 200 * v, v.RotatedBy(-MathHelper.PiOver2) * 10, ModContent.ProjectileType<VoidConcentrationBeam>(), (int)(Projectile.damage * 0.4f), Projectile.knockBack, Projectile.owner,1);
                    }
                }
            }

            if (Projectile.frameCounter > 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Type] - 1)
            {
                Projectile.frame = 0;
            }
            Projectile.frameCounter++;

            if (Projectile.timeLeft <= 60)
            {
                Projectile.alpha += 4;
            }
        }
    }
    public class VoidConcentrationBeam : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.MinionShot[Type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.timeLeft = 600;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.MaxUpdates = 3;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.penetrate = 1;
        }
        public override void AI()
        {
            if (Projectile.timeLeft == 600 && Projectile.ai[0] == 0)
            {
                GeneralParticleHandler.SpawnParticle(new CustomSprite(Projectile.Center, Vector2.Zero, 30, TextureAssets.Projectile[ModContent.ProjectileType<VoidVortexProj>()].Value, 1, Color.White, AddativeBlend: false, frameCount: 5));

            }
            NPC target = Projectile.Center.MinionHoming(5000f, Main.player[Projectile.owner]);
            // Move towards the target.
            if (target != null && Projectile.localNPCImmunity[target.whoAmI] <= 0)
            {
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * (Projectile.timeLeft < 300 ? 24f : 10f), 0.05f);
                Projectile.netUpdate = true;
                Projectile.Calamity().HomingTarget = target.whoAmI;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Projectile.damage <= 0)
            {
                Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 4;
                Projectile.timeLeft = (int)MathHelper.Min(8, Projectile.timeLeft);
            }

        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 12; i++)
            {
                Vector2 dustVelocity = Main.rand.NextVector2Circular(1f, 1f) * 6f;
                float dustScale = Main.rand.NextFloat(3f, 5f);
                Color dustColor = Color.Lerp(Color.Yellow, Color.Gold, Main.rand.NextFloat(0.5f, 1f));

                Dust dust = Dust.NewDustDirect(Projectile.Center, 1, 1, DustID.TintableDustLighted, dustVelocity.X, dustVelocity.Y, 0, dustColor, dustScale);
                dust.noGravity = true;
                dust.noLight = false;
                dust.noLightEmittence = false;
            }
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
                GameShaders.Misc["CalamityMod:ImpFlameTrail"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/ScarletDevilStreak"));
                PrimitiveRenderer.RenderTrail(Projectile.oldPos, new(FireWidthFunction, FireColorFunction, (_, _) => Projectile.Size * 0.5f, smoothen: true, pixelate: false, shader: GameShaders.Misc["CalamityMod:ImpFlameTrail"], useUnscaledMatrices: true), Projectile.oldPos.Length + 32);

                Vector2[] fireCoreLength = Projectile.oldPos.Take(8).ToArray();
                GameShaders.Misc["CalamityMod:ImpFlameTrail"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/SylvestaffStreak"));
                PrimitiveRenderer.RenderTrail(fireCoreLength, new(FireCoreWidthFunction, FireCoreColorFunction, (_, _) => Projectile.Size * 0.5f, smoothen: true, pixelate: false, shader: GameShaders.Misc["CalamityMod:ImpFlameTrail"], useUnscaledMatrices: true), fireCoreLength.Length + 24);
            }

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            Main.spriteBatch.Draw(lease.Target, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);
            Main.spriteBatch.End();

            Main.spriteBatch.Begin(ss);

            return false;
        }

        public float FireWidthFunction(float completion, Vector2 pos)
        {
            float width;
            float maxBodyWidth = 38f * Projectile.scale;
            float curveRatio = 0.2f;
            var positions = Projectile.oldPos.ToList();
            positions.RemoveAll(x => x == Vector2.Zero);
            // Crop the tip of the trail into a conic shape.
            if (completion < curveRatio)
                width = MathF.Pow(completion / curveRatio, 0.5f) * maxBodyWidth;
            else
                width = Utils.Remap(completion, curveRatio, 1f, maxBodyWidth, 0f);

            // Pulse inwards and outwards over time.
            float pulseInterpolant = MathF.Cos(MathHelper.Pi * completion - Main.GlobalTimeWrappedHourly * 20f) * 0.5f + 0.5f;
            float additionalPulseWidth = MathHelper.Lerp(0f, 12f, pulseInterpolant);
            return (width + additionalPulseWidth) * positions.Count() / (float)ProjectileID.Sets.TrailCacheLength[Type];
        }

        public Color FireColorFunction(float completion, Vector2 pos)
        {
            Color mainColor = Color.DarkSlateBlue * 1.3f;
            Color endColor = Color.Lerp(mainColor, Color.Transparent, Utils.GetLerpValue(0.8f, 1f, completion, true));
            return Color.Lerp(mainColor, endColor, completion) * Projectile.Opacity;
        }

        public float FireCoreWidthFunction(float completion, Vector2 pos)
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

        public Color FireCoreColorFunction(float completion, Vector2 pos)
        {
            Color mainColor = Color.SkyBlue;
            Color tipColor = Color.Lerp(mainColor, Color.Transparent, Utils.GetLerpValue(0.8f, 1f, completion, true));
            Color fullBodyColor = Color.Lerp(mainColor, tipColor, completion);
            return Color.Lerp(fullBodyColor, Color.White, 0.175f) * Projectile.Opacity;
        }
    }

    #endregion

    #region Summon Tag
    public class VoidTagDmgType : DamageClass //Custom damage class used to prevent the voids from activating themselves
    {
        public static VoidTagDmgType getInstance => ModContent.GetInstance<VoidTagDmgType>();
        public override bool UseStandardCritCalcs => false;
    }
    public class VoidTagNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public int StoredDamage = 0;
        int voidTimer = 0;
        float rot = 0;

        public override void PostAI(NPC npc)
        {
            voidTimer++;
            if (StoredDamage > 0)
            {
                if (voidTimer > 10)
                {
                    voidTimer = 0;
                    var dmg = (int)Math.Max(1, (StoredDamage));
                    StoredDamage -= dmg;
                    rot += 0.15f;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(npc.GetSource_OnHurt(null), npc.Center, Vector2.UnitX.RotatedBy(rot), ModContent.ProjectileType<VoidConcentrationSummonTagProj>(), (int)Math.Max((dmg * VoidConcentrationStaff.summonTag.MultiplicativeTagDamage), 1), 0, -1, npc.whoAmI);
                    }
                    }
            }
            base.PostAI(npc);
        }
    }
    public class VoidTagPlayer : ModPlayer
    {
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (hit.DamageType != VoidTagDmgType.getInstance && target.HasBuff<VoidConcentrationSummonTagBuff>())
                target.GetGlobalNPC<VoidTagNPC>().StoredDamage += damageDone;
        }

        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            if (Player.ownedProjectileCounts[ModContent.ProjectileType<VoidConcentrationDarkEnergy>()] > 0)
                modifiers.FinalDamage *= 0.85f; //15% less dmg taken, multipicative. This is not meant to be additive to avoid needing DR scaling.
        }
    }
    public class VoidConcentrationSummonTagProj : ModProjectile, ILocalizedModType
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public new string LocalizationCategory => "Projectiles.Summon";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 8;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 60;
            Projectile.penetrate = 1;
            Projectile.MaxUpdates = 2;
            Projectile.tileCollide = false;
            Projectile.scale = 1f;
            Projectile.DamageType = ModContent.GetInstance<VoidTagDmgType>();
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.ArmorPenetration = 1000;
        }

        NPC target => Main.npc[(int)Projectile.ai[0]];

        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.timeLeft < 5)
                return null;
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            for (float i = 0; i < MathHelper.TwoPi; i += MathHelper.TwoPi / 3f)
            {
                Texture2D texture = TextureAssets.Projectile[Type].Value;
                texture = TextureAssets.Npc[ModContent.NPCType<DarkEnergy>()].Value;
                var gt = DarkEnergy.GlowTexture.Value;
                Vector2 drawPos = Projectile.Center - Main.screenPosition + Projectile.velocity.RotatedBy(i);
                var frame = texture.Frame(1, 8, 0, Projectile.frame);
                Main.EntitySpriteDraw(texture, drawPos, frame, lightColor, 0, frame.Size() * 0.5f, Projectile.scale * 0.5f, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(gt, drawPos, frame, Color.White, 0, frame.Size() * 0.5f, Projectile.scale * 0.5f, SpriteEffects.None, 0);
            }
            return false;
        }
        public override void AI()
        {
            if (target == null || !target.active)
            {
                Projectile.Kill();
                return;
            }
            var w = target.width + 200;
            Projectile.Center = target.Center;
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.velocity = Projectile.rotation.ToRotationVector2() * w * Projectile.timeLeft / 60f;

            if (Projectile.FinalExtraUpdate())
                Projectile.frameCounter++;
            if (Projectile.frameCounter > 6)
            {
                Projectile.frame++;
                if (Projectile.frame >= Main.projFrames[Type])
                    Projectile.frame = 0;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame == 1)
                Projectile.frame++;
        }

        public override bool ShouldUpdatePosition() => false;
    }

    public class VoidConcentrationSummonTagBuff : ModBuff
    {
        public override string Texture => "CalamityMod/Buffs/Summon/Whips/SentinalLash"; //Shared placeholder by all summon tag buffs. This is never seen in game.

        public override void SetStaticDefaults()
        {
            BuffID.Sets.IsATagBuff[Type] = true;
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }
    }

    #endregion
}
