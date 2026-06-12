using System;
using CalamityMod.Enums;
using CalamityMod.Items.Materials;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    [LegacyName("XerocsGreatsword")]
    public class EntropicClaymore : ModItem, ILocalizedModType
    {
        private int swordDirection;
        public int time = 0;
        private float swingRotation = 0;
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override void SetDefaults()
        {
            Item.width = 130;
            Item.height = 130;
            Item.damage = 90;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 30;
            Item.useTurn = true;
            Item.knockBack = 5.25f;
            Item.UseSound = new SoundStyle("CalamityMod/Sounds/Item/SwingMid") with { Volume = 0.5f, Pitch = Main.rand.NextFloat(-0.3f, -0.4f) };
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.RarityRedBuyPrice;
            Item.rare = ItemRarityID.Red;
            Item.shoot = ModContent.ProjectileType<EntropicFlechette>();
            Item.shootSpeed = 12f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 5; i++)
            {
                Projectile.NewProjectile(source, position, velocity.RotatedByRandom(0.6f) * Main.rand.NextFloat(0.7f, 1.1f), type, damage / 2, knockback * 0.5f, player.whoAmI);
            }
            return false;
        }

        public override void UseAnimation(Player player)
        {
            swordDirection = (player.Center - player.Calamity().mouseWorld).X > 1 ? -1 : 1;
            time = 0;
            swingRotation = 0;
        }
        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            player.itemRotation = swingRotation - 1.7f * swordDirection;
            player.itemLocation = player.Center;
            player.direction = swordDirection;

            float val = MathF.Abs(time - player.itemAnimationMax * 0.75f) / player.itemAnimationMax;

            float goalRot = Utils.Remap(time, 0, player.itemAnimationMax, -0.5f, 4.9f * swordDirection);
            float swingEasing = Utils.GetLerpValue(0, player.itemAnimationMax * 0.4f, time, true) * (0.5f - val);
            if (time < player.itemAnimationMax)
            {
                swingRotation = MathHelper.Lerp(swingRotation, goalRot, swingEasing);
            }

            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, swingRotation + MathHelper.ToRadians(120f * swordDirection));

            if (Main.rand.NextBool())
            {
                Vector2 dustVel = new Vector2(5 * swordDirection, -5).RotatedByRandom(1.55f) * Main.rand.NextFloat(0.7f, 1.3f) * 2;
                Dust dust = Dust.NewDustPerfect(player.Center + dustVel * 9, DustID.RainbowTorch);
                dust.scale = Main.rand.NextFloat(0.5f, 0.75f);
                dust.velocity = dustVel * 0.85f;
                dust.color = Color.LightGreen;
                dust.noGravity = true;
            }
            if (Main.rand.NextBool())
            {
                Vector2 dustVel = new Vector2(5 * swordDirection, -5).RotatedBy(swingRotation - 1.7f * swordDirection);

                float partScale = Main.rand.NextFloat(0.6f, 0.9f);
                Vector2 partVel = (dustVel * Main.rand.NextFloat(0.2f, 0.3f)).RotatedBy(MathHelper.ToRadians(90f * swordDirection)).RotatedByRandom(-0.4) * -3;
                Vector2 partPos = player.Center + dustVel * 25 + Main.rand.NextVector2Circular(12, 12);

                Particle spark3 = new AltSparkParticle(partPos, partVel, false, 24, partScale, Color.Black);
                GeneralParticleHandler.SpawnParticle(spark3);
                Particle spark2 = new SparkParticle(partPos, partVel, false, 24, partScale * 0.6f, Color.LightGreen);
                GeneralParticleHandler.SpawnParticle(spark2, false, GeneralDrawLayer.AfterEverything);
            }
            Vector2 dustVel2 = new Vector2(5 * swordDirection, -5).RotatedBy(swingRotation - 1.7f * swordDirection);

            float partScale2 = Main.rand.NextFloat(0.8f, 1.2f);
            Vector2 partVel2 = dustVel2 * Main.rand.NextFloat(0.2f, 0.3f);
            Particle smoke = new HeavySmokeParticle(player.Center + dustVel2 * 20 + Main.rand.NextVector2Circular(12, 12), partVel2.RotatedBy(MathHelper.ToRadians(90f * swordDirection)).RotatedBy(-0.3 * swordDirection) * -5, Color.Black, 19, partScale2, 0.5f, Main.rand.NextFloat(-0.2f, 0.2f), false);
            GeneralParticleHandler.SpawnParticle(smoke);
            time++;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<MeldBlob>(18).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
