using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class FrostcrushValari : RogueWeapon
    {
        public static float Speed = 15f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Frostcrush Valari");
            Tooltip.SetDefault(@"Fires a long ranged boomerang that explodes into icicles on hit
Stealth strikes throw three short ranged boomerangs along with a spread of icicles");
        }

        public override void SafeSetDefaults()
        {
            item.damage = 100;
            item.knockBack = 12;
            item.thrown = true;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.rare = ItemRarityID.Lime;
            item.useTime = 19;
            item.useAnimation = 19;
            item.width = 32;
            item.height = 46;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.shootSpeed = Speed;
            item.shoot = ModContent.ProjectileType<ValariBoomerang>();
            item.noMelee = true;
            item.noUseGraphic = true;
            item.Calamity().rogue = true;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void GetWeaponCrit(Player player, ref int crit) => crit += 16;

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            CalamityPlayer modPlayer = Main.player[Main.myPlayer].Calamity();
            //If stealth is full, shoot a spread of 3 boomerangs with reduced range and 6 to 10 icicles
            if (modPlayer.StealthStrikeAvailable())
            {
                int spread = 10;
                for (int i = 0; i < 3; i++)
                {
                    Vector2 perturbedspeed = new Vector2(speedX, speedY).RotatedBy(MathHelper.ToRadians(spread));
                    int proj = Projectile.NewProjectile(position, perturbedspeed, type, Math.Max((int)(damage / 2.7272f), 1), knockBack / 3f, player.whoAmI, 0f, 1f);
                    if (proj.WithinBounds(Main.maxProjectiles))
                        Main.projectile[proj].Calamity().stealthStrike = true;
                    spread -= 10;
                }
                int spread2 = 3;
                int icicleAmt = Main.rand.Next(6,11);
                for (int i = 0; i < icicleAmt; i++)
                {
                    Vector2 perturbedspeed = new Vector2(speedX + Main.rand.Next(-3,4), speedY + Main.rand.Next(-3,4)).RotatedBy(MathHelper.ToRadians(spread2));
                    Projectile.NewProjectile(position, perturbedspeed, (Main.rand.NextBool(2) ? ModContent.ProjectileType<Valaricicle>() : ModContent.ProjectileType<Valaricicle2>()), Math.Max((int)(damage / 2.7272f), 1), 0f, player.whoAmI, 0f, 0f);
                    spread2 -= Main.rand.Next(1,4);
                }
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<Kylie>());
            recipe.AddIngredient(ModContent.ItemType<VerstaltiteBar>(), 6);
            recipe.AddIngredient(ModContent.ItemType<Voidstone>(), 40);
            recipe.AddIngredient(ModContent.ItemType<CoreofEleum>(), 5);
            recipe.AddTile(TileID.IceMachine);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
