using System;
using System.Collections.Generic;
using CalamityMod.Items.Materials;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Weapons.Melee
{
    [LegacyName("GalacticaBlade")]
    public class GalactusBlade : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        private int swordDirection;
        public int time = 0;
        private float swingRotation = 0;
        public Color useColor = Color.White;
        public bool spawnProj = true;

        public override void SetDefaults()
        {
            Item.width = 144;
            Item.height = 146;
            Item.damage = 185;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 27;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 27;
            Item.useTurn = true;
            Item.knockBack = 17f;
            Item.UseSound = SoundID.Item105;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            Item.rare = RarityType<Turquoise>();
            Item.shoot = ProjectileType<GalacticaComet>();
            Item.shootSpeed = 13f;
            Item.scale = 0.75f;
        }
        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, Request<Texture2D>(Texture + "Glow").Value);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return false;
        }
        public override void UseAnimation(Player player)
        {
            swordDirection = (player.Center - player.Calamity().mouseWorld).X > 1 ? -1 : 1;
            time = 0;
            swingRotation = 0;
            spawnProj = true;
        }
        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            int swordSize = (int)(33 * Item.scale);

            float rate = Main.GlobalTimeWrappedHourly * 18;
            List<Color> eColors = new List<Color>()
            {
                Color.Gold,
                Color.HotPink,
                Color.Cyan
            };
            int colorIndex = (int)(rate / 2 % eColors.Count);
            Color currentColor = eColors[colorIndex];
            Color nextColor = eColors[(colorIndex + 1) % eColors.Count];
            useColor = Color.Lerp(currentColor, nextColor, rate % 2f > 1f ? 1f : rate % 1f);

            player.itemRotation = swingRotation - 1.7f * swordDirection;
            player.itemLocation = player.Center;
            player.direction = swordDirection;

            float val = MathF.Abs(time - player.itemAnimationMax * 0.75f) / player.itemAnimationMax;

            float goalRot = Utils.Remap(time, 0, player.itemAnimationMax, -0.5f, 5.2f) * swordDirection;
            float swingEasing = Utils.GetLerpValue(0, player.itemAnimationMax * 0.4f, time, true) * (0.35f - val);
            if (time < player.itemAnimationMax)
            {
                swingRotation = MathHelper.Lerp(swingRotation, goalRot, swingEasing);
            }

            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, swingRotation + MathHelper.ToRadians(120f * swordDirection));

            if (val < 0.4f)
            {
                if (spawnProj)
                {
                    for (int i = 0; i < 7; i++)
                    {
                        Vector2 clampedMouse = player.ClampedMouseWorld();
                        Vector2 spawnSpot = new Vector2(clampedMouse.X, player.Center.Y) + new Vector2(Main.rand.NextFloat(-850, 850), Main.rand.NextFloat(-750, -1250));
                        Projectile comet = Projectile.NewProjectileDirect(player.GetSource_ItemUse(Item), spawnSpot, Utils.DirectionTo(spawnSpot, player.Calamity().mouseWorld + Main.rand.NextVector2Circular(50, 50)) * Item.shootSpeed, Item.shoot, Item.damage, Item.knockBack, player.whoAmI);
                        comet.extraUpdates = Main.rand.Next(2, 3 + 1);
                    }
                    spawnProj = false;
                }
                for (int i = 0; i < 3; i++)
                {
                    Vector2 dustVel = new Vector2(5 * swordDirection, -5).RotatedBy(swingRotation - 1.7f * swordDirection);

                    float partScale = Main.rand.NextFloat(0.6f, 0.9f);
                    Vector2 partVel = (dustVel * Main.rand.NextFloat(0.2f, 0.3f)).RotatedBy(MathHelper.ToRadians(90f * swordDirection)).RotatedByRandom(-0.2) * -3;
                    Vector2 partPos = player.Center + dustVel.RotatedByRandom(0.4f) * swordSize;

                    Particle spark2 = new SparkParticle(partPos, partVel, false, 14, partScale * 0.6f, useColor);
                    GeneralParticleHandler.SpawnParticle(spark2);
                }
            }
            
            Vector2 dustVel2 = new Vector2(5 * swordDirection, -5).RotatedBy(swingRotation - 1.7f * swordDirection);

            float partScale2 = Main.rand.NextFloat(0.3f, 0.7f);
            Vector2 partVel2 = dustVel2 * Main.rand.NextFloat(0.2f, 0.3f);
            for (int i = 0; i < 5; i++)
            {
                Particle smoke2 = new CustomSpark(player.Center + dustVel2 * Main.rand.Next(1, swordSize + 1) + Main.rand.NextVector2Circular(12, 12), partVel2.RotatedBy(MathHelper.ToRadians(90f * swordDirection)).RotatedBy(-0.3 * swordDirection) * Main.rand.NextFloat(-1, -10), "CalamityMod/Particles/SmallBloom", false, 12, partScale2 * 0.5f, useColor * 0.4f, Vector2.One, true, false, 3, false, false);
                GeneralParticleHandler.SpawnParticle(smoke2);
            }
            Lighting.AddLight(player.Center + dustVel2, Color.White.ToVector3() * 1.2f);
            time++;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.StarWrath).
                AddIngredient<DivineGeode>(10).
                AddIngredient(ItemID.FragmentSolar, 5).
                AddIngredient(ItemID.FragmentVortex, 5).
                AddIngredient(ItemID.FragmentNebula, 5).
                AddIngredient(ItemID.FragmentStardust, 5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
