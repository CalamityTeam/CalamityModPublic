using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class Hypothermia : RogueWeapon
    {
        // For more consistent DPS, always alternates between throwing 1 and 2 instead of picking randomly
        private bool throwTwo = true;

        public override void SetDefaults()
        {
            Item.width = 46;
            Item.height = 32;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item7;
            Item.value = CalamityGlobalItem.RarityDarkBlueBuyPrice;

            Item.damage = 200;
            Item.useTime = 3;
            Item.useAnimation = 21;
            Item.reuseDelay = 1;
            Item.useLimitPerAnimation = 7;
            Item.knockBack = 3f;
            Item.shoot = ModContent.ProjectileType<HypothermiaShard>();
            Item.shootSpeed = 8f;

            Item.rare = ModContent.RarityType<DarkBlue>();
            Item.DamageType = RogueDamageClass.Instance;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 16;

		public override float StealthDamageMultiplier => 1.2f;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                for (int i = 0; i < 4; ++i)
                {
                    Vector2 chunkVelocity = velocity.RotatedByRandom(0.07f) * Main.rand.NextFloat(1.1f, 1.18f);
                    int stealth = Projectile.NewProjectile(source, position, chunkVelocity, ModContent.ProjectileType<HypothermiaChunk>(), damage, knockback, player.whoAmI);
                    if (stealth.WithinBounds(Main.maxProjectiles))
                        Main.projectile[stealth].Calamity().stealthStrike = true;
                }

                // On a stealth strike, only the chunks are thrown.
                return false;
            }

            // Regular attacks alternate between throwing one and two shards at a time.
            int projAmt = throwTwo ? 2 : 1;
            throwTwo = !throwTwo;

            for (int i = 0; i < projAmt; ++i)
            {
                float SpeedX = velocity.X + Main.rand.NextFloat(-2f, 2f);
                float SpeedY = velocity.Y + Main.rand.NextFloat(-2f, 2f);
                int texID = Main.rand.Next(4);
                Projectile.NewProjectile(source, position, new Vector2(SpeedX, SpeedY), type, damage, knockback, player.whoAmI, texID, 0f);
            }

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.IceBlock, 100).
                AddIngredient<RuinousSoul>(6).
                AddIngredient<CosmiliteBar>(8).
                AddIngredient<EndothermicEnergy>(20).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
