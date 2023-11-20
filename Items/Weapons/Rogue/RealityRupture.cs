using Terraria.DataStructures;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using CalamityMod.Rarities;

namespace CalamityMod.Items.Weapons.Rogue
{
    [LegacyName("LuminousStriker")]
    public class RealityRupture : RogueWeapon
    {
        public static readonly SoundStyle ThrowSound = new("CalamityMod/Sounds/Item/RealityRupture") { Volume = 0.3f, PitchVariance = 0.3f };
        public static readonly SoundStyle ThrowSound2 = new("CalamityMod/Sounds/Item/LanceofDestinyStrong") { Volume = 0.4f, PitchVariance = 0.3f };
        public static readonly SoundStyle ThrowSound3 = new("CalamityMod/Sounds/Item/RealityRuptureStealth") { Volume = 0.5f, PitchVariance = 0.3f };
        private bool BigSpear = false;
        public override void SetDefaults()
        {
            Item.width = 86;
            Item.height = 102;
            Item.damage = 225;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 37;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 37;
            Item.knockBack = 9f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.shoot = ModContent.ProjectileType<RealityRuptureMini>();
            Item.shootSpeed = 10f;
            Item.DamageType = RogueDamageClass.Instance;
        }
        public override float StealthDamageMultiplier => 3.9f;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable()) //setting the stealth strike
            {
                int stealth = Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<RealityRuptureStealth>(), damage, knockback, player.whoAmI);
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
                int projType = BigSpear ? ModContent.ProjectileType<RealityRuptureLance>() : type;

                if (!BigSpear)
                    SoundEngine.PlaySound(ThrowSound, player.Center);
                else
                    SoundEngine.PlaySound(ThrowSound2, player.Center);

                if (BigSpear)
                {
                    Projectile.NewProjectile(source, position, velocity, projType, BigSpear ? damage * 4 : damage, knockback * 1.5f, player.whoAmI);
                }
                int index = 4;
                for (int i = -index; i <= index; i += index)
                {
                    if (!BigSpear)
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
                AddIngredient<SpearofDestiny>().
                AddIngredient<ArmoredShell>().
                AddIngredient<TwistingNether>().
                AddIngredient<DarkPlasma>().
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
