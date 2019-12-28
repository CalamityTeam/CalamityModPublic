using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class WulfrumKnife : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wulfrum Knife");
        }

        public override void SafeSetDefaults()
        {
            item.width = 22;
            item.damage = 8;
            item.noMelee = true;
            item.consumable = true;
            item.noUseGraphic = true;
            item.useAnimation = 15;
            item.useStyle = 1;
            item.useTime = 15;
            item.knockBack = 1f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 38;
            item.maxStack = 999;
            item.value = 100;
            item.rare = 1;
            item.shoot = ModContent.ProjectileType<WulfrumKnifeProj>();
            item.shootSpeed = 12f;
            item.Calamity().rogue = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<WulfrumShard>());
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this, 50);
            recipe.AddRecipe();
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int p = Projectile.NewProjectile(position, new Vector2(speedX, speedY) * 1.3f, ModContent.ProjectileType<WulfrumKnifeProj>(), damage, knockBack, player.whoAmI);
                Projectile proj = Main.projectile[p];
                proj.Calamity().stealthStrike = true;
                proj.penetrate = 4;
                proj.usesLocalNPCImmunity = true;
                proj.localNPCHitCooldown = 1;
                return false;
            }
            return true;
        }
    }
}
