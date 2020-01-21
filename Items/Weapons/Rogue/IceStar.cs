using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class IceStar : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ice Star");
            Tooltip.SetDefault("Throws homing ice stars\n" +
                "Stealth strikes pierce infinitely and spawn ice shards on hit\n" +
                "Ice Stars are too brittle to be recovered after being thrown");
        }

        public override void SafeSetDefaults()
        {
            item.width = 62;
            item.damage = 45;
            item.noMelee = true;
            item.consumable = true;
            item.noUseGraphic = true;
            item.useAnimation = 12;
            item.crit = 7;
            item.useStyle = 1;
            item.useTime = 12;
            item.knockBack = 2.5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 62;
            item.maxStack = 999;
            item.value = Item.sellPrice(0, 0, 1, 20);
            item.rare = 5;
            item.shoot = ModContent.ProjectileType<IceStarProjectile>();
            item.shootSpeed = 14f;
            item.Calamity().rogue = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<CryoBar>());
            recipe.AddTile(TileID.IceMachine);
            recipe.SetResult(this, 50);
            recipe.AddRecipe();
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int proj = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI, 0f, 0f);
                Main.projectile[proj].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }
    }
}
