using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class TheDanceofLight : ModItem
    {
        private static float SpawnAngleSpread = 0.4f * MathHelper.Pi;
        private static float SpeedRandomness = 0.08f;
        private static float Inaccuracy = 0.04f;
        private static float MinSpawnDist = 40;
        private static float MaxSpawnDist = 140;

        public static Color GetLightColor(float deviation) => new Color(1f, 0.5f + 0.35f * MathHelper.Clamp(deviation, 0f, 1f), 1f);
        public static Color GetSyncedLightColor() => GetLightColor(Main.DiscoG / 255f);
        public static Color GetRandomLightColor() => GetLightColor(Main.rand.NextFloat());

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Dance of Light");
            Tooltip.SetDefault("Barrages enemies with a hailstorm of Light Blades\nShow them the wrath of a Lightbearer");
        }

        public override void SetDefaults()
        {
            item.width = 40;
            item.height = 42;
            item.damage = 1800;
            item.crit += 20;
            item.knockBack = 4f;
            item.magic = true;
            item.mana = 6;

            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useTime = 2;
            item.useAnimation = 8;
            item.reuseDelay = 5;
            item.UseSound = SoundID.Item105;
            item.autoReuse = true;
            item.noMelee = true;

            item.shoot = ModContent.ProjectileType<LightBlade>();
            item.shootSpeed = 14f;

            item.value = Item.buyPrice(platinum: 5);
            item.rare = 10;
            item.Calamity().customRarity = CalamityRarity.ItemSpecific;
        }

        public override Vector2? HoldoutOffset()
        {
            return Vector2.Zero;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int numProjs = 2;
            for(int i = 0; i < numProjs; ++i)
            {
                // Pick a random direction somewhere behind the player
                float shootAngle = (float)Math.Atan2(speedY, speedX);
                Vector2 offset = Main.rand.NextVector2Unit(MathHelper.Pi - SpawnAngleSpread, 2f * SpawnAngleSpread).RotatedBy(shootAngle);
                // Set how far away this sword is spawning
                offset *= Main.rand.NextFloat(MinSpawnDist, MaxSpawnDist);

                Vector2 spawnPos = position + offset;
                float randSpeed = Main.rand.NextFloat(1f - SpeedRandomness, 1f + SpeedRandomness);
                float randAngle = Main.rand.NextFloat(-Inaccuracy, Inaccuracy);
                Vector2 velocity = randSpeed * new Vector2(speedX, speedY).RotatedBy(randAngle);
                Projectile.NewProjectileDirect(spawnPos, velocity, type, damage, knockBack, player.whoAmI);
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.SkyFracture);
            recipe.AddIngredient(ItemID.LunarFlareBook);
            recipe.AddIngredient(ModContent.ItemType<WrathoftheAncients>());
            recipe.AddIngredient(ModContent.ItemType<ShadowspecBar>(), 5);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
