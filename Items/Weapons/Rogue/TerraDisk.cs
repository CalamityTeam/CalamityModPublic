using Terraria.DataStructures;
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
Stealth strikes travel slower and are rapidly orbited by the smaller disks");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 46;
            Item.height = 46;
            Item.damage = BaseDamage;
            Item.knockBack = 4f;
            Item.useAnimation = Item.useTime = 30;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;

            Item.value = CalamityGlobalItem.RarityYellowBuyPrice;
            Item.rare = ItemRarityID.Yellow;

            Item.DamageType = RogueDamageClass.Instance;
            Item.shoot = ModContent.ProjectileType<TerraDiskProjectile>();
            Item.shootSpeed = Speed;
        }
        
        public override float StealthVelocityMultiplier => 0.75f;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int proj = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            if (player.Calamity().StealthStrikeAvailable() && proj.WithinBounds(Main.maxProjectiles))
            {
                Main.projectile[proj].Calamity().stealthStrike = true;
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Equanimity>().
                AddIngredient<Brimblade>().
                AddIngredient<LivingShard>(12).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
