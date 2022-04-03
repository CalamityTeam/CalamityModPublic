using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class DuneHopper : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wave Skipper"); // This will drop from the Sunken Sea Scourge miniboss once it's implemented.
            Tooltip.SetDefault(@"Throws a spear that bounces a lot
Stealth strikes throw three high speed spears");
        }

        public override void SafeSetDefaults()
        {
            Item.width = 44;
            Item.damage = 50;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 22;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 22;
            Item.knockBack = 4f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.height = 44;
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.shoot = ModContent.ProjectileType<DuneHopperProjectile>();
            Item.shootSpeed = 12f;
            Item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int numProj = 2;
                float rotation = MathHelper.ToRadians(3);
                for (int i = 0; i < numProj + 1; i++)
                {
                    Vector2 perturbedSpeed = new Vector2(speedX - 3f, speedY - 3f).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numProj - 1)));
                    int stealth = Projectile.NewProjectile(position, perturbedSpeed, ModContent.ProjectileType<DuneHopperProjectile>(), Math.Max(damage / 3, 1), knockBack, player.whoAmI);
                    if (stealth.WithinBounds(Main.maxProjectiles))
                        Main.projectile[stealth].Calamity().stealthStrike = true;
                }
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<ScourgeoftheDesert>()).AddIngredient(ModContent.ItemType<MolluskHusk>(), 5).AddIngredient(ModContent.ItemType<SeaPrism>(), 15).AddIngredient(ModContent.ItemType<PrismShard>(), 20).AddTile(TileID.Anvils).Register();
        }
    }
}
