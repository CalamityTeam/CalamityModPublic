using System;
using System.Collections.Generic;
using System.Linq;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Rarities;
using CalamityMod.Systems.Collections;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    [LegacyName("GreatswordofBlah")]
    [LegacyName("IridescentExcalibur")]
    public class Orderbringer : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public int time = 0;
        public Vector2 bladeHitboxPos;
        public float bladeRotation = 0;
        public int bladeDirection = 0;
        public float completion = 0;
        public bool canHit => (completion >= 0.35f && completion <= 0.8f);
        public int swingCount = 0;
        public bool spawnProj = true;
        public bool spawnTrueMeleeProj = true;
        public bool playSound = true;
        public float scaling = 1;
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<ElementalMix>()];
        }
        public override void SetDefaults()
        {
            Item.width = Item.height = 112;
            Item.damage = 800;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = Item.useTime = 16;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.knockBack = 8f;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.rare = ModContent.RarityType<BurnishedAuric>();
            Item.shoot = ModContent.ProjectileType<OrderbringerWaveProj>();
            Item.shootSpeed = 5.5f;
        }
        public override void ModifyItemScale(Player player, ref float scale)
        {
            scale = scaling = player.GetMeleeScale();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return false;
        }
        public override void UseItemHitbox(Player player, ref Rectangle hitbox, ref bool noHitbox)
        {
            // Basically just insures the enemy is always able to be hit if they're in range, the actual collision check is done in CanHitNPC
            float scale = 8f * scaling;
            Vector2 newSize = new Point(hitbox.Width, hitbox.Height).ToVector2() * scale;
            hitbox = new Rectangle((int)(bladeHitboxPos.X - newSize.X / 2f), (int)(bladeHitboxPos.Y - newSize.Y / 2f), (int)newSize.X, (int)newSize.Y);
        }
        public override bool? CanHitNPC(Player player, NPC target)
        {
            Vector2 mPos = player.Calamity().mouseWorld;
            Vector2 shootDir = player.Center.DirectionTo(mPos);
            float _ = float.NaN;
            bool hitCheck = Collision.CheckAABBvLineCollision(target.Hitbox.TopLeft(), target.Hitbox.Size(), player.Center - shootDir * 30 * scaling, player.Center + shootDir * 215 * scaling, Item.width * 3f * scaling, ref _);
            return ((canHit && hitCheck) ? null : false);
        }
        public override void UseAnimation(Player player)
        {
            swingCount++;
            time = 0;
            bladeDirection = player.direction;
            bladeHitboxPos = player.Center;
            bladeRotation = 0;
            spawnProj = true;
            spawnTrueMeleeProj = true;
            playSound = true;
            int dir = -Math.Sign(player.Center.X - player.Calamity().mouseWorld.X);
            float startRot = MathHelper.ToRadians(-90) * dir * (swingCount % 2 == 0 ? 1 : -1);
        }
        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            Vector2 mPos = player.Calamity().mouseWorld;
            completion = (float)((float)time / ((float)(Item.useAnimation / player.GetAttackSpeed(DamageClass.Melee))));
            int dir = -Math.Sign(player.Center.X - mPos.X);

            float startRot = MathHelper.ToRadians(-110) * dir * (swingCount % 2 == 0 ? 1 : -1);
            float endRot = MathHelper.ToRadians(-110) * dir * (swingCount % 2 == 0 ? -1 : 1);
            float minRot = MathHelper.ToRadians(-150) * dir * (swingCount % 2 == 0 ? 1 : -1);
            float cutoff = 0.2f;
            float cutoff2 = 0.95f;

            Vector2 shootDir = player.Center.DirectionTo(mPos) * Item.shootSpeed;

            if (completion <= cutoff)
            {
                float lerp = Utils.GetLerpValue(0, cutoff, completion, true);
                player.itemRotation = player.Center.DirectionTo(mPos).ToRotation() + MathHelper.Lerp(startRot, minRot, CalamityUtils.EaseInOutExp(lerp, 4f, 4f));
                player.itemRotation += (MathHelper.Pi) * (dir == 1 ? 0 : 1) + MathHelper.PiOver4 * dir;
            }
            else
            {
                if (playSound)
                {
                    SoundStyle swing = new("CalamityMod/Sounds/Item/SwooshMid");
                    SoundEngine.PlaySound(swing with { Pitch = Main.rand.NextFloat(0.1f, 0.3f), Volume = 1f }, player.Center);
                    playSound = false;
                }
                if (completion >= 0.65f && spawnProj)
                {
                    if (!player.Calamity().bladeArmEnchant) // Manually remove projectiles when Tainted
                    {
                        Projectile beam = Projectile.NewProjectileDirect(player.GetSource_FromThis(), player.Center, shootDir, Item.shoot, Item.damage, Item.knockBack, player.whoAmI, 0f);
                        beam.ai[1] = scaling;
                        
                        for (int i = 0; i < 4; i++)
                        {
                            float rot = Main.rand.NextFloat(0.5f, 0.65f) * (Main.rand.NextBool() ? 1 : -1);
                            Vector2 vel = (shootDir * 2.5f).RotatedByRandom(0.6f);
                            Projectile star = Projectile.NewProjectileDirect(player.GetSource_FromThis(), player.ClampedMouseWorld() - (shootDir * 40) + Main.rand.NextVector2Circular(130, 130), vel.RotatedBy(rot) * Main.rand.NextFloat(0.9f, 1.2f), ModContent.ProjectileType<StarofOrder>(), (int)(Item.damage * 0.12f), (int)(Item.knockBack * 0.2f), player.whoAmI, 0, rot, 1);
                            star.scale = scaling;
                        }
                    }

                    Particle swipe = new CustomSpark(player.Center - shootDir * 5 * scaling, shootDir.RotatedBy(0.4f * (dir * (swingCount % 2 == 0 ? 1 : -1))) * 2.5f, "CalamityMod/Particles/VerticalSmearLarge", false, (int)(14 / player.GetAttackSpeed(DamageClass.Melee)), 0.8f * scaling, player.Calamity().lightRGB, new Vector2(1f, 1f), true, false, 0, false, false);
                    GeneralParticleHandler.SpawnParticle(swipe);

                    SoundEngine.PlaySound(SoundID.Item60, player.Center);
                    spawnProj = false;
                }
                float lerp = Utils.GetLerpValue(cutoff, cutoff2, completion, true);
                player.itemRotation = player.Center.DirectionTo(mPos).ToRotation() + MathHelper.Lerp(minRot, endRot, CalamityUtils.EaseInOutExp(lerp, 6f, 2f));
                player.itemRotation += (MathHelper.Pi) * (dir == 1 ? 0 : 1) + MathHelper.PiOver4 * dir;
            }

            float extraRot = (dir == 1 ? -MathHelper.PiOver4 : MathHelper.ToRadians(225));
            bladeHitboxPos = player.Center + (player.itemRotation + extraRot).ToRotationVector2() * 240 * scaling;
            Vector2 tipPos = player.Center + (player.itemRotation + extraRot).ToRotationVector2() * 180 * scaling;

            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, player.itemRotation + MathHelper.ToRadians(-130) * dir);
            player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, player.itemRotation + MathHelper.ToRadians(-130) * dir);

            player.itemLocation = player.Center;
            player.direction = dir;

            time++;
        }
        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (spawnTrueMeleeProj)
            {
                int beamDamage = player.CalcIntDamage<MeleeDamageClass>(Item.damage * 0.10f);
                Vector2 mouseClamped = player.ClampedMouseWorld();
                SoundEngine.PlaySound(SoundID.Item84 with { Volume = 1f, Pitch = Main.rand.NextFloat(0.5f, 0.7f) }, player.Center);
                for (int i = 0; i < 2; i++)
                {
                    Vector2 targetPos = Main.MouseWorld;
                    NPC target2 = Main.MouseWorld.ClosestNPCAt(650);
                    if (target2 != null)
                        targetPos = target2.Center;

                    Vector2 spawnPos = mouseClamped + new Vector2(Main.rand.NextFloat(-300, 300), -900);
                    Vector2 vel = (targetPos - spawnPos).SafeNormalize(Vector2.UnitY) * 10;
                    Projectile skybeam = Projectile.NewProjectileDirect(player.GetSource_ItemUse(Item), spawnPos, vel, ModContent.ProjectileType<OrderbringerBeam>(), beamDamage, 0, player.whoAmI);
                    skybeam.scale = scaling;
                }
                spawnTrueMeleeProj = false;
            }
        }
        public override void ModifyTooltips(List<TooltipLine> list)
        {
            Player Owner = Main.LocalPlayer;
            if (Owner is null)
                return;

            float rate = Main.GlobalTimeWrappedHourly * 7;
            List<Color> eColors = new List<Color>()
                {
                    Color.PaleVioletRed,
                    Color.Coral,
                    Color.Khaki,
                    Color.PaleGreen,
                    Color.Turquoise,
                    Color.Violet
                };

            int colorIndex = (int)(rate / 2 % eColors.Count);
            Color currentColor = eColors[colorIndex];
            Color nextColor = eColors[(colorIndex + 1) % eColors.Count];
            Color finalColor = Color.Lerp(currentColor, nextColor, rate % 2f > 1f ? 1f : rate % 1f);
            TooltipLine line = list.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Tooltip2");
            if (line != null)
                line.OverrideColor = Color.Lerp(finalColor, Color.White, 0.3f);
        }
        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Melee/OrderbringerGlow").Value);
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<GreatswordofJudgement>().
                AddIngredient<AuricBar>(5).
                AddIngredient<LifeAlloy>(5).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
