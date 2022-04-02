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
            Tooltip.SetDefault("Stealth strikes make the knife fly further and hit several times at once");
        }

        public override void SafeSetDefaults()
        {
            item.width = 22;
            item.damage = 11;
            item.noMelee = true;
            item.consumable = true;
            item.noUseGraphic = true;
            item.useAnimation = 15;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 15;
            item.knockBack = 1f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 38;
            item.maxStack = 999;
            item.value = Item.sellPrice(0, 0, 0, 5);
            item.rare = ItemRarityID.Blue;
            item.shoot = ModContent.ProjectileType<WulfrumKnifeProj>();
            item.shootSpeed = 12f;
            item.Calamity().rogue = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<WulfrumShard>());
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this, 100);
            recipe.AddRecipe();
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int p = Projectile.NewProjectile(position, new Vector2(speedX, speedY) * 1.3f, ModContent.ProjectileType<WulfrumKnifeProj>(), damage, knockBack, player.whoAmI);
                Projectile proj = Main.projectile[p];
                if (p.WithinBounds(Main.maxProjectiles))
                {
                    proj.Calamity().stealthStrike = true;
                    proj.penetrate = 4;
                    proj.usesLocalNPCImmunity = true;
                    proj.localNPCHitCooldown = 1;
                }
                return false;
            }
            return true;
        }
    }
}
