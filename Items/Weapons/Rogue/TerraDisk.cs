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
            Item.width = 46;
            Item.height = 46;
            Item.damage = BaseDamage;
            Item.knockBack = 4f;
            Item.useAnimation = 16;
            Item.useTime = 16;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;

            Item.value = Item.buyPrice(gold: 80);
            Item.rare = ItemRarityID.Yellow;

            Item.Calamity().rogue = true;
            Item.shoot = ModContent.ProjectileType<TerraDiskProjectile>();
            Item.shootSpeed = Speed;
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
                if (proj.type == Item.shoot && !proj.Calamity().stealthStrike)
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
            CreateRecipe(1).AddIngredient(ModContent.ItemType<SeashellBoomerang>()).AddIngredient(ModContent.ItemType<Equanimity>()).AddIngredient(ItemID.ThornChakram).AddIngredient(ModContent.ItemType<LivingShard>(), 8).AddTile(TileID.MythrilAnvil).Register();
        }
    }
}
