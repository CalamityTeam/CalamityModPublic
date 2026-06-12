using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class SpearofDestiny : RogueWeapon
    {
        public static readonly SoundStyle ThrowSound = new("CalamityMod/Sounds/Item/SpearofDestiny") { Volume = 0.3f, PitchVariance = 0.3f };
        public static readonly SoundStyle ThrowSound2 = new("CalamityMod/Sounds/Item/LanceofDestiny") { Volume = 0.3f, PitchVariance = 0.3f };
        public static readonly SoundStyle ThrowSound3 = new("CalamityMod/Sounds/Item/LanceofDestinyStrong") { Volume = 0.5f, PitchVariance = 0.3f };
        private bool BigSpear = false;
        public override void SetDefaults()
        {
            Item.width = 100;
            Item.height = 100;
            Item.damage = 70;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = Item.useTime = 41;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 2f;
            Item.UseSound = ThrowSound;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.RarityLightPurpleBuyPrice;
            Item.rare = ItemRarityID.LightPurple;
            Item.shoot = ModContent.ProjectileType<SpearofDestinyProjectile>();
            Item.shootSpeed = 10f;
            Item.DamageType = RogueDamageClass.Instance;
        }

        public override float StealthDamageMultiplier => 2.8f;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable()) //setting the stealth strike
            {
                int stealth = Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<SpearofDestinyStealth>(), damage, knockback, player.whoAmI);
                SoundEngine.PlaySound(ThrowSound3, player.Center);
                if (stealth.WithinBounds(Main.maxProjectiles))
                {
                    Main.projectile[stealth].Calamity().stealthStrike = true;
                    Main.projectile[stealth].usesLocalNPCImmunity = true;
                }
                return false;
            }
            else
            {
                int projType = BigSpear ? ModContent.ProjectileType<LanceofDestiny>() : type;

                SoundEngine.PlaySound(BigSpear ? ThrowSound2 : ThrowSound, player.Center);

                if (BigSpear)
                {
                    Projectile.NewProjectile(source, position, velocity, projType, damage * 3, knockback, player.whoAmI);
                }
                else
                {
                    int index = 5;
                    for (int i = -index; i <= index; i += index)
                    {
                        Vector2 perturbedSpeed = velocity.RotatedBy(MathHelper.ToRadians(i));
                        int spear = Projectile.NewProjectile(source, position, perturbedSpeed, projType, damage, knockback, player.whoAmI);
                        if (spear.WithinBounds(Main.maxProjectiles))
                            Main.projectile[spear].Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
                    }
                }

                // Swap between firing small and big spears each throw.
                BigSpear = !BigSpear;
                return false;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
            AddIngredient<CursedDagger>().
            AddIngredient(ItemID.HallowedBar, 7).
            AddIngredient(ItemID.SoulofFright, 5).
            AddIngredient(ItemID.SoulofMight, 5).
            AddIngredient(ItemID.SoulofSight, 5).
            AddTile(TileID.MythrilAnvil).
            Register();
            CreateRecipe().
            AddIngredient<IchorSpear>().
            AddIngredient(ItemID.HallowedBar, 7).
            AddIngredient(ItemID.SoulofFright, 5).
            AddIngredient(ItemID.SoulofMight, 5).
            AddIngredient(ItemID.SoulofSight, 5).
            AddTile(TileID.MythrilAnvil).
            Register();
        }
    }
}
