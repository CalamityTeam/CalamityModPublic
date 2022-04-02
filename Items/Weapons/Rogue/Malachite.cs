using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class Malachite : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Malachite");
            Tooltip.SetDefault("Throws a stream of kunai that stick to enemies and explode\n" +
                "Right click to throw a single kunai that pierces, after piercing an enemy it emits a massive explosion on the next enemy hit\n" +
                "Stealth strikes fire three kunai that home in, stick to enemies, and explode");
        }

        public override void SafeSetDefaults()
        {
            item.width = 26;
            item.damage = 50;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useTime = item.useAnimation = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 1.25f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 58;
            item.shoot = ModContent.ProjectileType<MalachiteProj>();
            item.shootSpeed = 10f;
            item.Calamity().rogue = true;

            item.value = CalamityGlobalItem.Rarity8BuyPrice;
            item.rare = ItemRarityID.Yellow;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                item.UseSound = SoundID.Item109;
                item.shoot = ModContent.ProjectileType<MalachiteStealth>();
            }
            else if (player.altFunctionUse == 2)
            {
                item.UseSound = SoundID.Item109;
                item.shoot = ModContent.ProjectileType<MalachiteBolt>();
            }
            else
            {
                item.UseSound = SoundID.Item1;
                item.shoot = ModContent.ProjectileType<MalachiteProj>();
            }
            return base.CanUseItem(player);
        }

        public override float SafeSetUseTimeMultiplier(Player player)
        {
            if (player.Calamity().StealthStrikeAvailable() || player.altFunctionUse == 2)
                return 1f;
            return 2f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float dmgMult = 1f;
            if (player.Calamity().StealthStrikeAvailable())
            {
                for (float i = -6.5f; i <= 6.5f; i += 6.5f)
                {
                    Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedBy(MathHelper.ToRadians(i));
                    int stealth = Projectile.NewProjectile(position, perturbedSpeed, type, damage * 2, knockBack, player.whoAmI);
                    if (stealth.WithinBounds(Main.maxProjectiles))
                        Main.projectile[stealth].Calamity().stealthStrike = true;
                }
                return false;
            }
            else if (player.altFunctionUse == 2)
            {
                dmgMult = 1.75f;
            }
            else
            {
                dmgMult = 1f;
            }
            Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, (int)(damage * dmgMult), knockBack, player.whoAmI);
            return false;
        }
    }
}
