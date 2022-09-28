using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class PhantomLance : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phantom Lance");
            Tooltip.SetDefault(@"Fires a spectral javelin that rapidly releases lost souls
Fades away and slows down over time
Lost souls released later deal less damage
Stealth strikes don't slow down and souls always deal full damage");
            SacrificeTotal = 99;
        }

        public override void SetDefaults()
        {
            Item.damage = 70;
            Item.knockBack = 5f;

            Item.width = 62;
            Item.height = 68;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.noUseGraphic = true;

            Item.value = Item.buyPrice(0, 0, 50, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.useTime = 23;
            Item.useAnimation = 23;
            Item.maxStack = 999;
            Item.UseSound = SoundID.Item1;
            Item.consumable = true;
            Item.DamageType = RogueDamageClass.Instance;

            Item.autoReuse = true;
            Item.shootSpeed = 10f;
            Item.shoot = ModContent.ProjectileType<PhantomLanceProj>();
        }

		public override float StealthDamageMultiplier => 1.75f;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable()) //setting the stealth strike
            {
                int stealth = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
                if (stealth.WithinBounds(Main.maxProjectiles))
                    Main.projectile[stealth].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe(100).
                AddIngredient(ItemID.SpectreBar).
                AddIngredient<ScoriaBar>().
                AddIngredient<AshesofCalamity>().
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
