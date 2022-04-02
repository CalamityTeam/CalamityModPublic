using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class CrystalPiercer : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crystal Piercer");
            Tooltip.SetDefault("Throws a crystal javelin that pierces infinitely\n" +
            "Stealth strikes travel through blocks, ignore gravity, and summon crystal shards as they fly");
        }

        public override void SafeSetDefaults()
        {
            item.width = 62;
            item.damage = 52;
            item.noMelee = true;
            item.consumable = true;
            item.noUseGraphic = true;
            item.useAnimation = 17;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 17;
            item.knockBack = 6f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 62;
            item.maxStack = 999;
            item.value = 2500;
            item.rare = ItemRarityID.Pink;
            item.shoot = ModContent.ProjectileType<CrystalPiercerProjectile>();
            item.shootSpeed = 20f;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable()) //setting the stealth strike
            {
                int stealth = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
                if (stealth.WithinBounds(Main.maxProjectiles))
                {
                    Main.projectile[stealth].Calamity().stealthStrike = true;
                    Main.projectile[stealth].aiStyle = -1;
                    Main.projectile[stealth].tileCollide = false;
                    Main.projectile[stealth].usesLocalNPCImmunity = true;
                }
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<VerstaltiteBar>());
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this, 100);
            recipe.AddRecipe();
        }
    }
}
