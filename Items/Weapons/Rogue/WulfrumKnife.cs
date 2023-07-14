using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Particles;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class WulfrumKnife : RogueWeapon
    {
        public static readonly SoundStyle Throw3Sound = new("CalamityMod/Sounds/Item/WulfrumKnifeThrowFull") { PitchVariance = 0.4f };
        public static readonly SoundStyle Throw2Sound = new("CalamityMod/Sounds/Item/WulfrumKnifeThrowTwo") { PitchVariance = 0.4f };
        public static readonly SoundStyle Throw1Sound = new("CalamityMod/Sounds/Item/WulfrumKnifeThrowSingle") { PitchVariance = 0.4f };
        public static readonly SoundStyle TileHitSound = new("CalamityMod/Sounds/Item/WulfrumKnifeTileHit", 2) { PitchVariance = 0.4f , MaxInstances = 3};

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 99;
        }

        public int shootCount = 0;
        public bool stealthStrikeStarted = false;

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.damage = 9;
            Item.noMelee = true;
            Item.consumable = true;
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Swing;
            //Clockwork burst
            Item.useTime = 4;
            Item.useAnimation = 10;
            Item.reuseDelay = 24;
            Item.useLimitPerAnimation = 3;

            Item.knockBack = 1f;
            Item.UseSound = Throw3Sound;
            Item.autoReuse = true;
            Item.height = 38;
            Item.maxStack = 9999;
            Item.value = Item.sellPrice(0, 0, 0, 5);
            Item.rare = ItemRarityID.Blue;
            Item.shoot = ModContent.ProjectileType<WulfrumKnifeProj>();
            Item.shootSpeed = 4f;
            Item.DamageType = RogueDamageClass.Instance;
        }

        //Magnetization
        public override void HoldItem(Player player)
        {
            player.Calamity().rightClickListener = true;

            if (player.Calamity().mouseRight && Main.rand.NextBool(7))
            {
                Particle streak = new ManaDrainStreak(player, Main.rand.NextFloat(0.2f, 0.5f), Main.rand.NextVector2CircularEdge(1f, 1f) * Main.rand.NextFloat(170f, 670f), Main.rand.NextFloat(30f, 44f), Color.GreenYellow, Color.DeepSkyBlue, Main.rand.Next(15, 30));
                GeneralParticleHandler.SpawnParticle(streak);
            }
        }

        public override void GrabRange(Player player, ref int grabRange)
        {
            float rangeMult = 2f;
            if (player.Distance(Item.Center) < 670 && player.HeldItem.type == ModContent.ItemType<WulfrumKnife>() && player.Calamity().mouseRight)
                rangeMult = 20f;

            grabRange = (int)(grabRange * rangeMult);

            if (rangeMult > 2f && player.Distance(Item.Center) > 50f)
            {
                Item.velocity = (player.DirectionTo(Item.Center) * -20f);

                if (Main.rand.NextBool(3))
                {
                    Vector2 dustCenter = Item.Center + Main.rand.NextVector2Circular(4f, 4f);

                    Dust chust = Dust.NewDustPerfect(dustCenter, 15, -Item.velocity * Main.rand.NextFloat(0.2f, 0.1f), Scale: Main.rand.NextFloat(1.2f, 1.8f));
                    chust.noGravity = true;
                }
            }
        }

        
        //While this may look stupid, its necessary because ReuseDelay fucks up the consumption of the item otherwise.
        public override bool ConsumeItem(Player player) => shootCount < 0;
        public override bool? UseItem(Player player)
        {
            int placeholderStock = shootCount;
            shootCount = -1;

            if (ItemLoader.ConsumeItem(Item, player))
            {
                Item.stack--;

                if (Item.stack <= 0)
                    Item.TurnToAir();
            }

            shootCount = placeholderStock;

            shootCount++;
            return base.UseItem(player);
        }

        //Random spread
        public override void UseAnimation(Player player)
        {
            shootCount = 0;
            stealthStrikeStarted = false;

            Item.UseSound = Throw3Sound;

            if (Item.stack == 2)
                Item.UseSound = Throw2Sound;
            if (Item.stack == 1)
                Item.UseSound = Throw1Sound;
        }

		public override float StealthDamageMultiplier => 0.8f;
        public override bool AdditionalStealthCheck() => stealthStrikeStarted;

        public override void ModifyStatsExtra(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            bool stealthStrike = player.Calamity().StealthStrikeAvailable() || stealthStrikeStarted;
            float spread = stealthStrike ? MathHelper.PiOver4 * 0.04f : MathHelper.PiOver4 * 0.1f;
            float speedBoost = stealthStrike ? 1.25f : 1f;

            velocity = velocity.RotatedByRandom(shootCount / 2f * spread) * speedBoost;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable() || stealthStrikeStarted)
            {
                stealthStrikeStarted = true;

                int p = Projectile.NewProjectile(source, position, velocity * 1.3f, ModContent.ProjectileType<WulfrumKnifeProj>(), damage, knockback, player.whoAmI);
                Projectile proj = Main.projectile[p];
                if (p.WithinBounds(Main.maxProjectiles))
                {
                    proj.Calamity().stealthStrike = true;
                    proj.penetrate = 2;
                }
                return false;
            }
            return true;
        }


        public override void AddRecipes()
        {
            CreateRecipe(50).
                AddIngredient<WulfrumMetalScrap>().
                Register();
        }
    }
}
