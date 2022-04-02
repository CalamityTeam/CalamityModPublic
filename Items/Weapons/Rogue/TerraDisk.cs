using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class TerraDisk : RogueWeapon
    {
        public static int BaseDamage = 100;
        public static float Speed = 12f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Terra Disk");
            Tooltip.SetDefault(@"Throws a disk that has a chance to generate several disks if enemies are near it
A max of three disks can be active at a time
Stealth strikes travel slower and are rapidly orbited by the smaller disks");
        }

        public override void SafeSetDefaults()
        {
            item.width = 46;
            item.height = 46;
            item.damage = BaseDamage;
            item.knockBack = 4f;
            item.useAnimation = 16;
            item.useTime = 16;
            item.autoReuse = true;
            item.noMelee = true;
            item.noUseGraphic = true;

            item.useStyle = ItemUseStyleID.SwingThrow;
            item.UseSound = SoundID.Item1;

            item.value = Item.buyPrice(gold: 80);
            item.rare = ItemRarityID.Yellow;

            item.Calamity().rogue = true;
            item.shoot = ModContent.ProjectileType<TerraDiskProjectile>();
            item.shootSpeed = Speed;
        }

        public override bool CanUseItem(Player player)
        {
            //Stealth strikes ignore the proj cap
            int terraDiskCount = 0;
            for (int p = 0; p < Main.maxProjectiles; p++)
            {
                Projectile proj = Main.projectile[p];
                if (!proj.active || proj.owner != player.whoAmI)
                    continue;
                if (proj.type == item.shoot && !proj.Calamity().stealthStrike)
                {
                    terraDiskCount++;
                }
                if (terraDiskCount >= 3)
                    break;
            }
            return terraDiskCount < 3;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                speedX *= 0.75f;
                speedY *= 0.75f;
                damage = (int)(damage * 0.9f);
            }
            int proj = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
            if (player.Calamity().StealthStrikeAvailable() && proj.WithinBounds(Main.maxProjectiles))
            {
                Main.projectile[proj].Calamity().stealthStrike = true;
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.SetResult(this);
            recipe.AddIngredient(ModContent.ItemType<SeashellBoomerang>());
            recipe.AddIngredient(ModContent.ItemType<Equanimity>());
            recipe.AddIngredient(ItemID.ThornChakram);
            recipe.AddIngredient(ModContent.ItemType<LivingShard>(), 8);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.AddRecipe();
        }
    }
}
