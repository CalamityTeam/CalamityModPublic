using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Rarities;
using CalamityMod.Systems.Collections;
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
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<Voidfrost>(), BuffID.Frozen];
        }
        public override void SetDefaults()
        {
            Item.width = 46;
            Item.height = 32;
            Item.damage = 148;
            Item.DamageType = RogueDamageClass.Instance;
            Item.crit = 16;
            Item.useTime = 4;
            Item.useAnimation = 12;
            Item.reuseDelay = 1;
            Item.useLimitPerAnimation = 3;
            Item.knockBack = 3f;

            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item7;
            Item.shoot = ModContent.ProjectileType<HypothermiaShard>();
            Item.shootSpeed = 8f;

            Item.rare = ModContent.RarityType<CosmicPurple>();
            Item.value = CalamityGlobalItem.RarityDarkBlueBuyPrice;
        }

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
                Vector2 shardVel = velocity.RotatedByRandom(MathHelper.Pi / 30f) * Main.rand.NextFloat(0.9f, 1.1f);
                int texID = Main.rand.Next(4);
                Projectile.NewProjectile(source, position, shardVel, type, damage, knockback, player.whoAmI, texID);
            }

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<CosmiliteBar>(8).
                AddIngredient<EndothermicEnergy>(20).
                AddIngredient<RuinousSoul>(6).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
