using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class TheBurningSky : ModItem
    {
        private const int ProjectilesPerBarrage = 6;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Burning Sky");
            Tooltip.SetDefault("Hold the blade to the sky, and witness Armageddon");

            // Visually a sword, but with no true melee capability. The Burning Sky is held out like a staff.
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.width = 102;
            item.height = 146;
            item.damage = 244;
            item.knockBack = 2.5f;

            item.melee = true;
            item.noMelee = true;

            item.useTime = 9;
            item.useAnimation = 9;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.autoReuse = true;
            item.UseSound = SoundID.Item105;

            item.shoot = ModContent.ProjectileType<BurningMeteor>();
            item.shootSpeed = 14f;

            item.value = CalamityGlobalItem.Rarity15BuyPrice;
            item.rare = ItemRarityID.Purple;
            item.Calamity().customRarity = CalamityRarity.Violet;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            // Every time the item spawns more meteors, play a violent, bass heavy sound to add onto Star Wrath's use sound.
            Main.PlaySound(SoundID.Item70, player.Center);

            Vector2 originalVelocity = new Vector2(speedX, speedY);
            float speed = originalVelocity.Length();
            for (int i = 0; i < ProjectilesPerBarrage; ++i)
            {
                float randomSpeed = speed * Main.rand.NextFloat(0.7f, 1.4f);
                CalamityUtils.ProjectileRain(Main.MouseWorld, 290f, 130f, 850f, 1100f, randomSpeed, type, damage, knockBack, player.whoAmI);
            }
            return false;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit) => target.AddBuff(BuffID.Daybreak, 300);
        public override void OnHitPvp(Player player, Player target, int damage, bool crit) => target.AddBuff(BuffID.Daybreak, 300);
    }
}
