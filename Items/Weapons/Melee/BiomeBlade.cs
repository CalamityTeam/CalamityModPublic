using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.CalPlayer;
using CalamityMod.DataStructures;
using CalamityMod.Items.Placeables;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Weapons.Melee
{
    public class BiomeBlade : ModItem
    {
        public enum Attunement : byte { Default, Hot, Cold, Tropical, Evil }
        public Attunement? mainAttunement = null;
        public Attunement? secondaryAttunement = null;
        public int Combo = 0;
        public int CanLunge = 1;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Broken Biome Blade"); //Broken Ecoliburn lmfao. Tbh a proper name instead of just "biome blade" may be neat given the importance of the sword
            Tooltip.SetDefault("Use RMB while standing still on the ground to attune the weapon to the powers of the surrounding biome\n" +
                               "Using RMB while moving or in the air switches between the current attunement and an extra stored one\n" +
                               "Main attunement : None\n" +
                               "Secondary attunement: None\n"); //Theres potential for flavor text as well but im not a writer
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            Player player = Main.player[Main.myPlayer];
            if (player is null)
                return;
            CalamityPlayer modPlayer = player.Calamity();

            foreach (TooltipLine l in list) //I gave the attunement silly pseudo-fantasy names, ideas appreciated.
            {
                if (l.text.StartsWith("Main attunement"))
                {
                    switch (mainAttunement)
                    {
                        case Attunement.Default:
                            l.text = "Main Attumenent : [Pure Clarity]";
                            l.overrideColor = new Color(171, 180, 73);
                            break;
                        case Attunement.Hot:
                            l.text = "Main Attumenent : [Arid Grandeur]";
                            l.overrideColor = new Color(238, 156, 73);
                            break;
                        case Attunement.Cold:
                            l.text = "Main Attumenent : [Biting Embrace]";
                            l.overrideColor = new Color(165, 235, 235);
                            break;
                        case Attunement.Tropical:
                            l.text = "Main Attumenent : [Grovetender's Touch]";
                            l.overrideColor = new Color(162, 200, 85);
                            break;
                        case Attunement.Evil:
                            l.text = "Main Attumenent : [Decay's Retort]";
                            l.overrideColor = new Color(211, 64, 147);
                            break;
                        default:
                            l.text = "Main Attumenent : [None]";
                            break;
                    }
                }

                if (l.text.StartsWith("Secondary attunement"))
                {
                    switch (secondaryAttunement)
                    {
                        case Attunement.Default:
                            l.text = "Secondary Attumenent : [Pure Clarity]";
                            l.overrideColor = new Color(171, 180, 73);
                            break;
                        case Attunement.Hot:
                            l.text = "Secondary Attumenent : [Arid Grandeur]";
                            l.overrideColor = new Color(238, 156, 73);
                            break;
                        case Attunement.Cold:
                            l.text = "Secondary Attumenent : [Biting Embrace]";
                            l.overrideColor = new Color(165, 235, 235);
                            break;
                        case Attunement.Tropical:
                            l.text = "Secondary Attumenent : [Grovetender's Touch]";
                            l.overrideColor = new Color(162, 200, 85);
                            break;
                        case Attunement.Evil:
                            l.text = "Secondary Attumenent : [Decay's Retort]";
                            l.overrideColor = new Color(211, 64, 147);
                            break;
                        default:
                            l.text = "Secondary Attumenent : [None]";
                            break;
                    }
                }
            }
        }

        public override void SetDefaults()
        {
            item.width = item.height = 36;
            item.damage = 55;
            item.melee = true;
            item.useAnimation = 30;
            item.useTime = 30;
            item.useTurn = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 5f;
            item.autoReuse = true;
            item.value = Item.buyPrice(0, 4, 0, 0);
            item.rare = ItemRarityID.Orange;
            item.shoot = ModContent.ProjectileType<BiomeOrb>();
            item.shootSpeed = 12f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.WoodenSword);
            recipe.AddIngredient(ItemID.DirtBlock, 20);
            recipe.AddIngredient(ItemID.SandBlock, 20);
            recipe.AddIngredient(ItemID.IceBlock, 20); //intentionally not any ice
            recipe.AddRecipeGroup("AnyEvilBlock", 20);
            //           recipe.AddIngredient(ItemID.GlowingMushroom, 20); Cut these ingredients due to no longer synergizing with these specific biomes
            //           recipe.AddIngredient(ItemID.Marble, 20);
            //           recipe.AddIngredient(ItemID.Granite, 20);
            //           recipe.AddIngredient(ItemID.Hellstone, 20);
            //           recipe.AddIngredient(ItemID.Coral, 20);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(5))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 0);
            }
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CloneNewInstances => true;

        public override ModItem Clone(Item item)
        {
            var clone = base.Clone(item);

            if (Main.mouseItem.type == ItemType<BiomeBlade>())
                item.modItem.HoldItem(Main.player[Main.myPlayer]);

            (clone as BiomeBlade).mainAttunement = (item.modItem as BiomeBlade).mainAttunement;
            (clone as BiomeBlade).secondaryAttunement = (item.modItem as BiomeBlade).secondaryAttunement;

            return clone;
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write((byte)mainAttunement);
            writer.Write((byte)secondaryAttunement);
        }

        public override void NetRecieve(BinaryReader reader)
        {
            mainAttunement = (Attunement?)reader.ReadByte();
            secondaryAttunement = (Attunement?)reader.ReadByte();
        }

        public override void HoldItem(Player player)
        {
            //Change the swords function based on its attunement
            switch (mainAttunement)
            {
                case Attunement.Default:
                    item.channel = false;
                    item.noUseGraphic = false;
                    item.useStyle = ItemUseStyleID.SwingThrow;
                    item.shoot = ProjectileType<PurityProjection>();
                    item.shootSpeed = 12f;
                    item.UseSound = SoundID.Item1;
                    item.noMelee = false;

                    Combo = 0;
                    break;
                case Attunement.Hot:
                    item.channel = true;
                    item.noUseGraphic = true;
                    item.useStyle = ItemUseStyleID.HoldingOut;
                    item.shoot = ProjectileType<AridGrandeur>();
                    item.shootSpeed = 12f;
                    item.UseSound = null;
                    item.noMelee = true;

                    Combo = 0;
                    break;
                case Attunement.Cold:
                    item.channel = true;
                    item.noUseGraphic = true;
                    item.useStyle = ItemUseStyleID.HoldingOut;
                    item.shoot = ProjectileType<BitingEmbrace>();
                    item.shootSpeed = 12f;
                    item.UseSound = null;
                    item.noMelee = true;
                    break;
                case Attunement.Tropical:
                    break;
                case Attunement.Evil:
                    item.channel = false;
                    item.noUseGraphic = true;
                    item.useStyle = ItemUseStyleID.Stabbing;
                    item.shoot = ProjectileType<DecaysRetort>();
                    item.shootSpeed = 12f;
                    item.UseSound = null;
                    item.noMelee = true;

                    Combo = 0;
                    if (player.velocity.Y == 0) //Reset the lunge ability on ground contact
                        CanLunge = 1;
                    break;
                default:
                    item.noUseGraphic = false;
                    item.useStyle = ItemUseStyleID.SwingThrow;
                    item.shoot = 0;
                    item.shootSpeed = 12f;
                    item.UseSound = SoundID.Item1;

                    Combo = 0;
                    break;
            }

            if (player.altFunctionUse == 2)
            {
                if (Main.projectile.Any(n => n.active && n.type == ProjectileType<BrokenBiomeBladeVisuals>() && n.owner == player.whoAmI))
                    return;
                if (!player.StandingStill()) //Swap attunements
                {
                    if (secondaryAttunement == null) //Don't let the player swap to an empty attunement
                        return;
                    Projectile.NewProjectile(player.position, Vector2.Zero, ProjectileType<BrokenBiomeBladeVisuals>(), 0, 0, player.whoAmI, 0f);
                    Attunement? temporaryAttunementStorage = mainAttunement;
                    mainAttunement = secondaryAttunement;
                    secondaryAttunement = temporaryAttunementStorage;
                    if (player.whoAmI == Main.myPlayer)
                        Main.PlaySound(SoundID.DD2_DarkMageHealImpact);
                }

                else //Chargeup
                {
                    if (player.mount.Active)
                        return;
                    Vector2 displace = new Vector2((player.direction >= 0 ? 2 : -1) * 20f, 0);
                    Projectile.NewProjectile(player.position + displace, Vector2.Zero, ProjectileType<BrokenBiomeBladeVisuals>(), 0, 0, player.whoAmI, 1f);
                    if (player.whoAmI == Main.myPlayer)
                        Main.PlaySound(SoundID.DD2_DarkMageHealImpact);
                }
            }


        }
        public override bool CanUseItem(Player player)
        {
            return !Main.projectile.Any(n => n.active && n.owner == player.whoAmI &&
            (n.type == ProjectileType<BitingEmbrace>()));
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse == 2)
                return false;

            switch (mainAttunement)
            {
                case Attunement.Cold:
                    switch (Combo)
                    {
                        case 0:
                            Projectile.NewProjectile(player.Center, new Vector2(speedX, speedY), ProjectileType<BitingEmbrace>(), damage, knockBack, player.whoAmI, 0, 15);
                            break;

                        case 1:
                            Projectile.NewProjectile(player.Center, new Vector2(speedX, speedY), ProjectileType<BitingEmbrace>(), damage, knockBack, player.whoAmI, 1, 20);
                            break;

                        case 2:
                            Projectile.NewProjectile(player.Center, new Vector2(speedX, speedY), ProjectileType<BitingEmbrace>(), damage, knockBack, player.whoAmI, 2, 50);
                            break;
                    }
                    Combo++;
                    if (Combo > 2)
                        Combo = 0;
                    return false;
                case Attunement.Evil:
                    Projectile.NewProjectile(player.Center, new Vector2(speedX, speedY), ProjectileType<DecaysRetort>(), damage*2, knockBack, player.whoAmI, 26, (float) CanLunge);
                    CanLunge = 0;
                    return false;

                default:
                    return base.Shoot(player, ref position, ref speedX, ref speedY, ref type, ref damage, ref knockBack);
            }
        }

            


        internal static ChargingEnergyParticleSet BiomeEnergyParticles = new ChargingEnergyParticleSet(-1, 2, Color.DarkViolet, Color.White, 0.04f, 20f);
        internal static void UpdateAllParticleSets()
        {
            BiomeEnergyParticles.Update();
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            // I want to strangle somebody.
            Texture2D itemTexture = Main.itemTexture[item.type];
            Rectangle itemFrame = (Main.itemAnimations[item.type] == null) ? itemTexture.Frame() : Main.itemAnimations[item.type].GetFrame(itemTexture);

            if (mainAttunement == null)
                return true;

            // Draw all particles.

            Vector2 particleDrawCenter = position + new Vector2(12f, 16f) * Main.inventoryScale;

            switch (mainAttunement)
            {
                case Attunement.Default:
                    BiomeEnergyParticles.EdgeColor = new Color(117, 126, 72);
                    BiomeEnergyParticles.CenterColor = new Color(200, 184, 136);
                    break;
                case Attunement.Hot:
                    BiomeEnergyParticles.EdgeColor = new Color(137, 32, 0);
                    BiomeEnergyParticles.CenterColor = new Color(209, 154, 0);
                    break;
                case Attunement.Cold:
                    BiomeEnergyParticles.EdgeColor = new Color(165, 235, 235);
                    BiomeEnergyParticles.CenterColor = new Color(58, 110, 141);
                    break;
                case Attunement.Tropical:
                    BiomeEnergyParticles.EdgeColor = new Color(53, 112, 4);
                    BiomeEnergyParticles.CenterColor = new Color(131, 173, 39);
                    break;
                case Attunement.Evil:
                    BiomeEnergyParticles.EdgeColor = new Color(112, 4, 35);
                    BiomeEnergyParticles.CenterColor = new Color(195, 42, 200);
                    break;
            }
            BiomeEnergyParticles.InterpolationSpeed = 0.1f;
            BiomeEnergyParticles.DrawSet(particleDrawCenter + Main.screenPosition);

            Vector2 displacement = Vector2.UnitX.RotatedBy(Main.GlobalTime * 3f) * 2f * (float)Math.Sin(Main.GlobalTime);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);
            spriteBatch.Draw(itemTexture, position + displacement, itemFrame, BiomeEnergyParticles.CenterColor, 0f, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(itemTexture, position - displacement, itemFrame, BiomeEnergyParticles.CenterColor, 0f, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.End();
            spriteBatch.Begin(default, default);

            return true;
        }


    }

    public class BrokenBiomeBladeVisuals : ModProjectile //Visuals
    {
        private Player Owner => Main.player[projectile.owner];
        public ref float AnimationMode => ref projectile.ai[0];
        private BezierCurve bezierCurve;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Broken Biome Blade");
        }
        public override string Texture => "CalamityMod/Items/Weapons/Melee/BiomeBlade";
        public override void SetDefaults()
        {
            projectile.width = projectile.height = 36;
            projectile.aiStyle = -1;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 30;
            projectile.tileCollide = false;
            projectile.damage = 0;
        }

        public override void AI()
        {
            if (Owner.HeldItem.type != ItemType<BiomeBlade>())
            {
                projectile.timeLeft = 0;
                return;
            }

            if (AnimationMode == 1f)
            {
                if (!Owner.StandingStill())
                {
                    projectile.timeLeft = 0;
                    return;
                }
                if (projectile.timeLeft == 30)
                {
                    List<Vector2> bezierPoints = new List<Vector2>() { projectile.position, projectile.position - Vector2.UnitY * 16f, projectile.position + Vector2.UnitY * 56f, projectile.position + Vector2.UnitY * 42f };
                    bezierCurve = new BezierCurve(bezierPoints.ToArray());
                }

                projectile.position = bezierCurve.Evaluate(1f - projectile.timeLeft / 30f);
                //projectile.rotation = MathHelper.Lerp(MathHelper.PiOver4, -(MathHelper.PiOver2 + MathHelper.PiOver4), 1f - MathHelper.Clamp((projectile.timeLeft / 10), 0f, 1f));
                //projectile.rotation = MathHelper.PiOver4 + MathHelper.PiOver2;
                projectile.rotation = Utils.AngleLerp(MathHelper.PiOver4 + MathHelper.PiOver2, -MathHelper.PiOver4, MathHelper.Clamp(((projectile.timeLeft - 20f) / 10f), 0f, 1f));

                if (projectile.timeLeft == 2)
                {
                    bool jungle = Owner.ZoneJungle;
                    bool ocean = Owner.ZoneBeach;
                    bool snow = Owner.ZoneSnow;
                    bool evil = Owner.ZoneCorrupt || Owner.ZoneCrimson;
                    bool desert = Owner.ZoneDesert;
                    bool hell = Owner.ZoneUnderworldHeight;

                    BiomeBlade.Attunement attunement = BiomeBlade.Attunement.Default;

                    if (jungle || ocean)
                    {
                        attunement = BiomeBlade.Attunement.Tropical;
                    }
                    if (desert || hell)
                    {
                        attunement = BiomeBlade.Attunement.Hot;
                    }
                    if (snow)
                    {
                        attunement = BiomeBlade.Attunement.Cold;
                    }
                    if (evil)
                    {
                        attunement = BiomeBlade.Attunement.Evil;
                    }

                    if ((Owner.HeldItem.modItem as BiomeBlade).mainAttunement == attunement)
                    {
                        Main.PlaySound(SoundID.DD2_LightningBugZap, projectile.Center);
                        return;
                    }
                    Main.PlaySound(SoundID.DD2_MonkStaffGroundImpact, projectile.Center);
                    (Owner.HeldItem.modItem as BiomeBlade).secondaryAttunement = (Owner.HeldItem.modItem as BiomeBlade).mainAttunement;
                    (Owner.HeldItem.modItem as BiomeBlade).mainAttunement = attunement;

                    //Lots of particles!!! yay!! visuals!!
                }
            }

            //Cool particles
        }

        public override void Kill(int timeLeft)
        {
            //Cool particles
        }
    }

    public class PurityProjection : ModProjectile //The boring plain one
    {
        private int dustType = 3;

        public override string Texture => "CalamityMod/Projectiles/Melee/BrokenBiomeBlade_PurityProjection";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Purity Projection");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 32;
            projectile.aiStyle = 27;
            aiType = ProjectileID.LightBeam;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 100;
            projectile.melee = true;
            projectile.tileCollide = false;
        }

        public override void AI() //Dust doesnt look great
        {
            if (projectile.timeLeft < 95)
                projectile.tileCollide = true;
            int dustParticle = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, dustType, 0f, 0f, 100, default, 0.9f);
            Main.dust[dustParticle].noGravity = true;
            Main.dust[dustParticle].velocity *= 0.5f;
            Main.dust[dustParticle].velocity += projectile.velocity * 0.1f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.timeLeft > 295)
                return false;

            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            //Explode into dust or whatever,
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            int debuffTime = 90;
            target.AddBuff(ModContent.BuffType<ArmorCrunch>(), debuffTime);
        }
    }

    public class DecaysRetort : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Melee/BrokenBiomeBlade_DecaysRetort";
        private bool initialized = false;
        Vector2 direction = Vector2.Zero;
        public ref float MaxTime => ref projectile.ai[0];
        public ref float CanLunge => ref projectile.ai[1];
        public float Timer => MaxTime - projectile.timeLeft;
        public Player Owner => Main.player[projectile.owner];
        public const float LungeSpeed = 16;
        public ref float CanBounce => ref projectile.localAI[0];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Decay's retort");
        }
        public override void SetDefaults()
        {
            projectile.melee = true;
            projectile.width = projectile.height = 84;
            projectile.width = projectile.height = 84;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.extraUpdates = 1;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;
            float bladeLenght = 120f * projectile.scale;
            Vector2 displace = direction * ((float)Math.Sin(Timer / MaxTime * 3.14f) * 60);

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Owner.Center + displace, Owner.Center + displace + (direction * bladeLenght), 24, ref collisionPoint);
        }

        public override void AI()
        {
            if (!initialized) //Initialization
            {
                CanBounce = 1f;
                projectile.timeLeft = (int)MaxTime;
                //Take the direction the sword is swung. FUCK not controlling the swing direction more than just left/right :|
                //The direction to mouseworld may need to be turned into the custom synced player mouse variables . not on the branch currently tho
                direction = Owner.DirectionTo(Main.MouseWorld);
                direction.Normalize();
                projectile.rotation = direction.ToRotation();
                if (CanLunge == 1f)
                    Lunge();
                Main.PlaySound(SoundID.Item103, projectile.Center);
                initialized = true;
            }

            //Manage position and rotation
            projectile.scale = 1f + ((float)Math.Sin(Timer / MaxTime * MathHelper.Pi) * 0.6f); //SWAGGER
            projectile.Center = Owner.Center + (direction * ((float)Math.Sin(Timer / MaxTime * MathHelper.Pi) * 60));

            //Make the owner look like theyre holding the sword bla bla
            Owner.heldProj = projectile.whoAmI;
            Owner.direction = Math.Sign(direction.X);
            Owner.itemRotation = direction.ToRotation();
            if (Owner.direction != 1)
            {
                Owner.itemRotation -= 3.14f;
            }
            Owner.itemRotation = MathHelper.WrapAngle(Owner.itemRotation);
        }

        public void Lunge()
        {
            if (Main.myPlayer != projectile.owner)
                return;
            //Check if the owner is on the ground
            Owner.velocity = direction.SafeNormalize(Vector2.UnitX * Owner.direction) * LungeSpeed;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) => OnHitEffects(!target.canGhostHeal || Main.player[projectile.owner].moonLeech);
        public override void OnHitPvp(Player target, int damage, bool crit) => OnHitEffects(Main.player[projectile.owner].moonLeech);
        
        private void OnHitEffects(bool cannotLifesteal)
        {
            if (!cannotLifesteal) //trolled
            {
                Owner.statLife += 3;
                Owner.HealEffect(3); //Idk if its too much or what but at the same time its close range as fuck
            }
            if (Main.myPlayer != Owner.whoAmI || CanBounce == 0f)
                return;
            // Bounce off
            float bounceStrength = Math.Max((LungeSpeed / 2f), Owner.velocity.Length());
            bounceStrength *= Owner.velocity.Y == 0 ? 0.2f : 1f; //Reduce the bounce if the player is on the ground 
            Owner.velocity = -direction.SafeNormalize(Vector2.Zero) * MathHelper.Clamp(bounceStrength, 0f, 22f);
            CanBounce = 0f;
            Owner.GiveIFrames(10); // 10 i frames for free!
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D handle = GetTexture("CalamityMod/Items/Weapons/Melee/BiomeBlade");
            Texture2D tex = GetTexture("CalamityMod/Projectiles/Melee/BrokenBiomeBlade_DecaysRetort");

            float drawAngle = direction.ToRotation();
            float drawRotation = drawAngle + MathHelper.PiOver4;

            Vector2 displace = direction * ((float)Math.Sin(Timer / MaxTime * 3.14f) * 60);
            Vector2 drawOrigin = new Vector2(0f, handle.Height);
            Vector2 drawOffset = Owner.Center + direction * 10f - Main.screenPosition;

            spriteBatch.Draw(handle, drawOffset + displace, null, lightColor, drawRotation, drawOrigin, projectile.scale, 0f, 0f);

            //Turn on additive blending
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            //Update the parameters
            drawOrigin = new Vector2(0f, tex.Height);
            drawOffset = Owner.Center + (drawAngle.ToRotationVector2() * 24f * projectile.scale) - Main.screenPosition;

            spriteBatch.Draw(tex, drawOffset + displace, null, lightColor * 0.9f, drawRotation, drawOrigin, projectile.scale, 0f, 0f);
                //Back to normal
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

    }

    public class BitingEmbrace : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Melee/BrokenBiomeBlade_BitingEmbraceSmall";
        private bool initialized = false;
        Vector2 direction = Vector2.Zero;
        public float rotation;
        public ref float SwingMode => ref projectile.ai[0]; //0 = Up-Down small slash, 1 = Down-Up large slash, 2 = Thrust
        public ref float MaxTime => ref projectile.ai[1];
        public float Timer => MaxTime - projectile.timeLeft;

        public int SwingDirection
        {
            get
            {
                switch (SwingMode)
                {
                    case 0:
                        return -1 * Math.Sign(direction.X);
                    case 1:
                        return 1 * Math.Sign(direction.X);
                    default:
                        return 0;

                }
            }
        }
        public float SwingWidth
        {
            get
            {
                switch (SwingMode)
                {
                    case 0:
                        return 2.3f;
                    default:
                        return 1.8f;
                }
            }
        }

        public Player Owner => Main.player[projectile.owner];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Biting Embrace");
            Main.projFrames[projectile.type] = 6; //The true trolling is that we only really use this for the third swing.
        }
        public override void SetDefaults()
        {
            projectile.melee = true;
            projectile.width = projectile.height = 75;
            projectile.width = projectile.height = 75;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.extraUpdates = 1;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            //The hitbox is simplified into a line collision.
            float collisionPoint = 0f;
            float bladeLenght = 0f;
            Vector2 displace = Vector2.Zero;
            switch (SwingMode)
            {
                case 0:
                    bladeLenght = 90f * projectile.scale;
                    break;
                case 1:
                    bladeLenght = 110f * projectile.scale;
                    break;
                case 2:
                    bladeLenght = projectile.frame <= 2 ? 85f : 150f; //Only use the extended hitbox after the blade actually extends. For realism.
                    bladeLenght *= projectile.scale;
                    displace = direction * ((float)Math.Sin(Timer / MaxTime * 3.14f) * 60);
                    break;

            }
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Owner.Center + displace, Owner.Center + displace +(rotation.ToRotationVector2() * bladeLenght), 24, ref collisionPoint);
        }

        public override void AI()
        {
            if (!initialized) //Initialization
            {
                projectile.timeLeft = (int)MaxTime;
                switch (SwingMode)
                {
                    case 0:
                        projectile.width = projectile.height = 100;
                        Main.PlaySound(SoundID.DD2_MonkStaffSwing, projectile.Center);
                        projectile.damage = (int)(projectile.damage * 1.5);
                        break;
                    case 1:
                        projectile.width = projectile.height = 140;
                        Main.PlaySound(SoundID.DD2_OgreSpit, projectile.Center);
                        projectile.damage = (int)(projectile.damage * 1.8);
                        break;
                    case 2:
                        projectile.width = projectile.height = 130;
                        Main.PlaySound(SoundID.DD2_PhantomPhoenixShot, projectile.Center);
                        projectile.damage *= 3;
                        break;
                }

                //Take the direction the sword is swung. FUCK not controlling the swing direction more than just left/right :|
                //The direction to mouseworld may need to be turned into the custom synced player mouse variables . not on the branch currently tho
                direction = Owner.DirectionTo(Main.MouseWorld);
                direction.Normalize();
                projectile.rotation = direction.ToRotation();

                initialized = true;
            }
            //Manage position and rotation
            projectile.Center = Owner.Center + (direction * 30);
            //rotation = projectile.rotation + MathHelper.SmoothStep(SwingWidth / 2 * SwingDirection, -SwingWidth / 2 * SwingDirection, Timer / MaxTime); 
            float factor = 1 - (float)Math.Pow((double)-(Timer / MaxTime) + 1, 2d);
            rotation = projectile.rotation + MathHelper.Lerp(SwingWidth / 2 * SwingDirection, -SwingWidth / 2 * SwingDirection, factor); 
            projectile.scale = 1f + ((float)Math.Sin(Timer / MaxTime * MathHelper.Pi) * 0.6f); //SWAGGER

            //Add the thrust motion & animation for the third combo state
            if (SwingMode == 2)
            {
                projectile.scale = 1f + (thrustRatio() * 0.6f);
                projectile.Center = Owner.Center + (direction * thrustRatio() * 60);
                
                projectile.frameCounter++;
                 if (projectile.frameCounter % 5 == 0 && projectile.frame + 1 < Main.projFrames[projectile.type])
                     projectile.frame++;
                
            }

            //Make the owner look like theyre holding the sword bla bla
            Owner.heldProj = projectile.whoAmI;
            Owner.direction = Math.Sign(rotation.ToRotationVector2().X);
            Owner.itemRotation = rotation;
            if (Owner.direction != 1)
            {
                Owner.itemRotation -= 3.14f;
            }
            Owner.itemRotation = MathHelper.WrapAngle(Owner.itemRotation);
        }

        internal float thrustRatio()
        {
            float ratio; //L + Ratio
            if (Timer / MaxTime < 0.2)
                ratio = (float)(Math.Sin(Timer / (MaxTime) * MathHelper.PiOver2) * 0.2f); //Small anticipation period.
            else
                ratio = MathHelper.Clamp((float)Math.Sin((Timer / (MaxTime * 2) + 0.5f) * 3.14f) * 1.5f, 0f, 1f);


            //if (Timer/MaxTime > 0.6)
            //{
            //    ratio = MathHelper.Lerp(ratio, 1f, ((Timer - (MaxTime / 5f * 3)) / (MaxTime - (MaxTime / 5f * 2))) + 0.6f);
            //}

            return ratio;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Color color = Color.White;
            color.A = 120;
            color *= 0.8f;
            Texture2D handle = GetTexture("CalamityMod/Items/Weapons/Melee/BiomeBlade");

            if (SwingMode != 2)
            {
                Texture2D tex = GetTexture("CalamityMod/Projectiles/Melee/BrokenBiomeBlade_BitingEmbrace" + (SwingMode == 0 ? "Small" : "Big"));
                float drawAngle = rotation;
                float drawRotation = rotation + MathHelper.PiOver4;
                Vector2 drawOrigin = new Vector2(0f, handle.Height);
                Vector2 drawOffset = Owner.Center + drawAngle.ToRotationVector2() * 10f - Main.screenPosition;

                spriteBatch.Draw(handle, drawOffset, null, lightColor, drawRotation, drawOrigin, projectile.scale, 0f, 0f);
                //Turn on additive blending
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                //Update the parameters
                drawOrigin = new Vector2(0f, tex.Height);
                drawOffset = Owner.Center + (drawAngle.ToRotationVector2() * 28f * projectile.scale)- Main.screenPosition;
                spriteBatch.Draw(tex, drawOffset, null, lightColor * 0.8f, drawRotation, drawOrigin, projectile.scale, 0f, 0f);
                //Back to normal
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            }
            else
            {
                Texture2D tex = GetTexture("CalamityMod/Projectiles/Melee/BrokenBiomeBlade_BitingEmbraceThrust");
                Vector2 displace = direction * (thrustRatio() * 60);

                float drawAngle = rotation;
                float drawRotation = rotation + MathHelper.PiOver4;
                Vector2 drawOrigin = new Vector2(0f, handle.Height);
                Vector2 drawOffset = Owner.Center + drawAngle.ToRotationVector2() * 10f - Main.screenPosition + (direction * 20f * CalamityUtils.Convert01To010((Timer - (MaxTime / 5f)) / (MaxTime / 5f)));

                spriteBatch.Draw(handle, drawOffset + displace, null, lightColor, drawRotation, drawOrigin, projectile.scale, 0f, 0f);
                //Turn on additive blending
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                //Update the parameters
                drawOrigin = new Vector2(0f, 114f);
                drawOffset = Owner.Center + (drawAngle.ToRotationVector2() * 28f * projectile.scale) - Main.screenPosition + (direction * 20f * CalamityUtils.Convert01To010((Timer - (MaxTime / 5f)) / (MaxTime / 5f)));
                //Anim stuff
                Rectangle frameRectangle = new Rectangle(0, 0 + (116 * projectile.frame), 114, 114);

                spriteBatch.Draw(tex, drawOffset + displace, frameRectangle, lightColor * 0.9f, drawRotation, drawOrigin, projectile.scale, 0f, 0f);
                //Back to normal
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            }

            return false;
        }

    }

    public class AridGrandeur : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Melee/BrokenBiomeBlade_AridGrandeur";
        private bool initialized = false;
        Vector2 direction = Vector2.Zero;
        public ref float Shred => ref projectile.ai[0]; //How much the attack is, attacking
        public float ShredRatio => MathHelper.Clamp(Shred / (maxShred * 0.5f), 0f, 1f);
        public ref float PogoCooldown => ref projectile.ai[1]; //Cooldown for the pogo
        public Player Owner => Main.player[projectile.owner];
        public bool CanPogo => Owner.velocity.Y != 0 && PogoCooldown <= 0; //Only pogo when in the air and if the cooldown is zero
        private bool OwnerCanShoot => Owner.channel && !Owner.noItems && !Owner.CCed;

        public const float pogoStrenght = 16f; //How much the player gets pogoed up
        public const float maxShred = 500; //How much shred you get

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Arid Grandeur");
        }
        public override void SetDefaults()
        {
            projectile.melee = true;
            projectile.width = projectile.height = 70;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.extraUpdates = 1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 16;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;
            float bladeLenght = 84 * projectile.scale;
            float bladeWidth = 76 * projectile.scale;

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Owner.Center, Owner.Center + (direction * bladeLenght), bladeWidth, ref collisionPoint);
        }

        public void Pogo()
         {
             if (CanPogo && Main.myPlayer == Owner.whoAmI)
            {
                Owner.velocity = -direction.SafeNormalize(Vector2.Zero) * pogoStrenght; //Bounce
                Owner.fallStart = (int)(Owner.position.Y / 16f);
                PogoCooldown = 30; //Cooldown
                Main.PlaySound(SoundID.DD2_MonkStaffGroundImpact, projectile.position);     
            }
        }

        public override void AI()
        {
            if (!initialized) //Initialization. Here its litterally just playing a sound tho lmfao
            {
                Main.PlaySound(SoundID.Item90, projectile.Center);
                initialized = true;
            }

            if (!OwnerCanShoot)
            {
                projectile.Kill();
                return;
            }


            if (Shred >= maxShred)
                Shred = maxShred;
            if (Shred < 0)
                Shred = 0;

            //Manage position and rotation
            direction = Owner.DirectionTo(Main.MouseWorld);
            direction.Normalize();
            projectile.rotation = direction.ToRotation();
            projectile.Center = Owner.Center + (direction * 60);
            
            //Scaling based on shred
            projectile.localNPCHitCooldown = 16 - (int)(MathHelper.Lerp(0, 8, ShredRatio)); //Increase the hit frequency
            projectile.scale = 1f + (ShredRatio * 1f); //SWAGGER


            if (Collision.SolidCollision(Owner.Center + (direction * 84 * projectile.scale) - Vector2.One*5f , 10, 10))
            {
                Pogo();
            }

            //Make the owner look like theyre holding the sword bla bla
            Owner.heldProj = projectile.whoAmI;
            Owner.direction = Math.Sign(direction.X);
            Owner.itemRotation = direction.ToRotation();
            if (Owner.direction != 1)
            {
                Owner.itemRotation -= 3.14f;
            }
            Owner.itemRotation = MathHelper.WrapAngle(Owner.itemRotation);
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;

            Shred--;
            PogoCooldown--;
            projectile.timeLeft = 2;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) => ShredTarget(target);
        public override void OnHitPvp(Player target, int damage, bool crit) => ShredTarget(target);

        private void ShredTarget(Entity target)
        {
            Main.PlaySound(SoundID.NPCHit30, projectile.Center); //Sizzle
            Shred += 62; //Augment the shredspeed
            if (Main.myPlayer != Owner.whoAmI)
                return;
            // get lifted up
            if (PogoCooldown <= 0)
            {
                if (Owner.velocity.Y > 0)
                    Owner.velocity.Y = -2f ; //Get "stuck" into the enemy partly

                Owner.GiveIFrames(5); // i framez. Do 5 iframes even matter? idk but you get a lot of em so lol...
                PogoCooldown = 20;
            }
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.NPCHit43, projectile.Center);
            if (ShredRatio > 0.5 && Owner.whoAmI == Main.myPlayer)
            {
                Projectile.NewProjectile(projectile.Center, direction * 16f, ProjectileType<AridGrandeurShot>(), projectile.damage, projectile.knockBack, Owner.whoAmI, Shred);
                //sharticles
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D handle = GetTexture("CalamityMod/Items/Weapons/Melee/BiomeBlade");
            Texture2D tex = GetTexture("CalamityMod/Projectiles/Melee/BrokenBiomeBlade_AridGrandeur");

            int bladeAmount = 4;

            float drawAngle = direction.ToRotation();
            float drawRotation = drawAngle + MathHelper.PiOver4;

            Vector2 drawOrigin = new Vector2(0f, handle.Height);
            Vector2 drawOffset = Owner.Center + direction * 10f - Main.screenPosition;

            spriteBatch.Draw(handle, drawOffset, null, lightColor, drawRotation, drawOrigin, projectile.scale, 0f, 0f);

            //Turn on additive blending
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            //Update the parameters
            drawOrigin = new Vector2(0f, tex.Height);
            drawOffset = Owner.Center + (drawAngle.ToRotationVector2() * 32f * projectile.scale) - Main.screenPosition;

            spriteBatch.Draw(tex, drawOffset, null, lightColor * 0.9f, drawRotation, drawOrigin, projectile.scale, 0f, 0f);


            for (int i = 0; i < bladeAmount; i++) //Draw extra copies
            {
                tex = GetTexture("CalamityMod/Projectiles/Melee/BrokenBiomeBlade_AridGrandeurExtra");

                drawAngle = direction.ToRotation();

                float circleCompletion = (float)Math.Sin(Main.GlobalTime * 5 + i * MathHelper.PiOver2);
                drawRotation = drawAngle + MathHelper.PiOver4 + (circleCompletion * MathHelper.Pi / 10f) - (circleCompletion * (MathHelper.Pi/7f) * ShredRatio);

                drawOrigin = new Vector2(0f, tex.Height);


                Vector2 drawOffsetStraight = Owner.Center + direction * (float)Math.Sin(Main.GlobalTime*7)*10 - Main.screenPosition; //How far from the player
                Vector2 drawDisplacementAngle =  direction.RotatedBy(MathHelper.PiOver2) * circleCompletion.ToRotationVector2().Y * (20 + 40 * ShredRatio); //How far perpendicularly

                spriteBatch.Draw(tex, drawOffsetStraight + drawDisplacementAngle, null, lightColor * 0.8f, drawRotation, drawOrigin, projectile.scale, 0f, 0f);
            }
                
            //Back to normal
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

    }

    public class AridGrandeurShot : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Melee/BrokenBiomeBlade_AridGrandeurExtra";
        private bool initialized = false;
        Vector2 direction = Vector2.Zero;
        public ref float Shred => ref projectile.ai[0]; 
        public float ShredRatio => MathHelper.Clamp(Shred / (maxShred * 0.5f), 0f, 1f);
        public Player Owner => Main.player[projectile.owner];

        public const float pogoStrenght = 16f; //How much the player gets pogoed up
        public const float maxShred = 500; //How much shred you get

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Arid Shredder");
        }
        public override void SetDefaults()
        {
            projectile.melee = true;
            projectile.width = projectile.height = 70;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.extraUpdates = 1;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;
            float bladeLenght = 84 * projectile.scale;
            float bladeWidth = 76 * projectile.scale;

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center - direction * bladeLenght / 2, projectile.Center + direction * bladeLenght / 2, bladeWidth, ref collisionPoint);
        }

        public override void AI()
        {
            if (!initialized) 
            {
                Main.PlaySound(SoundID.Item90, projectile.Center);
                projectile.timeLeft = (int)(30f + ShredRatio * 30f);
                initialized = true;

                direction = Owner.DirectionTo(Main.MouseWorld);
                direction.Normalize();
                projectile.rotation = direction.ToRotation();

                projectile.velocity = direction * 6f;
                projectile.damage *= 10;

                projectile.scale = 1f + (ShredRatio * 1f); //SWAGGER
                projectile.netUpdate = true;

            }

            projectile.position += projectile.velocity;
        
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            for (int i = 0; i < 4; i++) //Draw extra copies
            {
                var tex = GetTexture("CalamityMod/Projectiles/Melee/BrokenBiomeBlade_AridGrandeurExtra");

                float drawAngle = direction.ToRotation();

                float circleCompletion = (float)Math.Sin(Main.GlobalTime * 5 + i * MathHelper.PiOver2);
                float drawRotation = drawAngle + MathHelper.PiOver4 + (circleCompletion * MathHelper.Pi / 10f) - (circleCompletion * (MathHelper.Pi / 7f) * ShredRatio);

                Vector2 drawOrigin = new Vector2(0f, tex.Height);


                Vector2 drawOffsetStraight = projectile.Center + direction * (float)Math.Sin(Main.GlobalTime * 7) * 10 - Main.screenPosition; //How far from the player
                Vector2 drawDisplacementAngle = direction.RotatedBy(MathHelper.PiOver2) * circleCompletion.ToRotationVector2().Y * (20 + 40 * ShredRatio); //How far perpendicularly

                spriteBatch.Draw(tex, drawOffsetStraight + drawDisplacementAngle, null, lightColor * 0.8f, drawRotation, drawOrigin, projectile.scale, 0f, 0f);
            }

            //Back to normal
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

    }

}


