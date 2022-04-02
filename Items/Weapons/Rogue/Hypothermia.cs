using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
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
            DisplayName.SetDefault("Hypothermia");
            Tooltip.SetDefault("Throws a constant barrage of black ice shards\n" +
                               "Stealth strikes hurl a set of razor sharp ice chunks that shatter on impact");
        }

        public override void SafeSetDefaults()
        {
            item.width = 46;
            item.height = 32;
            item.autoReuse = true;
            item.noUseGraphic = true;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.UseSound = SoundID.Item7;
            item.value = Item.buyPrice(1, 80, 0, 0);
            item.rare = ItemRarityID.Red;

            item.damage = 200;
            item.useAnimation = 21;
            item.useTime = 3;
            item.reuseDelay = 1;
            item.knockBack = 3f;
            item.shoot = ModContent.ProjectileType<HypothermiaShard>();
            item.shootSpeed = 8f;

            item.Calamity().customRarity = CalamityRarity.DarkBlue;
            item.Calamity().rogue = true;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void GetWeaponCrit(Player player, ref int crit) => crit += 16;

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int stealthDamage = (int)(damage * 1.2f);
                for (int i = 0; i < 4; ++i)
                {
                    Vector2 chunkVelocity = new Vector2(speedX, speedY).RotatedByRandom(0.07f) * Main.rand.NextFloat(1.1f, 1.18f);
                    int stealth = Projectile.NewProjectile(position, chunkVelocity, ModContent.ProjectileType<HypothermiaChunk>(), stealthDamage, knockBack, player.whoAmI);
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
                float SpeedX = speedX + Main.rand.NextFloat(-2f, 2f);
                float SpeedY = speedY + Main.rand.NextFloat(-2f, 2f);
                int texID = Main.rand.Next(4);
                Projectile.NewProjectile(position, new Vector2(SpeedX, SpeedY), type, damage, knockBack, player.whoAmI, texID, 0f);
            }

            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.IceBlock, 100);
            recipe.AddIngredient(ModContent.ItemType<RuinousSoul>(), 6);
            recipe.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 8);
            recipe.AddIngredient(ModContent.ItemType<EndothermicEnergy>(), 20);
            recipe.AddTile(ModContent.TileType<CosmicAnvil>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
